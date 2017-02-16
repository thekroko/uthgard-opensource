using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using CEM.Client;
using CEM.Client.ZoneExporter;
using CEM.Datastructures;
using CEM.Utils;
using OpenTK;

namespace CEM.World {
  /// <summary>
  /// Pathing using Recast
  /// (c) TheSchaf
  /// </summary>
  internal static class NavmeshMgr {
    public const float CONVERSION_FACTOR = 1.0f / 32f;
    private const float INV_FACTOR = (1f / CONVERSION_FACTOR);
    private const int MAX_POLY = 256;    // max vector3 when looking up a path (for straight paths too)

    public static readonly Vector3 zAxis = new Vector3(0, 0, 1);

    [Flags]
    public enum dtPolyFlags : ushort {
      WALK = 0x01,    // Ability to walk (ground, grass, road)
      SWIM = 0x02,    // Ability to swim (water).
      DOOR = 0x04,    // Ability to move through doors.
      JUMP = 0x08,    // Ability to jump.
      DISABLED = 0x10,    // Disabled polygon
      DOOR_ALB = 0x20,
      DOOR_MID = 0x40,
      DOOR_HIB = 0x80,
      ALL = 0xffff      // All abilities.
    }

    [Flags]
    public enum dtStatus : uint {
      // High level status.
      DT_FAILURE = 1u << 31,        // Operation failed.
      DT_SUCCESS = 1u << 30,        // Operation succeed.
      DT_IN_PROGRESS = 1u << 29,    // Operation still in progress.

      // Detail information for status.
      DT_STATUS_DETAIL_MASK = 0x0ffffff,
      DT_WRONG_MAGIC = 1 << 0,      // Input data is not recognized.
      DT_WRONG_VERSION = 1 << 1,    // Input data is in wrong version.
      DT_OUT_OF_MEMORY = 1 << 2,    // Operation ran out of memory.
      DT_INVALID_PARAM = 1 << 3,    // An input parameter was invalid.
      DT_BUFFER_TOO_SMALL = 1 << 4, // Result buffer for the query was too small to store all results.
      DT_OUT_OF_NODES = 1 << 5,     // Query ran out of nodes during search.
      DT_PARTIAL_RESULT = 1 << 6,  	// Query did not reach the end location, returning best guess. 
    }

    public enum dtStraightPathOptions : uint {
      DT_STRAIGHTPATH_NO_CROSSINGS = 0x00,    // Do not add extra vertices on polygon edge crossings.
      DT_STRAIGHTPATH_AREA_CROSSINGS = 0x01,  // Add a vertex at every polygon edge crossing where area changes.
      DT_STRAIGHTPATH_ALL_CROSSINGS = 0x02,	  // Add a vertex at every polygon edge crossing.
    }

    public struct PathPoint {
      public Vector3 Position;
      public dtPolyFlags Flags;
    }
    private static readonly Dictionary<ushort, RecastMesh> _loadedZones = new Dictionary<ushort, RecastMesh>();

