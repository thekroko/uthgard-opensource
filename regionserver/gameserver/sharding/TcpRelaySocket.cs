using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using log4net;
using Timer = System.Timers.Timer;

namespace DOL.GS.Sharding
{
  /// <summary>
  /// Relays server packets via a simple raw-byte tcp stream in both directions
  /// </summary>
  public class TcpRelaySocket {
    private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    private Socket socket;
    private bool connected;
    byte[] sendBuf = new byte[4096];

    public string Host { get; set; }
    public ushort Port { get; set; }
    public bool IsReachable => connected && socket?.Connected == true;
    public bool TryingToConnect { get; set; }
    public int QueueCount => SendAction.InputCount;

    // I/O
    public ActionBlock<Tuple<ushort[], byte[], int, int>> SendAction { get; }
    public event Action<byte[],int> OnPacketReceived;

    public TcpRelaySocket(string host, ushort port) {
      SendAction = new ActionBlock<Tuple<ushort[], byte[], int, int>>(
        x => Send(x.Item1, x.Item2, x.Item3, x.Item4), new ExecutionDataflowBlockOptions()
        {
          MaxDegreeOfParallelism = 1, // sendbuffer is member, consider if changing it
          SingleProducerConstrained = false // send will be called from multiple threads
        });
      Host = host;
      Port = port;
    }

    public void Init() {
      var connectTimer = new Timer();
      connectTimer.Interval = 10000;
      connectTimer.Elapsed += (o, e) => {
        if (!IsReachable && !TryingToConnect) {
          TryConnect();
        }
      };
      connectTimer.Start();

      TryConnect();
    }

    public void TryConnect() {
      try {
        Log.Info("Trying to connect to RegionServer TcpRelay ...");
        TryingToConnect = true;
        lock (this) {
          if (socket?.Connected == true) {
            try {
              socket.Dispose();
              socket = null;
            }
            catch (Exception ex) {
              Log.Error("Could not disconnect socket", ex);
            }
          }
          socket = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
          socket.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.IPv6Only, false);
          socket.Connect(Host, Port);
          connected = true;
          Log.Info("Connected to RegionServer TcpRelay!");

          // Start listeners ..
          StartListener(RecvThread);
        }
      }
      catch (Exception ex) {
        Log.Error("Could not connect to RegionServer TcpRelay", ex);
        try {
          // Clean up socket
          socket?.Dispose();
        }
        catch (Exception cex) {
          Log.Error("RegionServer socket cleanup failed", cex);
        }
      } finally {
	TryingToConnect = false;
      }
    }

    public void Close()
    {
      connected = false;
      socket.Close();
    }

    private void StartListener(Action listener) {
      Task.Factory.StartNew(() => {
        try {
          Log.Info("Starting RegionServer TcpRelay Listener ...");
          listener();
        }
        catch (Exception ex) {
          Log.Error("RegionServer TcpRelay Listener error", ex);
        }
        finally {
          connected = false;
        }
      }, TaskCreationOptions.LongRunning);
    }

    const int SEND_HDR_SIZE = 4;
    const int RECV_HDR_SIZE = 12;

    private void Send(ushort[] sessionIds, byte[] dataBuf, int dataOffset, int dataLen)
    {
      if (!connected) return; // fail quick

      int sessionLen = sizeof(ushort) * sessionIds.Length;
      sendBuf[0] = (byte)(sessionLen >> 8);
      sendBuf[1] = (byte)(sessionLen);
      sendBuf[2] = (byte)(dataLen >> 8);
      sendBuf[3] = (byte)(dataLen);
      Buffer.BlockCopy(sessionIds, 0, sendBuf, SEND_HDR_SIZE, sessionLen);
      Buffer.BlockCopy(dataBuf, dataOffset, sendBuf, SEND_HDR_SIZE + sessionLen, dataLen);

      int toSend = SEND_HDR_SIZE + sessionLen + dataLen;
      int sent = 0;
      try {
        while (sent < toSend)
          sent += socket.Send(sendBuf, sent, toSend - sent, SocketFlags.None);
      } catch (SocketException ex) {
        Log.Error("RegionServer TcpRelay Send error", ex);
        try { Close(); } catch (Exception expected) {}
        return;
      }

      Statistics.PacketsOutRegionServer++;
      Statistics.RegionServerQueue = SendAction.InputCount; // this doesn't work with multiple servers
    }

    private void RecvThread() {
      byte[] recvBuf = new byte[4096];
      while (connected) {
        // Read the udp pak header ..
        int read = 0;
        while (read < 2) read += socket.Receive(recvBuf, read, 2, SocketFlags.None);
        ushort pakSize = (ushort)((recvBuf[0] << 8) | recvBuf[1]); // << content size
        pakSize += RECV_HDR_SIZE;

        // Read the full packet
        while (read < pakSize) read += socket.Receive(recvBuf, read, pakSize - read, SocketFlags.None);

        // Handle packet
        OnPacketReceived?.Invoke(recvBuf, pakSize);
      }
    }
  }
}
