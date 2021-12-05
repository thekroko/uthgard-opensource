using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Text;
using System.Windows.Forms;
using HMapEdit.Properties;
using Microsoft.DirectX.Direct3D;

namespace HMapEdit.Engine {
  /// <summary>
  /// Patch Map
  /// </summary>
  public static class PatchMap {
    public const int COUNT = 5;

    public static readonly Color BLACK = Color.FromArgb(0, 0, 0);
    public static readonly Color BLUE = Color.FromArgb(0, 0, 255);
    public static readonly Color GREEN = Color.FromArgb(0, 255, 0);
    public static readonly Color RED = Color.FromArgb(255, 0, 0);

    private static readonly PatchMapInfo[,] m_PatchInfos = new PatchMapInfo[8,8];
    private static EffectHandle effBorders;

    //private static EffectHandle effPatch, effR, effG, effB, effRTiles, effGTiles, effBTiles, effLayer;
    private static EffectHandle effCursor;
    private static EffectHandle effMode;
    private static EffectHandle effRadius;
    private static Shader m_Shader;
    private static bool m_Changed;

    //Edit
    public static PatchMapInfo CurrentInfo { get; private set; }
    public static PatchMapInfo[,] AllMaps { get { return m_PatchInfos; } }
    public static int SelectedLayer { get; set; }
    public static Color SelectedColor { get; set; }

    public static bool Active {
      get { return m_Shader != null; }
    }

