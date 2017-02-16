using System;
using System.Linq;
using System.Reflection;
using CuttingEdge.Conditions;
using Grpc.Core;
using Ninject;

namespace Nodes.Rpc {
  /// <summary>
  /// Binds an RPC service
  /// </summary>
  public abstract class RpcServiceModule : AbstractModule {
    protected override void ConfigureModules() {
      Install(new RpcServerTask());
    }

    protected override void ConfigureBindings() {
      ConfigureServices();
    }

    /// <summary>
    ///   Configures services by calling the BindService() method
    /// </summary>
    protected abstract void ConfigureServices();

    /// <summary>
    ///   Binds the specified service to the specified implementation
    /// </summary>
    /// <param name="serviceType">Static gRPC Service Class</param>
    protected ServiceBinder BindService(Type serviceType) {
      Condition.Requires(serviceType, "serviceType").IsNotNull();
      return new ServiceBinder {
        ServiceClass = serviceType,
        BindAction = (service, impl) => {
          var serviceName =
            (string) serviceType.GetField("__ServiceName", BindingFlags.NonPublic | BindingFlags.Static)
              .GetValue(null);
          Condition.Requires(serviceName, "serviceName").IsNotNullOrWhiteSpace();

          Bind<ServerServiceDefinition>()
            .ToMethod(
              ctx =>
                (ServerServiceDefinition)
                  serviceType.GetMethods(BindingFlags.Static | BindingFlags.Public).Last(m => m.Name == "BindService")
                    .Invoke(null, new[] {ctx.Kernel.Get(impl)})).InSingletonScope();
          Bind<string>()
            .ToConstant(serviceName)
            .WhenTargetHas(typeof (BoundServiceAttribute))
            .InSingletonScope();
        }
      };
    }

    protected sealed class ServiceBinder {
      internal ServiceBinder() {
      }

      internal Type ServiceClass { get; set; }
      internal Action<Type, Type> BindAction { get; set; }

      /// <summary>
      ///   Binds the service to the specified implementation
      /// </summary>
      /// <typeparam name="TImpl"></typeparam>
      public void To<TImpl>() {
        BindAction(ServiceClass, typeof (TImpl));
      }
    }
  }
}