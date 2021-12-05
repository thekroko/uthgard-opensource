using System.Linq;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace HMapEdit {
  /// <summary>
  /// 3D Model
  /// </summary>
  public class OBJModel {
    ///<summary>
    /// SubMeshes
    ///</summary>
    public Mesh[] SubMeshes { get;  set; }

    ///<summary>
    /// Full Mesh
    ///</summary>
    public Mesh Mesh { get; private set; }

    /// <summary>
    /// Renders the model
    /// </summary>
    public void Render() {
      if (SubMeshes.Length > 0) {
        foreach (Mesh t in SubMeshes) {
          t.DrawSubset(0);
        }
      }
      else if (Mesh != null)
        Mesh.DrawSubset(0);
    }

    /// <summary>
    /// Checks whether the ray intersects the mesh or not
    /// </summary>
    /// <param name="src"></param>
    /// <param name="dir"></param>
    /// <returns></returns>
    public bool Intersect(Vector3 src, Vector3 dir) {
      if (SubMeshes.Length > 0) {
        // 1. Ray
        if (SubMeshes.Any(m => m.Intersect(src, dir)))
          return true;
      }
      if (Mesh != null && Mesh.Intersect(src, dir))
        return true;
      return false;
    }
  }
}