    /// <summary>
    /// Init
    /// </summary>
    public static void Init() {
      if (m_Shader != null)
        return;

      //Init
      string err;
      string shaderSrc = Encoding.Default.GetString(Resources.patchmap);
      Effect s = Effect.FromString(Program.FORM.renderControl1.DEVICE, shaderSrc, null, null, ShaderFlags.Debug, null,
                                   out err);

      if (s == null || !string.IsNullOrEmpty(err)) {
        // There are Compilation errors show them and then close app
        MessageBox.Show(err ?? "ERROR");
        return;
      }

      s.Technique = "PM";

      var sh = new Shader();
      sh.Effect = s;
      sh.Patch = sh.Effect.GetParameter(null, "tex");
      sh.R = sh.Effect.GetParameter(null, "texR");
      sh.G = sh.Effect.GetParameter(null, "texG");
      sh.B = sh.Effect.GetParameter(null, "texB");
      sh.RTiles = sh.Effect.GetParameter(null, "numR");
      sh.GTiles = sh.Effect.GetParameter(null, "numG");
      sh.BTiles = sh.Effect.GetParameter(null, "numB");
      sh.Layer = sh.Effect.GetParameter(null, "layer");
      m_Shader = sh;


      effCursor = s.GetParameter(null, "cursor");
      effRadius = s.GetParameter(null, "radius");
      effMode = s.GetParameter(null, "mode");
      effBorders = s.GetParameter(null, "border");

      //Load patchmap data..
      string pTer = Program.ZONE.PATH + string.Format(@"\ter{0}.mpk\", Program.ZONE.ZoneID.ToString("D3"));
      var r = new StreamReader(pTer + "textures.csv");
      r.ReadLine();

      int cX = -1;
      int cY = -1;
      PatchMapInfo cur = null;

      while (!r.EndOfStream) {
        string l = r.ReadLine();

        if (String.IsNullOrEmpty(l))
          continue;

        string[] split = l.Split(',');

        int pX = int.Parse(split[0]);
        int pY = int.Parse(split[1]);
        string tex = split[2] + ".dds";
        float tiles = int.Parse(split[9]);

        if (cX != pX || cY != pY || cur == null) {
          cur = new PatchMapInfo();
          cur.Exists = true;
          cur.X = pX;
          cur.Y = pY;
          cur.Changed = false;
          m_PatchInfos[pX, pY] = cur;
          cX = pX;
          cY = pY;

          string ppp = pTer + "patch{0:D2}{1:D2}-{2:D2}.dds";

          for (int i = 0; i < COUNT; i++) {
            if (File.Exists(string.Format(ppp, pX, pY, i))) {
              Texture p = LocalTextures.Get(string.Format(ppp, pX, pY, i), false);
              cur.Patch[i] = p;
            }
          }
        }

        Texture t = TerrainTex.Get(tex);
        float n = tiles;

        for (int i = 0; i < COUNT; i++) {
          if (cur.R[i] == null) {
            cur.R[i] = t;
            cur.RTiles[i] = n;
            break;
          }
          if (cur.G[i] == null) {
            cur.G[i] = t;
            cur.GTiles[i] = n;
            break;
          }
          if (cur.B[i] == null) {
            cur.B[i] = t;
            cur.BTiles[i] = n;
            break;
          }
        }
      }
      r.Close();

      //Cur Thingies
      SelectCurrent(0, 0);
      SelectedLayer = 1;
      SelectedColor = Color.Red;
    }

    /// <summary>
    /// Deinit
    /// </summary>
    public static void Deinit() {
      if (m_Shader != null) {
        m_Shader.Effect.Dispose();
        m_Shader = null;
      }
    }

    /// <summary>
    /// Begin
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="m"></param>
    public static void Render(int x, int y, Mesh m) {
      //Get Info
      PatchMapInfo ci = m_PatchInfos[x, y];

      if (ci == null) {
        ci = new PatchMapInfo();
        ci.X = x;
        ci.Y = y;
        ci.Changed = false;
        ci.Exists = false;
        m_PatchInfos[x, y] = ci;
      }

      //1.) Patches
      if (m_Shader != null) {
        Shader s = m_Shader;
        Effect sh = s.Effect;
        sh.Begin(0);

        //a) Begin
        for (int l = 0; l < COUNT; l++) {
          if (ci.Patch[l] == null || ci.R[l] == null) {
            if (l == 0)
              m.DrawSubset(0); //Render Base
            break;
          }

          if (s.Patch != null) sh.SetValue(s.Patch, ci.Patch[l]);
          if (s.R != null) sh.SetValue(s.R, ci.R[l]);
          if (s.G != null) sh.SetValue(s.G, ci.G[l]);
          if (s.B != null) sh.SetValue(s.B, ci.B[l]);

          if (s.RTiles != null) sh.SetValue(s.RTiles, ci.RTiles[l]);
          if (s.GTiles != null) sh.SetValue(s.GTiles, ci.GTiles[l]);
          if (s.BTiles != null) sh.SetValue(s.BTiles, ci.BTiles[l]);

          if (s.Layer != null) sh.SetValue(s.Layer, l);

          for (int c = 1; c <= 3; c++) {
            //Three passes - for each color one
            sh.BeginPass(c);
            m.DrawSubset(0);
            sh.EndPass();
          }
        }

        //2. Render Effects
        if (effRadius != null)
          sh.SetValue(effRadius, Program.MODE == Program.eMode.Texturing ? Program.TOOL_TEXTURE.Radius/65536f : 0);
        if (effCursor != null)
          sh.SetValue(effCursor, new[] {Program.FORM.lastMapPos.X/65536f, Program.FORM.lastMapPos.Y/65536f});
        if (effMode != null) sh.SetValue(effMode, (int) Program.TOOL_TEXTURE.PaintMode);
        if (effBorders != null)
          sh.SetValue(effBorders, /*(Program.FORM.m_TexCtrl != null && Program.FORM.m_TexCtrl.Visible)*/
                      Program.TOOL_TEXTURE.ShowGrid);

        sh.BeginPass(0);
        m.DrawSubset(0);
        sh.EndPass();
        sh.End();
      }
    }

    /// <summary>
    /// Current
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public static void SelectCurrent(int x, int y) {
      CurrentInfo = m_PatchInfos[x, y];
    }

    /// <summary>
    /// Save
    /// </summary>
    /// <param name="path"></param>
    /// <param name="backup"></param>
    public static void Save(string path, bool backup) {
      if (m_Shader == null || !m_Changed)
        return; //not even loaded...

      if (!backup)
        m_Changed = false;
      bool changed = false;

      #region patchmaps

      Loading.Update("Saving " + Path.GetFileName(path) + " - Patchmaps");
      foreach (PatchMapInfo pi in m_PatchInfos) {
        if (!pi.Changed)
          continue;
        pi.Exists = true;
        changed = true;

        if (!backup)
          pi.Changed = false;

        for (int i = 0; i < COUNT; i++) {
          if (pi.Patch[i] != null) {
            Texture p = pi.Patch[i];
            string pp = path + string.Format("patch{0:D2}{1:D2}-{2:D2}.dds", pi.X, pi.Y, i);
            TextureLoader.Save(pp, ImageFileFormat.Dds, p);

            //Mips
            try {
              using (
                Texture mip = TextureLoader.FromFile(p.Device, pp, 0, 0, 0, Usage.Dynamic, Format.A8R8G8B8, Pool.Default,
                                                     Filter.Linear, Filter.Linear, 0)) {
                //mip.GenerateMipSubLevels();
                TextureLoader.Save(pp, ImageFileFormat.Dds, mip);
                mip.Dispose();
              }
            }
            catch (Exception ex) {
              CrashDialog.Show(ex);
            }
          }
        }
      }

      #endregion

      #region textures.csv

      if (changed) {
        Loading.Update("Saving " + Path.GetFileName(path) + "... - textures.csv");

        var w = new StreamWriter(string.Format("{0}textures.csv", path), false);
        w.WriteLine(
          "patch x,patch y,base texture filename,rotate,u translate,v translate,u scale,v scale,visible,num tiles,tileable,normal map filename,specular tint color,specular power,mask type,mask window size");
        w.WriteLine();

        for (int y = 0; y < m_PatchInfos.GetLength(1); y++) {
          for (int x = 0; x < m_PatchInfos.GetLength(0); x++) {
            PatchMapInfo i = m_PatchInfos[x, y];

            const string format = "{0},{1},{2},0.00,0.00,0.00,1.00,1.00,1,{3},{4},,0,32.0,0,16";

            for (int j = 0; j < COUNT; j++) {
              if (i.Patch[j] != null) {
                w.WriteLine(string.Format(format,
                                          x, y, Path.GetFileNameWithoutExtension(TerrainTex.Get(i.R[j])),
                                          i.RTiles[j], i.RTiles[j] > 1 ? "1" : "0"));
                w.WriteLine(string.Format(format,
                                          x, y, Path.GetFileNameWithoutExtension(TerrainTex.Get(i.G[j])),
                                          i.GTiles[j], i.GTiles[j] > 1 ? "1" : "0"));
                w.WriteLine(string.Format(format,
                                          x, y, Path.GetFileNameWithoutExtension(TerrainTex.Get(i.B[j])),
                                          i.BTiles[j], i.BTiles[j] > 1 ? "1" : "0"));
                w.WriteLine();
              }
            }
          }
        }

        w.Flush();
        w.Close();
      }

      #endregion
    }

    #region Painting

    /// <summary>
    /// Draw
    /// </summary>
    /// <param name="mX"></param>
    /// <param name="mY"></param>
    public static void Paint(int mX, int mY) {
      Program.TexturTool.eGradient mode = Program.TOOL_TEXTURE.PaintMode;
      int radius = Program.TOOL_TEXTURE.Radius;
      m_Changed = true;

      string curTex = null;

      if (Program.TOOL_TEXTURE.IntelligentLayers) {
        curTex = TerrainTex.Get(GetTex(CurrentInfo, SelectedLayer, SelectedColor));
      }

      foreach (PatchMapInfo info in m_PatchInfos) {
        int cx = mX - info.X*8192;
        int cy = mY - info.Y*8192;

        if (Program.TOOL_TEXTURE.LimitSubzones) {
          if (cx < 0 || cy < 0 || cx >= 8192 || cy >= 8192)
            continue;
        }

        if (SelectedLayer < 1 || SelectedLayer > COUNT)
          return;

        if (!(radius > 4096 || (cx >= 0 && cy >= 0 && cx < 8192 && cy < 8192) || info.Exists))
          continue;

        int layer = SelectedLayer;
        Color col = SelectedColor;

        //Intelligent Layers..
        if (!string.IsNullOrEmpty(curTex)) {
          //1. Look for exact match
          if (TerrainTex.Get(GetTex(info, layer, col)) != curTex) {
            bool found = false;
            //2. Look for alternative
            for (int i = 1; i <= COUNT; i++) {
              for (int cc = 1; cc <= 3; cc++) {
                Color ccc = BLACK;

                switch (cc) {
                  case 1:
                    ccc = RED;
                    break;
                  case 2:
                    ccc = GREEN;
                    break;
                  case 3:
                    ccc = BLUE;
                    break;
                }

                if (TerrainTex.Get(GetTex(info, i, ccc)) == curTex) {
                  layer = i;
                  col = ccc;
                  found = true;
                  break;
                }
              }
            }

            if (!found)
              continue; //next map
          }
        }

        Texture t = info.Patch[layer - 1];

        if (t == null) {
          t = new Texture(Program.FORM.renderControl1.DEVICE, 256, 256, 1, Usage.Dynamic, Format.A8R8G8B8, Pool.Default);
          Surface ss = t.GetSurfaceLevel(0);
          Graphics gg = ss.GetGraphics();
          Color cc = layer > 1 ? Color.Black : Color.Red;
          gg.FillRectangle(new SolidBrush(cc), 0, 0, t.GetLevelDescription(0).Width, t.GetLevelDescription(0).Height);
          ss.ReleaseGraphics();
          info.Patch[layer - 1] = t;
        }

        lock (info) {
          info.Changed = true;
        }

        Surface s = t.GetSurfaceLevel(0);
        SurfaceDescription sd = s.Description;
        int alpha = Math.Min(Program.TOOL_TEXTURE.Strength, 255);
        Color c = Color.FromArgb(alpha, col);

        Graphics g;

        try {
          g = s.GetGraphics();
        }
        catch (Exception) {
          return; //Cancel this tick
        }

        g.SmoothingMode = SmoothingMode.AntiAlias;
        //g.CompositingMode = CompositingMode.SourceOver;

        int px = cx*sd.Width/8192;
        int py = cy*sd.Height/8192;
        int relX = radius*sd.Width/8192;
        int relY = radius*sd.Height/8192;
        relX = Math.Max(relX, 1);
        relY = Math.Max(relY, 1);

        Brush b = new SolidBrush(c);

        if (mode != 0) {
          //Paths..
          var ell = new GraphicsPath();
          ell.AddEllipse(px - relX, py - relY, relX*2, relY*2);

          var GradientShaper = new PathGradientBrush(ell);
          var GradientSpecifications = new ColorBlend(3);

          float intens = 0.5f;

          if (mode == Program.TexturTool.eGradient.Cubic)
            intens = 0.25f;

          GradientSpecifications.Positions = new[] {0, intens, 1};
          GradientSpecifications.Colors = new[] {
                                                  Color.FromArgb(0, c),
                                                  Color.FromArgb(alpha/2, c),
                                                  Color.FromArgb(alpha, c)
                                                };

          // Pass off color blend to PathGradientBrush to instruct it how to generate the gradient
          GradientShaper.InterpolationColors = GradientSpecifications;
          // Draw polygon (circle) using our point array and gradient brush
          g.FillPolygon(GradientShaper, ell.PathPoints);
        }
        else
          g.FillEllipse(b, px - relX, py - relY, relX*2, relY*2);

        s.ReleaseGraphics();
      }
    }

    private static Texture GetTex(PatchMapInfo pm, int layer, Color col) {
      if (pm == null)
        return null;
      if (col == BLACK)
        return null;
      if (layer < 1 || layer > COUNT)
        return null;

      Texture t = null;

      if (col == RED) t = pm.R[layer - 1];
      if (col == GREEN) t = pm.G[layer - 1];
      if (col == BLUE) t = pm.B[layer - 1];

      return t;
    }

    #endregion

    #region Nested type: PatchMapInfo

    /// <summary>
    /// Patch Map Info
    /// </summary>
    public class PatchMapInfo {
      public Texture[] B = new Texture[COUNT];
      public float[] BTiles = new float[COUNT];
      public Texture[] G = new Texture[COUNT];
      public float[] GTiles = new float[COUNT];
      public Texture[] Patch = new Texture[COUNT];
      public Texture[] R = new Texture[COUNT];
      public float[] RTiles = new float[COUNT];
      public int X { get; set; }
      public int Y { get; set; }
      public bool Changed { get; set; }
      public bool Exists { get; set; }
    }

    #endregion

    #region Nested type: Shader

    private class Shader {
      public Effect Effect { get; set; }
      public EffectHandle Patch { get; set; }
      public EffectHandle R { get; set; }
      public EffectHandle G { get; set; }
      public EffectHandle B { get; set; }
      public EffectHandle RTiles { get; set; }
      public EffectHandle GTiles { get; set; }
      public EffectHandle BTiles { get; set; }
      public EffectHandle Layer { get; set; }
    }

    #endregion
  }
}