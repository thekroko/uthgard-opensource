using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Timers;
using CuttingEdge.Conditions;
using DOL.GS.PacketHandler.Cryptography;
using Google.Protobuf;
using Grpc.Core;
using log4net;
using RegionServer;
using Timer = System.Timers.Timer;

namespace DOL.GS.Sharding
{
  /// <summary>
  /// Region Server implemented by a RPC Node
  /// </summary>
  public class NodeRegionServer : IRegionServer {
    private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    public readonly RegionService.RegionServiceClient rpc;
    public readonly TcpRelaySocket tcpRelaySocket;
    public readonly Channel channel;
    public ServerInfoResponse serverInfo;

    private bool isReachable = false;

    public string IpStringForClient => serverInfo.PublicIp4;
    public ushort UdpPortForClient => (ushort)serverInfo.UdpPort;
    public bool IsAvailable => isReachable && tcpRelaySocket.IsReachable && IsEnabled;
    public bool IsEnabled { get; set; } = true;

    private Timer PingTimer;

    private NodeRegionServer(Channel chan, RegionService.RegionServiceClient clt, TcpRelaySocket socket) {
      channel = chan;
      rpc = clt;
      tcpRelaySocket = socket;
    }

    public static async Task<NodeRegionServer> Connect(string host, ushort rpcPort, ushort tcpPort) {
      // RPC
      var options = new List<ChannelOption>();
      options.Add(new ChannelOption(ChannelOptions.MaxMessageLength, 65536 /* needs to match server */));
      Channel channel = new Channel(host, rpcPort, ChannelCredentials.Insecure, options);
      await channel.ConnectAsync(DateTime.UtcNow.AddSeconds(20));

      // TCP
      TcpRelaySocket sock = new TcpRelaySocket(host, tcpPort);
      
      var server = new NodeRegionServer(channel, new RegionService.RegionServiceClient(channel), sock);
      await server.Init();
      return server;
    }

    protected async Task Init() {
      // Get Info
      serverInfo = await rpc.ServerInfoAsync(new ServerInfoRequest());

      // TCP relay
      tcpRelaySocket.OnPacketReceived += (buf, len) => {
        OnPacketReceived?.Invoke(buf, len);
      };
      tcpRelaySocket.Init();

      // Periodically restart listeners & reconnect
      PingTimer = new Timer();
      PingTimer.Interval = 15000;
      PingTimer.Elapsed += PingServer;
      PingTimer.Start();

      // Sync clients periodically
      Task.Run(SyncTask);
    }

    public void Stop()
    {
      PingTimer.Stop();
      tcpRelaySocket.Close();
    }

    private void PingServer(object sender, ElapsedEventArgs e) {
      // Check server reachable
      try {
        bool wasReachable = IsAvailable;
        serverInfo = rpc.ServerInfo(new ServerInfoRequest(), new CallOptions(deadline: DateTime.UtcNow.AddSeconds(10)));
        isReachable = true;

        // Start listening stream if it failed
        if (!wasReachable && IsAvailable) {
          Log.Info("Region server reinitialized after being unreachable");
        }
      }
      catch (Exception ex) {
        isReachable = false;
        Log.Error("Ping RegionServer failed", ex);
      }
    }

    private async Task SyncTask()
    {
      while (true)
      {
        await Task.Delay(60000).ConfigureAwait(false);
        if (!IsAvailable) continue;

        try { await PerformSync(); } 
        catch (Exception ex) {
          Log.Error("Sync RegionServer failed", ex);
        }
      }
    }

