using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Ninject;
using Nodes.Tasks;

namespace Nodes.Pathing.Pathing
{
  /// <summary>
  /// Loads all navmeshes
  /// </summary>
  class NavmeshLoadingModule : AbstractTaskModule
  {
    [Inject] private ILogger Logger { get; set; }
    [Inject] private LoadedNavmeshes Navmeshes { get; set; }

		[Flag("--navmesh_shard_index", "Shard Index/Offset")]
		public static int ShardIndex = 0;
		
		[Flag("--navmesh_shard_count", "Shard Count")]
		public static int ShardCount = 1;

    private bool healthy;
    private Random navmeshInitRandom = new Random();

    public override bool IsHealthy => healthy;

    protected override void ConfigureModules() {
    }

    public override Task Run() {
      try
      {
        var dummy = IntPtr.Zero;
        LoadNavMesh("this file does not exists!", ref dummy, ref dummy);
        Logger.Information("Initialized Recast!");
      }
      catch (Exception e)
      {
        Logger.Error("The current process is a {0} bit process!", (IntPtr.Size == 8 ? "64bit" : "32bit"));
        Logger.Error("PathingMgr did not find the ReUth.dll! Starting server anyway but pathing will not work! Error message: {0}", e.ToString());
      }

      for (ushort i = 0; i < 500; i++) {
				if (ShouldLoadNavmesh(i)) {
					LoadNavMesh(i);
				}
			}
      Logger.Information($"Loaded {Navmeshes.Count} navmeshes!");
      Navmeshes.IsLoadingComplete = true;
      healthy = true;
      return Task.CompletedTask;
    }

		private bool ShouldLoadNavmesh(ushort id) {
			return (id + ShardIndex) % ShardCount == 0;
		}

    [DllImport("ReUth", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern bool LoadNavMesh(string file, ref IntPtr meshPtr, ref IntPtr queryPtr);

    /// <summary>
    /// Loads the navmesh for the specified zone (if available)
    /// </summary>
    /// <param name="zone"></param>
    private void LoadNavMesh(ushort id)
    {
      var file = string.Format("pathing{1}zone{0:D3}.nav", id, Path.DirectorySeparatorChar);
      file = Path.GetFullPath(file); // not sure if c dll can load relative stuff
      if (!File.Exists(file)) {
        return;
      }

      var meshPtr = IntPtr.Zero;
      var queryPtr = IntPtr.Zero;

      if (!LoadNavMesh(file, ref meshPtr, ref queryPtr))
      {
        Logger.Error("Loading NavMesh failed for zone {0}!", id);
        return;
      }

      if (meshPtr == IntPtr.Zero || queryPtr == IntPtr.Zero)
      {
        Logger.Error("Loading NavMesh failed for zone {0}! (Pointer was zero!)", id);
        return;
      }
      Logger.Information("Loading NavMesh sucessful for zone {0}", id);
      Navmeshes[id] = new Navmesh(meshPtr, queryPtr, new Random(navmeshInitRandom.Next()));
    }
  }
}
