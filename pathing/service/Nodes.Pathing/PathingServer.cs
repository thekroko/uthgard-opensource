using System.Diagnostics;
using System.Threading;
using Grpc.Core;
using Nodes.Pathing.Pathing;
using Nodes.Rpc;

namespace Nodes.Pathing {
  internal class PathingServer : BaseServer {
    private static void Main(string[] args) {
      //Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.Normal;
      RpcServerTask.RpcServerPort = 20023;
      RpcServerTask.RpcMaxConcurrent = 1000;
      GrpcEnvironment.SetThreadPoolSize(50);
      GrpcEnvironment.SetCompletionQueueCount(50);
      ThreadPool.SetMinThreads(50, 50);
      new PathingServer().Run();
    }

    protected override void Configure() {
      Install(new BindingModule());
      Install(new PathingServiceModule());
    }

    private class BindingModule : AbstractModule {
      protected override void ConfigureModules() {}
      protected override void ConfigureBindings() {
        Bind<ServerCredentials>().ToConstant(ServerCredentials.Insecure);
      }
    }
  }
}