using System;
using System.Collections.Generic;
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
	public class ShardedRpcPathingMgr : IPathingMgr {
		public static int DEADLINE_MS = 300;
		public static int NUM_SHARDS = 6;
		public static int FIRST_PORT = 20024;

		private readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private readonly List<int>[] NavmeshAvailableOnWhichShards = new List<int>[500];
		private readonly int[] LastUsedIndex = new int[500];
		private PathingService.PathingServiceClient[] clients;

		private PathingService.PathingServiceClient InitClient(int shard) {
			Log.Info("Connecting to Pathing Service #" + shard + "...");
			var channel = new Channel(GameServer.Instance.Configuration.PathingNodeIp, FIRST_PORT + shard, ChannelCredentials.Insecure);
			return new PathingService.PathingServiceClient(channel);
		}

		private PathingService.PathingServiceClient PickClient(Zone zone) {
			var shardsThatCanServe = NavmeshAvailableOnWhichShards[zone.ClientID];
			var index = LastUsedIndex[zone.ClientID]++;
			return shardsThatCanServe.Count > 0 ? clients[shardsThatCanServe[index % shardsThatCanServe.Count]] : clients[index % clients.Length];
		}

		/// <summary>
		///   Initializes the PathingMgr  by loading all available navmeshes
		/// </summary>
		/// <returns></returns>
		public bool Init() {
			// Init lists
			for (int i = 0; i < NavmeshAvailableOnWhichShards.Length; i++) {
				NavmeshAvailableOnWhichShards[i] = new List<int>();
			}

			// Init Clients
			clients = new PathingService.PathingServiceClient[NUM_SHARDS];
			for (int shard = 0; shard < NUM_SHARDS; shard++) {
				var client = InitClient(shard);
				clients[shard] = client;

				Log.InfoFormat("Fetching list of navmeshes from shard {0}", shard);
				try {
					var response = client.ListLoadedNavmeshes(new ListLoadedNavmeshesRequest(),
							new CallOptions(deadline: DateTime.UtcNow.AddSeconds(5)));
					foreach (var mesh in response.Navmeshes) {
						NavmeshAvailableOnWhichShards[mesh].Add(shard);;
						Log.InfoFormat("PathingServer #{0} has mesh for zone {1}", shard, mesh);
					}
				}
				catch (RpcException ex) {
					Log.Error("Pathing server unavailable for shard #"+shard, ex);
				}
			}
			Log.Info("Pathing subsystem is ready");
			return true;
		}

		/// <summary>
		///   Stops the PathingMgr and releases all loaded navmeshes
		/// </summary>
		public void Stop() {
			Log.Info("TODO: Implement ShardedPathingMgr.Stop()");
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
			if (NavmeshAvailableOnWhichShards[zone.ClientID].Count == 0) {
				return new WrappedPathingResult {Error = PathingError.NavmeshUnavailable};
			}
			GSStatistics.Paths.Inc();

			var response = await PickClient(zone).GetPathStraightAsync(new PathingRequest {
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
			if (NavmeshAvailableOnWhichShards[zone.ClientID].Count == 0) {
				return position;
			}
			GSStatistics.Paths.Inc();
			var response = await PickClient(zone).GetRandomPointAsync(new GetRandomPointRequest {
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
			if (NavmeshAvailableOnWhichShards[zone.ClientID].Count == 0) {
				return position; // Assume the point is safe if we don't have a navmesh
			}
			var response = await PickClient(zone).GetClosestPointAsync(new GetClosestPointRequest {
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
			return zone != null && NavmeshAvailableOnWhichShards[zone.ClientID].Count > 0;
		}

		public bool IsAvailable { get { return true; /* TODO? */ } }
	}
}
