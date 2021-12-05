using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using HMapEdit.Engine;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace HMapEdit {
  public class Zone {
    public static bool BUSY;
    public static long LAST_SAVE = DateTime.UtcNow.Ticks;
    public int[,] HeightMap;
    public string PATH = "";
    public Texture TextureMap;

    public int ZoneID { get; set; }

    public int TerrainScaleFactor { get; set; }

    public int TerrainOffsetFactor { get; set; }

    public static void LoadZone(string path) {
      if (BUSY)
        return;

      BUSY = true;
      var w = new BackgroundWorker();
      w.DoWork += delegate {
                    try {
                      InternalLoadZone(path);
                      BUSY = false;
                    }
                    catch (Exception ex) {
                      CrashDialog.Show(ex);
                    }
                  };
      w.RunWorkerAsync();
    }

    private void LoadSectorDat(string path) {
      string p = string.Format("dat{0}.mpk", ZoneID.ToString("D3"));
      string pp = path + "\\" + p;

      Loading.Update("Loading " + p + "...");

      lock (Polygon.Polygons) {
        Loading.Update(p + "... - sector.dat");
        string sectorFile = string.Format("{0}\\SECTOR.DAT" + (path.Contains("backup") ? ".bak" : ""), pp);
        if (!File.Exists(sectorFile))
          throw new IOException("File not found: " + sectorFile);
        var s = new INIStreamer(sectorFile);
        s.ReadIni();

        TerrainScaleFactor = int.Parse(s.GetItem("terrain", "scalefactor"));
        TerrainOffsetFactor = int.Parse(s.GetItem("terrain", "offsetfactor"));

        //Rivers
        int rivers = Parse(s.GetItem("waterdefs", "num"));

        for (int a = 0; a < rivers; a++) {
          string pre = string.Format("river{0:D2}", a);
          var pl = new Polygon();
          pl.Type = ePolygon.Water;
          pl.WTexture = s.GetItem(pre, "texture");
          pl.WMultiTexture = s.GetItem(pre, "multitexture");
          pl.WFlow = Parse(s.GetItem(pre, "flow"));
          pl.WHeight = Parse(s.GetItem(pre, "height"));
          pl.WTesselation = Parse(s.GetItem(pre, "Tesselation"));
          pl.WType = (eWater) Enum.Parse(typeof (eWater), s.GetItem(pre, "type"), true);

          int pointnum = int.Parse(s.GetItem(pre, "bankpoints"));

          for (int b = 0; b < pointnum; b++) {
            string[] left = s.GetItem(pre, string.Format("left{0:D2}", b)).Split(',');
            string[] right = s.GetItem(pre, string.Format("right{0:D2}", b)).Split(',');

            pl.Points.Add(new Vector2(Parse(left[0])*256, Parse(left[1])*256));
            pl.Points.Add(new Vector2(Parse(right[0])*256, Parse(right[1])*256));
          }

          Polygon.Polygons.Add(pl);
        }
      }
    }

    private void LoadHeightmap(string path) {
      string p = string.Format("dat{0}.mpk", ZoneID.ToString("D3"));
      string pp = path + "\\" + p;

      #region terrain.pcx/offset.pcx

      {
        Loading.Update(p + "... - Heightmaps");

        byte[,] ter = PCXImage.Load(string.Format("{0}\\terrain.pcx", pp));
        byte[,] off = PCXImage.Load(string.Format("{0}\\offset.pcx", pp));
        HeightMap = new int[256, 256];

        for (int x = 0; x <= 255; x++)
        {
          for (int y = 0; y <= 255; y++)
            HeightMap[x, y] = (ter[x, y] * TerrainScaleFactor + off[x, y] * TerrainOffsetFactor);
        }
        //z.TerrainMap = ByteToFloat(PCXImage.Load(string.Format("{0}\\terrain.pcx", p)));
        //z.OffsetMap = ByteToFloat(PCXImage.Load(string.Format("{0}\\offset.pcx", p)));
        Program.FORM.renderControl1.CreateZone();
      }

      #endregion
    }

    private void LoadBoundings(string path) {
      string p = string.Format("dat{0}.mpk", ZoneID.ToString("D3"));
      string pp = path + "\\" + p;

      lock (Polygon.Polygons)
      {
        Loading.Update(p + "... - Boundings");

        var rs = new StreamReader(string.Format("{0}\\bound.csv", pp));

        while (!rs.EndOfStream)
        {
          //?, count, x,y, x2,y2, x3, y3, ..
          string line = rs.ReadLine();
          var pl = new Polygon();
          pl.Type = ePolygon.Bounding;
          string[] split = line.Split(',');

          for (int a = 2; a < split.Length; a += 2)
            pl.Points.Add(new Vector2(int.Parse(split[a]), int.Parse(split[a + 1])));

          Polygon.Polygons.Add(pl);
        }

        rs.Close();
      }
    }

    private static void InternalLoadZone(string path) {
      LAST_SAVE = DateTime.UtcNow.Ticks;
      Loading.ShowLoading();

      Polygon.Polygons.Clear();
      var z = new Zone();
      Program.ZONE = z;
      z.PATH = path;
      z.ZoneID = int.Parse(Path.GetFileName(path).Remove(0, 4));

      Program.FORM.Text = "DAoC Zone Editor - " + z.ZoneID;

      {
        string p = string.Format("dat{0}.mpk", z.ZoneID.ToString("D3"));
        string pp = path + "\\" + p;

        Loading.Update("Loading " + p + "...");
        z.LoadSectorDat(path);
        z.LoadHeightmap(path);
        z.LoadBoundings(path);

        #region zonejump.csv

        {
          Loading.Update(p + "... - Zonejumps");

          Zonejump.Zonejumps.Clear();
          var zonejumpcsv = string.Format("{0}\\zonejump.csv", pp);
          if (File.Exists(zonejumpcsv))
          {
            var rs = new StreamReader(zonejumpcsv);

            while (!rs.EndOfStream)
            {
              string line = rs.ReadLine();
              string[] s = line.Split(',');

              //ID, Name, X, Y, X2, Y2, Z1, Z2, JID

              var jp = new Zonejump();
              jp.Name = s[1];
              jp.ID = Parse(s[8]);
              jp.First = new Vector3(Parse(s[2]), Parse(s[3]), Parse(s[6]));
              jp.Second = new Vector3(Parse(s[4]), Parse(s[5]), Parse(s[7]));
              Zonejump.Zonejumps.Add(jp);
            }

            rs.Close();
          }
        }

        #endregion

        #region lights.csv

        {
          Loading.Update(p + "... - Lights");
          Light.Lights.Clear();

          string f = string.Format("{0}\\lights.csv", pp);
          if (File.Exists(f))
          {
            var rs = new StreamReader(f);

            while (!rs.EndOfStream)
            {
              string line = rs.ReadLine();
              string[] s = line.Split(',');

              //46473, 54362, 2910, 3
              //X, Y, Z, Intensity

              var l = new Light();
              l.X = Parse(s[0]);
              l.Y = Parse(s[1]);
              int zz = Parse(s[2]);
              int x = Math.Min((int)Math.Round(l.X / 256f), 255);
              int y = Math.Min((int)Math.Round(l.Y / 256f), 255);
              l.Z = z.HeightMap[x, y];
              l.ZOffset = zz - l.Z;
              int col = Parse(s[3]);
              l.Intensity = col / 10 + 1;
              l.Color = (LightColor)(col % 10);
              Light.Lights.Add(l);
            }

            rs.Close();
          }
        }

        #endregion

        Loading.Update("Loading patchmaps ...");
        Program.CONFIG.UsePatchmaps = true;

        #region texXXX.mpk

        {
          string px = string.Format("tex{0}.mpk", z.ZoneID.ToString("D3"));
          string ppx = path + "\\" + px;

          Loading.Update(px + "... - Terrain Texture");

          //z.TextureSubZones = new Texture[8,8];

          //Load all textures
          var Textures = new Image[8, 8];
          foreach (string file in Directory.GetFiles(ppx, "*.bmp"))
          {
            string f = Path.GetFileName(file);
            int num1 = int.Parse(f.Substring(3, 2));
            int num2 = int.Parse(f.Substring(6, 2));
            //tex01-02.bmp
            Textures[num1, num2] = Image.FromFile(file);
          }

          //Merge
          var bmp = new Bitmap(2048, 2048);
          Graphics g = Graphics.FromImage(bmp);
          g.Clear(Color.Violet);

          for (int x = 0; x < 8; x++)
          {
            for (int y = 0; y < 8; y++)
            {
              Image t = Textures[x, y];

              if (t == null)
                continue;

              g.DrawImage(t, x * 256, y * 256, 256, 256);
            }
          }

          g.Save();
          //bmp.RotateFlip(RotateFlipType.Rotate90FlipX);

          var ms = new MemoryStream();
          bmp.Save(ms, ImageFormat.Bmp);
          ms.Capacity = (int)ms.Length;
          ms.Position = 0;
          Texture m = TextureLoader.FromStream(Program.FORM.renderControl1.DEVICE, ms);
          //Texture m = new Texture(Program.FORM.renderControl1.DEVICE, ms, Usage.None, Pool.SystemMemory);
          z.TextureMap = m;
        }

        #endregion

        var info = (NumberFormatInfo)NumberFormatInfo.CurrentInfo.Clone();
        info.NumberDecimalSeparator = ".";

        Loading.Update("Loading Sounds..");
        SoundMgr.Load(); 

        #region nifs.csv

        lock (Objects.NIFs)
        {
          Objects.NIFs.Clear();
          Loading.Update(p + "... - NIFs");

          var rs = new StreamReader(string.Format("{0}\\nifs.csv", pp));

          int index = 0;

          while (!rs.EndOfStream)
          {
            index++;
            string line = rs.ReadLine();

            if (index <= 2) continue; //header

            string[] s = line.Split(',');

            var nif = new Objects.NIF();
            nif.ID = Parse(s[0]);
            //1 = name
            string name = s[1];
            int pos = name.IndexOf('|');

            if (pos > 0)
              nif.Group = name.Substring(pos + 1);

            nif.FileName = s[2];
            //3 = ambient
            //4 = merlin
            //5 = color
            nif.IsDoor = s[6] == "1"; //6 = animate
            nif.Collision = Parse(s[7]); //7 = collide
            //8 = ground
            //9 = minangle
            //10 = maxangle
            nif.MinScale = Parse(s[11]);
            nif.MaxScale = Parse(s[12]);
            nif.CollideRadius = Parse(s[13]); //13 = radius
            Objects.NIFs.Add(nif.ID, nif);

            if (nif.ID > Objects.NIFS_Max) Objects.NIFS_Max = nif.ID;
          }

          rs.Close();
        }

        #endregion

        #region fixtures.csv

        lock (Objects.Fixtures)
        {
          Loading.Update(p + "... - Fixtures");

          Objects.Fixture_Max = -1;
          Objects.Fixtures.Clear();

          var rs = new StreamReader(string.Format("{0}\\fixtures.csv", pp));

          int index = 0;

          while (!rs.EndOfStream)
          {
            index++;
            string line = rs.ReadLine();

            if (index <= 2) continue; //header

            string[] s = line.Split(',');

            try
            {
              var fix = new Objects.Fixture();
              fix.ID = Parse(s[0]);
              fix.NIF = Objects.NIFs[Parse(s[1])];
              fix.Name = s[2];
              fix.X = float.Parse(s[3], info);
              fix.Y = float.Parse(s[4], info);
              fix.Z = float.Parse(s[5], info);
              //6 = A (angle?)
              fix.Scale = Parse(s[7]);
              //8 = nif collide
              //9 = collide radius
              //10 = animate
              fix.OnGround = s[11] == "1";
              //12 = flip
              //13 = cave
              //14 = uid
              if (s.Length > 15)
              {
                fix.Rotation = float.Parse(s[15], info);
                fix.AxisX = float.Parse(s[16], info);
                fix.AxisY = float.Parse(s[17], info);
                fix.AxisZ = float.Parse(s[18], info);
              }

              Objects.Fixtures.Add(fix);

              if (fix.ID > Objects.Fixture_Max) Objects.Fixture_Max = fix.ID;
            }
            catch (ArgumentOutOfRangeException) { }
            catch (IndexOutOfRangeException) { }
          }

          rs.Close();
        }

        #endregion

        {
          int total = Objects.NIFs.Values.Count;
          int cur = 0;
          var array = new Objects.NIF[Objects.NIFs.Values.Count];
          Objects.NIFs.Values.CopyTo(array, 0);
          for (int i = array.Length - 1; i >= 0; i--)
          {
            Objects.NIF n = array[i];
            Loading.Update(p + "... - NIFs (" + (++cur) + "/" + total + "): " + n.FileName);
            n.LoadData();
            //Application.DoEvents();
          }
        }
      }

      LAST_SAVE = DateTime.UtcNow.Ticks;
      Loading.CloseLoading();
    }

    public void SaveZone(bool backup) {
      if (BUSY && !backup) {
        if (
          MessageBox.Show("Zone Edit is busy - save as backup?", "Warning!", MessageBoxButtons.YesNo,
                          MessageBoxIcon.Warning) !=
          DialogResult.Yes)
          return;


        backup = true;
      }
      BUSY = true;

      string path = (backup ? PATH + "_backup" : PATH);

      if (!backup) {
        //if (Directory.Exists(PATH + "_backup")) {
        //    Directory.Delete(PATH + "_backup", true);
        //}
        Loading.Update("Initiating save process..");
        Loading.ShowLoading(true);
      }
      else {
        if (Directory.Exists(PATH + "_backup")) {
          if (Directory.Exists(PATH + "_backup_bak"))
            Directory.Delete(PATH + "_backup_bak", true);

          Directory.Move(PATH + "_backup", PATH + "_backup_bak");
        }
      }

      if (!Directory.Exists(path))
        Directory.CreateDirectory(path);

      #region datXXX.mpk

      int bwillow = 0;
      {
        string p = string.Format("dat{0}.mpk", ZoneID.ToString("D3"));
        string pp = path + "\\" + p;
        Loading.Update("Saving " + p + "...");

        if (!Directory.Exists(pp)) Directory.CreateDirectory(pp);

        #region SECTOR.DAT

        {
          Loading.Update("Saving " + p + "... - sector.dat");

          var s = new INIStreamer(string.Format("{0}\\SECTOR.DAT" + (backup ? ".bak" : ""), pp));
          s.ReadIni();
          s.Header.Clear();

          s.SetItem("terrain", "scalefactor", TerrainScaleFactor.ToString());
          s.SetItem("terrain", "offsetfactor", TerrainOffsetFactor.ToString());

          //Rivers
          int rivers = 0;

          foreach (Polygon pl in Polygon.Polygons) {
            if (pl.Type != ePolygon.Water) continue;

            //Save this river

            string pre = string.Format("river{0:D2}", rivers);

            if (s.GetTopic(pre) != null) s.RemTopic(pre);

            s.SetItem(pre, "texture", pl.WTexture);
            s.SetItem(pre, "multitexture", pl.WMultiTexture);
            s.SetItem(pre, "flow", pl.WFlow.ToString());
            s.SetItem(pre, "height", pl.WHeight.ToString());
            s.SetItem(pre, "Tesselation", pl.WTesselation.ToString());
            s.SetItem(pre, "type", pl.WType.ToString().ToUpper());
            s.SetItem(pre, "bankpoints", (pl.Points.Count/2).ToString());
            s.SetItem(pre, "name", pre);

            for (int a = 0; a < pl.Points.Count; a += 2) {
              var x1 = (int) (pl.Points[a].X/256);
              var y1 = (int) (pl.Points[a].Y/256);
              var x2 = (int) (pl.Points[a + 1].X/256);
              var y2 = (int) (pl.Points[a + 1].Y/256);

              lock (pl.Points) {
                pl.Points[a] = new Vector2(x1*256, y1*256);
                pl.Points[a + 1] = new Vector2(x2*256, y2*256);
              }

              s.SetItem(pre, string.Format("left{0:D2}", a/2),
                        string.Format("{0},{1},0", x1, y1));
              s.SetItem(pre, string.Format("right{0:D2}", a/2),
                        string.Format("{0},{1},0", x2, y2));
            }

            rivers++;
          }


          s.SetItem("waterdefs", "num", rivers.ToString());

          s.WriteIni();
        }

        #endregion

        #region terrain.pcx/offset.pcx

        {
          Loading.Update("Saving " + p + "... - Heightmaps");

          var terrain = new byte[256,256];
          var offset = new byte[256,256];

          for (int x = 0; x <= 255; x++) {
            for (int y = 0; y <= 255; y++) {
              int o = HeightMap[x, y]/TerrainOffsetFactor;

              if (o > 255) o = 255;

              int remaining = HeightMap[x, y] - o*TerrainOffsetFactor;

              int t = remaining/TerrainScaleFactor;

              if (t > 255) t = 255; //some information may get lost

              offset[x, y] = (byte) (o);
              terrain[x, y] = (byte) (t);
            }
          }

          PCXImage.Save(string.Format("{0}\\terrain.pcx", pp), terrain);
          PCXImage.Save(string.Format("{0}\\offset.pcx", pp), offset);
        }

        #endregion

        #region bound.csv

        {
          Loading.Update("Saving " + p + "... - Bounds");

          var rs = new StreamWriter(string.Format("{0}\\bound.csv", pp));

          foreach (Polygon pl in Polygon.Polygons) {
            if (pl.Type != ePolygon.Bounding) continue;

            //?, count, x,y, x2,y2, x3, y3, ..
            string vectors = "";

            foreach (Vector2 v in pl.Points) vectors += string.Format(",{0},{1}", (int) v.X, (int) v.Y);

            rs.WriteLine("{0},{1}{2}", 0, pl.Points.Count, vectors);
          }

          rs.Flush();
          rs.Close();
        }

        #endregion

        #region water.pcx

        {
          Loading.Update("Saving " + p + "... - Water");

          var data = new byte[256,256];

          for (int x = 0; x < 256; x++) {
            for (int y = 0; y < 256; y++) {
              int height = (HeightMap[x, y]);
              bool isUnderwater = false;
              int n = -1;

              foreach (Polygon pl in Polygon.Polygons) {
                if (pl.Type != ePolygon.Water) continue;

                if (!isUnderwater) n++;

                if (pl.WHeight < height) continue;

                //Check if point is inside
                var pnt = new Vector2(x, y);

                for (int idx = 0; idx < pl.Points.Count - 2; idx += 2) {
                  Vector2 l1 = pl.Points[idx];
                  Vector2 r1 = pl.Points[idx + 1];
                  Vector2 l2 = pl.Points[idx + 2];
                  Vector2 r2 = pl.Points[idx + 3];

                  var _l1 = new Vector3(l1.X, l1.Y, 0);
                  var _r1 = new Vector3(r1.X, r1.Y, 0);
                  var _l2 = new Vector3(l2.X, l2.Y, 0);
                  var _r2 = new Vector3(r2.X, r2.Y, 0);

                  var rayStart = new Vector3(pnt.X*256, pnt.Y*256, 1.0f);
                  var rayDir = new Vector3(0, 0, -1);

                  IntersectInformation i;
                  if (Geometry.IntersectTri(_l1, _r1, _l2, rayStart, rayDir, out i) ||
                      Geometry.IntersectTri(_r1, _r2, _l2, rayStart, rayDir, out i)) {
                    isUnderwater = true;
                    break;
                  }
                }
              }

              data[x, y] = (byte) (isUnderwater ? n : 255);
            }
          }

          for (int i = 0; i < 2; i++)
            WaterDilatation(data);
          PCXImage.Save(string.Format("{0}\\water.pcx", pp), data);
        }

        #endregion

        #region nifs.csv

        {
          Loading.Update("Saving " + p + "... - NIFs");

          var rs = new StreamWriter(string.Format("{0}\\nifs.csv", pp));

          rs.WriteLine("Grid Nifs,,,Ambient,Merlin Data");
          rs.WriteLine(
            "NIF,Textual Name,Filename,Only,Shadow,Color,Animate,Collide,Ground,MinAngle,MaxAngle,MinScale,MaxScale,Radius,LOD 1,LOD 2,LOD 3,LOD 4,Ref Height,Ref Width,Unique,Local,Terrain");


          int id = 1;

          var clones = new List<Objects.NIF>();
          foreach (Objects.NIF n in Objects.NIFs.Values) clones.Add(n);
          Objects.NIFs.Clear();

          foreach (Objects.NIF n in clones) {
            n.ID = id;
            Objects.NIFs.Add(n.ID, n);

            rs.Write(id);
            rs.Write("," + n.FileName + "|" + n.Group); //name
            rs.Write("," + n.FileName); //filename
            rs.Write("," + 0); //ambient only
            rs.Write("," + 0); //merlin shadow data
            rs.Write("," + 0); //color
            rs.Write("," + (n.IsDoor ? 1 : 0)); //animate
            rs.Write("," + n.Collision); //collide
            rs.Write("," + 1); //on ground
            rs.Write("," + 0); //min angle
            rs.Write("," + 0); //max angle
            rs.Write("," + n.MinScale); //min scale
            rs.Write("," + n.MaxScale); //max scale
            rs.Write("," + n.CollideRadius); //radius
            rs.Write("," + 0); //lod1
            rs.Write("," + 0); //lod2
            rs.Write("," + 0); //lod3
            rs.Write("," + 0); //lod4
            rs.Write("," + 16); //ref height
            rs.Write("," + 16); //ref width
            rs.Write("," + 0); //unique
            rs.Write("," + 0); //local
            rs.Write("," + 0); //terrain
            rs.WriteLine();

            id++;
          }

          rs.Flush();
          rs.Close();
        }

        #endregion

        #region fixtures.csv

        {
          Loading.Update("Saving " + p + "... - Fixtures");
          var rs = new StreamWriter(string.Format("{0}\\fixtures.csv", pp));

          rs.WriteLine("Fixtures,,,,,,,,NIF,Collide,,On,,,,,,,");
          rs.WriteLine(
            "ID,NIF #,Textual Name,X,Y,Z,A,Scale,Collide,Radius,Animate,Ground,Flip,Cave,Unique ID, 3D Angle, 3D Axis X, 3D Axis Y, 3D Axis Z");

          var freeIDs = new List<int>();

          int id = 1;
          foreach (Objects.Fixture f in Objects.Fixtures) {
            if (f.NIF.FileName.ToLower().Contains("bwillow")) {
              bwillow++;
            }

            if (f.NIF.IsDoor) //lock ID
            {
              for (int i = id; i < f.ID; i++)
                freeIDs.Add(i);
              id = f.ID + 1;
            }
            else {
              if (freeIDs.Count > 0) {
                f.ID = freeIDs[0];
                freeIDs.RemoveAt(0);
              }
              else {
                f.ID = id++;
              }
            }

            rs.Write(f.ID);
            rs.Write("," + f.NIF_ID);
            rs.Write("," + f.Name);
            rs.Write("," + f.X.ToString().Replace(',', '.'));
            rs.Write("," + f.Y.ToString().Replace(',', '.'));
            rs.Write("," + f.Z.ToString().Replace(',', '.'));
            rs.Write("," + 0); //A (angle?)
            rs.Write("," + f.Scale);
            rs.Write("," + f.NIF.Collision); //collide
            rs.Write("," + f.NIF.CollideRadius); //radius
            rs.Write("," + 0); //animate
            rs.Write("," + (f.OnGround ? 1 : 0)); //ground
            rs.Write("," + 0); //flip
            rs.Write("," + 0); //cave
            rs.Write("," + f.ID); //uid
            rs.Write("," + f.Rotation.ToString().Replace(',', '.')); //3d angle
            rs.Write("," + f.AxisX.ToString().Replace(',', '.')); //); //3d x
            rs.Write("," + f.AxisY.ToString().Replace(',', '.')); //3d y
            rs.Write("," + f.AxisZ.ToString().Replace(',', '.')); //3d z
            rs.WriteLine();
          }

          rs.Flush();
          rs.Close();
        }

        #endregion

        #region zonejump.csv

        {
          Loading.Update("Saving " + p + "... - Zonejumps");

          var rs = new StreamWriter(string.Format("{0}\\zonejump.csv", pp));
          int index = 1;

          foreach (Zonejump j in Zonejump.Zonejumps) {
            //ID, Name, X, Y, X2, Y2, Z1, Z2, JID

            rs.Write(index++);
            rs.Write("," + j.Name);
            rs.Write("," + (int) j.First.X);
            rs.Write("," + (int) j.First.Y);
            rs.Write("," + (int) j.Second.X);
            rs.Write("," + (int) j.Second.Y);
            rs.Write("," + (int) j.First.Z);
            rs.Write("," + (int) j.Second.Z);
            rs.Write("," + j.ID);

            rs.WriteLine();
          }

          rs.Flush();
          rs.Close();
        }

        #endregion

        #region lights.csv

        {
          Loading.Update("Saving " + p + "... - Lights");

          var rs = new StreamWriter(string.Format("{0}\\lights.csv", pp));

          foreach (Light l in Light.Lights) {
            //46473, 54362, 2910, 3
            //X, Y, Z, Intensity

            rs.Write(l.X);
            rs.Write(", " + l.Y);
            rs.Write(", " + (l.Z + l.ZOffset));
            rs.Write(", " + ((int) l.Color + ((l.Intensity - 1)*10)));
            rs.WriteLine();
          }

          rs.Flush();
          rs.Close();
        }

        #endregion
      }

      #endregion

      #region terXXX.mpk

      {
        string p = string.Format("ter{0}.mpk", ZoneID.ToString("D3"));
        string pp = path + "\\" + p;
        Loading.Update("Saving Textures..");

        if (!Directory.Exists(pp)) Directory.CreateDirectory(pp);

        PatchMap.Save(pp + "\\", backup);
      }

      #endregion

      Loading.Update("Saving Sounds...");
      SoundMgr.Save(path);

      if (!backup) {
        if (bwillow >= 2000) {
          MessageBox.Show("For every bwillow you will now have to close one Messagebox..!");
          Loading.Update("Annoying you...");

          for (int i = bwillow; i <= bwillow; i++)
            MessageBox.Show("bwillow " + bwillow);
        }
      }

      if (!backup)
        LAST_SAVE = DateTime.UtcNow.Ticks;

      GC.Collect();
      Loading.Update("Done.");
      Loading.CloseLoading();
      BUSY = false;
    }

    public int[,] CreateHeights() {
      var h = new int[256,256];

      for (int x = 0; x <= 255; x++) for (int y = 0; y <= 255; y++) h[x, y] = HeightMap[x, y];

      return h;
    }

    public int GetMaxHeight() {
      return (TerrainScaleFactor*255 + TerrainOffsetFactor*255);
    }

    public static int Parse(string num) {
      if (string.IsNullOrEmpty(num)) return 0;

      int res;

      if (int.TryParse(num, out res)) return res;
      else return -1;
    }

    private static void WaterDilatation(byte[,] data) {
      // Create a copy
      var src = (byte[,])data.Clone();
      var stamp = new[] {
                          new {x = -1, y = 0},
                          new {x = 1, y = 0},
                          new {x = 0, y = 0},
                          new {x = 0, y = -1},
                          new {x = 0, y = 1 },
                        };
      int xlen = data.GetLength(0);
      int ylen = data.GetLength(1);
      for (int y = 0; y < ylen; y++) {
        for (int x = 0; x < xlen; x++) {
          // Get the surroundings ...
          byte val = 255;
          foreach (var px in stamp) {
            int nx = x + px.x;
            int ny = y + px.y;
            if (nx < 0 || ny < 0 || nx >= xlen || ny >= ylen)
              continue;
            byte val2 = src[nx, ny];
            if (val2 != 255) {
              val = val2;
              break;
            }
          }
          data[x, y] = val;
        }
      }
    }
  }
}