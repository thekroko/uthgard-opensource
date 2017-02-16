using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DOL.Geometry;

namespace DOL.GS
{
  static class Extensions
  {
    public static float[] ToRecastFloats(this Vector3 value)
    {
      return new[] { (float)(value.X * LocalPathingMgr.CONVERSION_FACTOR), (float)(value.Z * LocalPathingMgr.CONVERSION_FACTOR), (float)(value.Y * LocalPathingMgr.CONVERSION_FACTOR) };
    }
  }
}
