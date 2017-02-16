using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nodes.Pathing.Pathing
{
  /// <summary>
  /// Containsloaded navmeshes
  /// </summary>
  class LoadedNavmeshes : Dictionary<uint, Navmesh>
  {
    public bool IsLoadingComplete { get; set; }
  }

  /// <summary>
  /// Per-Navmesh data
  /// </summary>
  class Navmesh {
    /// <summary>
    /// Recast mesh ptr
    /// </summary>
    public IntPtr MeshPtr { get; private set; }

    /// <summary>
    /// Recast Query Ptr
    /// </summary>
    public IntPtr QueryPtr { get; private set; }

    /// <summary>
    /// Per-Navmesh random gen
    /// </summary>
    public Random Random { get; private set; }

    public Navmesh(IntPtr meshPtr, IntPtr queryPtr, Random rand) {
      MeshPtr = meshPtr;
      QueryPtr = queryPtr;
      Random = rand;
    }
  }
}
