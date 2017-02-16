using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Grpc.Core;
using Ninject;
using System.Threading;
using Google.Protobuf;

namespace Nodes.Pathing.Pathing {
  /// <summary>
  ///   Impl of the Pathing Service
  /// </summary>
  internal class PathingServiceImpl : PathingService.PathingServiceBase {
    private const float CONVERSION_FACTOR = 1.0f / 32f;
    private const float INV_FACTOR = 1f / CONVERSION_FACTOR;
    private const int MAX_POLY = 256; // max vector3 when looking up a path (for straight paths too)

    [Inject]
    private ILogger Logger { get; set; }

    [Inject]
    private LoadedNavmeshes Navmeshes { get; set; }

		[Inject]
    private RequestMetrics Metrics { get; set; }

    public override Task<RestartResponse> Restart(RestartRequest request, ServerCallContext context) {
      Logger.Information("{@request}", request);
      Logger.Warning("Server is being asked to restart");
      Environment.Exit(0);
      return null;
		}

		public override Task<ListLoadedNavmeshesResponse> ListLoadedNavmeshes(ListLoadedNavmeshesRequest request,
				ServerCallContext context) {
			Logger.Information("{@request}", request);
			return Task.FromResult(new ListLoadedNavmeshesResponse {Navmeshes = {Navmeshes.Keys}});
		}

    public override async Task<HangRequest> HangTest(HangRequest request, ServerCallContext context)
    {
      var onDequeue = Metrics.Record("HangTest", 0);
      switch (request.Test)
      {
        default:
          onDequeue();
          throw new NotImplementedException();
        case "SLEEP":
          Thread.Sleep(5000);
          onDequeue();
          return request;
        case "AWAIT_RUN_SLEEP":
          await Task.Factory.StartNew(() => Thread.Sleep(5000), TaskCreationOptions.LongRunning);
          onDequeue();
          return request;
        case "AWAIT_SLEEP":
          await Task.Factory.StartNew(() => {
            Thread.Sleep(5000);
            onDequeue();
          }, TaskCreationOptions.LongRunning);
          return request;
      }
    }

    public override async Task<PathingResponse> GetPathStraight(PathingRequest request, ServerCallContext context) {
      // NOTE: Consider using GetPathStreamed instead
      Navmesh mesh;
			if (!Navmeshes.TryGetValue(request.Navmesh, out mesh)) {
        Metrics.QPSByResult["NavmeshUnavailable"].Increment();
				return new PathingResponse {
					ResultCode = PathingResult.NavmeshUnavailable
				};
			}
			var onDequeue = Metrics.Record("Path", request.Navmesh);
			return await Task.Factory.StartNew(() => {
					lock (mesh) {
					  onDequeue();
					var startFloats = ToRecastFloats(request.StartingPoint.X, request.StartingPoint.Y, request.StartingPoint.Z + 8);
					var endFloats = ToRecastFloats(request.DestinationPoint.X, request.DestinationPoint.Y,
							request.DestinationPoint.Z + 8);

					var numNodes = 0;
					var buffer = new float[MAX_POLY * 3];
					var flags = new dtPolyFlags[MAX_POLY];
					var includeFilter = dtPolyFlags.ALL ^ dtPolyFlags.DISABLED;
					var excludeFilter = (dtPolyFlags) 0;
					var filter = new[] {includeFilter, excludeFilter};
					var polyExt = ToRecastFloats(64f, 64f, 256f);
					var options = dtStraightPathOptions.DT_STRAIGHTPATH_ALL_CROSSINGS;

					var status = PathStraight(mesh.QueryPtr, startFloats, endFloats, polyExt, filter, options, ref numNodes, buffer, flags);
					if ((status & dtStatus.DT_SUCCESS) == 0) {
            Metrics.QPSByResult["NoPathFound"].Increment();
            return new PathingResponse {
					    ResultCode = PathingResult.NoPathFound
					  };
					}

					var points = new PathPoint[numNodes];
					var positions = Vector3ArrayFromRecastFloats(buffer, numNodes);

					for (var i = 0; i < numNodes; i++) {
						points[i] = new PathPoint { Flags = (uint) flags[i], Position = positions[i]};
					}

					if ((status & dtStatus.DT_PARTIAL_RESULT) == 0) {
            Metrics.QPSByResult["PathFound"].Increment();
            return new PathingResponse {
							ResultCode = PathingResult.PathFound,
												 Path = {points}
						};
					}
          Metrics.QPSByResult["PartialPathFound"].Increment();
          return new PathingResponse {
						ResultCode = PathingResult.PartialPathFound,
											 Path = {points}
					};
					}
			}, TaskCreationOptions.LongRunning);
		}

