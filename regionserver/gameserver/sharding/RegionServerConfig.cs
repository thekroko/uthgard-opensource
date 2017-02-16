using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DOL.Database.Migration;
using log4net;

namespace DOL.GS.Sharding
{
	/// <summary>
	/// Region Server Configuration (can't be changed at runtime)
	/// </summary>
	public static class RegionServerConfig {
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public static NodeRegionServer Default;

		/// <summary>
		/// Inits all connections
		/// </summary>
		public static bool Init() {
			log.Info("Connecting to Region Servers ..");

			if (GameServer.Instance.IsLive)
			{
				Default = TryConnect("myserver.example", 10400);
			} else {
				Default = TryConnect("localhost", 10401);
			}
			return true;
		}

		/// <summary>
		/// Tries connecting
		/// </summary>
		/// <param name="creds"></param>
		/// <returns></returns>
		public static NodeRegionServer TryConnect(string host, int port)
		{
			try {
				return ConnectToServer(Tuple.Create(host, port));
			}
			catch (Exception ex) {
				log.Error("Could not init region server: ", ex);
				return null;
			}
		}

		/// <summary>
		/// Connects to server
		/// </summary>
		/// <param name="creds"></param>
		/// <returns></returns>
		public static NodeRegionServer ConnectToServer(Tuple<string, int> creds) {
			var s = NodeRegionServer.Connect(creds.Item1, (ushort)(creds.Item2 + 10000), (ushort)creds.Item2).Result;
			s.OnPacketReceived += (pak,len) => GameServer.Instance.HandleValidUdpPacket(pak, pak.Length);
			return s;
		}

		/// <summary>
		/// Region server for region
		/// </summary>
		/// <param name="r"></param>
		/// <returns></returns>
		public static IRegionServer ForRegion(Region r) {
			// Can add per-region sharding here
			return Default;
		}
	}
}
