using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Net;
using System.Windows.Forms;
using HMapEdit.Engine;
using HMapEdit.Forms;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using FillMode = Microsoft.DirectX.Direct3D.FillMode;

namespace HMapEdit {
  /// <summary>
  /// Main Form
  /// </summary>
  public partial class MainForm : Form {
    #region Vars

    private readonly SelectNIFDialog SELNIF = new SelectNIFDialog();
    private readonly List<ArrayList> undoList = new List<ArrayList>();
    public Objects.Fixture CurrentFixture;
    public Light CurrentLight;
    public Vector3 lastMapPos = Vector3.Empty;
    public Vector3 lastObjPos = Vector3.Empty;
    public int m_CurrentPolyIndex = -1;
    public Polygon m_CurrentPolygon;
    public Zonejump m_CurrentZonejump;
    private bool m_LeftMouseDown;
    private bool m_PolyMoving;
    private Point mouseMiddle = Point.Empty;
    private Point mouseRight = Point.Empty;
    public bool ortho;
    private PatchMap.PatchMapInfo srcInfo;

    #endregion

    private int helpnr = 1;
    public SoundForm m_SoundForm;
    public TexForm m_TexCtrl;
    private string zone = "";

    public MainForm() {
      InitializeComponent();
      undoList.Add(new ArrayList());
    }

    private void MainForm_Load(object sender, EventArgs e) {
      string[] args = Environment.GetCommandLineArgs();
      Enabled = false;
      zone = Program.Arguments.ZoneDirectory ?? "";
    }

    private void MainForm_FormClosing(object sender, FormClosingEventArgs e) {
      var diff = (int) ((DateTime.UtcNow.Ticks - Zone.LAST_SAVE)/10000/1000/60);

      if (diff >= 3) {
        if (
          MessageBox.Show("You haven't saved your changes since " + (diff) + " mins. Really quit?", "Warning",
                          MessageBoxButtons.OKCancel,
                          MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) ==
          DialogResult.Cancel) {
          e.Cancel = true;
          return;
        }
      }

      Visible = false;
      PatchMap.Deinit();
      renderControl1.Deinitialize();
      Environment.Exit(0);
    }

    private void renderControl1_Resize(object sender, EventArgs e) {
      if (WindowState == FormWindowState.Minimized) return;
      //if (Program.ZONE != null && !Zone.BUSY)
      //    Program.ZONE.SaveZone(true);
      if (renderControl1.DEVICE != null)
        renderControl1.DEVICE.Reset(renderControl1.GetPP());
    }

    private void grid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e) {
      renderControl1.Render();
    }

    private void renderControl1_Click(object sender, EventArgs e) {
      if (!renderControl1.Focused) renderControl1.Focus();
    }

    private int CoordRound(float x) {
      var r = (int) Math.Round(x);
      int pre = r/100*100;

      if ((r%100) >= 75) pre += 100;
      else if ((r%100) >= 25) pre += 50;

      return pre;
    }

    private void helpToolStripMenuItem1_Click(object sender, EventArgs e) {
      MessageBox.Show(this, "Never had time to implement this :(", "Help?");
    }

    private void shortcutsToolStripMenuItem_Click(object sender, EventArgs e) {
      Process.Start("Resources\\shortcuts.txt");
    }

    private void MainForm_Shown(object sender, EventArgs e) {
      WindowState = FormWindowState.Maximized;
      renderControl1.Initialize();
      Enabled = true;

      if (!string.IsNullOrEmpty(zone))
        Zone.LoadZone(zone);
    }

    private void toolStripButton9_Click(object sender, EventArgs e) {
      cUSTOMToolStripMenuItem_Click(sender, e);
    }

    private void reloadToolStripMenuItem_Click(object sender, EventArgs e) {
      string z = Program.ZONE.PATH;
      closeToolStripMenuItem_Click(sender, e);
      Zone.LoadZone(z);
    }

    private void toolStripButton10_Click(object sender, EventArgs e) {
      status_text.Text = "Switched mode to 'Texturing'";
      Program.MODE = Program.eMode.Texturing;
      Program.CONFIG.ShowCursor = false;
      grid.SelectedObject = Program.TOOL_TEXTURE;
      Program.CONFIG.UsePatchmaps = true;
    }

    private void renderControl1_KeyDown(object sender, KeyEventArgs e) {
      if (e.KeyCode == Keys.T && PatchMap.Active) {
        e.Handled = true;

        if (e.Control) {
          toolStripButton10_Click(sender, e);
          return;
        }

        PatchMap.SelectCurrent((int) lastMapPos.X/8192, (int) lastMapPos.Y/8192);

        if (m_TexCtrl == null) {
          m_TexCtrl = new TexForm();
          m_TexCtrl.Location = new Point(Cursor.Position.X - Location.X - 32, Cursor.Position.Y - Location.Y - 48);
        }

        m_TexCtrl.UpdateData();

        if (!m_TexCtrl.Visible)
          m_TexCtrl.Show();
      }
      if (e.KeyCode == Keys.S && e.Control) {
        saveToolStripMenuItem_Click(this, e);
      }
    }

    private void resetBusyFlagDEBUGToolStripMenuItem_Click(object sender, EventArgs e) {
      Zone.BUSY = false;
      Loading.CloseLoading();
    }

    private void toolStripButton11_Click(object sender, EventArgs e) {
      status_text.Text = "Switched mode to 'Sound Edit'";
      Program.MODE = Program.eMode.SoundEdit;
      Program.CONFIG.ShowCursor = false;
      grid.SelectedObject = SoundMgr.CurrentRegion;
      SoundMgr.Touched = true;

      if (m_SoundForm == null)
        m_SoundForm = new SoundForm();

      if (!m_SoundForm.Visible)
        m_SoundForm.Show();
    }

    #region Menu/Toolbar

    private void loadToolStripMenuItem_Click(object sender, EventArgs e) {
      if (folder.ShowDialog(this) == DialogResult.OK) Zone.LoadZone(folder.SelectedPath);
    }

    private void editorSettingsToolStripMenuItem_Click(object sender, EventArgs e) {
      grid.SelectedObject = Program.CONFIG;
    }

    private void zoneToolStripMenuItem_Click(object sender, EventArgs e) {
      grid.SelectedObject = Program.ZONE;
    }

