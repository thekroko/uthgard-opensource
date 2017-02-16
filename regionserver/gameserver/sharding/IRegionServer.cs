using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DOL.GS.Sharding
{
  /// <summary>
  /// Defines an optional per-region server
  /// </summary>
  public interface IRegionServer
  {
    /// <summary>
    /// IP String to send to the client for this region server
    /// </summary>
    string IpStringForClient { get; }

    /// <summary>
    /// UDP port to send to the client for this region server
    /// </summary>
    ushort UdpPortForClient { get; }
    
    /// <summary>
    /// True if currently up and running
    /// </summary>
    bool IsAvailable { get; }

    /// <summary>
    /// True if enabled
    /// </summary>
    bool IsEnabled { get; set; }

    /// <summary>
    /// Add client to region server
    /// </summary>
    /// <param name="client"></param>
    /// <returns></returns>
    Task AddClientToRegion(GameClient client);

    /// <summary>
    /// Remove client from region server
    /// </summary>
    /// <param name="client"></param>
    /// <returns></returns>
    Task RemoveClientFromRegion(GameClient client);

    /// <summary>
    /// Broadcast a packet to clients on that region server
    /// </summary>
    /// <param name="packet"></param>
    /// <param name="toClients"></param>
    /// <returns></returns>
    Task BroadcastPacket(byte[] packet, int len, IEnumerable<GameClient> toClients);

    /// <summary>
    /// Streams packets from the RegionServer back to the GameServer
    /// </summary>
    event Action<byte[],int> OnPacketReceived;
  }
}