    public override async Task GetPathStreamed(IAsyncStreamReader<PathingRequest> requestStream, IServerStreamWriter<PathingResponse> responseStream, ServerCallContext context) {
      while (await requestStream.MoveNext(new CancellationToken(false))) {
        var request = requestStream.Current;
        var response = GetPathForStreaming(request);
        await responseStream.WriteAsync(response);
      }
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct PathPointStruct {
      public dtPolyFlags Flags;
      public float X;
      public float Y;
      public float Z;
    }

    private unsafe PathingResponse GetPathForStreaming(PathingRequest request) {
      Navmesh mesh;
      if (!Navmeshes.TryGetValue(request.Navmesh, out mesh)) {
        Metrics.QPSByResult["NavmeshUnavailable"].Increment();
        return new PathingResponse() {
          SequenceID = request.SequenceID,
          ResultCode = PathingResult.NavmeshUnavailable,
        };
      }
      var onDequeue = Metrics.Record("PathStreamed", request.Navmesh);
      lock (mesh)
        {
          onDequeue();
          var startFloats = ToRecastFloats(request.StartingPoint.X, request.StartingPoint.Y, request.StartingPoint.Z + 8);
          var endFloats = ToRecastFloats(request.DestinationPoint.X, request.DestinationPoint.Y,
              request.DestinationPoint.Z + 8);

          var numNodes = 0;
          var buffer = new float[MAX_POLY * 3];
          var flags = new dtPolyFlags[MAX_POLY];
          var includeFilter = dtPolyFlags.ALL ^ dtPolyFlags.DISABLED;
          var excludeFilter = (dtPolyFlags)0;
          var filter = new[] { includeFilter, excludeFilter };
          var polyExt = ToRecastFloats(64f, 64f, 256f);
          var options = dtStraightPathOptions.DT_STRAIGHTPATH_ALL_CROSSINGS;

          var status = PathStraight(mesh.QueryPtr, startFloats, endFloats, polyExt, filter, options, ref numNodes, buffer, flags);
          if ((status & dtStatus.DT_SUCCESS) == 0) {
            Metrics.QPSByResult["NoPathFound"].Increment();
          return new PathingResponse() {
              SequenceID = request.SequenceID,
              ResultCode = PathingResult.NoPathFound,
            };
          }


        var code = PathingResult.PathFound;
        if ((status & dtStatus.DT_PARTIAL_RESULT) != 0) {
          Metrics.QPSByResult["PartialPathFound"].Increment();
          code = PathingResult.PartialPathFound;
        } else {
          Metrics.QPSByResult["PathFound"].Increment();
        }

        // Create nodes
        var positions = Vector3ArrayFromRecastFloats(buffer, numNodes);
        var oneStructSize = Marshal.SizeOf<PathPointStruct>();
        var oneStruct = Marshal.AllocHGlobal(oneStructSize);
        byte[] buf = new byte[oneStructSize * numNodes];
        for (int i = 0; i < numNodes; i++) {
          var vec = positions[i];
          var pp = new PathPointStruct {
            Flags = flags[i],
            X = (float)vec.X,
            Y = (float)vec.Y,
            Z = (float)vec.Z,
          };
          Marshal.StructureToPtr(pp, oneStruct, false);
          Marshal.Copy(oneStruct, buf, i * oneStructSize, oneStructSize);
        }
        
        return new PathingResponse() {
          SequenceID = request.SequenceID,
          PathNodes = (uint)numNodes,
          ResultCode = code,
          SerializedNodes = ByteString.CopyFrom(buf, 0, buf.Length)
        };
      }
    }

    public override async Task<GetRandomPointResponse> GetRandomPoint(GetRandomPointRequest request, ServerCallContext context) {
      Navmesh mesh;
      if (!Navmeshes.TryGetValue(request.Navmesh, out mesh)) {
        Metrics.QPSByResult["NavmeshUnavailable"].Increment();
        return new GetRandomPointResponse();
			}

			var onDequeue = Metrics.Record("RandomPoint", request.Navmesh);
		  return await Task.Factory.StartNew(() => {
		    lock (mesh) {
		      onDequeue();
		      var center = ToRecastFloats(request.Position.X, request.Position.Y, request.Position.Z + 8);
		      var cradius = request.Radius * CONVERSION_FACTOR;
		      var outVec = new float[3];

		      var defaultInclude = dtPolyFlags.ALL ^ dtPolyFlags.DISABLED;
		      var defaultExclude = (dtPolyFlags) 0;
		      var filter = new[] {defaultInclude, defaultExclude};

		      var polyPickEx = new float[3] {2.0f, 4.0f, 2.0f};

		      var status = FindRandomPointAroundCircle(mesh.QueryPtr, center, cradius, polyPickEx, filter, outVec, () => (float)mesh.Random.NextDouble());

		      if ((status & dtStatus.DT_SUCCESS) == 0) {
            Metrics.QPSByResult["NoRandomPointFound"].Increment();
            return new GetRandomPointResponse();
		      }

          Metrics.QPSByResult["RandomPointFound"].Increment();
          return new GetRandomPointResponse {
		        Point = new Vec3 {X = outVec[0] * INV_FACTOR, Y = outVec[2] * INV_FACTOR, Z = outVec[1] * INV_FACTOR}
		      };
		    }
		  }, TaskCreationOptions.LongRunning);
		}

		public override async Task<GetClosestPointResponse> GetClosestPoint(GetClosestPointRequest request, ServerCallContext context) {
      Navmesh mesh;
      if (!Navmeshes.TryGetValue(request.Navmesh, out mesh))
      {
        Metrics.QPSByResult["NavmeshUnavailable"].Increment();
        return new GetClosestPointResponse();
			}
			var onDequeue = Metrics.Record("ClosestPoint", request.Navmesh);
		  return await Task.Factory.StartNew(() => {
		    lock (mesh) {
		      onDequeue();
		      var center = ToRecastFloats(request.Position.X, request.Position.Y, request.Position.Z + 8);
		      var outVec = new float[3];

		      var defaultInclude = dtPolyFlags.ALL ^ dtPolyFlags.DISABLED;
		      var defaultExclude = (dtPolyFlags) 0;
		      var filter = new[] {defaultInclude, defaultExclude};

		      var polyPickEx = ToRecastFloats(request.Range.X, request.Range.Y, request.Range.Z);

		      var status = FindClosestPoint(mesh.QueryPtr, center, polyPickEx, filter, outVec);

		      if ((status & dtStatus.DT_SUCCESS) == 0) {
            Metrics.QPSByResult["NoClosestPointFound"].Increment();
            return new GetClosestPointResponse();
		      }

          Metrics.QPSByResult["ClosestPointFound"].Increment();
          return new GetClosestPointResponse {
		        Point = new Vec3 {X = outVec[0] * INV_FACTOR, Y = outVec[2] * INV_FACTOR, Z = outVec[1] * INV_FACTOR}
		      };
		    }
		  }, TaskCreationOptions.LongRunning);
		}

		[DllImport("ReUth", CallingConvention = CallingConvention.Cdecl)]
			private static extern dtStatus PathStraight(IntPtr queryPtr, float[] start, float[] end, float[] polyPickExt,
					dtPolyFlags[] queryFilter, dtStraightPathOptions pathOptions, ref int pointCount, float[] pointBuffer,
					dtPolyFlags[] pointFlags);

    delegate float Rand();
    [DllImport("ReUth", CallingConvention = CallingConvention.Cdecl)]
			private static extern dtStatus FindRandomPointAroundCircle(IntPtr queryPtr, float[] center, float radius,
					float[] polyPickExt, dtPolyFlags[] queryFilter, float[] outputVector, [MarshalAs(UnmanagedType.FunctionPtr)]Rand rand);

		[DllImport("ReUth", CallingConvention = CallingConvention.Cdecl)]
			private static extern dtStatus FindClosestPoint(IntPtr queryPtr, float[] center, float[] polyPickExt,
					dtPolyFlags[] queryFilter, float[] outputVector);

		private static float[] ToRecastFloats(double x, double y, double z) {
			return new[] {(float) (x * CONVERSION_FACTOR), (float) (z * CONVERSION_FACTOR), (float) (y * CONVERSION_FACTOR)};
		}

		private static Vec3[] Vector3ArrayFromRecastFloats(float[] buffer, int numNodes) {
			var result = new Vec3[numNodes];
			for (var i = 0; i < numNodes; i++) {
				result[i] = new Vec3 {
					X = buffer[i * 3 + 0] * INV_FACTOR,
						Y = buffer[i * 3 + 2] * INV_FACTOR,
						Z = buffer[i * 3 + 1] * INV_FACTOR
				};
			}
			return result;
		}

		[Flags]
			private enum dtPolyFlags : ushort {
				WALK = 0x01, // Ability to walk (ground, grass, road)
				SWIM = 0x02, // Ability to swim (water).
				DOOR = 0x04, // Ability to move through doors.
				JUMP = 0x08, // Ability to jump.
				DISABLED = 0x10, // Disabled polygon
				DOOR_ALB = 0x20,
				DOOR_MID = 0x40,
				DOOR_HIB = 0x80,
				ALL = 0xffff // All abilities.
			}

		[Flags]
			private enum dtStatus : uint {
				// High level status.
				DT_FAILURE = 1u << 31, // Operation failed.
				DT_SUCCESS = 1u << 30, // Operation succeed.
				DT_IN_PROGRESS = 1u << 29, // Operation still in progress.

				// Detail information for status.
				DT_STATUS_DETAIL_MASK = 0x0ffffff,
				DT_WRONG_MAGIC = 1 << 0, // Input data is not recognized.
				DT_WRONG_VERSION = 1 << 1, // Input data is in wrong version.
				DT_OUT_OF_MEMORY = 1 << 2, // Operation ran out of memory.
				DT_INVALID_PARAM = 1 << 3, // An input parameter was invalid.
				DT_BUFFER_TOO_SMALL = 1 << 4, // Result buffer for the query was too small to store all results.
				DT_OUT_OF_NODES = 1 << 5, // Query ran out of nodes during search.
				DT_PARTIAL_RESULT = 1 << 6 // Query did not reach the end location, returning best guess. 
			}

		[Flags]
			private enum dtStraightPathOptions : uint {
				DT_STRAIGHTPATH_NO_CROSSINGS = 0x00, // Do not add extra vertices on polygon edge crossings.
				DT_STRAIGHTPATH_AREA_CROSSINGS = 0x01, // Add a vertex at every polygon edge crossing where area changes.
				DT_STRAIGHTPATH_ALL_CROSSINGS = 0x02 // Add a vertex at every polygon edge crossing.
			}
	}
}