    [DllImport("ReUth.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern bool LoadNavMesh(string file, ref IntPtr meshPtr, ref IntPtr queryPtr);

    [DllImport("ReUth.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern bool FreeNavMesh(IntPtr meshPtr, IntPtr queryPtr);

    [DllImport("ReUth.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern dtStatus PathStraight(IntPtr queryPtr, float[] start, float[] end, float[] polyPickExt, dtPolyFlags[] queryFilter, dtStraightPathOptions pathOptions, ref int pointCount, float[] pointBuffer, dtPolyFlags[] pointFlags);

    [DllImport("ReUth.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern dtStatus FindRandomPointAroundCircle(IntPtr queryPtr, float[] center, float radius, float[] polyPickExt, dtPolyFlags[] queryFilter, float[] outputVector);

    [DllImport("ReUth.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern dtStatus FindClosestPoint(IntPtr queryPtr, float[] center, float[] polyPickExt, dtPolyFlags[] queryFilter, float[] outputVector);

    [DllImport("ReUth.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern dtStatus GetPolyAt(IntPtr queryPtr, float[] center, float[] polyPickExt, dtPolyFlags[] queryFilter, ref uint polyRef, float[] outPoint);

    [DllImport("ReUth.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern dtStatus SetPolyFlags(IntPtr meshPtr, uint polyRef, dtPolyFlags flags);

    [DllImport("ReUth.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern dtStatus QueryPolygons(IntPtr queryPtr, float[] center, float[] polyPickExt, dtPolyFlags[] queryFilter, uint[] polyIds, ref int polyCount, int maxPolys);


    /// <summary>
    /// Tries to initialize Recast
    /// </summary>
    /// <returns></returns>
    public static bool InitRecast() {
      try {
        IntPtr dummy = IntPtr.Zero;
        LoadNavMesh("this file does not exists!", ref dummy, ref dummy);
      }
      catch (Exception e) {
        Log.Error("The current process is a {0} bit process!", (IntPtr.Size == 8 ? "64bit" : "32bit"));
        Log.Error("PathingMgr did not find the ReUth.dll. Pathing will not work! Error message: {0}", e.Message);
        return false;
      }

      return true;
    }

    /// <summary>
    /// Gets or loads a recast mesh
    /// </summary>
    private static RecastMesh LoadZoneMesh(Zone2 zone) {
      lock (_loadedZones) {
        if (_loadedZones.ContainsKey(zone.ID))
          return _loadedZones[zone.ID];

        RecastMesh mesh = null;
        try {
          string file = zone.NavFile;
          if (!File.Exists(file))
            return null; // no navmesh available

          file = Path.GetFullPath(file); // not sure if c dll can load relative stuff
          IntPtr meshPtr = IntPtr.Zero;
          IntPtr queryPtr = IntPtr.Zero;

          if (!LoadNavMesh(file, ref meshPtr, ref queryPtr))
          {
            Log.Error("Loading NavMesh failed for {0}!", zone);
            return null;
          }

          if (meshPtr == IntPtr.Zero || queryPtr == IntPtr.Zero)
          {
            Log.Error("Loading NavMesh failed for {0}! (Pointer was zero!)", zone);
            return null;
          }
          Log.Normal("Loading NavMesh sucessful for region {0}", zone);
          return mesh = new RecastMesh(meshPtr, queryPtr);
        }
        finally {
          _loadedZones.Add(zone.ID, mesh);
        }
      }
    }

    /// <summary>
    /// Clears all loaded navmeshes
    /// </summary>
    public static void Dispose() {
      foreach (var mesh in _loadedZones.Values.Where(x => x != null)) {
        FreeNavMesh(mesh.meshPtr, mesh.queryPtr);
      }
      _loadedZones.Clear();
    }

    /// <summary>
    /// Unloads a zone
    /// </summary>
    /// <param name="z"></param>
    public static void Unload(Zone2 z) {
      lock (_loadedZones)
      {
        if (_loadedZones.ContainsKey(z.ID))
        {
          RecastMesh m = _loadedZones[z.ID];
          if (m != null)
            FreeNavMesh(m.meshPtr, m.queryPtr);
          _loadedZones.Remove(z.ID);
        }
      }
    }

    /// <summary>
    /// Returns a path that prevents collisions with the navmesh, but floats freely otherwise
    /// </summary>
    /// <param name="zone"></param>
    /// <param name="start">Start in GlobalXYZ</param>
    /// <param name="end">End in GlobalXYZ</param>
    /// <returns></returns>
    public static PathPoint[] GetPathStraight(Zone2 zone, Vector3 start, Vector3 end, dtPolyFlags includeFilter = dtPolyFlags.ALL ^ dtPolyFlags.DISABLED, dtPolyFlags excludeFilter = 0, float polyExtX = 64.0f, float polyExtY = 64.0f, float polyExtZ = 256.0f, dtStraightPathOptions options = dtStraightPathOptions.DT_STRAIGHTPATH_ALL_CROSSINGS) {

      if (!_loadedZones.ContainsKey(zone.ID))
        return null;

      var ptrs = _loadedZones[zone.ID];
      var startFloats = (start + zAxis * 8).ToRecastFloats();
      var endFloats = (end + zAxis * 8).ToRecastFloats();

      var numNodes = 0;
      var buffer = new float[MAX_POLY * 3];
      var flags = new dtPolyFlags[MAX_POLY];
      var filter = new[] { includeFilter, excludeFilter };

      PathStraight(ptrs.queryPtr, startFloats, endFloats, new Vector3(polyExtX, polyExtY, polyExtZ).ToRecastFloats(), filter, options, ref numNodes, buffer, flags);

      var points = new PathPoint[numNodes];
      var positions = Vector3ArrayFromRecastFloats(buffer, numNodes);

      for (var i = 0; i < numNodes; i++) {
        points[i].Position = positions[i];
        points[i].Flags = flags[i];
      }

      return points;
    }

    /// <summary>
    /// Returns a random point on the navmesh around the given position
    /// </summary>
    /// <param name="zone">Zone</param>
    /// <param name="position">Start in GlobalXYZ</param>
    /// <param name="radius">End in GlobalXYZ</param>
    /// <returns></returns>
    public static Vector3 GetRandomPoint(Zone2 zone, Vector3 position, float radius) {
      if (!_loadedZones.ContainsKey(zone.ID))
        return Vector3.Zero;

      var ptrs = _loadedZones[zone.ID];
      var center = (position + zAxis * 8).ToRecastFloats();
      var cradius = (radius * CONVERSION_FACTOR);
      var outVec = new float[3];

      var defaultInclude = (dtPolyFlags.ALL ^ dtPolyFlags.DISABLED);
      var defaultExclude = (dtPolyFlags)0;
      var filter = new dtPolyFlags[] { defaultInclude, defaultExclude };

      var polyPickEx = new float[3] { 2.0f, 4.0f, 2.0f };

      FindRandomPointAroundCircle(ptrs.queryPtr, center, cradius, polyPickEx, filter, outVec);

      return new Vector3(outVec[0] * INV_FACTOR, outVec[2] * INV_FACTOR, outVec[1] * INV_FACTOR);
    }

    /// <summary>
    /// Returns the closest point on the navmesh (UNTESTED! EXPERIMENTAL! WILL GO SUPERNOVA ON USE! MAYBE!?)
    /// </summary>
    public static Vector3? GetClosestPoint(Zone2 zone, Vector3 position, float xRange = 256f, float yRange = 256f, float zRange = 256f) {
      if (!_loadedZones.ContainsKey(zone.ID))
        return position;

      var ptrs = _loadedZones[zone.ID];
      var center = (position + zAxis * 8).ToRecastFloats();
      var outVec = new float[3];

      var defaultInclude = (dtPolyFlags.ALL ^ dtPolyFlags.DISABLED);
      var defaultExclude = (dtPolyFlags)0;
      var filter = new dtPolyFlags[] { defaultInclude, defaultExclude };

      var polyPickEx = new Vector3(xRange, yRange, zRange).ToRecastFloats();

      FindClosestPoint(ptrs.queryPtr, center, polyPickEx, filter, outVec);

      var result = new Vector3(outVec[0] * INV_FACTOR, outVec[2] * INV_FACTOR, outVec[1] * INV_FACTOR);
      return result == Vector3.Zero ? null : (Vector3?)result;
    }

    public static bool GetPolyAt(Zone2 zone, Vector3 center, Vector3 extents, dtPolyFlags includeFilter, dtPolyFlags excludeFilter, ref uint polyRef, ref Vector3 point) {
      if (!_loadedZones.ContainsKey(zone.ID))
        return false;

      float[] outPoint = new float[3];

      var ptrs = _loadedZones[zone.ID];
      var status = GetPolyAt(ptrs.queryPtr, center.ToRecastFloats(), extents.ToRecastFloats(), new dtPolyFlags[] { includeFilter, excludeFilter }, ref polyRef, outPoint);
      point = new Vector3(outPoint[0] * INV_FACTOR, outPoint[2] * INV_FACTOR, outPoint[1] * INV_FACTOR);
      if ((status & dtStatus.DT_FAILURE) == dtStatus.DT_FAILURE) {
        return false;
      }
      return true;
    }

    public static bool SetPolyFlags(Zone2 zone, uint polyRef, dtPolyFlags flags) {
      if (!_loadedZones.ContainsKey(zone.ID))
        return false;

      var ptrs = _loadedZones[zone.ID];
      var status = SetPolyFlags(ptrs.meshPtr, polyRef, flags);
      if ((status & dtStatus.DT_FAILURE) == dtStatus.DT_FAILURE) {
        return false;
      }
      return true;
    }

    public static bool QueryPolygons(Zone2 zone, Vector3 center, Vector3 polyPickExt, dtPolyFlags includeFlags, dtPolyFlags excludeFlags, ref uint[] results, int maxResults = 32) {
      if (!_loadedZones.ContainsKey(zone.ID))
        return false;

      var resultIdBuffer = new uint[maxResults];
      var resultCount = 0;

      var ptrs = _loadedZones[zone.ID];
      var status = QueryPolygons(ptrs.queryPtr, center.ToRecastFloats(), polyPickExt.ToRecastFloats(), new dtPolyFlags[] { includeFlags, excludeFlags }, resultIdBuffer, ref resultCount, maxResults);
      if ((status & dtStatus.DT_FAILURE) == dtStatus.DT_FAILURE) {
        return false;
      }

      results = new uint[resultCount];
      Array.Copy(resultIdBuffer, results, resultCount);

      return true;
    }

    private static float[] ToRecastFloats(this Vector3 value) {
      return new[] { (float)(value.X * CONVERSION_FACTOR), (float)(value.Z * CONVERSION_FACTOR), (float)(value.Y * CONVERSION_FACTOR) };
    }

    private static Vector3[] Vector3ArrayFromRecastFloats(float[] buffer, int numNodes) {
      numNodes = Math.Min(numNodes, buffer.Length/3);
      var result = new Vector3[numNodes];
      for (int i = 0; i < numNodes; i++) {
        result[i] = new Vector3(buffer[i*3 + 0]*INV_FACTOR, buffer[i*3 + 2]*INV_FACTOR, buffer[i*3 + 1]*INV_FACTOR);
      }
      return result;
    }

    /// <summary>
    /// True if pathing is enabled for the specified region
    /// </summary>
    public static bool IsPathingEnabled(Zone2 zone) {
      return _loadedZones.ContainsKey(zone.ID);
    }

    /// <summary>
    /// Builds the navmesh for the specified zone
    /// </summary>
    public static void BuildNavMesh(Zone2 z) {
      if (z.Name == "ArtOutside" || z.Name == "ArtInside") {
        Log.Normal("Skipping zone {0} because it has name {1}", z, z.Name);
        return;
      }
      if (z.ProxyZone != 0) {
        Log.Normal("Skipping zone {0} because it has a proxy zone id {1}", z, z.ProxyZone);
        return;
      }

      string obj = z.ObjFile;
      string nav = z.NavFile.Replace(".gz", "");

      for (int i = 0; i <= 1; i++)
      {
        // Create .obj
        if (i == 1)
          LoadZoneMesh(z);
        else
          Unload(z);
        DateTime start = DateTime.Now;
        Log.Normal("Building navmesh for zone {0} (pass={1})...", z, i);
        if (File.Exists(obj))
          File.Delete(obj);
        using (var exp = new Zone2Obj(z))
          exp.Export();
        Unload(z);

        if (Program.Arguments.ExportObjOnly)
          return;

        // .obj -> .nav
        //if (i == 0)
        {
          Log.Normal("Running buildnav.exe for {0}", z.Name);
          Process buildnav = Process.Start("buildnav.exe", Util.MakeProcessArguments(new[] { obj.Replace(".obj", ".geomset"), nav }));
          buildnav.PriorityClass = ProcessPriorityClass.BelowNormal;
          buildnav.WaitForExit();
          if (buildnav.ExitCode > 0)
            throw new InvalidOperationException("buildnav.exe failed with " + buildnav.ExitCode);
          if (!File.Exists(nav)) {
            Log.Error("Did not generate navmesh for file {0} for unknown reasons", nav);
          } else if (new FileInfo(nav).Length < 2048)
          {
            // empty mesh
            Log.Warn("{0} was empty :(", nav);
            File.Delete(nav);
          }
        }

        Log.Normal("Zone {0} finished in {1}", z, DateTime.Now - start);
        lock (_loadedZones)
        {
          if (_loadedZones.ContainsKey(z.ID))
            _loadedZones.Remove(z.ID);
        }
      }
    }

    /// <summary>
    /// Recast Region Mesh
    /// </summary>
    private class RecastMesh {
      public readonly IntPtr meshPtr;
      public readonly IntPtr queryPtr;

      public RecastMesh(IntPtr meshPtr, IntPtr queryPtr) {
        this.meshPtr = meshPtr;
        this.queryPtr = queryPtr;
      }
    }
  }
}