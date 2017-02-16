using System;
using Grpc.Core;

namespace Nodes.Rpc
{
  /// <summary>
  /// Service resolver for tcp://route:port routes
  /// </summary>
  internal class TcpServiceChannelResolver : IServiceChannelResolver
  {
    public Channel ResolveChannelForService(string servicePath) {
      Uri uri;
      if (!Uri.TryCreate(servicePath, UriKind.Absolute, out uri)) {
        return null;
      }
      return new Channel(uri.Host, uri.Port, ChannelCredentials.Insecure);
    }
  }
}
