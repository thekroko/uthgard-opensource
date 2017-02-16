using DOL.Numbers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DOL.GS.Sharding
{
  /// <summary>
  /// Queue for sending combined TCP/UDP updates. Used for RegionServer integration
  /// </summary>
  public class DispatchingQueue {
    private static readonly Pool<DispatchingQueue> QueuePool = new Pool<DispatchingQueue>(5000, () => new DispatchingQueue());
    private readonly List<GameClient> forUdpBroadcast = new List<GameClient>();
    private Action<GameClient> forTcp;
    private bool shouldDispatch;

    /// <summary>
    /// Aquires a broadcast Queue
    /// </summary>
    public static DispatchingQueue Acquire(Action<GameClient> tcpAction, double dispatchChance = 1) {
      var q = QueuePool.Pop();
      q.forTcp = tcpAction;
      q.shouldDispatch = GameServer.DispatchEnable && Rand.ChanceDouble(dispatchChance);
      return q;
    }

    /// <summary>
    /// Add client to Queue
    /// </summary>
    public void Add(GameClient client, bool forceLegacyBroadcast = false) {
      if (!shouldDispatch || forceLegacyBroadcast || !client.UsesRegionServer) {
        forTcp(client); // individual, single updates
        return;
      }
      forUdpBroadcast.Add(client);
    }

    /// <summary>
    /// Process all UDP broadcasts
    /// </summary>
    /// <param name="buildUdpPacket"></param>
    public void FinishUdpBroadcast(Func<Tuple<byte[],int>> buildUdpPacket) {
      if (forUdpBroadcast.Count == 0)
        return;
      var pak = buildUdpPacket();
      // TODO(mlinder): Simplify ths code once we only have one region server per region
      var regionServer = forUdpBroadcast.First(x => x.RegionServer != null).RegionServer;
      regionServer.BroadcastPacket(pak.Item1, pak.Item2, forUdpBroadcast);

      // Return Queue
      forTcp = null;
      forUdpBroadcast.Clear();
      QueuePool.Push(this);
    }
  }
}
