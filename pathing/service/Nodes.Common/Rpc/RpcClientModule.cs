using System;
using System.Linq;
using System.Reflection;
using CuttingEdge.Conditions;
using Ninject;
using Ninject.Activation;

namespace Nodes.Rpc
{
  /// <summary>
  /// Registers an RPC client and automatically connects it on-demand
  /// </summary>
  public abstract class RpcClientModule : AbstractModule {
    protected override void ConfigureModules()
    {
    }

    protected override void ConfigureBindings()
    {
      ConfigureClients();
    }

    /// <summary>
    /// Configures services by calling the BindClient() method
    /// </summary>
    protected abstract void ConfigureClients();

    protected void BindClient<TClientInterface>(Type serviceClass, string route)
    {
      Condition.Requires(route, "route").IsNotNull();
      Bind<TClientInterface>()
        .ToProvider<TClientInterface>(new ClientProvider<TClientInterface>(Logger, serviceClass, route))
        .InSingletonScope();

      Bind<IServiceChannelResolver>().To<TcpServiceChannelResolver>();
    }

    internal sealed class ClientProvider<TClientInterface> : Provider<TClientInterface> {
      private readonly ILogger logger;
      private readonly Type serviceClass;
      private readonly string route;

      public ClientProvider(ILogger logger, Type serviceClass, string route)
      {
        this.logger = Condition.Requires(logger, "logger").IsNotNull().Value;
        this.serviceClass = Condition.Requires(serviceClass, "serviceClass").IsNotNull().Value;
        this.route = Condition.Requires(route, "route").IsNotNullOrEmpty().Value;
      } 

      protected override TClientInterface CreateInstance(IContext ctx) {
        logger.Information("Connecting RPC client {type} to {route}", typeof(TClientInterface), route);

        // Find the proper resolver
        var resolvers = ctx.Kernel.GetAll<IServiceChannelResolver>();

        // Resolve service
        var channel = resolvers.Select(r => r.ResolveChannelForService(route)).FirstOrDefault(c => c != null);
        Condition.Requires(channel, route).IsNotNull("No resolver was found for the specified route");
        var client = (TClientInterface)serviceClass.GetMethod("NewClient", BindingFlags.Public | BindingFlags.Static)
          .Invoke(null, new object[] { channel });
        return client;
      }
    }
  }
}
