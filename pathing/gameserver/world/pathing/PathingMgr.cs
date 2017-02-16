using System.Reflection;
using JetBrains.Annotations;
using log4net;

namespace DOL.GS
{
  /// <summary>
  /// Wrapper for the currently active pathing mgr
  /// </summary>
  public static class PathingMgr {
    private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    /// <summary>
    /// PathingMgr that does nothing
    /// </summary>
    public static readonly IPathingMgr NullPathingMgr = new NullPathingMgr();

    /// <summary>
    /// Calculates data locally and performs calls here
    /// </summary>
    public static readonly LocalPathingMgr LocalPathingMgr = new LocalPathingMgr();

    /// <summary>
    /// Calculates paths via a different RPC server
    /// </summary>
    public static readonly RpcPathingMgr RpcPathingMgr = new RpcPathingMgr();
		
		/// <summary>
    /// Calculates paths via a different RPC server
    /// </summary>
    public static readonly ShardedRpcPathingMgr ShardedRpcPathingMgr = new ShardedRpcPathingMgr();


    public static bool Init() {
      log.Info("Starting PathingMgr");
      if (RpcPathingMgr.Init()) SetPathingMgr(RpcPathingMgr);
      //else if (ShardedRpcPathingMgr.Init()) SetPathingMgr(ShardedRpcPathingMgr);
      else if (LocalPathingMgr.Init()) SetPathingMgr(LocalPathingMgr);
      else SetPathingMgr(NullPathingMgr);
			return true;
    }

    /// <summary>
    /// Changes the active pathing mgr
    /// </summary>
    /// <param name="mgr"></param>
    public static void SetPathingMgr(IPathingMgr mgr) {
      log.Info($"Setting PathingMgr to {mgr}");
      Instance = (mgr == null) ? NullPathingMgr : new FailureTolerantPathingMgr(mgr);
    }

    public static void Stop() {
      log.Info("Stopping PathingMgr");
      LocalPathingMgr.Stop();
      RpcPathingMgr.Stop();
      ShardedRpcPathingMgr.Stop();
    }

    /// <summary>
    /// Currently used instance
    /// </summary>
    [NotNull]
    public static IPathingMgr Instance { get; private set; } = NullPathingMgr;
  }
}