    private void rebuildHeightmapToolStripMenuItem_Click(object sender, EventArgs e) {
      if (Program.ZONE != null) {
        renderControl1.CreateZone();
        renderControl1.Render();
        status_text.Text = "Recreated heightmap.";
      }
    }

    private void toolStripButton2_Click(object sender, EventArgs e) {
      Program.MODE = Program.eMode.Heightmap;
      grid.SelectedObject = Program.TOOL_HMAP;
      Program.CONFIG.ShowCursor = true;
      status_text.Text = "Switched mode to 'Edit Heightmap'";
    }

    private void toolStripButton1_Click(object sender, EventArgs e) {
      Program.MODE = Program.eMode.Cursor;
      grid.SelectedObject = null;
      Program.CONFIG.ShowCursor = true;
      status_text.Text = "Switched mode to 'Cursor'";
    }

    private void saveToolStripMenuItem_Click(object sender, EventArgs e) {
      if (Program.ZONE != null) Program.ZONE.SaveZone(false);
    }

    private void clearUndolistToolStripMenuItem_Click(object sender, EventArgs e) {
      undoList.Clear();
      undoList.Add(new ArrayList());
    }

    private void toolStripButton3_Click(object sender, EventArgs e) {
      Program.MODE = Program.eMode.Smooth;
      Program.CONFIG.ShowCursor = true;
      grid.SelectedObject = Program.TOOL_SMOOTH;
      status_text.Text = "Switched mode to 'Smooth'";
    }

    private void toolStripButton4_Click(object sender, EventArgs e) {
      Program.MODE = Program.eMode.Polygon;
      Program.CONFIG.ShowCursor = false;
      grid.SelectedObject = null;
      status_text.Text = "Switched mode to 'Polygon'";
    }

    private void toolStripButton5_Click(object sender, EventArgs e) {
      Program.MODE = Program.eMode.Object;
      Program.CONFIG.ShowCursor = false;
      grid.SelectedObject = null;
      status_text.Text = "Switched mode to 'Object'";
    }

    private void exitToolStripMenuItem_Click(object sender, EventArgs e) {
      Close();
    }

    private void aboutToolStripMenuItem_Click(object sender, EventArgs e) {
      new AboutForm().ShowDialog();
    }

