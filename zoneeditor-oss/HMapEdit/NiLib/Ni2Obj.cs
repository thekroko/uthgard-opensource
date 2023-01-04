using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using CreateOBJ;
using MNL;
using SlimDX;

namespace HMapEdit.NiLib {
  /// <summary>
  /// NiFile -> Obj
  /// Stolen from Schaf / ObjCreator
  /// </summary>
  static class Ni2Obj {
    private static NiAVObject FindNode(NiAVObject root, string name)
    {
      if (root.Name.ToString().ToLower() == name.ToLower())
        return root;
      if (root is NiNode)
      {
        foreach (var node in ((NiNode)root).Children)
        {
          if (!node.IsValid())
            continue;
          NiAVObject res = FindNode(node.Object, name);
          if (res != null)
            return res;
        }
      }
      return null;
    }

    public static WavefrontObjFile ToObj(NiFile model) {
      var obj = new WavefrontObjFile();
      //var matrix = Matrix.RotationZ((float)Math.PI) * Matrix.RotationX(-(float)Math.PI/2f);
      float d90 = (float) Math.PI/2f;
      var matrix = Matrix.Scaling(1, 1, -1) * Matrix.RotationYawPitchRoll(0, d90, 0);
      AddModelToObj(obj, model, matrix, new[] {Program.Arguments.NifLayer}, false);
      if (obj != null && obj.MeshCount == 0) {
        AddModelToObj(obj, model, matrix, new string[0], true);  
      }
      return obj;
    }

    public static void AddModelToObj(WavefrontObjFile objFile, NiFile model, Matrix worldMatrix, string[] filter, bool exclude) {
      if (model == null) return;

      bool foundMesh = false;

      // find all trimeshs and tristrips
      foreach (NiObject obj in model.ObjectsByRef.Values) {
        var avNode = obj as NiAVObject;
        if (avNode == null || ((exclude && IsFiltered(avNode, filter)) || (!exclude && !IsFiltered(avNode, filter))))
          continue;

        var shape = obj as NiTriShape;
        if (shape != null) {
          eFaceDrawMode mode = FindDrawMode(shape);
          foundMesh = true;
          Matrix myMatrix = ComputeWorldMatrix(shape)*worldMatrix;

          var data = shape.Data.Object as NiTriShapeData;
          if (data.Triangles.Length == 0) continue;

          var transformedVertices = new Vector3[data.Vertices.Length];

          for (int i = 0; i < data.Vertices.Length; i++) {
            Vector4 transformedVector = Vector3.Transform(data.Vertices[i], myMatrix);
            transformedVertices[i] = new Vector3(transformedVector.X, transformedVector.Y, transformedVector.Z);
          }

          var triangles = new List<Triangle>();

          // CCW
          if (mode == eFaceDrawMode.DRAW_BOTH ||
              mode == eFaceDrawMode.DRAW_CCW ||
              mode == eFaceDrawMode.DRAW_CCW_OR_BOTH) {
            for (int i = 0; i < data.Triangles.Length; i++)
              triangles.Add(new Triangle(data.Triangles[i].Z, data.Triangles[i].Y, data.Triangles[i].X));
          }

          // CC
          if (mode == eFaceDrawMode.DRAW_BOTH ||
              mode == eFaceDrawMode.DRAW_CW) {
            foreach (Triangle t in data.Triangles)
              triangles.Add(t);
          }

          objFile.AddMesh(transformedVertices, triangles.ToArray());
        }

        var strips = obj as NiTriStrips;
        if (strips != null) {
          eFaceDrawMode mode = FindDrawMode(strips);
          foundMesh = true;
          Matrix myMatrix = ComputeWorldMatrix(strips)*worldMatrix;

          var data = strips.Data.Object as NiTriStripsData;

          var transformedVertices = new Vector3[data.Vertices.Length];

          for (int i = 0; i < data.Vertices.Length; i++) {
            Vector4 transformedVector = Vector3.Transform(data.Vertices[i], myMatrix);
            transformedVertices[i] = new Vector3(transformedVector.X, transformedVector.Y,
                                                 transformedVector.Z);
          }


          var triangles = new List<Triangle>();

          foreach (var points in data.Points) {
            // CCW
            if (mode == eFaceDrawMode.DRAW_BOTH ||
                mode == eFaceDrawMode.DRAW_CCW ||
                mode == eFaceDrawMode.DRAW_CCW_OR_BOTH) {
              for (int i = 0; i < points.Length - 2; i++) {
                triangles.Add(i%2 == 0
                                ? new Triangle(points[i + 2], points[i + 1], points[i + 0])
                                : new Triangle(points[i + 2], points[i + 0], points[i + 1]));
              }
            }

            // CC
            if (mode == eFaceDrawMode.DRAW_BOTH ||
                mode == eFaceDrawMode.DRAW_CW) {
              for (int i = 0; i < points.Length - 2; i++) {
                triangles.Add(i%2 == 0
                                ? new Triangle(points[i + 0], points[i + 1], points[i + 2])
                                : new Triangle(points[i + 0], points[i + 2], points[i + 1]));
              }
            }
          }


          objFile.AddMesh(transformedVertices, triangles.ToArray());
        }
      }

      if (!foundMesh && filter!=null && filter.Length!=0 && filter[0] == "pickee") {
        // no pickee found, use collide
        Console.WriteLine("Warning: Using collidee!");
        AddModelToObj(objFile, model, worldMatrix, new[] {"collide"}, false);
      }
    }

    private static Matrix ComputeWorldMatrix(NiAVObject obj) {
      var path = new List<NiAVObject>();
      NiAVObject current = obj;
      while (current != null) {
        path.Add(current);
        current = current.Parent;
      }
      Matrix worldMatrix = Matrix.Identity;
      for (int i = 0; i < path.Count; i++) {
        NiAVObject node = path[i];
        worldMatrix *= node.Rotation;
        worldMatrix *= Matrix.Scaling(node.Scale, node.Scale, node.Scale);
        worldMatrix *= Matrix.Translation(node.Translation.X, node.Translation.Y, node.Translation.Z);
      }

      return worldMatrix;
    }

    private static bool IsFiltered(NiAVObject obj, string[] filter) {
      NiAVObject current = obj;
      while (current != null) {
        foreach (string str in filter)
          if (current.Name.Value.ToLower().StartsWith(str.ToLower())) return true;

        current = current.Parent;
      }

      return false;
    }

    private static eFaceDrawMode FindDrawMode(NiAVObject obj) {
      NiAVObject current = obj;
      while (current != null) {
        foreach (var propRef in current.Properties) {
          var sprop = propRef.Object as NiStencilProperty;
          if (sprop == null) continue;
          return sprop.FaceDrawMode;
        }

        current = current.Parent;
      }

      return eFaceDrawMode.DRAW_CCW_OR_BOTH;
    }
  }
}