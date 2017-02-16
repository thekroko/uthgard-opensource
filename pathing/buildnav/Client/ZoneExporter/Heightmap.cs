using System;
using System.Collections.Generic;
using MNL;
using OpenTK;

namespace CEM.Client.ZoneExporter
{
  /// <summary>
  /// HeightMap
  /// </summary>
  partial class Zone2Obj
  {
    private void ExportHeightmap()
    {
      int[,] heightmap = LoadHeightmapData();

      #region rivers
      // Export rivers
      List<List<Vector3>> riverPoints = Zone.GetRiverPoints();
      int numWater = riverPoints.Count;
      int[] waterHeights = Zone.GetWaterHeights();
      for (int i = 0; i < numWater; i++)
      {
        int points = riverPoints[i].Count / 2;
        int index = 0;
        while (index < points)
        {
          int toWrite = Math.Min(points - index, 2);
          GeomSetWriter.WriteConvexVolume(toWrite * 2, 0, waterHeights[i], CEM.GeomSetWriter.eAreas.Water);

          for (int j = 0; j < toWrite; j++)
          {
            float x = riverPoints[i][(index + j) * 2].X;
            float y = riverPoints[i][(index + j) * 2].Y;
            float z = waterHeights[i];
            GeomSetWriter.WriteConvexVolumeVertex(new Vector3(x, y, z));
          }

          for (int j = toWrite - 1; j >= 0; j--)
          {
            float x = riverPoints[i][(index + j) * 2 + 1].X;
            float y = riverPoints[i][(index + j) * 2 + 1].Y;
            float z = waterHeights[i];

            GeomSetWriter.WriteConvexVolumeVertex(new Vector3(x, y, z));
          }

          if (points - index > toWrite)
            index += toWrite - 1;
          else
            index += toWrite;
        }
      }
      #endregion

      // Export Heightmap (but pay attention to water map)
      var water = Zone.LoadWaterMap();
      for (int sx = 0; sx < 8; sx++)
      {
        for (int sy = 0; sy < 8; sy++)
        {
          Matrix4 myWorldMatrix = Matrix4.CreateTranslation(Zone.OffsetVector);
          myWorldMatrix *= Matrix4.CreateTranslation(8192 * (sx), 8192 * (sy), 0);
          const int xVectors = 33;
          const int yVectors = 33;

          var myVerticesFakeWater = new List<Vector3>(); // with water.Z
          var myTriangles = new List<Triangle>();

          var myVerticesReal = new List<Vector3>(); // no water.Z

          for (int y = 0; y < yVectors; y++)
          {
            for (int x = 0; x < xVectors; x++)
            {
              int z = 0;
              var waterZ = -1;
              if (sx == 7 && x == (xVectors - 1))
              {
                if (sy == 7 && y == (yVectors - 1))
                {
                  z = heightmap[sx * 32 + (x - 1), sy * 32 + (y - 1)];
                  waterZ = water[sx * 32 + (x - 1), sy * 32 + (y - 1)];
                }
                else
                {
                  z = heightmap[sx * 32 + (x - 1), sy * 32 + y];
                  waterZ = water[sx * 32 + (x - 1), sy * 32 + y];
                }
              }
              else if (sy == 7 && y == (yVectors - 1))
              {
                z = heightmap[sx * 32 + x, sy * 32 + (y - 1)];
                waterZ = water[sx * 32 + x, sy * 32 + (y - 1)];
              }
              else
              {
                z = heightmap[sx * 32 + x, sy * 32 + y];
                waterZ = water[sx * 32 + x, sy * 32 + y];
              }
              Vector3 vector = Vector3.Transform(new Vector3(x * 256, y * 256, z), myWorldMatrix);
              myVerticesReal.Add(vector);

              if (waterZ != 255 && waterZ < waterHeights.Length && waterHeights[waterZ] > vector.Z)
              {
                vector.Z = waterHeights[waterZ];
              }
              myVerticesFakeWater.Add(new Vector3(vector.X, vector.Y, vector.Z));

              if (y == yVectors - 1 || x == xVectors - 1) continue;

              myTriangles.Add(new Triangle((ushort)(x + ((y + 1) * xVectors)), (ushort)(x + 1 + (y * xVectors)),
                                           (ushort)(x + (y * xVectors))));
              myTriangles.Add(new Triangle((ushort)(x + ((y + 1) * xVectors)), (ushort)(x + 1 + ((y + 1) * xVectors)),
                                           (ushort)(x + 1 + (y * xVectors))));
            }
          }

          //ObjWriter.AddMesh(myVerticesReal.ToArray(), myTriangles.ToArray());
          ObjWriter.AddMesh(myVerticesFakeWater.ToArray(), myTriangles.ToArray());
        }
      }
    }

    private int[,] LoadHeightmapData()
    {
      return Zone.Heightmap.ToIntArray();
    }
  }
}