    private void nIFSToolStripMenuItem_Click(object sender, EventArgs e) {
      if (Zone.BUSY) {
        MessageBox.Show("ZoneEdit is busy!", "Operation Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      new NIFForm().ShowDialog();
    }

    private void toolStripButton6_Click(object sender, EventArgs e) {
      Program.MODE = Program.eMode.Zonejump;
      Program.CONFIG.ShowCursor = false;
      grid.SelectedObject = null;
      status_text.Text = "Switched mode to 'Zonejump'";
    }

    private void closeToolStripMenuItem_Click(object sender, EventArgs e) {
      Program.ZONE = null;
      Objects.Fixtures.Clear();
      Objects.NIFs.Clear();
      Polygon.Polygons.Clear();
      Zonejump.Zonejumps.Clear();
    }

    private void undoToolStripMenuItem_Click(object sender, EventArgs e) {
      //Get last list
      ArrayList undo = undoList[undoList.Count - 1];

      //remove list
      if (undoList.Count > 1) undoList.Remove(undo);

      //Undo
      undo.Reverse();

      //hmap patchlist
      var patched = new List<Vector3>();

      foreach (object o in undo) {
        if (o is Vector3) //hmap
        {
          var vec = (Vector3) o;
          int[,] map = Program.ZONE.HeightMap;
          map[(int) vec.X, (int) vec.Y] = (ushort) vec.Z;
          patched.Add(new Vector3(vec.X, vec.Y, Program.ZONE.HeightMap[(int) vec.X, (int) vec.Y]));
        }
        if (o is PolyUndo) {
          var u = (PolyUndo) o;
          lock (u.Poly.Points) {
            u.Poly.Points[u.Index] = u.Position;
          }
        }
        if (o is FixtureUndo) {
          var u = (FixtureUndo) o;
          u.Fixture.X = u.Position.X;
          u.Fixture.Y = u.Position.Y;
          u.Fixture.Z = u.Position.Z;

          if (u.Deleted) Objects.Fixtures.Add(u.Fixture);
        }
      }

      if (patched.Count > 0) Program.FORM.renderControl1.PatchMesh(patched.ToArray());

      status_text.Text = "Undone " + undo.Count + " actions.";

      undo.Clear();

      Program.FORM.renderControl1.Render();
    }

    private void smoothEdgesToolStripMenuItem_Click(object sender, EventArgs e) {
      var undo = new ArrayList();
      int[,] hmap = Program.ZONE.HeightMap;

      for (int x = 0; x <= 255; x++) {
        for (int y = 0; y <= 255; y++) {
          //Get all pixels in the environment
          float total = 0;
          int cnt = 0;

          for (int sx = x - 1; sx <= x + 1; sx++) {
            for (int sy = y - 1; sy <= y + 1; sy++) {
              //if (sx == x && sy == y)
              //	continue; //middle

              if (sx < 0 || sx > 255 || sy < 0 || sy > 255) continue; //out of border

              total += hmap[sx, sy];
              cnt++;
            }
          }

          const float ratio = 0.5f;
          float avg = total/cnt;
          float height = (hmap[x, y]*ratio) + (avg*(1 - ratio));

          undo.Add(new Vector3(x, y, hmap[x, y]));
          hmap[x, y] = (int) height;
        }
      }

      undoList.Add(undo);
      renderControl1.CreateZone();
      renderControl1.Render();
    }

    private void toolStripButton7_Click(object sender, EventArgs e) {
      Program.MODE = Program.eMode.Ruler;
      Program.CONFIG.ShowCursor = false;
      grid.SelectedObject = Program.TOOL_RULER;
      status_text.Text = "Switched mode to 'Ruler'";
    }

    private void toolStripButton8_Click(object sender, EventArgs e) {
      Program.MODE = Program.eMode.Lights;
      Program.CONFIG.ShowCursor = false;
      grid.SelectedObject = null;
      status_text.Text = "Switched mode to 'Lights'";
    }

    private void cUSTOMToolStripMenuItem_Click(object sender, EventArgs e) {
      ScriptForm.ShowForm();
    }

    private void unhideAllToolStripMenuItem_Click(object sender, EventArgs e) {
      foreach (Objects.Fixture f in Objects.Fixtures) f.Hidden = false;
    }

    private void switchToOrtoLHMatrixToolStripMenuItem_Click(object sender, EventArgs e) {
      ortho = !ortho;
    }

    private void renderToTexToolStripMenuItem_Click(object sender, EventArgs e) {
      var t =
        new Texture(renderControl1.DEVICE, 2048, 2048, 1, Usage.RenderTarget, Format.X8R8G8B8, Pool.Default);
      Surface s = t.GetSurfaceLevel(0);
      Surface old = renderControl1.DEVICE.GetRenderTarget(0);

      renderControl1.DEVICE.SetRenderTarget(0, s);
      renderControl1.Render();
      renderControl1.DEVICE.SetRenderTarget(0, old);

      SurfaceLoader.Save("tex.bmp", ImageFileFormat.Bmp, s);
      t.Dispose();
    }

    private void resetDeviceToolStripMenuItem_Click(object sender, EventArgs e) {
      renderControl1.DEVICE.Reset(renderControl1.GetPP());
    }

    private void switchAlphamodeToolStripMenuItem_Click(object sender, EventArgs e) {
      renderControl1.DEVICE.TextureState[0].AlphaOperation = TextureOperation.SelectArg1;
      renderControl1.DEVICE.TextureState[0].AlphaArgument1 = TextureArgument.TextureColor;
    }

    private void setCameraHeightToolStripMenuItem_Click(object sender, EventArgs e) {
      var f = new InputForm("Set Camera Height...");

      if (f.ShowDialog(this) == DialogResult.OK) renderControl1.CAMERA_TARGET.Z = int.Parse(f.Input);
    }

    private void unfreezeAllToolStripMenuItem_Click(object sender, EventArgs e) {
      foreach (Objects.Fixture f in Objects.Fixtures) f.WireFrame = false;
    }

    #endregion

    #region Mouse/Shortcuts

    private void renderControl1_MouseMove(object sender, MouseEventArgs e) {
      if (Program.ZONE == null) return;

      int XYScale = 75;
      const float ROTScale = (float) (Math.PI/180);

      Vector3 exact = renderControl1.GetExactMapVectorByClick(e.Location);
      Vector3 round = renderControl1.GetRoundedVector(exact);
      lastMapPos = exact;
      lastObjPos = renderControl1.GetGridLoc(e.Location);

      status_coords.Text = string.Format("({0},{1},{2})", exact.X, exact.Y, exact.Z);
      status_obj.Text = string.Format("({0},{1},{2})", lastObjPos.X, lastObjPos.Y, lastObjPos.Z);

      var time = (int) ((DateTime.UtcNow.Ticks - Zone.LAST_SAVE)/10000/1000/60);
      toolUnsaved.Text = time + " mins";
      toolUnsaved.ForeColor = time >= 3 ? Color.Red : Color.Black;

      if (renderControl1.CAMERA_DIST < 8192) XYScale = (int) (XYScale*renderControl1.CAMERA_DIST/8192);

      if (e.Button == MouseButtons.Right) {
        int x = (e.X - mouseRight.X)*XYScale;
        int y = (e.Y - mouseRight.Y)*XYScale;

        var cos = (float) Math.Cos(renderControl1.CAMERA_ROT_SIDE);
        var sin = (float) Math.Sin(renderControl1.CAMERA_ROT_SIDE);

        renderControl1.CAMERA_TARGET.X += (sin*y) + (cos*x);
        renderControl1.CAMERA_TARGET.Y += (cos*y) - (sin*x);
      }

      if (e.Button == MouseButtons.Middle) {
        int x = e.X - mouseMiddle.X;
        int y = e.Y - mouseMiddle.Y;

        renderControl1.CAMERA_ROT_SIDE += x*ROTScale;
        renderControl1.CAMERA_ROT_HEIGHT += y*ROTScale;

        if (renderControl1.CAMERA_ROT_HEIGHT > (Math.PI/2))
          renderControl1.CAMERA_ROT_HEIGHT = (float) (Math.PI/2) - 0.0001f;
        if (renderControl1.CAMERA_ROT_HEIGHT < 0) renderControl1.CAMERA_ROT_HEIGHT = 0;
      }

      #region LeftClick

      if (m_LeftMouseDown) {
        //Tools
        if (Program.MODE == Program.eMode.Heightmap) {
          if (round != Vector3.Empty) {
            //get the map
            int[,] map = Program.ZONE.HeightMap;

            var patched = new List<Vector3>();

            //Plane-Hittest
            for (int x = (int) round.X - Program.TOOL_HMAP.Radius;
                 x < round.X + Program.TOOL_HMAP.Radius;
                 x++) {
              for (int y = (int) round.Y - Program.TOOL_HMAP.Radius;
                   y < round.Y + Program.TOOL_HMAP.Radius;
                   y++) {
                //check if this point is in the radius
                int relX = x - (int) round.X;
                int relY = y - (int) round.Y;

                if (x < 0 || x > 255 || y < 0 || y > 255) continue; //invalid

                var dist = (float) Math.Sqrt(relX*relX + relY*relY);

                if (dist > Program.TOOL_HMAP.Radius) //distance
                  continue; //too far away

                //edit the hmap
                float currentHeight = map[x, y];
                float targetHeight = (Program.TOOL_HMAP.Flatten ? 0 : currentHeight) +
                                     1*Program.TOOL_HMAP.Strength;
                float ratio = dist/Program.TOOL_HMAP.Radius*Program.TOOL_HMAP.Smooth;
                float endheight = currentHeight*ratio + targetHeight*(1f - ratio);


                if (endheight < 0) endheight = 0;
                if (endheight > Program.ZONE.GetMaxHeight()) endheight = Program.ZONE.GetMaxHeight();

                map[x, y] = (int) endheight;

                patched.Add(new Vector3(x, y, Program.ZONE.HeightMap[x, y]));
                undoList[undoList.Count - 1].Add(new Vector3(x, y, currentHeight));
              }
            }

            //Patch the mesh
            Program.FORM.renderControl1.PatchMesh(patched.ToArray());
            //Program.FORM.renderControl1.CreateZone();
            Program.FORM.renderControl1.Render();
          }
        }
        if (Program.MODE == Program.eMode.Smooth) {
          if (round != Vector3.Empty) {
            //get the map
            int[,] map = Program.ZONE.HeightMap;

            var toEdit = new List<Vector3>();
            float totalHeight = 0.0f;

            //Step 1: Find all used vertices + the average height
            for (int x = (int) round.X - Program.TOOL_SMOOTH.Radius;
                 x < round.X + Program.TOOL_SMOOTH.Radius;
                 x++) {
              for (int y = (int) round.Y - Program.TOOL_SMOOTH.Radius;
                   y < round.Y + Program.TOOL_SMOOTH.Radius;
                   y++) {
                //check if this point is in the radius
                int relX = x - (int) round.X;
                int relY = y - (int) round.Y;

                if (x < 0 || x > 255 || y < 0 || y > 255) continue; //invalid

                var dist = (float) Math.Sqrt(relX*relX + relY*relY);

                if (dist > Program.TOOL_HMAP.Radius) //distance
                  continue; //too far away

                //edit the hmap
                float currentHeight = map[x, y];
                toEdit.Add(new Vector3(x, y, currentHeight));
                totalHeight += currentHeight;
              }
            }

            //Step 2: Smooth
            float avgHeight = totalHeight/toEdit.Count;

            var patched = new List<Vector3>();

            foreach (Vector3 vec in toEdit) {
              int relX = (int) vec.X - (int) round.X;
              int relY = (int) vec.Y - (int) round.Y;
              var dist = (float) Math.Sqrt(relX*relX + relY*relY);

              float ratio = (Program.TOOL_SMOOTH.Method == Program.eSmooth.NearbyAVG
                               ? Program.TOOL_SMOOTH.Radius
                               : dist)/
                            Program.TOOL_SMOOTH.Radius*Program.TOOL_SMOOTH.Factor;
              //dist / distmax * modifier
              //float ratio = Program.TOOL_SMOOTH.Factor;

              if (ratio > 1.0f) ratio = 1.0f;

              if (Program.TOOL_SMOOTH.Method == Program.eSmooth.NearbyAVG) {
                totalHeight = 0;
                int cnt = 0;

                for (int sx = (int) vec.X - 1; sx <= (int) vec.X + 1; sx++) {
                  for (int sy = (int) vec.Y - 1; sy <= (int) vec.Y + 1; sy++) {
                    if (sx < 0 || sx > 255 || sy < 0 || sy > 255) continue; //out of border

                    totalHeight += Program.ZONE.HeightMap[sx, sy];
                    cnt++;
                  }
                }

                avgHeight = totalHeight/cnt;
              }

              float endheight = vec.Z*ratio + avgHeight*(1f - ratio);

              if (endheight < 0) endheight = 0;
              if (endheight > Program.ZONE.GetMaxHeight()) endheight = Program.ZONE.GetMaxHeight();

              map[(int) vec.X, (int) vec.Y] = (int) endheight;

              patched.Add(new Vector3(vec.X, vec.Y, Program.ZONE.HeightMap[(int) vec.X, (int) vec.Y]));
              undoList[undoList.Count - 1].Add(new Vector3(vec.X, vec.Y, vec.Z));
            }


            //Patch the mesh
            Program.FORM.renderControl1.PatchMesh(patched.ToArray());
            Program.FORM.renderControl1.Render();
          }
        }
        if (Program.MODE == Program.eMode.Texturing) {
          if (exact != Vector3.Empty) {
            //get the map
            PatchMap.Paint((int) (exact.X), (int) (exact.Y));
            Program.FORM.renderControl1.Render();
          }
        }
        if (Program.MODE == Program.eMode.Polygon && m_CurrentPolygon != null && exact != Vector3.Empty) {
          if (m_PolyMoving) {
            var u = new PolyUndo();
            u.Poly = m_CurrentPolygon;
            u.Index = m_CurrentPolyIndex;
            u.Position = m_CurrentPolygon.Points[m_CurrentPolyIndex];

            undoList[undoList.Count - 1].Add(u);
            m_CurrentPolygon.Points[m_CurrentPolyIndex] = new Vector2(exact.X, exact.Y);
          }
          else {
            m_PolyMoving = true;
          }
        }
        if (Program.MODE == Program.eMode.Ruler) {
          Program.TOOL_RULER.Destination = lastMapPos;
          grid.Refresh();
        }
      }

      #endregion

      mouseMiddle = e.Location;
      mouseRight = e.Location;

      renderControl1.Render();
    }

    private void renderControl1_MouseWheel(object sender, MouseEventArgs e) {
      float ZScale = -Program.CONFIG.MouseWheelScroll;
      ZScale *= renderControl1.CAMERA_DIST/10000f;

      if (ZScale > 4)
        ZScale = 4;


      renderControl1.CAMERA_DIST += e.Delta*ZScale;

      if (renderControl1.CAMERA_DIST < 0) renderControl1.CAMERA_DIST = 0;

      renderControl1.Render();
    }

    private void renderControl1_MouseDown(object sender, MouseEventArgs e) {
      if (e.Button == MouseButtons.Left) {
        undoList.Add(new ArrayList());

        m_LeftMouseDown = true;
        renderControl1_MouseMove(sender, e);

        if (Program.MODE == Program.eMode.Polygon) {
          int index;
          Polygon p = renderControl1.GetPolygonByClick(e.Location, out index);
          m_CurrentPolygon = p;
          m_CurrentPolyIndex = index;

          grid.SelectedObject = p;
        }
        if (Program.MODE == Program.eMode.Object) {
          Objects.Fixture f = renderControl1.GetFixtureByClick(e.Location);
          CurrentFixture = f;
          grid.SelectedObject = f;
        }
        if (Program.MODE == Program.eMode.SoundEdit)
          grid.SelectedObject = SoundMgr.CurrentShape = renderControl1.GetSoundByClick(e.Location);
        if (Program.MODE == Program.eMode.Zonejump) {
          int index;
          Zonejump j = renderControl1.GetZonejumpByClick(e.Location, out index);
          m_CurrentZonejump = j;
          grid.SelectedObject = j;

          if (j != null) j.EditIndex = index;
        }
        if (Program.MODE == Program.eMode.Lights)
          grid.SelectedObject = CurrentLight = renderControl1.GetLightByClick(e.Location);
        if (Program.MODE == Program.eMode.Ruler) {
          Program.TOOL_RULER.Source = lastMapPos;
          grid.Refresh();
        }
      }
    }

    private void renderControl1_MouseUp(object sender, MouseEventArgs e) {
      if (e.Button == MouseButtons.Left) m_LeftMouseDown = false;
      m_PolyMoving = false;
    }

    private void renderControl1_KeyUp(object sender, KeyEventArgs e) {
      switch (e.KeyCode) {
          //Tools
        case Keys.F1:
          if (!e.Control) toolStripButton1_Click(sender, e);
          else toolStripButton5_Click(sender, e);
          break;
        case Keys.F2:
          if (!e.Control) toolStripButton2_Click(sender, e);
          else toolStripButton6_Click(sender, e);
          break;
        case Keys.F3:
          if (!e.Control) toolStripButton3_Click(sender, e);
          else toolStripButton7_Click(sender, e);
          break;
        case Keys.F4:
          if (!e.Control) toolStripButton4_Click(sender, e);
          else toolStripButton8_Click(sender, e);
          break;
        case Keys.Back:
          cUSTOMToolStripMenuItem_Click(sender, EventArgs.Empty);
          break;

          //Quickswitches
        case Keys.E:
          Program.CONFIG.FillMode = (FillMode) ((((int) (Program.CONFIG.FillMode))%3) + 1);
          grid.Refresh();
          break;
        case Keys.Q:
          if (Program.MODE == Program.eMode.Heightmap)
            Program.TOOL_HMAP.Strength = -Program.TOOL_HMAP.Strength;
          if (Program.MODE == Program.eMode.Object)
            Program.CONFIG.Objects.AlwaysShowBounding = !Program.CONFIG.Objects.AlwaysShowBounding;
          grid.Refresh();
          break;
        case Keys.NumPad0: {
          if (Program.MODE == Program.eMode.Object && CurrentFixture != null) {
            CurrentFixture.OnGround = true;
            CurrentFixture.Z = CurrentFixture.Z;
          }
        }
          break;
          //Point modification
        case Keys.Delete: {
          if (Program.MODE == Program.eMode.Polygon && m_CurrentPolygon != null) {
            m_CurrentPolygon.Points.RemoveAt(m_CurrentPolyIndex);

            if (m_CurrentPolygon.Points.Count == 0) Polygon.Polygons.Remove(m_CurrentPolygon);

            m_CurrentPolygon = null;
          }
          if (Program.MODE == Program.eMode.Object && CurrentFixture != null) {
            var undo = new ArrayList();
            var f = new FixtureUndo();
            f.Fixture = CurrentFixture;
            f.Position = new Vector3(CurrentFixture.X, CurrentFixture.Y, CurrentFixture.Z);
            f.Deleted = true;
            undo.Add(f);
            undoList.Add(undo);

            Objects.Fixtures.Remove(CurrentFixture);
            CurrentFixture = null;
          }
          if (Program.MODE == Program.eMode.Zonejump && m_CurrentZonejump != null) {
            Zonejump.Zonejumps.Remove(m_CurrentZonejump);
            m_CurrentZonejump = null;
          }
          if (Program.MODE == Program.eMode.Lights && CurrentLight != null) {
            Light.Lights.Remove(CurrentLight);
            CurrentLight = null;
          }
          if (Program.MODE == Program.eMode.SoundEdit && SoundMgr.CurrentShape != null) {
            if (SoundMgr.CurrentRegion.Shapes.Contains(SoundMgr.CurrentShape))
              SoundMgr.CurrentRegion.Shapes.Remove(SoundMgr.CurrentShape);
            if (SoundMgr.CurrentRegion.xShapes.Contains(SoundMgr.CurrentShape))
              SoundMgr.CurrentRegion.xShapes.Remove(SoundMgr.CurrentShape);
            SoundMgr.CurrentShape = null;
          }
        }
          break;
        case Keys.Insert: {
          if (Program.MODE == Program.eMode.Polygon && m_CurrentPolygon != null &&
              lastMapPos != Vector3.Empty)
            m_CurrentPolygon.Points.Insert(m_CurrentPolyIndex, new Vector2(lastMapPos.X, lastMapPos.Y));
          if (Program.MODE == Program.eMode.Object) {
            if (SELNIF.ShowDialog(this) == DialogResult.OK) {
              Objects.NIF n = SELNIF.SelectedNIF;
              var f = new Objects.Fixture();
              f.ID = ++Objects.Fixture_Max;
              f.Name = n.FileName;
              f.NIF = n;
              f.Rotation = 0;
              f.Scale = 100;
              f.X = lastMapPos.X;
              f.Y = lastMapPos.Y;
              f.Z = lastMapPos.Z;
              Objects.Fixtures.Add(f);
            }
          }
          if (Program.MODE == Program.eMode.Zonejump) {
            var j = new Zonejump();
            j.Name = "???";
            j.ID = 0;
            j.First = lastMapPos;
            j.Second = new Vector3(j.First.X, j.First.Y, j.First.Z + 512);
            Zonejump.Zonejumps.Add(j);
          }
          if (Program.MODE == Program.eMode.Lights) {
            var l = new Light();
            l.X = (int) lastMapPos.X;
            l.Y = (int) lastMapPos.Y;
            l.Z = (int) lastMapPos.Z;
            Light.Lights.Add(l);
          }
          if (Program.MODE == Program.eMode.SoundEdit) {
            var s = new Shape();
            s.Type = ShapeType.Circle;
            s.Radius = 2048;
            s.X = (int) lastMapPos.X;
            s.Y = (int) lastMapPos.Y;
            SoundMgr.CurrentShape = s;
            grid.SelectedObject = s;

            if (e.Control)
              SoundMgr.CurrentRegion.xShapes.Add(s);
            else SoundMgr.CurrentRegion.Shapes.Add(s);
          }
        }
          break;
        case Keys.G: {
          Program.TOOL_TEXTURE.ShowGrid = !Program.TOOL_TEXTURE.ShowGrid;
        }
          break;
        case Keys.End: {
          if (Program.MODE == Program.eMode.Polygon) {
            Polygon p = m_CurrentPolygon;
            if (p == null) {
              p = new Polygon();
              Polygon.Polygons.Add(p);
            }
            p.Points.Add(new Vector2(lastMapPos.X, lastMapPos.Y));
            m_CurrentPolyIndex = p.Points.Count - 1;
          }
          if (Program.MODE == Program.eMode.Object && CurrentFixture != null) {
            var f = new Objects.Fixture();
            f.ID = ++Objects.Fixture_Max;

            f.AxisX = CurrentFixture.AxisX;
            f.AxisY = CurrentFixture.AxisY;
            f.AxisZ = CurrentFixture.AxisZ;
            f.Name = CurrentFixture.Name;
            f.NIF = CurrentFixture.NIF;
            f.OnGround = CurrentFixture.OnGround;
            f.Rotation = CurrentFixture.Rotation;
            f.Scale = CurrentFixture.Scale;
            f.X = lastMapPos.X;
            f.Y = lastMapPos.Y;
            f.Z = CurrentFixture.Z;
            Objects.Fixtures.Add(f);
            CurrentFixture = f;
            grid.SelectedObject = f;
          }

          if (Program.MODE == Program.eMode.Lights && CurrentLight != null) {
            var l = new Light();
            l.Intensity = CurrentLight.Intensity;
            l.Color = CurrentLight.Color;
            l.X = (int) lastMapPos.X;
            l.Y = (int) lastMapPos.Y;
            l.Z = (int) lastMapPos.Z;
            l.ZOffset = CurrentLight.ZOffset;
            Light.Lights.Add(l);
            CurrentLight = l;
          }

          if (Program.MODE == Program.eMode.SoundEdit) {
            var s = new Shape();
            s.Type = ShapeType.Circle;
            s.Radius = 2048;
            s.X = (int) lastMapPos.X;
            s.Y = (int) lastMapPos.Y;

            if (SoundMgr.CurrentShape != null) {
              s.Type = SoundMgr.CurrentShape.Type;
              s.Radius = SoundMgr.CurrentShape.Radius;
              s.EndX = s.X + (SoundMgr.CurrentShape.EndX - SoundMgr.CurrentShape.X);
              s.EndY = s.Y + (SoundMgr.CurrentShape.EndY - SoundMgr.CurrentShape.Y);
            }

            SoundMgr.CurrentShape = s;
            grid.SelectedObject = s;

            if (e.Control)
              SoundMgr.CurrentRegion.xShapes.Add(s);
            else
              SoundMgr.CurrentRegion.Shapes.Add(s);
          }
        }
          break;
        case Keys.Home: {
          if (!e.Control) {
            if (Program.MODE == Program.eMode.Polygon && m_CurrentPolygon != null)
              m_CurrentPolygon.Points[m_CurrentPolyIndex] = new Vector2(lastMapPos.X, lastMapPos.Y);

            if (Program.MODE == Program.eMode.Object && CurrentFixture != null) {
              var undo = new ArrayList();
              var f = new FixtureUndo();
              f.Fixture = CurrentFixture;
              f.Position = new Vector3(CurrentFixture.X, CurrentFixture.Y, CurrentFixture.Z);
              f.Deleted = false;
              undo.Add(f);
              undoList.Add(undo);

              if (CurrentFixture.OnGround || lastObjPos == Vector3.Empty || lastObjPos.X == 0 ||
                  lastObjPos.Y == 0) {
                if (lastMapPos.X == 0 || lastMapPos.Y == 0)
                  return;

                CurrentFixture.X = lastMapPos.X;
                CurrentFixture.Y = lastMapPos.Y;

                if (CurrentFixture.OnGround) CurrentFixture.Z = lastMapPos.Z;
              }
              else {
                CurrentFixture.X = lastObjPos.X;
                CurrentFixture.Y = lastObjPos.Y;
              }
            }
            if (Program.MODE == Program.eMode.SoundEdit && SoundMgr.CurrentShape != null) {
              var x = (int) lastMapPos.X;
              var y = (int) lastMapPos.Y;
              int relX = x - SoundMgr.CurrentShape.X;
              int relY = y - SoundMgr.CurrentShape.Y;
              SoundMgr.CurrentShape.X = x;
              SoundMgr.CurrentShape.Y = y;

              if (SoundMgr.CurrentShape.Type == ShapeType.Rectangle) {
                SoundMgr.CurrentShape.EndX += relX;
                SoundMgr.CurrentShape.EndY += relY;
              }
            }
            if (Program.MODE == Program.eMode.Zonejump && m_CurrentZonejump != null) {
              if (m_CurrentZonejump.EditIndex == 1) {
                m_CurrentZonejump.First =
                  new Vector3(lastMapPos.X, lastMapPos.Y, m_CurrentZonejump.First.Z);
              }
              else if (m_CurrentZonejump.EditIndex == 2) {
                m_CurrentZonejump.Second =
                  new Vector3(lastMapPos.X, lastMapPos.Y, m_CurrentZonejump.Second.Z);
              }
            }
            if (Program.MODE == Program.eMode.Lights && CurrentLight != null) {
              CurrentLight.X = (int) lastMapPos.X;
              CurrentLight.Y = (int) lastMapPos.Y;
              CurrentLight.Z = (int) lastMapPos.Z;
            }
          }
          else {
            if (CurrentFixture != null) {
              renderControl1.CAMERA_TARGET =
                new Vector3(CurrentFixture.X, CurrentFixture.Y, CurrentFixture.Z);
            }
          }
        }
          break;
        case Keys.PageUp: {
          if (Program.MODE == Program.eMode.Object && CurrentFixture != null) {
            var undo = new ArrayList();
            var f = new FixtureUndo();
            f.Fixture = CurrentFixture;
            f.Position = new Vector3(CurrentFixture.X, CurrentFixture.Y, CurrentFixture.Z);
            f.Deleted = false;
            undo.Add(f);
            undoList.Add(undo);

            CurrentFixture.X = renderControl1.CAMERA_TARGET.X;
            CurrentFixture.Y = renderControl1.CAMERA_TARGET.Y;
          }
          if (Program.MODE == Program.eMode.SoundEdit && SoundMgr.CurrentShape != null) {
            SoundMgr.CurrentShape.X = (int) lastMapPos.X;
            SoundMgr.CurrentShape.Y = (int) lastMapPos.Y;
          }
          if (Program.MODE == Program.eMode.Texturing)
            srcInfo = PatchMap.CurrentInfo;
        }
          break;
        case Keys.PageDown: {
          if (Program.MODE == Program.eMode.Object && CurrentFixture != null) {
            var f = new Objects.Fixture();
            f.ID = ++Objects.Fixture_Max;

            f.AxisX = CurrentFixture.AxisX;
            f.AxisY = CurrentFixture.AxisY;
            f.AxisZ = CurrentFixture.AxisZ;
            f.Name = CurrentFixture.Name;
            f.NIF = CurrentFixture.NIF;
            f.OnGround = CurrentFixture.OnGround;
            f.Rotation = (float) (Utils.RandomDouble()*Math.PI*2);

            if (!Program.CONFIG.Rotate)
              f.Rotation = CurrentFixture.Rotation;

            f.Scale = Utils.RandomInt(f.NIF.MinScale, f.NIF.MaxScale);
            f.X = lastMapPos.X;
            f.Y = lastMapPos.Y;
            f.Z = CurrentFixture.Z;
            Objects.Fixtures.Add(f);
            CurrentFixture = f;
            grid.SelectedObject = f;
          }
          if (Program.MODE == Program.eMode.SoundEdit && SoundMgr.CurrentShape != null) {
            SoundMgr.CurrentShape.EndX = (int) lastMapPos.X;
            SoundMgr.CurrentShape.EndY = (int) lastMapPos.Y;
          }
          if (Program.MODE == Program.eMode.Texturing && srcInfo != null && PatchMap.CurrentInfo != null) {
            PatchMap.CurrentInfo.R = new List<Texture>(srcInfo.R).ToArray();
            PatchMap.CurrentInfo.G = new List<Texture>(srcInfo.G).ToArray();
            PatchMap.CurrentInfo.B = new List<Texture>(srcInfo.B).ToArray();
            PatchMap.CurrentInfo.RTiles = new List<float>(srcInfo.RTiles).ToArray();
            PatchMap.CurrentInfo.GTiles = new List<float>(srcInfo.GTiles).ToArray();
            PatchMap.CurrentInfo.BTiles = new List<float>(srcInfo.BTiles).ToArray();

            if (e.Control && e.Alt && e.Shift) { // copy all
              foreach (var pm in PatchMap.AllMaps) {
                pm.R = new List<Texture>(srcInfo.R).ToArray();
                pm.G = new List<Texture>(srcInfo.G).ToArray();
                pm.B = new List<Texture>(srcInfo.B).ToArray();
                pm.RTiles = new List<float>(srcInfo.RTiles).ToArray();
                pm.GTiles = new List<float>(srcInfo.GTiles).ToArray();
                pm.BTiles = new List<float>(srcInfo.BTiles).ToArray();
              }
            }
          }
        }
          break;
        case Keys.B: {
          if (Program.MODE == Program.eMode.Object && CurrentFixture != null) {
            CurrentFixture.Rotation = (float) (Utils.RandomInt(0, 3) + Utils.RandomDouble());
            grid.Update();
          }
        }
          break;
        case Keys.Space: {
          if (CurrentFixture != null) {
            if (e.Control) CurrentFixture.WireFrame = !CurrentFixture.WireFrame;
            else CurrentFixture.Hidden = !CurrentFixture.Hidden;
            grid.Update();
          }
        }
          break;
        case Keys.M: {
          if (Program.MODE == Program.eMode.Object && CurrentFixture != null) {
            CurrentFixture.Scale += Utils.RandomInt(-30, 30);
            grid.Update();
          }
        }
          break;
        case Keys.NumPad1: {
          if (Program.MODE == Program.eMode.Object && CurrentFixture != null) {
            CurrentFixture.Rotation = 0;
            grid.Update();
          }
        }
          break;
        case Keys.NumPad3: {
          if (Program.MODE == Program.eMode.Object && CurrentFixture != null) {
            CurrentFixture.Rotation = (float) (Math.PI/2f);
            grid.Update();
          }
        }
          break;
        case Keys.NumPad9: {
          if (Program.MODE == Program.eMode.Object && CurrentFixture != null) {
            CurrentFixture.Rotation = (float) (Math.PI);
            grid.Update();
          }
        }
          break;
        case Keys.NumPad7: {
          if (Program.MODE == Program.eMode.Object && CurrentFixture != null) {
            CurrentFixture.Rotation = (float) ((Math.PI/2f) + Math.PI);
            grid.Update();
          }
        }
          break;
        case Keys.NumPad5: {
          if (Program.MODE == Program.eMode.Object && CurrentFixture != null) {
            CurrentFixture.X = CoordRound(CurrentFixture.X);
            CurrentFixture.Y = CoordRound(CurrentFixture.Y);
            CurrentFixture.Z = CoordRound(CurrentFixture.Z);
            grid.Update();
          }
        }
          break;
      }

      renderControl1.Render();
    }

    private void renderControl1_KeyPress(object sender, KeyPressEventArgs e) {
      const int moveFactor = 2000;
      int moveX = 0;
      int moveY = 0;

      switch (e.KeyChar) {
          //Point mods
        case '4': {
          if (Program.MODE == Program.eMode.Polygon && m_CurrentPolygon != null)
            m_CurrentPolygon.Points[m_CurrentPolyIndex] += new Vector2(-128, 0);
          if (Program.MODE == Program.eMode.Zonejump && m_CurrentZonejump != null) {
            if (m_CurrentZonejump.EditIndex == 1) m_CurrentZonejump.First += new Vector3(-64, 0, 0);
            else if (m_CurrentZonejump.EditIndex == 2)
              m_CurrentZonejump.Second += new Vector3(-64, 0, 0);
          }
        }
          break;
        case '6': {
          if (Program.MODE == Program.eMode.Polygon && m_CurrentPolygon != null)
            m_CurrentPolygon.Points[m_CurrentPolyIndex] += new Vector2(+128, 0);
          if (Program.MODE == Program.eMode.Zonejump && m_CurrentZonejump != null) {
            if (m_CurrentZonejump.EditIndex == 1) m_CurrentZonejump.First += new Vector3(+64, 0, 0);
            else if (m_CurrentZonejump.EditIndex == 2)
              m_CurrentZonejump.Second += new Vector3(+64, 0, 0);
          }
        }
          break;
        case '8': {
          if (Program.MODE == Program.eMode.Polygon && m_CurrentPolygon != null)
            m_CurrentPolygon.Points[m_CurrentPolyIndex] += new Vector2(0, -128);
          if (Program.MODE == Program.eMode.Zonejump && m_CurrentZonejump != null) {
            if (m_CurrentZonejump.EditIndex == 1) m_CurrentZonejump.First += new Vector3(0, -64, 0);
            else if (m_CurrentZonejump.EditIndex == 2)
              m_CurrentZonejump.Second += new Vector3(0, -64, 0);
          }
        }
          break;
        case '2': {
          if (Program.MODE == Program.eMode.Polygon && m_CurrentPolygon != null)
            m_CurrentPolygon.Points[m_CurrentPolyIndex] += new Vector2(0, +128);
          if (Program.MODE == Program.eMode.Zonejump && m_CurrentZonejump != null) {
            if (m_CurrentZonejump.EditIndex == 1) m_CurrentZonejump.First += new Vector3(0, +64, 0);
            else if (m_CurrentZonejump.EditIndex == 2)
              m_CurrentZonejump.Second += new Vector3(0, +64, 0);
          }
        }
          break;
        case '7': {
          if (Program.MODE == Program.eMode.Zonejump && m_CurrentZonejump != null) {
            if (m_CurrentZonejump.EditIndex == 1) m_CurrentZonejump.First += new Vector3(0, 0, -64);
            else if (m_CurrentZonejump.EditIndex == 2)
              m_CurrentZonejump.Second += new Vector3(0, 0, -64);
          }
        }
          break;
        case '9': {
          if (Program.MODE == Program.eMode.Zonejump && m_CurrentZonejump != null) {
            if (m_CurrentZonejump.EditIndex == 1) m_CurrentZonejump.First += new Vector3(0, 0, +64);
            else if (m_CurrentZonejump.EditIndex == 2)
              m_CurrentZonejump.Second += new Vector3(0, 0, +64);
          }
        }
          break;

          //values
        case 't':
          if (Program.MODE == Program.eMode.Heightmap) Program.TOOL_HMAP.Radius++;
          else if (Program.MODE == Program.eMode.Smooth) Program.TOOL_SMOOTH.Radius++;
          else if (Program.MODE == Program.eMode.Object && CurrentFixture != null) CurrentFixture.Scale += 10;
          break;
        case 'g':
          if (Program.MODE == Program.eMode.Heightmap)
            Program.TOOL_HMAP.Radius = Math.Abs(Program.TOOL_HMAP.Radius - 1);
          else if (Program.MODE == Program.eMode.Smooth)
            Program.TOOL_SMOOTH.Radius = Math.Abs(Program.TOOL_SMOOTH.Radius - 1);
          else if (Program.MODE == Program.eMode.Object && CurrentFixture != null)
            CurrentFixture.Scale -= 10;
          break;

        case 'x':
          if (Program.MODE == Program.eMode.Heightmap) Program.TOOL_HMAP.Strength += 16;
          else if (Program.MODE == Program.eMode.Smooth) Program.TOOL_SMOOTH.Factor += 0.1f;
          else if (Program.MODE == Program.eMode.Object && CurrentFixture != null)
            CurrentFixture.Rotation += 0.03f;
          break;
        case 'y':
          if (Program.MODE == Program.eMode.Heightmap) Program.TOOL_HMAP.Strength -= 16;
          else if (Program.MODE == Program.eMode.Smooth)
            Program.TOOL_SMOOTH.Factor = Math.Abs(Program.TOOL_SMOOTH.Factor - 0.1f);
          else if (Program.MODE == Program.eMode.Object && CurrentFixture != null)
            CurrentFixture.Rotation -= 0.03f;
          break;

        case '/': {
          if (Program.MODE == Program.eMode.Object && CurrentFixture != null) {
            CurrentFixture.OnGround = false;
            CurrentFixture.Z += 20;
          }
          if (Program.MODE == Program.eMode.Lights && CurrentLight != null) CurrentLight.ZOffset += 25;
        }
          break;
        case '*': {
          if (Program.MODE == Program.eMode.Object && CurrentFixture != null) {
            CurrentFixture.OnGround = false;
            CurrentFixture.Z -= 20;
          }
          if (Program.MODE == Program.eMode.Lights && CurrentLight != null)
            CurrentLight.ZOffset -= 25;
        }
          break;

          //Camera movement
        case 'a':
          moveX -= moveFactor;
          break;
        case 'd':
          moveX += moveFactor;
          break;
        case 'w':
          moveY -= moveFactor;
          break;
        case 's':
          moveY += moveFactor;
          break;

        case 'r':
          renderControl1.CAMERA_DIST -= 100;
          break;
        case 'f':
          renderControl1.CAMERA_DIST += 100;
          break;

        case '<':
          Program.CONFIG.MouseWheelScroll *= 1.1f;
          break;
        case '>':
          Program.CONFIG.MouseWheelScroll *= 0.9f;
          break;

        case '+':
          renderControl1.CAMERA_TARGET.Z += 50;
          break;
        case '-':
          renderControl1.CAMERA_TARGET.Z -= 50;
          break;
      }

      if (moveX != 0 || moveY != 0) {
        var cos = (float) Math.Cos(renderControl1.CAMERA_ROT_SIDE);
        var sin = (float) Math.Sin(renderControl1.CAMERA_ROT_SIDE);

        renderControl1.CAMERA_TARGET.X += (sin*moveY) + (cos*moveX);
        renderControl1.CAMERA_TARGET.Y += (cos*moveY) - (sin*moveX);
      }

      renderControl1.Render();
    }

    #endregion

    #region Nested type: FixtureUndo

    private class FixtureUndo {
      public bool Deleted;
      public Objects.Fixture Fixture;
      public Vector3 Position;
    }

    #endregion

    #region Nested type: PolyUndo

    private class PolyUndo {
      public int Index;
      public Polygon Poly;
      public Vector2 Position;
    }

    #endregion

    private void dbgCmdDEBUGToolStripMenuItem_Click(object sender, EventArgs e)
    {
      foreach (var pm in PatchMap.AllMaps) {
        var gnd = pm.Patch[0];
        pm.Changed = true;
        Surface s = gnd.GetSurfaceLevel(0);
        SurfaceDescription sd = s.Description;
        int alpha = Math.Min(Program.TOOL_TEXTURE.Strength, 255);
        Graphics g;

        try
        {
          g = s.GetGraphics();
        }
        catch (Exception)
        {
          return; //Cancel this tick
        }

        g.SmoothingMode = SmoothingMode.AntiAlias;

        // Draw.
        int w = sd.Width;
        int h = sd.Height;
        var br = new SolidBrush(Color.FromArgb(32, 0, 0, 0));
        g.FillRectangle(br, 0, 0, w, h);

        s.ReleaseGraphics();
      }
    }
  }
}