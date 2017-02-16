using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Ninject;
using Nodes.Tasks;

namespace Nodes.Rpc
{
  /// <summary>
  /// Configures an RPC server with all Services that have been bound
  /// </summary>
  public sealed class RpcServerTask : AbstractTaskModule {
    private bool isHealthy;

    [Flag("--rpc_server_port", "Port for the RPC server (-1 for random)")]
    public static int RpcServerPort = -1;

    [Flag("--rpc_server_max_concurrent", "Maximum concurrent connections")]
    public static int RpcMaxConcurrent = -1;

    [Inject] [BoundService] string[] BoundServices { get; set; }
    [Inject] ServerCredentials Credentials { get; set; }

    protected override void ConfigureModules() {
    }

    public override async Task Run() {
      var options = new List<ChannelOption>();
      if (RpcMaxConcurrent > 0) {
        options.Add(new ChannelOption(ChannelOptions.MaxConcurrentStreams, RpcMaxConcurrent));
      }

      // Create a server
      int port = RpcServerPort > 0 ? RpcServerPort : ServerPort.PickUnused;
      var server = new Server(options) {
        Ports = { new ServerPort("0.0.0.0", port, Credentials) },
      };
      foreach (var service in Kernel.GetAll<ServerServiceDefinition>()) {
        server.Services.Add(service);
      }
      int count = server.Services.Count();
      var ports = server.Ports.Select(x => x.BoundPort);
      Logger.Information("Registered {count} RPC Services on port {ports}: {serviceNames}", count, ports, BoundServices);

      // Start it.
      server.Start();
      isHealthy = true;
      await server.ShutdownTask;
    }

    public override bool IsHealthy { get { return isHealthy; } }
  }
}
