using System;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using DOL.Core;
using DOL.Geometry;
using JetBrains.Annotations;

namespace DOL.GS
{
  public class FailureTolerantPathingMgr : IPathingMgr {
    /// <summary>
    /// Rate limiter used for preventing the TimeMgr from going out of sync during massive failure
    /// </summary>
    public readonly ErrorRateLimiter ErrorLimiter = new ErrorRateLimiter(30 /* errors */, TimeSpan.FromMilliseconds(5000), TimeSpan.FromMinutes(2));

    //private readonly Meter PathingFailures = Metric.Meter("PathingFailures", Unit.Calls);
    private readonly IPathingMgr pathingMgr;

    public FailureTolerantPathingMgr([NotNull] IPathingMgr pathingMgr) {
      this.pathingMgr = Condition.Requires(pathingMgr).IsNotNull().Value;
    }

    public bool Init() {
      throw new NotImplementedException();
    }

    public void Stop() {
      throw new NotImplementedException();
    }

    public async Task<WrappedPathingResult> GetPathStraightAsync(Zone zone, Vector3 start, Vector3 end) {
      var result = (WrappedPathingResult?)await ErrorLimiter.CallAsync(() => pathingMgr.GetPathStraightAsync(zone, start, end));
      if (result != null) {
        return result.Value;
      }
      //PathingFailures.Mark();
      return new WrappedPathingResult() {
        Error = PathingError.NavmeshUnavailable
      };
    }

    public async Task<Vector3?> GetRandomPointAsync(Zone zone, Vector3 position, float radius) {
      var result = await ErrorLimiter.CallAsync(() => pathingMgr.GetRandomPointAsync(zone, position, radius));
      if (result == null) {
        //PathingFailures.Mark();
      }
      return result;
    }

    public async Task<Vector3?> GetClosestPointAsync(Zone zone, Vector3 position, float xRange = 256, float yRange = 256, float zRange = 256) {
      bool error = true;
      var result = await ErrorLimiter.CallAsync(() => {
        var res = pathingMgr.GetClosestPointAsync(zone, position, xRange, yRange, zRange);
        error = false;
        return res;
      });
      if (error) {
        //PathingFailures.Mark();
        return position;
      }
      return result;
    }

    public bool HasNavmesh(Zone zone) {
      return pathingMgr.HasNavmesh(zone);
    }

    public bool IsAvailable => !ErrorLimiter.IsBlocking && pathingMgr.IsAvailable;

		public override string ToString() {
			return "FailureTolerant-" + pathingMgr.ToString();
		}
  }
}
