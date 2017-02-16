using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DOL.Geometry;

namespace DOL.GS
{
  /// <summary>
  /// Always unavailable pathing mgr
  /// </summary>
  public class NullPathingMgr : IPathingMgr {
    public bool Init() {
      return true;
    }

    public void Stop() {
    }

    public Task<WrappedPathingResult> GetPathStraightAsync(Zone zone, Vector3 start, Vector3 end) {
      return Task.FromResult(new WrappedPathingResult() { Error = PathingError.NavmeshUnavailable });
    }

    public Task<Vector3?> GetRandomPointAsync(Zone zone, Vector3 position, float radius) {
      return Task.FromResult<Vector3?>(null);
    }

    public Task<Vector3?> GetClosestPointAsync(Zone zone, Vector3 position, float xRange = 256, float yRange = 256, float zRange = 256) {
      return Task.FromResult<Vector3?>(position);
    }

    public bool HasNavmesh(Zone zone) {
      return false;
    }

    public bool IsAvailable => false;
  }
}
