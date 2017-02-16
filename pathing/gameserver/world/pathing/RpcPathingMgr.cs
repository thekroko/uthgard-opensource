using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using DOL.Geometry;
using Grpc.Core;
using log4net;
using Nodes.Pathing;

namespace DOL.GS {
	/// <summary>
	///   Pathing
	/// </summary>
	public class RpcPathingMgr : IPathingMgr {
		private const int DEADLINE_MS = 500;

		private readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private readonly bool[] NavmeshAvailable = new bool[500];
		private Channel channel;
		private PathingService.PathingServiceClient client;

		/// <summary>
		///   Initializes the PathingMgr  by loading all available navmeshes
		/// </summary>
		/// <returns></returns>
		public bool Init() {
			Log.Info("Connecting to Pathing Service ...");
			channel = new Channel(GameServer.Instance.Configuration.PathingNodeIp, 20023, ChannelCredentials.Insecure);
			client = new PathingService.PathingServiceClient(channel);

			Log.Info("Fetching list of navmeshes");
			try {
				var response = client.ListLoadedNavmeshes(new ListLoadedNavmeshesRequest(),
						new CallOptions(deadline: DateTime.UtcNow.AddSeconds(15)));
				foreach (var mesh in response.Navmeshes) {
					NavmeshAvailable[mesh] = true;
					Log.InfoFormat("PathingServer has mesh for {0}", mesh);
				}
				Log.Info("Pathing subsystem is ready");
				return true;
			}
			catch (RpcException ex) {
				Log.Error("Pathing server is unavailable: ", ex);
				return false;
			}
		}


		/// <summary>
		///   Stops the PathingMgr and releases all loaded navmeshes
		/// </summary>
		public async void Stop() {
			if (channel == null)
				return;

			Log.Info("Stopping Pathing subsystem is ready");
			await channel.ShutdownAsync();
		}

		private CallOptions GetRPCOptions() {
			return new CallOptions(deadline: DateTime.UtcNow.AddMilliseconds(DEADLINE_MS));
		}

		private Vec3 ToVec3(Vector3 v) {
			return new Vec3 {X = v.X, Y = v.Y, Z = v.Z};
		}

		private Vector3 ToVector3(Vec3 v) {
			return new Vector3(v.X, v.Y, v.Z);
		}

		/// <summary>
		///   Returns a path that prevents collisions with the navmesh, but floats freely otherwise
		/// </summary>
		/// <param name="zone"></param>
		/// <param name="start">Start in GlobalXYZ</param>
		/// <param name="end">End in GlobalXYZ</param>
		/// <returns></returns>
		public async Task<WrappedPathingResult> GetPathStraightAsync(Zone zone, Vector3 start, Vector3 end) {
			if (!NavmeshAvailable[zone.ClientID]) {
				return new WrappedPathingResult {Error = PathingError.NavmeshUnavailable};
			}
			GSStatistics.Paths.Inc();

			var response = await client.GetPathStraightAsync(new PathingRequest {
					Navmesh = zone.ClientID,
					StartingPoint = ToVec3(start),
					DestinationPoint = ToVec3(end)
					}, GetRPCOptions()).ResponseAsync;

			return new WrappedPathingResult {
				Error = (PathingError) response.ResultCode,
				      Points =
					      response.Path.Select(
							      x =>
							      new WrappedPathPoint {
							      Flags = (dtPolyFlags) x.Flags,
							      Position = ToVector3(x.Position)
							      }).ToArray()
			};
		}

		/// <summary>
		///   Returns a random point on the navmesh around the given position
		/// </summary>
		/// <param name="zone">Zone</param>
		/// <param name="position">Start in GlobalXYZ</param>
		/// <param name="radius">End in GlobalXYZ</param>
		/// <returns>null if no point found, Vector3 with point otherwise</returns>
		public async Task<Vector3?> GetRandomPointAsync(Zone zone, Vector3 position, float radius) {
			if (!NavmeshAvailable[zone.ClientID]) {
				return null;
			}
			GSStatistics.Paths.Inc();
			var response = await client.GetRandomPointAsync(new GetRandomPointRequest {
					Navmesh = zone.ClientID,
					Radius = radius,
					Position = ToVec3(position)
					}, GetRPCOptions()).ResponseAsync;
			return response.Point == null ? null : (Vector3?)ToVector3(response.Point);
		}


		/// <summary>
		///   Returns the closest point on the navmesh (UNTESTED! EXPERIMENTAL! WILL GO SUPERNOVA ON USE! MAYBE!?)
		/// </summary>
		public async Task<Vector3?> GetClosestPointAsync(Zone zone, Vector3 position, float xRange = 256f, float yRange = 256f,
				float zRange = 256f) {
			if (!NavmeshAvailable[zone.ClientID]) {
				return position; // Assume the point is safe if we don't have a navmesh
			}
			var response = await client.GetClosestPointAsync(new GetClosestPointRequest {
					Navmesh = zone.ClientID,
					Range = new Vec3 {X = xRange, Y = yRange, Z = zRange},
					Position = ToVec3(position)
					}, GetRPCOptions()).ResponseAsync;
			return response.Point == null ? null : (Vector3?)ToVector3(response.Point);
		}

		/// <summary>
		///   True if pathing is enabled for the specified zone
		/// </summary>
		/// <param name="zone"></param>
		/// <returns></returns>
		public bool HasNavmesh(Zone zone) {
			return zone != null && NavmeshAvailable[zone.ClientID];
		}

		public bool IsAvailable => channel.State == ChannelState.Ready || channel.State == ChannelState.Idle;
	}
}
