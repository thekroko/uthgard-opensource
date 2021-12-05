//#define TEST

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Threading;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace HMapEdit {
  public static class OBJLoader {
    public static OBJModel Load(StreamReader r) {
      var result = new OBJModel();

      var vertexes = new List<CustomVertex.PositionOnly>();
      var faces = new List<ushort>();
      var allfaces = new List<ushort>();
      var allverts = new List<CustomVertex.PositionOnly>();
      var meshes = new List<Mesh>();
      int start = 0;

      //Load Model
      Console.WriteLine("--- Reading File...");
      int end = 0;
      while (end != 2) {
        if (r.EndOfStream)
          end++;

        if (end == 2)
          break;

        string line = (end == 1 ? "g Finish" : r.ReadLine());
        string[] split = line.Split(' ');

        if (split.Length != 0) {
          switch (split[0]) {
            case "v": //vertex buffer
            {
              try {
                float x = float.Parse(split[1], CultureInfo.InvariantCulture);
                float y = float.Parse(split[2], CultureInfo.InvariantCulture);
                float z = float.Parse(split[3], CultureInfo.InvariantCulture);

                allverts.Add(new CustomVertex.PositionOnly(x, y, z));

                if (Program.NEW_RENDERING)
                  vertexes.Add(new CustomVertex.PositionOnly(x, y, z));
              }
              catch {
                ;
              }
            }
              break;
            case "g": //group
            {
              if (!Program.NEW_RENDERING)
                continue;
              //old
              //cur.VertexCount = vertexes.Count - cur.VertexStart;
              //cur.FaceCount = (faces.Count - cur.FaceStart)/3;
              if (vertexes.Count == 0 || faces.Count < 3) {
                Console.WriteLine("--- Invalid Submesh! [" + vertexes.Count + "|" + faces.Count + "]");
                vertexes.Clear();
                faces.Clear();
                continue;
              }

              Console.WriteLine("--- Creating Submesh " + meshes.Count + " | Next -> " + split[1]);
              var m =
                new Mesh(faces.Count/3, vertexes.Count, MeshFlags.Managed,
                         CustomVertex.PositionOnly.Format,
                         Program.FORM.renderControl1.DEVICE);
              m.SetVertexBufferData(vertexes.ToArray(), LockFlags.Discard);
              m.SetIndexBufferData(faces.ToArray(), LockFlags.Discard);

              //allfaces.AddRange(faces.ToArray());
              faces.Clear();

              start += vertexes.Count;
              vertexes.Clear();
              meshes.Add(m);
            }
            break;
            case "f": //index buffer
            {
              try {
                ushort v1, v2, v3;

                v1 = (ushort)int.Parse(split[1].Split('/')[0]);
                v2 = (ushort)int.Parse(split[2].Split('/')[0]);
                v3 = (ushort)int.Parse(split[3].Split('/')[0]);

                allfaces.Add((ushort) (v1 - 1));
                allfaces.Add((ushort) (v2 - 1));
                allfaces.Add((ushort) (v3 - 1));

                v1 -= (ushort) (1 + start);
                v2 -= (ushort) (1 + start);
                v3 -= (ushort) (1 + start);

                faces.Add(v1);
                faces.Add(v2);
                faces.Add(v3);
                
              }
              catch (Exception e) {
#if TEST
                                Console.WriteLine(e);
#endif
              }
            }
              break;
          }
        }
      }
      //old
      //cur.VertexCount = vertexes.Count - cur.VertexStart;
      //cur.FaceStart = (faces.Count - cur.FaceStart)/3;

      r.Close();

      if (allfaces.Count < 4) {
        Console.WriteLine("--- No Mesh; aborting...");
        return null;
      }

      Mesh mx = null;

      try {
        if (!Program.NEW_RENDERING) {
          Console.WriteLine("--- Creating Mesh...");
          mx =
            new Mesh(allfaces.Count/3, allverts.Count, MeshFlags.Managed, CustomVertex.PositionOnly.Format,
                     Program.FORM.renderControl1.DEVICE);
          mx.SetVertexBufferData(allverts.ToArray(), LockFlags.Discard);
          mx.SetIndexBufferData(allfaces.ToArray(), LockFlags.Discard);
          //m.SetAttributeTable(attr.ToArray());
        }
      }
      catch (Exception e) {
        //MessageBox.Show("Invalid mesh (" + Path.GetFileName(file) + "): \r\n" + e);
        Console.WriteLine("Invalid mesh: \r\n" + e);
        return null;
      }


      result.SubMeshes = meshes.ToArray();
      Console.WriteLine("--- Object finished!");

      //result.Mesh = mx;
      //result.Mats = matlist;

      return result;
    }

    public static void GenerateBounding(string file, Objects.NIF n) {
      string bb = file + ".bb";

      if (File.Exists(bb)) {
        Console.WriteLine("--- Reading cached .bb...");
        var rr = new StreamReader(bb);
        n.Position = new Vector3(float.Parse(rr.ReadLine()),
                                 float.Parse(rr.ReadLine()),
                                 float.Parse(rr.ReadLine()));

        n.Scale = new Vector3(float.Parse(rr.ReadLine()),
                              float.Parse(rr.ReadLine()),
                              float.Parse(rr.ReadLine()));
        rr.Close();
        return;
      }

      var r = new StreamReader(file);

      float minX = float.MaxValue;
      float minY = float.MaxValue;
      float minZ = float.MaxValue;

      float maxX = float.MinValue;
      float maxY = float.MinValue;
      float maxZ = float.MinValue;

      int v = 0;

      while (!r.EndOfStream) {
        string line = r.ReadLine().Replace('.', ',');
        string[] split = line.Split(' ');

        if (split.Length == 0) continue;

        switch (split[0]) {
          case "v": //vertex
          {
            try {
              float x = float.Parse(split[1]);
              float y = float.Parse(split[2]);
              float z = float.Parse(split[3]);

              if (x > maxX) maxX = x;
              if (x < minX) minX = x;

              if (y > maxY) maxY = y;
              if (y < minY) minY = y;

              if (z > maxZ) maxZ = z;
              if (z < minZ) minZ = z;

              v++;
            }
            catch {
              Console.WriteLine("invalid node: " + line);
            }
          }
            break;
        }
      }

      r.Close();

      if (v > 0) {
        n.Position = new Vector3(minX,
                                 minY,
                                 minZ);

        n.Scale = new Vector3(maxX - minX,
                              maxY - minY,
                              maxZ - minZ);

        var w = new StreamWriter(bb);
        w.WriteLine(n.Position.X);
        w.WriteLine(n.Position.Y);
        w.WriteLine(n.Position.Z);
        w.WriteLine(n.Scale.X);
        w.WriteLine(n.Scale.Y);
        w.WriteLine(n.Scale.Z);
        w.Flush();
        w.Close();
        Console.WriteLine("--- BB saved!");
      }
    }
  }
}