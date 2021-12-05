using System.Collections.Generic;
using System.Globalization;
using System.IO;
using MNL;
using SlimDX;

namespace CreateOBJ {
  /// <summary>
  /// .obj
  /// </summary>
  internal class WavefrontObjFile {
    private readonly List<Triangle[]> _indices;
    private readonly List<Vector3[]> _meshs;

    public int MeshCount {
      get { return _indices.Count; }
    }

    internal WavefrontObjFile() {
      _meshs = new List<Vector3[]>();
      _indices = new List<Triangle[]>();
    }

    internal void AddMesh(Vector3[] vertices, Triangle[] indices) {
      _meshs.Add(vertices);
      _indices.Add(indices);
    }

    internal void Save(StreamWriter writer) {
      int offset = 1;
      for (int index = 0; index < _meshs.Count; index++)
      {
        Vector3[] m = _meshs[index];

        writer.WriteLine("g object " + index);
        foreach (Vector3 v in m)
        {
          writer.WriteLine("v {0} {1} {2}", v.X.ToString(CultureInfo.InvariantCulture),
                           v.Z.ToString(CultureInfo.InvariantCulture),
                           v.Y.ToString(CultureInfo.InvariantCulture));
        }

        Triangle[] i = _indices[index];
        foreach (Triangle t in i)
        {
          writer.WriteLine("f {0} {1} {2}", offset + t.X, offset + t.Y, offset + t.Z);
        }
        offset += _meshs[index].Length;
      }
      writer.Flush();
    }

    internal void Save(string path) {
      using (var writer = new StreamWriter(path)) {
        Save(writer);
      }
    }
  }
}