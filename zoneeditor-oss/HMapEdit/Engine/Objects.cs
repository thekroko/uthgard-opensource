using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using HMapEdit.Tools;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace HMapEdit {
  /// <summary>
  /// All Objects
  /// </summary>
  public static class Objects {
    public const string DIR_OBJECTS = "objects";
    public static int Fixture_Max;
    public static List<Fixture> Fixtures = new List<Fixture>();
    public static Dictionary<int, NIF> NIFs = new Dictionary<int, NIF>();
    public static int NIFS_Max;

    private static string GetFile(string filename) {
      if (!Directory.Exists(DIR_OBJECTS))
        return null;
      string[] res = Directory.GetFiles(DIR_OBJECTS, filename, SearchOption.AllDirectories);

      if (res.Length > 0) return res[0];
      return null;
    }

    #region Nested type: Fixture

    public class Fixture {
      /* Missing:
			 * Animate => 0
			 * Ground => 1
			 * Flip = 0
			 * Cave = 0
			 * UID = ID
			 * 3D Angle
			 * */

      private float m_AxisZ = 1;

      private NIF m_NIF;
      private bool m_OnGround = true;
      private float m_Z;

      [Category("Fixture")]
      public int ID { get; set; }

      [Category("Fixture")]
      [TypeConverter(typeof (ExpandableObjectConverter))]
      public NIF NIF {
        get { return m_NIF; }
        set { m_NIF = value; }
      }

      [Category("Fixture")]
      public int NIF_ID {
        get { return NIF.ID; }
      }

      [Category("Fixture")]
      public string Name { get; set; }

      [Category("Fixture")]
      public float X { get; set; }

      [Category("Fixture")]
      public float Y { get; set; }

      [Category("Fixture")]
      public float Z {
        get {
          if (OnGround) {
            int x = Math.Min((int) Math.Round(X/256f), 255);
            int y = Math.Min((int) Math.Round(Y/256f), 255);
            return Program.ZONE.HeightMap[x, y];
          }

          return m_Z;
        }

        set { m_Z = value; }
      }

      [Category("Fixture")]
      public int Scale { get; set; }

      [Category("Fixture")]
      public float Rotation { get; set; }

      [Category("Fixture")]
      public float AxisX { get; set; }

      [Category("Fixture")]
      public float AxisY { get; set; }

      [Category("Fixture")]
      public float AxisZ {
        get { return m_AxisZ; }
        set { m_AxisZ = value; }
      }

      [Category("Fixture")]
      public bool OnGround {
        get { return m_OnGround; }
        set { m_OnGround = value; }
      }

      [Category("Fixture Misc")]
      public int Collison {
        get { return m_NIF.Collision; }
      }

      [Category("Fixture Misc")]
      public int CollideRadius {
        get { return m_NIF.CollideRadius; }
      }

      [Category("Visible")]
      public bool Hidden { get; set; }

      [Category("Visible")]
      public bool WireFrame { get; set; }

      [Category("Misc")]
      public string Group {
        get { return m_NIF.Group; }
        set { m_NIF.Group = value; }
      }

      [Category("Misc")]
      public int MinScale {
        get { return m_NIF.MinScale; }
        set { m_NIF.MinScale = value; }
      }

      [Category("Misc")]
      public int MaxScale {
        get { return m_NIF.MaxScale; }
        set { m_NIF.MaxScale = value; }
      }
    }

    #endregion

    #region Nested type: NIF

    [TypeConverter(typeof (ExpandableObjectConverter))]
    public class NIF {
      /* Missing:
			 * Ambient Only			=> 0
			 * Merlin Data Shadow	=> 0
			 * Animate				=> 0
			 * MinAngel, MaxAngel	=> 0
			 * MinScale, MaxScale	=> 100
			 * LOD1-4				=> 0
			 * Ref Height/Width		=> 16
			 * Unique				=> 0
			 * Local				=> 0
			 * Terrain				=> 0
			 * OnGround				=> 0/1
			 * Color
			 */

      public NIFModel Model;

      public Vector3 Position = new Vector3(-128, -128, -128);
      public Vector3 Scale = new Vector3(256, 256, 256);
      public Texture Texture;
      private int m_Collision = 512;
      private string m_FileName;
      protected string m_Group = "";
      protected bool m_IsDoor;
      protected int m_MaxScale = 100;
      protected int m_MinScale = 70;

      [Category("NIF")]
      public int ID { get; set; }

      [Category("NIF")]
      public string FileName {
        get { return m_FileName; }
        set { m_FileName = value; }
      }

      [Category("NIF")]
      public string Group {
        get { return m_Group; }
        set { m_Group = value; }
      }

      [Category("NIF Misc")]
      public int Collision {
        get { return m_Collision; }
        set { m_Collision = value; }
      }

      [Category("NIF Misc")]
      public int CollideRadius { get; set; }

      ///<summary>
      /// MinScale
      ///</summary>
      [Category("NIF Editing")]
      public int MinScale {
        get { return m_MinScale; }
        set { m_MinScale = value; }
      }

      ///<summary>
      /// MaxScale
      ///</summary>
      [Category("NIF Editing")]
      public int MaxScale {
        get { return m_MaxScale; }
        set { m_MaxScale = value; }
      }

      ///<summary>
      /// IsDoor
      ///</summary>
      [Category("NIF Editing")]
      public bool IsDoor {
        get { return m_IsDoor; }
        set { m_IsDoor = value; }
      }


      public override string ToString() {
        return FileName;
      }

      public void LoadData() {
        Console.WriteLine("# Loading Objects '" + m_FileName + "'..");
        var w = new Stopwatch();
        w.Start();
        string tex = GetFile(m_FileName + ".jpg"); //texture
        string bb = GetFile(m_FileName + ".bb"); //boundingbox
        string obj = GetFile(m_FileName + ".obj"); //model

        if (tex != null) {
          Console.WriteLine("-- Loading Preview Image...");
          Texture = TextureLoader.FromFile(Program.FORM.renderControl1.DEVICE, tex);
        }

        Stream fs = GameData.FindNIF(m_FileName);
        if (fs != null) {
          Model = new NIFModel(fs);
        }

        if (bb != null) {
          Console.WriteLine("-- Generating BB...");
          var r = new StreamReader(bb);

          Position = new Vector3(int.Parse(r.ReadLine()),
                                 int.Parse(r.ReadLine()),
                                 int.Parse(r.ReadLine()));

          Scale = new Vector3(int.Parse(r.ReadLine()),
                              int.Parse(r.ReadLine()),
                              int.Parse(r.ReadLine()));

          if (Scale.X == 0 || Scale.Y == 0 || Scale.Z == 0) Scale = new Vector3(256, 256, 256);

          r.Close();
        }

        /*if (obj != null) {
                    Console.WriteLine("-- Loading Mesh...");
                    Model = OBJLoader.Load(obj);

                    Console.WriteLine("-- Generating BB...");
                    OBJLoader.GenerateBounding(obj, this);
                }*/

        Console.WriteLine("done (took " + w.ElapsedMilliseconds + "ms)");
      }
    }

    #endregion
  }
}