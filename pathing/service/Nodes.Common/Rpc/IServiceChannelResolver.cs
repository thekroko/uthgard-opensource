using Grpc.Core;

namespace Nodes.Rpc
{
  /// <summary>
  /// Resolves a service destination
  /// </summary>
  public interface IServiceChannelResolver {
    /// <summary>
    /// Resolves the channel for the implementation-specific service name
    /// </summary>
    /// <param name="servicePath"></param>
    /// <returns>Channel, or null if this resolver can't handle the specified service path</returns>
    Channel ResolveChannelForService(string servicePath);
  }
}
