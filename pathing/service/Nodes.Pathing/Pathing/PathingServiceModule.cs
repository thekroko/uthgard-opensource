using Nodes.Rpc;

namespace Nodes.Pathing.Pathing
{
  /// <summary>
  /// AccountServiceModule
  /// </summary>
  public class PathingServiceModule : RpcServiceModule
  {
    protected override void ConfigureServices() {
      Bind<LoadedNavmeshes>().ToSelf().InSingletonScope();
      Bind<RequestMetrics>().ToSelf().InSingletonScope();
      BindService(typeof(PathingService)).To<PathingServiceImpl>();
    }

    protected override void ConfigureModules() {
      base.ConfigureModules();
      Install(new NavmeshLoadingModule());
      Install(new GUIModule());
    }
  }
}