    private async Task PerformSync()
    {
      int changes = 0;
      using (var call = rpc.ListClients(new ListClientsRequest()))
      {
        while (await call.ResponseStream.MoveNext())
        {
          var clt = call.ResponseStream.Current;
          var gsClient = WorldMgr.GetClientBySessionId((ushort)clt.SessionId);

          // =============== Below this point we are not sure if the RS client is valid  =============
          if (clt.IsKnown && (gsClient == null || clt.UniqueId != gsClient.RegionServerUniqueId || gsClient.RegionServer != this))
          {
            // RS should not know about this client as it doesn't exist or is the wrong one
            await rpc.RemoveClientAsync(new RemoveClientRequest() { SessionId = clt.SessionId });
            clt.IsKnown = false;
            changes++;
          }

          // =============== Below this point we assume that there is a valid gs client =============
          if (gsClient == null)
              continue; // Client wasn't supposed to exist
	  if (gsClient.RegionServer != this) 
              continue; // We're not responsible for this client, so don't interferere
          if (clt.IsKnown && !string.IsNullOrEmpty(clt.Endpoint) && clt.Endpoint != gsClient.RegionServerUdpEndpoint)
          {
            // RS knows, and GS should know about this valid client as well
            gsClient.RegionServerUdpEndpoint = clt.Endpoint;
            changes++;
          }
          if (!clt.IsKnown)
          {
            // RS should know about this client
            await AddClientToRegion(gsClient);
            changes++;
          }
	  bool newUdpState = clt.IsKnown && clt.Endpoint.Length > 0;
	  changes += (newUdpState != gsClient.RegionServerUdpActivated) ? 1 : 0;
	  gsClient.RegionServerUdpActivated = newUdpState;
        }
      }
      if (changes > 0) {
        Log.InfoFormat("{1} sycned {0} changes", changes, this);
      }
    }

    public async Task AddClientToRegion(GameClient client) {
      var enc = client.PacketProcessor.Encoding;
      var encryptionKey = (byte[])enc.GetType().GetProperty("CommonKey",BindingFlags.Public|BindingFlags.Instance).GetValue(enc);
      // var encryptionKey = ((PacketEncoding1110) client.PacketProcessor.Encoding).CommonKey;
      var req = new AddClientRequest() {
          SessionId = client.ToSessionId(),
          EncryptionKey = ByteString.CopyFrom(encryptionKey),
          Endpoint = client.RegionServerUdpEndpoint ?? "",
          UniqueId = client.RegionServerUniqueId,
      };
      try {
          await rpc.AddClientAsync(req, new CallOptions(deadline: DateTime.UtcNow.AddSeconds(10))).ResponseAsync.ConfigureAwait(false);
      }
      catch (Exception ex) {
        Log.Error($"Adding Client req=[{req}] to RegionServer failed", ex);
      }
    }

    public async Task RemoveClientFromRegion(GameClient client) {
      try {
        client.RegionServerUdpActivated = false;
        await rpc.RemoveClientAsync(new RemoveClientRequest() {
          SessionId = client.ToSessionId(),
          // TODO(mlinder): Should include unique id here
        }, new CallOptions(deadline: DateTime.UtcNow.AddSeconds(10))).ResponseAsync.ConfigureAwait(false);
      }
      catch (Exception ex) {
        Log.Error("Removing Client from RegionServer failed", ex);
      }
    }

    public Task BroadcastPacket(byte[] packet, int len, IEnumerable<GameClient> toClients) {
      if (!IsAvailable) {
        return Task.CompletedTask; // discard due to unavailable server
      }

      ushort[] sessionIds = toClients.Select(x => (ushort)x.ToSessionId()).ToArray();
      tcpRelaySocket.SendAction.Post(Tuple.Create(sessionIds, packet, 0, len));
      return Task.CompletedTask;
    }

    public event Action<byte[],int> OnPacketReceived;

    public override string ToString() {
      return $"NodeRegionServer[IP={IpStringForClient},UDP={UdpPortForClient}]";
    }
  }

  internal static class Extensions
  {
    public static int ToSessionId(this GameClient clt) {
      Condition.Requires(clt.SessionId != 0, "sessionId");
      return clt.SessionId;
    }

    public static GameClient ToGameClient(this int sessionId) {
      var gameClient = WorldMgr.GetClientBySessionId((ushort)sessionId);
      Condition.Requires(gameClient != null, "gameClient");
      return gameClient;
    }
  }
}
