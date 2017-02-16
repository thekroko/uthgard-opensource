using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using CEM.Utils;
using CEM.World;
using MNL;
using OpenTK;

namespace CEM.Client.ZoneExporter {
  /// <summary>
  ///   Ladders
  /// </summary>
  partial class Zone2Obj {
    private static readonly Regex LadderRegex = new Regex("^climb([0-9:])+");

    private void ExtractLadder(NiFile model, Matrix4 worldMatrix) {
      // take everything that is under a climbX node and use it for the hull...
      if (model == null) {
        return;
      }

      var climbVertices = new Dictionary<string /* climbXXX */, List<Vector3>>();

      // Find all trimeshs and tristrips that belong to climb nodes
      foreach (var obj in model.ObjectsByRef.Values) {
        var avNode = obj as NiAVObject;
        if (avNode == null) {
          continue;
        }

        var climbName = FindMatchRegex(avNode, new[] {LadderRegex});
        if (climbName == string.Empty) {
          // No ladder in node
          continue;
        }

        Vector3[] vertices = null;
        Triangle[] triangles = null;

        TryExtractTriShape(obj, worldMatrix, false, false, ref vertices, ref triangles);
        if (vertices == null) {
          TryExtractTriStrips(obj, worldMatrix, false, false, ref vertices, ref triangles);
        }

        if (vertices == null) {
          continue;
        }

        if (!climbVertices.ContainsKey(climbName)) {
          climbVertices.Add(climbName, new List<Vector3>());
        }
        climbVertices[climbName].AddRange(vertices);
      }

      // Compute each Ladder individually
      foreach (var climbNode in climbVertices.Keys) {
        var minZ = float.MaxValue;
        var maxZ = float.MinValue;

        var verts = climbVertices[climbNode];

        // Find min/max
        foreach (var vert in verts) {
          minZ = Math.Min(vert.Z, minZ);
          maxZ = Math.Max(vert.Z, maxZ);
        }

        // Divide points into two sets
        var minVecs = new List<Vector3>();
        var maxVecs = new List<Vector3>();

        foreach (var vert in verts) {
          if (vert.Z < minZ + (maxZ - minZ) / 2) {
            minVecs.Add(vert);
          } else {
            maxVecs.Add(vert);
          }
        }

        if (minVecs.Count == 0 || maxVecs.Count == 0) {
          throw new InvalidDataException("ladder seems invalid");
        }

        var minExt = new Vector3(128, 128, 128);
        var maxExt = new Vector3(128, 128, 128);

        var minPt = GetClosestNavmeshPoint(Average(minVecs), minExt.X, minExt.Y, minExt.Z);
        var maxPt = GetClosestNavmeshPoint(Average(maxVecs), maxExt.X, maxExt.Y, maxExt.Z);

        if (minPt == Vector3.Zero || maxPt == Vector3.Zero) {
          Log.Error("Could not fit ladder {0} at {2} {3} {4} to navmesh in {1}; ignoring", climbNode, Zone.Name, minVecs[0].X,
            minVecs[0].Y, minVecs[0].Z);
          return;
        }

        // Write the off-mesh connection from min to max that is the primary ladder. This connection has to be connected the navmesh.
        Log.Debug("Extracted Ladder: {0} with verts={1}", climbNode, verts.Count);
        
        // Have some visual markers that show us how well we can fit ladders to the navmesh
        if (FirstPass) {
          foreach (var endPoint in new[] {new[] {minPt, minExt}, new[] {maxPt, maxExt}}) {
            var pt = endPoint[0];
            var ext = endPoint[1];
            var area = (pt == minPt) ? GeomSetWriter.eAreas.Road : GeomSetWriter.eAreas.Grass;
            GeomSetWriter.WriteConvexVolume(4, pt.Z - ext.Z, pt.Z + ext.Z, area);
            GeomSetWriter.WriteConvexVolumeVertex(new Vector3(pt.X - ext.X, pt.Y - ext.Y, pt.Z - ext.Z));
            GeomSetWriter.WriteConvexVolumeVertex(new Vector3(pt.X + ext.X, pt.Y - ext.Y, pt.Z - ext.Z));
            GeomSetWriter.WriteConvexVolumeVertex(new Vector3(pt.X + ext.X, pt.Y + ext.Y, pt.Z - ext.Z));
            GeomSetWriter.WriteConvexVolumeVertex(new Vector3(pt.X - ext.X, pt.Y + ext.Y, pt.Z - ext.Z));
          }
        } else {
          GeomSetWriter.WriteOffMeshConnection(minPt, maxPt, true, GeomSetWriter.eAreas.Jump, GeomSetWriter.eFlags.Jump);
        }
      }
    }
  }
}