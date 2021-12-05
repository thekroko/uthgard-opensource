using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HMapEdit.Engine;
using HMapEdit.Properties;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using SlimDevice = SlimDX.Direct3D9.Device;

namespace HMapEdit {
  public partial class RenderControl : Control {
    public static int ADCounter = 100;
    public static int ALPHACOLOR = Color.White.ToArgb();
    public static int ARROW_COLOR = Color.Black.ToArgb();
    public static float fps = 100;
    public Texture OBJSOLID;
    public Texture BLUE;
    public Vector3 CAMERA = new Vector3(65536/2f, 65536/2f, 2000);
    public float CAMERA_DIST = 65536;
    public float CAMERA_ROT_HEIGHT = (float) (Math.PI/4f);
    public float CAMERA_ROT_SIDE = (float) (Math.PI/4f);
    public Vector3 CAMERA_TARGET = new Vector3(65536/2, 65536/2, 2000);
    public Mesh CURSOR;
    public Texture DARKGRAY;
    public Texture DARKGREEN;
    public Device DEVICE;
    public Texture GREEN;
    public Texture GREENALPHA;
    public Mesh GRID;
    public VertexDeclaration GridDecl;
    public VertexElement[] GridElem;
    public int GridSize;
    public Texture HC;
    public Mesh LIGHT;
    public Mesh OBJECT;
    public Texture ORANGE;
    public Mesh POLYGON;
    public Texture RED;
    public Texture REDALPHA;
    public Effect SHADER;
    public Mesh SHAPE_BOX;
    public Mesh SHAPE_CIRCLE;
    public Mesh[,] SUBZONES = new Mesh[8,8];
    public CustomVertex.PositionTextured[] VERTEX;
    public Texture VIOLETT;
    public Stopwatch Watch = new Stopwatch();
    public Texture YELLOW;
    public Mesh _BOX;

    #region Vertexes

    private static float mid;
    private static float neg = -0.5f;
    private static float pos = +0.5f;

    public CustomVertex.PositionColored[] ARROW = new[] {
                                                          new CustomVertex.PositionColored(new Vector3(mid, mid, mid),
                                                                                           ARROW_COLOR),
                                                          new CustomVertex.PositionColored(
                                                            new Vector3(mid, neg - 0.5f, mid), ARROW_COLOR),
                                                          new CustomVertex.PositionColored(
                                                            new Vector3(mid, neg - 0.5f, mid), ARROW_COLOR),
                                                          new CustomVertex.PositionColored(
                                                            new Vector3(mid, neg - 0.3f, pos), ARROW_COLOR),
                                                          new CustomVertex.PositionColored(
                                                            new Vector3(mid, neg - 0.5f, mid), ARROW_COLOR),
                                                          new CustomVertex.PositionColored(
                                                            new Vector3(mid, neg - 0.3f, neg), ARROW_COLOR),
                                                        };

    public CustomVertex.PositionColoredTextured[] BOX = new[] {
                                                                new CustomVertex.PositionColoredTextured(neg, neg, pos,
                                                                                                         ALPHACOLOR, 0,
                                                                                                         0),
                                                                //top left top
                                                                new CustomVertex.PositionColoredTextured(pos, neg, pos,
                                                                                                         ALPHACOLOR, 1,
                                                                                                         0),
                                                                //top right top
                                                                new CustomVertex.PositionColoredTextured(neg, pos, pos,
                                                                                                         ALPHACOLOR, 0,
                                                                                                         1),
                                                                //bottom left top

                                                                new CustomVertex.PositionColoredTextured(neg, pos, pos,
                                                                                                         ALPHACOLOR, 0,
                                                                                                         1),
                                                                //bottom left top
                                                                new CustomVertex.PositionColoredTextured(pos, neg, pos,
                                                                                                         ALPHACOLOR, 1,
                                                                                                         0),
                                                                //top right top
                                                                new CustomVertex.PositionColoredTextured(pos, pos, pos,
                                                                                                         ALPHACOLOR, 1,
                                                                                                         1),
                                                                //bottom right top


                                                                /*
				new CustomVertex.PositionColoredTextured(neg, neg, neg, BOXCOLOR, 0,0), //top left bottom
				new CustomVertex.PositionColoredTextured(neg, pos, neg, BOXCOLOR, 0,1), //bottom left bottom
				new CustomVertex.PositionColoredTextured(pos, neg, neg, BOXCOLOR, 1,0), //top right bottom
				
				new CustomVertex.PositionColoredTextured(neg, pos, neg, BOXCOLOR, 0,1), //bottom left bottom
				new CustomVertex.PositionColoredTextured(pos, pos, neg, BOXCOLOR, 1,1), //bottom right bottom
				new CustomVertex.PositionColoredTextured(pos, neg, neg, BOXCOLOR, 1,0), //top right bottom
				*/

                                                                new CustomVertex.PositionColoredTextured(pos, neg, neg,
                                                                                                         ALPHACOLOR, 1,
                                                                                                         1),
                                                                //top left sideR
                                                                new CustomVertex.PositionColoredTextured(pos, pos, neg,
                                                                                                         ALPHACOLOR, 0,
                                                                                                         1),
                                                                //bottom left sideR
                                                                new CustomVertex.PositionColoredTextured(pos, neg, pos,
                                                                                                         ALPHACOLOR, 1,
                                                                                                         0),
                                                                //top right sideR
				
                                                                new CustomVertex.PositionColoredTextured(pos, pos, neg,
                                                                                                         ALPHACOLOR, 0,
                                                                                                         1),
                                                                //bottom left sideR
                                                                new CustomVertex.PositionColoredTextured(pos, pos, pos,
                                                                                                         ALPHACOLOR, 0,
                                                                                                         0),
                                                                //bottom right sideR
                                                                new CustomVertex.PositionColoredTextured(pos, neg, pos,
                                                                                                         ALPHACOLOR, 1,
                                                                                                         0),
                                                                //top right sideR


                                                                new CustomVertex.PositionColoredTextured(neg, neg, neg,
                                                                                                         ALPHACOLOR, 1,
                                                                                                         1),
                                                                //top left sideL
                                                                new CustomVertex.PositionColoredTextured(neg, neg, pos,
                                                                                                         ALPHACOLOR, 1,
                                                                                                         0),
                                                                //top right sidel
                                                                new CustomVertex.PositionColoredTextured(neg, pos, neg,
                                                                                                         ALPHACOLOR, 0,
                                                                                                         1),
                                                                //bottom left sideL

                                                                new CustomVertex.PositionColoredTextured(neg, pos, neg,
                                                                                                         ALPHACOLOR, 0,
                                                                                                         1),
                                                                //bottom left sideL
                                                                new CustomVertex.PositionColoredTextured(neg, neg, pos,
                                                                                                         ALPHACOLOR, 1,
                                                                                                         0),
                                                                //top right sideL
                                                                new CustomVertex.PositionColoredTextured(neg, pos, pos,
                                                                                                         ALPHACOLOR, 0,
                                                                                                         0),
                                                                //bottom right sideL


                                                                new CustomVertex.PositionColoredTextured(neg, pos, neg,
                                                                                                         ALPHACOLOR, 0,
                                                                                                         1),
                                                                //top left front
                                                                new CustomVertex.PositionColoredTextured(neg, pos, pos,
                                                                                                         ALPHACOLOR, 0,
                                                                                                         0),
                                                                //bottom left front
                                                                new CustomVertex.PositionColoredTextured(pos, pos, neg,
                                                                                                         ALPHACOLOR, 1,
                                                                                                         1),
                                                                //top right front
				
                                                                new CustomVertex.PositionColoredTextured(neg, pos, pos,
                                                                                                         ALPHACOLOR, 0,
                                                                                                         0),
                                                                //bottom left front
                                                                new CustomVertex.PositionColoredTextured(pos, pos, pos,
                                                                                                         ALPHACOLOR, 1,
                                                                                                         0),
                                                                //bottom right front
                                                                new CustomVertex.PositionColoredTextured(pos, pos, neg,
                                                                                                         ALPHACOLOR, 1,
                                                                                                         1),
                                                                //top right front	


                                                                new CustomVertex.PositionColoredTextured(neg, neg, neg,
                                                                                                         ALPHACOLOR, 0,
                                                                                                         1),
                                                                //top left back
                                                                new CustomVertex.PositionColoredTextured(pos, neg, neg,
                                                                                                         ALPHACOLOR, 1,
                                                                                                         1),
                                                                //top right back
                                                                new CustomVertex.PositionColoredTextured(neg, neg, pos,
                                                                                                         ALPHACOLOR, 0,
                                                                                                         0),
                                                                //bottom left back

                                                                new CustomVertex.PositionColoredTextured(neg, neg, pos,
                                                                                                         ALPHACOLOR, 0,
                                                                                                         0),
                                                                //bottom left back
                                                                new CustomVertex.PositionColoredTextured(pos, neg, neg,
                                                                                                         ALPHACOLOR, 1,
                                                                                                         1),
                                                                //top right back
                                                                new CustomVertex.PositionColoredTextured(pos, neg, pos,
                                                                                                         ALPHACOLOR, 1,
                                                                                                         0),
                                                                //bottom right back
                                                              };

    #endregion

    public RenderControl() {
      InitializeComponent();
    }

    protected override void OnPaint(PaintEventArgs pe) {
      if (Visible) Render();

      // OnPaint-Basisklasse wird aufgerufen
      base.OnPaint(pe);
    }

    public void Initialize() {
      Caps hardware = Manager.GetDeviceCaps(0, DeviceType.Hardware);


      CreateFlags flags = CreateFlags.SoftwareVertexProcessing;

      if (hardware.DeviceCaps.SupportsHardwareTransformAndLight)
        flags = CreateFlags.HardwareVertexProcessing;

      //if (hardware.DeviceCaps.SupportsPureDevice)
      //    flags |= CreateFlags.PureDevice;

      DEVICE = new Device(0, DeviceType.Hardware, this, flags, GetPP());
      DEVICE.DeviceReset += delegate {
                              DEVICE.RenderState.Lighting = false;
                              DEVICE.RenderState.CullMode = Cull.None;
                              DEVICE.RenderState.FillMode = Program.CONFIG.FillMode;
                              DEVICE.RenderState.AntiAliasedLineEnable = true;
                              DEVICE.RenderState.PointSize = 3.0f;

                              if (Program.Arguments.AntiAlias)
                                DEVICE.RenderState.MultiSampleAntiAlias = true;

                              //Alpha
                              DEVICE.RenderState.SourceBlend = Blend.SourceAlpha;
                              DEVICE.RenderState.DestinationBlend = Blend.InvSourceAlpha;
                              DEVICE.RenderState.AlphaBlendEnable = true;
                              DEVICE.RenderState.AlphaTestEnable = true;

                              //Texture Blending
//                                          DEVICE.TextureState[0].AlphaOperation = TextureOperation.BlendTextureAlpha;
//                                          DEVICE.TextureState[0].AlphaArgument0 = TextureArgument.TextureColor;
//                                          DEVICE.TextureState[0].AlphaArgument1 = TextureArgument.AlphaReplicate;
                              DEVICE.TextureState[0].AlphaOperation = TextureOperation.SelectArg1;
                              DEVICE.TextureState[0].AlphaArgument1 = TextureArgument.TextureColor;
                            };
      POLYGON = Mesh.Sphere(DEVICE, 256f, 4, 2);
      CURSOR = Mesh.Cylinder(DEVICE, 128.0f, 0.0f, 256f, 6, 2);
      OBJECT = Mesh.Box(DEVICE, 1f, 1f, 1f);
      LIGHT = Mesh.Cylinder(DEVICE, 64f, 64f, 48f, 8, 1);
      _BOX = Mesh.Box(DEVICE, 1f, 1f, 1f);
      SHAPE_BOX = Mesh.Box(DEVICE, 1f, 1f, 100f);
      SHAPE_CIRCLE = Mesh.Cylinder(DEVICE, 1f, 1f, 100f, 24, 1);

      var redalpha = new Bitmap(1, 1);
      redalpha.SetPixel(0, 0, Color.FromArgb(128, Color.Red));
      REDALPHA = new Texture(DEVICE, redalpha, Usage.None, Pool.Managed);

      var greenalpha = new Bitmap(1, 1);
      greenalpha.SetPixel(0, 0, Color.FromArgb(128, Color.Green));
      GREENALPHA = new Texture(DEVICE, greenalpha, Usage.None, Pool.Managed);


      var red = new Bitmap(1, 1);
      red.SetPixel(0, 0, Color.Red);
      RED = new Texture(DEVICE, red, Usage.None, Pool.Managed);

      var blue = new Bitmap(1, 1);
      blue.SetPixel(0, 0, Color.Blue);
      BLUE = new Texture(DEVICE, blue, Usage.None, Pool.Managed);

      var yellow = new Bitmap(1, 1);
      yellow.SetPixel(0, 0, Color.Yellow);
      YELLOW = new Texture(DEVICE, yellow, Usage.None, Pool.Managed);

      var ora = new Bitmap(1, 1);
      ora.SetPixel(0, 0, Color.Orange);
      ORANGE = new Texture(DEVICE, ora, Usage.None, Pool.Managed);

      var green = new Bitmap(1, 1);
      green.SetPixel(0, 0, Color.LightGreen);
      GREEN = new Texture(DEVICE, green, Usage.None, Pool.Managed);

      var dgreen = new Bitmap(1, 1);
      dgreen.SetPixel(0, 0, Color.DarkGreen);
      DARKGREEN = new Texture(DEVICE, dgreen, Usage.None, Pool.Managed);

      var OBJFRAME = new Bitmap(1, 1);
      OBJFRAME.SetPixel(0, 0, Color.DarkGray);
      DARKGRAY = new Texture(DEVICE, OBJFRAME, Usage.None, Pool.Managed);

      var alph = new Bitmap(1, 1);
      //alph.SetPixel(0, 0, Color.FromArgb(160, Color.White));
      alph.SetPixel(0, 0, Color.WhiteSmoke);
      OBJSOLID = new Texture(DEVICE, alph, Usage.None, Pool.Managed);

      var hc = new Bitmap(1, 1);
      hc.SetPixel(0, 0, Color.White);
      HC = new Texture(DEVICE, hc, Usage.None, Pool.Managed);

      var v = new Bitmap(1, 1);
      v.SetPixel(0, 0, Color.Violet);
      VIOLETT = new Texture(DEVICE, v, Usage.None, Pool.Managed);

      string err;
      SHADER = Effect.FromString(DEVICE, Encoding.Default.GetString(Resources.render), null, null, ShaderFlags.None, null, out err);

      if (SHADER == null || !string.IsNullOrEmpty(err)) {
        MessageBox.Show(err ?? "shader not found", "Shader Error");
        Environment.Exit(0);
      }

      DEVICE.Reset(GetPP());

      RebuildGrid();
    }

    public PresentParameters GetPP() {
      Format f = Manager.Adapters[0].CurrentDisplayMode.Format;
      var pp = new PresentParameters();
      pp.Windowed = true;
      pp.SwapEffect = SwapEffect.Discard;
      pp.EnableAutoDepthStencil = true;
      pp.AutoDepthStencilFormat = DepthFormat.D24S8;
      pp.BackBufferCount = 1;
      pp.BackBufferFormat = f;
      pp.BackBufferHeight = Height;
      pp.BackBufferWidth = Width;

      if (Program.Arguments.AntiAlias) {
        int res, h;
        if (
          !Manager.CheckDeviceMultiSampleType(0, DeviceType.Hardware, f, true, MultiSampleType.NonMaskable, out res,
                                              out h))
          Program.Arguments.AntiAlias = false;
        else
        {
          pp.MultiSample = MultiSampleType.NonMaskable;
          pp.MultiSampleQuality = Math.Min(2, h - 1);
        }
      }

      return pp;
    }

    public void Deinitialize() {
      SHADER.Dispose();
      Environment.Exit(0); //clean..!
      DEVICE.Dispose();
      DEVICE = null;
    }

    public void Render() {
      if (DEVICE == null || (!Visible)) return;

      Watch.Start();
      {
        if (!Zone.BUSY && Program.CONFIG.AdaptiveDegeneration) {
          if (fps < 40)
            ADCounter--;
          else if (fps > 60)
            ADCounter++;

          ADCounter = Math.Min(Math.Max(50, ADCounter), 150);
        }

        SurfaceDescription s = DEVICE.GetRenderTarget(0).Description;

        var vp = new Viewport();
        vp.Width = s.Width;
        vp.Height = s.Height;
        vp.X = DEVICE.Viewport.X;
        vp.Y = DEVICE.Viewport.Y;
        vp.MinZ = DEVICE.Viewport.MinZ;
        vp.MaxZ = DEVICE.Viewport.MaxZ;
        DEVICE.Viewport = vp;

        DEVICE.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Program.CONFIG.Background, 1.0f, 0);

        DEVICE.BeginScene();

        #region Camera

        bool ortho = Program.FORM.ortho || Program.MODE == Program.eMode.SoundEdit;

        var cam = new Vector3();

        if (ortho) {
          CAMERA_ROT_SIDE = 0; //no rots on ortho
          CAMERA_ROT_HEIGHT = (float) Math.PI/2 - 0.01f;
        }

        var c = (float) (Math.Cos(CAMERA_ROT_HEIGHT)*CAMERA_DIST); //teilstück von dist

        cam.X = (float) (CAMERA_TARGET.X + (Math.Sin(CAMERA_ROT_SIDE)*c));
        cam.Y = (float) (CAMERA_TARGET.Y + (Math.Cos(CAMERA_ROT_SIDE)*c));

        if (!ortho) cam.Z = (float) (CAMERA_TARGET.Z + (Math.Sin(CAMERA_ROT_HEIGHT)*CAMERA_DIST));
        else cam.Z = CAMERA_TARGET.Z + 10000;

        CAMERA = cam;

        float nearp = 100.0f;
        float farp = Program.CONFIG.Clipping;

        if (!ortho) {
          if (CAMERA_DIST < 4096) farp = 8192*2;
          if (CAMERA_DIST < 2048)
            farp = 8192*1;
          if (CAMERA_DIST < 1024)
            farp = 8192*0.5f;
          if (CAMERA_DIST < 512)
            farp = 8192*0.25f;

          DEVICE.Transform.Projection =
            Matrix.PerspectiveFovLH((float) Math.PI/4, (float) s.Width/s.Height, nearp, farp);
        }
        else {
          float fl = CAMERA_DIST/100;
          DEVICE.Transform.Projection = Matrix.OrthoLH(s.Width*fl, s.Height*fl, 0.00000001f, 6500000f);
        }

        DEVICE.Transform.View = Matrix.LookAtLH(cam, CAMERA_TARGET, new Vector3(0, 0, 1));
        //DEVICE.Transform.View = Matrix.LookAtLH(Vector3.Empty, Vector3.Empty, new Vector3(0, 0, 1));

        #endregion

        #region Heightmap

        if (Program.ZONE != null && Program.CONFIG.ShowHeightmap) {
          DEVICE.SetTexture(0, Program.ZONE.TextureMap);
          bool usePM = Program.CONFIG.UsePatchmaps;

          for (int a = 0; a < 8; a++) {
            for (int b = 0; b < 8; b++) {
              Mesh m = SUBZONES[a, b];

              if (m == null) continue;

              DEVICE.Transform.World = GetSubMatrix(a, b);

              if (usePM)
                PatchMap.Render(a, b, SUBZONES[a, b]);
              else if (Program.ZONE.TextureMap != null)
                SUBZONES[a, b].DrawSubset(0);

              if (Program.MODE == Program.eMode.Heightmap || Program.MODE == Program.eMode.Smooth) {
                if (Program.CONFIG.FillMode != FillMode.WireFrame) {
                  DEVICE.RenderState.FillMode = FillMode.WireFrame;
                  DEVICE.SetTexture(0, HC);
                  SUBZONES[a, b].DrawSubset(0);

                  DEVICE.SetTexture(0, Program.ZONE.TextureMap);
                  DEVICE.RenderState.FillMode = Program.CONFIG.FillMode;
                }
              }
            }
          }
        }

        #endregion

        #region Objects

        if (Program.CONFIG.ShowObjects) {
          DEVICE.SetSamplerState(0, SamplerStageStates.MagFilter, (int) TextureFilter.Anisotropic);
          DEVICE.SetSamplerState(0, SamplerStageStates.MinFilter, (int) TextureFilter.Anisotropic);
          DEVICE.SetSamplerState(0, SamplerStageStates.MipFilter, (int) TextureFilter.Anisotropic);

          Texture col = Program.CONFIG.ObjectWireColor ? RED : DARKGRAY;
          lock (Objects.Fixtures) {
            foreach (Objects.Fixture f in Objects.Fixtures) {
              double dist = Utils.GetDistance(new Vector3(f.X, f.Y, f.Z), CAMERA);

              if (ortho)
                dist *= 2;

              if (f.Hidden || dist > Program.CONFIG.ObjectRenderDistance || dist > farp)
                continue;

              bool hasModel = f.NIF.Model != null;
              bool solid = hasModel && Program.CONFIG.Objects.ShowModelSolid && !f.WireFrame;
              bool wire = hasModel && Program.CONFIG.Objects.ShowModelWire &&
                          (f == Program.FORM.CurrentFixture /*|| !f.NIF.Model.hasTextures*/);
              bool bb = !hasModel || Program.CONFIG.Objects.AlwaysShowBounding;

              if ( /*Program.CONFIG.AdaptiveDegeneration*/ true) {
                int fac = 1;

                if (ortho)
                  fac = 2;

                int far = Program.CONFIG.ObjectRenderDistance*ADCounter*fac/100;
                int middle = Program.CONFIG.ObjectRenderDistance/2*ADCounter*fac/100;
                //int near = Program.CONFIG.ObjectRenderDistance / 2 * ADCounter * fac / 100;

                if (dist > far) continue;
                if (dist > middle) {
                  solid = false;
                  wire = false;
                  bb = true;
                }
                //else if (dist > near) {
                //    wire = true;
                //    solid = false;
                //}
              }

              if (hasModel) //.obj model
              {
                DEVICE.Transform.World = GetFixtureMatrix(f, false);

                if (solid) {
                  DEVICE.SetTexture(0, OBJSOLID);
                  f.NIF.Model.Render();
                  if (Program.CONFIG.Objects.AlwaysWireframe)
                    wire = true;
                }

                if (wire) {
                  DEVICE.SetTexture(0, (f == Program.FORM.CurrentFixture ? BLUE : col));
                  DEVICE.RenderState.FillMode = FillMode.WireFrame;
                  f.NIF.Model.Render();
                  DEVICE.RenderState.FillMode = Program.CONFIG.FillMode;
                }
              }

              if (bb) //bounding box
              {
                DEVICE.Transform.World = GetFixtureMatrix(f, true);
                DEVICE.SetTexture(0, (f == Program.FORM.CurrentFixture ? BLUE : null));
                //DEVICE.VertexFormat = CustomVertex.PositionColoredTextured.Format;

                DEVICE.RenderState.FillMode = FillMode.WireFrame;
                //Cull prev = DEVICE.RenderState.CullMode;
                //DEVICE.RenderState.CullMode = Cull.Clockwise; //needed because of alpha
                //DEVICE.DrawUserPrimitives(PrimitiveType.TriangleList, BOX.Length/3, BOX);
                _BOX.DrawSubset(0);
                //DEVICE.RenderState.CullMode = prev;
                DEVICE.RenderState.FillMode = Program.CONFIG.FillMode;

                //DEVICE.SetTexture(0, null);
                //DEVICE.VertexFormat = CustomVertex.PositionColored.Format;
                //DEVICE.DrawUserPrimitives(PrimitiveType.LineList, 3, ARROW);
              }
            }
          }
        }

        #endregion

        #region Lights

        lock (Light.Lights) {
          foreach (Light l in Light.Lights) {
            switch (l.Color) {
              default:
                DEVICE.SetTexture(0, YELLOW);
                break;
              case LightColor.White:
                DEVICE.SetTexture(0, HC);
                break;
              case LightColor.GreenWhite:
                DEVICE.SetTexture(0, GREEN);
                break;
              case LightColor.GreenYellow:
                DEVICE.SetTexture(0, GREEN);
                break;
              case LightColor.OrangeWhite:
                DEVICE.SetTexture(0, ORANGE);
                break;
              case LightColor.OrangeYellow:
                DEVICE.SetTexture(0, ORANGE);
                break;
              case LightColor.BlueWhite:
                DEVICE.SetTexture(0, BLUE);
                break;
              case LightColor.RedWhite:
                DEVICE.SetTexture(0, RED);
                break;
              case LightColor.TurqoiseWhite:
                DEVICE.SetTexture(0, BLUE);
                break;
              case LightColor.VioletWhite:
                DEVICE.SetTexture(0, VIOLETT);
                break;
              case LightColor.Yellow:
                DEVICE.SetTexture(0, YELLOW);
                break;
            }

            DEVICE.Transform.World = GetLightMatrix(l);

            if (Program.MODE == Program.eMode.Lights) {
              LIGHT.DrawSubset(0);
              DEVICE.SetTexture(0, null);
            }
            DEVICE.RenderState.FillMode = FillMode.WireFrame;
            LIGHT.DrawSubset(0);
            DEVICE.RenderState.FillMode = Program.CONFIG.FillMode;
          }
        }

        #endregion

        #region Grid

        if (GRID != null && Program.CONFIG.ShowGrid) {
          DEVICE.SetTexture(0, null);
          DEVICE.RenderState.FillMode = FillMode.WireFrame;
          DEVICE.Transform.World = GetGridMatrix();
          GRID.DrawSubset(0);
          DEVICE.RenderState.FillMode = Program.CONFIG.FillMode;
        }

        #endregion

        #region Filled Polygons

        if (Program.CONFIG.ShowFilledPolygons) {
          //Settings
          DEVICE.Transform.World = Matrix.Identity;
          DEVICE.VertexFormat = CustomVertex.PositionColored.Format;

          #region Water

          lock (Polygon.Polygons) {
            DEVICE.SetTexture(0, null);

            Cull prev = DEVICE.RenderState.CullMode;
            DEVICE.RenderState.CullMode = Cull.Clockwise;

            foreach (Polygon p in Polygon.Polygons) {
              if (p.Type == ePolygon.Water) {
                var tri = new CustomVertex.PositionColored[p.Points.Count];
                int index = 0;

                int al = Color.FromArgb(128, p.Color).ToArgb();

                foreach (Vector2 vec in p.Points) {
                  float x = vec.X;
                  float y = vec.Y;
                  float z = p.WHeight;

                  tri[index] = new CustomVertex.PositionColored(x, y, z, al);
                  index++;
                }

                if (tri.Length > 2)
                  DEVICE.DrawUserPrimitives(PrimitiveType.TriangleStrip, tri.Length - 2, tri);
              }
            }

            DEVICE.RenderState.CullMode = prev;
          }

          #endregion

          #region Zonejump

          {
            DEVICE.SetTexture(0, YELLOW);

            Cull prev = DEVICE.RenderState.CullMode;
            DEVICE.RenderState.CullMode = Cull.CounterClockwise;

            lock (Zonejump.Zonejumps) {
              foreach (Zonejump j in Zonejump.Zonejumps) {
                var tri = new CustomVertex.PositionColored[4];

                tri[0] = new CustomVertex.PositionColored(j.First, ALPHACOLOR); //top left
                tri[1] =
                  new CustomVertex.PositionColored(new Vector3(j.Second.X, j.Second.Y, j.First.Z), ALPHACOLOR);
                //top right
                tri[2] =
                  new CustomVertex.PositionColored(new Vector3(j.First.X, j.First.Y, j.Second.Z), ALPHACOLOR);
                //bottom left
                tri[3] = new CustomVertex.PositionColored(j.Second, ALPHACOLOR); //bottom right

                DEVICE.DrawUserPrimitives(PrimitiveType.TriangleStrip, 2, tri);
              }
            }

            DEVICE.RenderState.CullMode = prev;
          }

          #endregion
        }

        #endregion

        //Reset device to 2D Settings
        DEVICE.Clear(ClearFlags.ZBuffer, Color.Empty, 1.0f, 0);

        #region Sounds

        if (Program.MODE == Program.eMode.SoundEdit && SoundMgr.CurrentRegion != null) {
          SHADER.Begin(FX.None);


          for (int i = 1; i <= 2; i++) {
            var col = new float[4];

            foreach (Shape sh in (i == 1 ? SoundMgr.CurrentRegion.Shapes : SoundMgr.CurrentRegion.xShapes)) {
              if (SoundMgr.CurrentShape == sh) col = new float[4] {0, 0, 1, 0.25f};
              else if (i == 1) col = new float[4] {1, 0, 0, 0.25f};
              else if (i == 2) col = new float[4] {0, 1, 0, 0.25f};

              SHADER.SetValue(EffectHandle.FromString("color"), col);
              SHADER.BeginPass(0); //colorize
              Matrix mat;
              Mesh mesh = GetShapeData(sh, out mat);
              DEVICE.Transform.World = mat;
              mesh.DrawSubset(0);
              SHADER.EndPass();
            }
          }
          SHADER.End();
        }

        #endregion

        #region Polygons

        if (Program.CONFIG.ShowPolygons) {
          float dist = CAMERA_DIST/5000/4;

          if (ortho)
            dist *= 5;

          dist = Math.Min(dist, 1);
          Matrix sc = Matrix.Scaling(dist, dist, dist);

          lock (Polygon.Polygons) {
            foreach (Polygon p in Polygon.Polygons) {
              lock (p.Points) {
                int color = p.Color.ToArgb();

                if (p.Type == ePolygon.None || p.Type == ePolygon.Bounding) {
                  //Create 3D Vectors
                  var vecs = new CustomVertex.PositionColored[p.Points.Count];
                  int index = 0;

                  foreach (Vector2 vec in p.Points) {
                    float x = vec.X;
                    float y = vec.Y;
                    float z = GetPolygonZ(p, vec);

                    vecs[index++] = new CustomVertex.PositionColored(x, y, z, color);

                    //Draw 'Dot'
                    DEVICE.Transform.World = sc*Matrix.Translation(x, y, z);

                    if (Program.FORM.m_CurrentPolygon == p && Program.FORM.m_CurrentPolyIndex == (index - 1))
                      DEVICE.SetTexture(0, YELLOW);
                    else DEVICE.SetTexture(0, (index == 1 ? BLUE : RED));
                    POLYGON.DrawSubset(0);

                    DEVICE.RenderState.FillMode = FillMode.WireFrame;
                    DEVICE.SetTexture(0, HC);
                    POLYGON.DrawSubset(0);
                    DEVICE.RenderState.FillMode = Program.CONFIG.FillMode;
                  }

                  //Add last
                  //vecs[index] = vecs[0];

                  //Draw Lines
                  DEVICE.SetTexture(0, null);
                  DEVICE.Transform.World = Matrix.Identity;
                  DEVICE.VertexFormat = CustomVertex.PositionColored.Format;
                  DEVICE.DrawUserPrimitives(PrimitiveType.LineStrip, vecs.Length - 1, vecs);

                  if (p.Type == ePolygon.Bounding && p.Points.Count > 1) {
                    var arr = new CustomVertex.PositionColored[(vecs.Length - 1)*2];
                    for (int i = 0; i < vecs.Length - 1; i++) {
                      CustomVertex.PositionColored v1 = vecs[i];
                      CustomVertex.PositionColored v2 = vecs[i + 1];

                      float middleX = (v1.X + v2.X)/2;
                      float middleY = (v1.Y + v2.Y)/2;
                      float middleZ = (v1.Z + v2.Z)/2;

                      float relX = v2.X - v1.X;
                      float relY = v2.Y - v1.Y;

                      float steigungBound = (relY)/(relX);
                      float steigungPfeil = (-1f)/steigungBound;


                      float pdist = 128;
                      bool inv = false;

                      if (relY >= 0) inv = true;

                      if (inv)
                        pdist = -pdist;

                      float velUn = steigungPfeil*pdist;
                      float velLim = Math.Max(Math.Min(velUn, Math.Abs(pdist)), -Math.Abs(pdist));

                      float targetX = middleX + pdist*(Math.Abs(velLim)/Math.Abs(velUn));
                      float targetY = middleY + velLim;

                      int pcol = (Color.Orange).ToArgb();

                      arr[i*2] = new CustomVertex.PositionColored(middleX, middleY, middleZ, pcol);
                      arr[i*2 + 1] = new CustomVertex.PositionColored(targetX, targetY, middleZ, pcol);
                    }
                    DEVICE.DrawUserPrimitives(PrimitiveType.LineList, arr.Length/2, arr);
                  }
                }
                if (p.Type == ePolygon.Water) {
                  var left = new CustomVertex.PositionColored[p.Points.Count/2];
                  var right = new CustomVertex.PositionColored[p.Points.Count/2];
                  int index = 0;
                  int total = 0;
                  int num = 0;

                  foreach (Vector2 vec in p.Points) {
                    float x = vec.X;
                    float y = vec.Y;
                    float z = GetPolygonZ(p, vec);

                    if (index < p.Points.Count/2) {
                      if (num%2 == 0) left[index] = new CustomVertex.PositionColored(x, y, z, color);
                      else right[index] = new CustomVertex.PositionColored(x, y, z, color);
                    }

                    //Draw 'Dot'
                    DEVICE.Transform.World = sc*Matrix.Translation(x, y, z);

                    num++;

                    if (Program.FORM.m_CurrentPolygon == p && Program.FORM.m_CurrentPolyIndex == total)
                      DEVICE.SetTexture(0, YELLOW);
                    else if (index == 0) DEVICE.SetTexture(0, BLUE);
                    else DEVICE.SetTexture(0, (num%2 == 0 ? GREEN : DARKGREEN));
                    POLYGON.DrawSubset(0);

                    DEVICE.RenderState.FillMode = FillMode.WireFrame;
                    DEVICE.SetTexture(0, HC);
                    POLYGON.DrawSubset(0);
                    DEVICE.RenderState.FillMode = Program.CONFIG.FillMode;

                    if (num%2 == 0) index++;

                    total++;
                  }

                  //Draw Lines

                  DEVICE.SetTexture(0, null);
                  DEVICE.Transform.World = Matrix.Identity;
                  DEVICE.VertexFormat = CustomVertex.PositionColored.Format;
                  if (left.Length > 1)
                    DEVICE.DrawUserPrimitives(PrimitiveType.LineStrip, left.Length - 1, left);
                  if (right.Length > 1)
                    DEVICE.DrawUserPrimitives(PrimitiveType.LineStrip, right.Length - 1, right);
                }
              }
            }
          }
        }

        #endregion

        #region Zonejump

        if (Program.CONFIG.ShowZonejumps) {
          Matrix nodeScale = Matrix.Scaling(0.1f, 0.1f, 0.1f);

          foreach (Zonejump j in Zonejump.Zonejumps) {
            var lines = new CustomVertex.PositionColored[6];

            Vector3 topleft = j.First;
            var topright = new Vector3(j.Second.X, j.Second.Y, j.First.Z);
            var bottomleft = new Vector3(j.First.X, j.First.Y, j.Second.Z);
            Vector3 bottomright = j.Second;

            lines[0] = new CustomVertex.PositionColored(topleft, Color.Orange.ToArgb());
            lines[1] = new CustomVertex.PositionColored(topright, Color.Orange.ToArgb());
            lines[2] = new CustomVertex.PositionColored(bottomright, Color.Orange.ToArgb());
            lines[3] = new CustomVertex.PositionColored(bottomleft, Color.Orange.ToArgb());
            lines[4] = lines[0];
            lines[5] = lines[2];

            //Draw Nodes
            DEVICE.SetTexture(0, (Program.FORM.m_CurrentZonejump == j && j.EditIndex == 1 ? YELLOW : ORANGE));
            DEVICE.Transform.World = nodeScale*Matrix.Translation(j.First);
            POLYGON.DrawSubset(0);
            DEVICE.SetTexture(0, (Program.FORM.m_CurrentZonejump == j && j.EditIndex == 2 ? YELLOW : ORANGE));
            DEVICE.Transform.World = nodeScale*Matrix.Translation(j.Second);
            POLYGON.DrawSubset(0);

            //Draw Lines
            DEVICE.SetTexture(0, null);
            DEVICE.Transform.World = Matrix.Identity;
            DEVICE.VertexFormat = CustomVertex.PositionColored.Format;
            DEVICE.DrawUserPrimitives(PrimitiveType.LineStrip, 5, lines);
          }
        }

        #endregion

        #region Cursor

        if (Program.CONFIG.ShowCursor) {
          float dist = CAMERA_DIST/5000/3;
          DEVICE.Transform.World = Matrix.Scaling(dist, dist, dist)*
                                   Matrix.Translation(Program.FORM.lastMapPos.X, Program.FORM.lastMapPos.Y,
                                                      Program.FORM.lastMapPos.Z);
          DEVICE.SetTexture(0, BLUE);
          CURSOR.DrawSubset(0);
          DEVICE.SetTexture(0, HC);

          DEVICE.RenderState.FillMode = FillMode.WireFrame;
          CURSOR.DrawSubset(0);
          DEVICE.RenderState.FillMode = Program.CONFIG.FillMode;
        }

        #endregion

        #region Ruler

        if (Program.MODE == Program.eMode.Ruler) {
          //Draw Line
          DEVICE.SetTexture(0, null);
          DEVICE.Transform.World = Matrix.Identity;

          var l = new CustomVertex.PositionColored[10];

          const int xsize = 96;

          l[0] =
            new CustomVertex.PositionColored(Program.TOOL_RULER.Source + new Vector3(-xsize, -xsize, 0),
                                             Color.Red.ToArgb());
          l[1] =
            new CustomVertex.PositionColored(Program.TOOL_RULER.Source + new Vector3(+xsize, +xsize, 0),
                                             Color.Red.ToArgb());

          l[2] =
            new CustomVertex.PositionColored(Program.TOOL_RULER.Source + new Vector3(-xsize, +xsize, 0),
                                             Color.Red.ToArgb());
          l[3] =
            new CustomVertex.PositionColored(Program.TOOL_RULER.Source + new Vector3(+xsize, -xsize, 0),
                                             Color.Red.ToArgb());

          l[4] =
            new CustomVertex.PositionColored(Program.TOOL_RULER.Destination + new Vector3(-xsize, -xsize, 0),
                                             Color.Red.ToArgb());
          l[5] =
            new CustomVertex.PositionColored(Program.TOOL_RULER.Destination + new Vector3(+xsize, +xsize, 0),
                                             Color.Red.ToArgb());

          l[6] =
            new CustomVertex.PositionColored(Program.TOOL_RULER.Destination + new Vector3(-xsize, +xsize, 0),
                                             Color.Red.ToArgb());
          l[7] =
            new CustomVertex.PositionColored(Program.TOOL_RULER.Destination + new Vector3(+xsize, -xsize, 0),
                                             Color.Red.ToArgb());

          l[8] = new CustomVertex.PositionColored(Program.TOOL_RULER.Source, Color.Red.ToArgb());
          l[9] = new CustomVertex.PositionColored(Program.TOOL_RULER.Destination, Color.Red.ToArgb());


          DEVICE.VertexFormat = CustomVertex.PositionColored.Format;
          DEVICE.DrawUserPrimitives(PrimitiveType.LineList, 5, l);
        }

        #endregion

        DEVICE.EndScene();

        DEVICE.Present();
      }
      fps = ((fps*49 + 1000f/Watch.ElapsedMilliseconds)/50f);
      Watch.Stop();
      Watch.Reset();
    }

    public void CreateZone() {
      foreach (Mesh m in SUBZONES) if (m != null) m.Dispose();

      //We divide every zone into 8x8 Subzones for performance
      for (int a = 0; a < 8; a++) for (int b = 0; b < 8; b++) CreateSubZone(a, b);
    }

    public void CreateSubZone(int subx, int suby) {
      int xVectors = (subx < 7 ? 33 : 32); //connection points
      int yVectors = (suby < 7 ? 33 : 32);

      int xTri = xVectors - 1;
      int yTri = yVectors - 1;

      var elem =
        new[] {
                new VertexElement(0, 0, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.Position, 0)
                ,
                new VertexElement(0, 12, DeclarationType.Float2, DeclarationMethod.Default,
                                  DeclarationUsage.TextureCoordinate, 0),
                new VertexElement(0, 20, DeclarationType.Float2, DeclarationMethod.Default,
                                  DeclarationUsage.TextureCoordinate, 1),
                VertexElement.VertexDeclarationEnd
              };

      GridElem = elem;
      GridDecl = new VertexDeclaration(DEVICE, elem);
      GridSize = 28;

      var m =
        new Mesh(xTri*yTri*2, xVectors*yVectors, MeshFlags.Managed, elem, DEVICE);

      using (VertexBuffer vb = m.VertexBuffer) {
        GraphicsStream vd = vb.Lock(0, 0, LockFlags.None);
        for (int y = 0; y < yVectors; y++) //0-31 => 32x32 points (256/8)
        {
          for (int x = 0; x < xVectors; x++) {
            //vd.Write(
            //    new CustomVertex.PositionTextured(x*256, y*256,
            //                                      Program.ZONE.HeightMap[subx*32 + x, subY*32 + y],
            //                                      //((subx*32) + x)/256f, ((subY*32) + y)/256f));
            //                                      (x / 32.99f)+0.01f, (y / 32.99f)+0.01f));
            vd.Write(new float[] {x*256, y*256, Program.ZONE.HeightMap[subx*32 + x, suby*32 + y]});
            vd.Write(new[] {((subx*32) + x)/256f, ((suby*32) + y)/256f});
            vd.Write(new[] {Utils.LimitTC(x/32f), Utils.LimitTC(y/32f)});
          }
        }

        vb.Unlock();
      }

      using (IndexBuffer ib = m.IndexBuffer) {
        GraphicsStream id = ib.Lock(0, 0, LockFlags.None);

        for (int y = 0; y < yTri; y++) {
          for (int x = 0; x < xTri; x++) {
            id.Write(new[] {
                             (ushort) (x + (y*xVectors)), (ushort) (x + 1 + (y*xVectors)),
                             (ushort) (x + ((y + 1)*xVectors)),
                             (ushort) (x + 1 + (y*xVectors)), (ushort) (x + 1 + ((y + 1)*xVectors)),
                             (ushort) (x + ((y + 1)*xVectors))
                           });
          }
        }

        ib.Unlock();
      }

      SUBZONES[subx, suby] = m;
    }

    public void PatchMesh(Vector3[] data) {
      foreach (Vector3 patch in data) {
        //get subzone
        int subX = (int) patch.X/32;
        int subY = (int) patch.Y/32;

        //get the relative xy
        int relX = (int) patch.X%32;
        int relY = (int) patch.Y%32;

        PatchVector(subX, subY, relX, relY, patch);

        if (relX == 0) PatchVector(subX - 1, subY, 32, relY, patch);
        if (relY == 0) PatchVector(subX, subY - 1, relX, 32, patch);
        if (relX == 0 && relY == 0) PatchVector(subX - 1, subY - 1, 32, 32, patch);
      }
    }

    public void PatchVector(int subX, int subY, int relX, int relY, Vector3 patch) {
      if (subX < 0 || subX > 7 || subY < 0 || subY > 7) return; //invalid

      Mesh sub = SUBZONES[subX, subY];

      int yVectors = (subX < 7 ? 33 : 32);

      //get the vector
      int vector = relY*yVectors + relX;

      //change the vector
      using (VertexBuffer vb = sub.VertexBuffer) {
        GraphicsStream d =
          vb.Lock(vector*GridSize, GridSize,
                  LockFlags.Discard);
        d.Write(new[] {relX*256, relY*256, patch.Z});
        //d.Write(new CustomVertex.PositionTextured(relX*256, relY*256, patch.Z, patch.X/256f, patch.Y/256f));
        vb.Unlock();
      }
    }

    public void RebuildGrid() {
      //we have max 65534 indices
      //so we can have 
      int s = Program.CONFIG.GridSize;
      const int c = 128;
      int md = s*c/2;

      var m = new Mesh(c*c*2, c*c, MeshFlags.Managed, CustomVertex.PositionColored.Format, DEVICE);

      using (VertexBuffer vb = m.VertexBuffer) {
        GraphicsStream vd = vb.Lock(0, 0, LockFlags.None);

        for (int y = 0; y < c; y++) //0-31 => 32x32 points (256/8)
        {
          for (int x = 0; x < c; x++) {
            vd.Write(
              new CustomVertex.PositionColored(x*s - md, y*s - md, 0, Program.CONFIG.GridColor.ToArgb()));
          }
        }

        vb.Unlock();
      }

      using (IndexBuffer ib = m.IndexBuffer) {
        GraphicsStream id = ib.Lock(0, 0, LockFlags.None);

        for (int y = 0; y < c - 1; y++) {
          for (int x = 0; x < c - 1; x++) {
            id.Write(new[] {
                             (ushort) (x + (y*c)),
                             (ushort) (x + 1 + (y*c)),
                             (ushort) (x + ((y + 1)*c)),
                             (ushort) (x + 1 + (y*c)),
                             (ushort) (x + 1 + ((y + 1)*c)),
                             (ushort) (x + ((y + 1)*c))
                           });
          }
        }

        ib.Unlock();
      }

      GRID = m;
    }

    public Matrix GetSubMatrix(int x, int y) {
      return Matrix.Translation(8192*x, 8192*y, 0.0f);
    }

    public Matrix GetGridMatrix() {
      int s = Program.CONFIG.GridSize;

      return
        Matrix.Translation((int) CAMERA_TARGET.X/s*s, (int) CAMERA_TARGET.Y/s*s,
                           (Program.FORM.CurrentFixture == null
                              ? CAMERA_TARGET.Z
                              : Program.FORM.CurrentFixture.Z));
    }

    public Matrix GetFixtureMatrix(Objects.Fixture f, bool applyBounding) {
      float scale = f.Scale/100f;

      Matrix result = Matrix.Identity;

      if (applyBounding) {
        result *= Matrix.Translation(0.5f, 0.5f, 0.5f);
        result *= Matrix.Scaling(f.NIF.Scale);
        result *= Matrix.Translation(f.NIF.Position);
      }

      result *= Matrix.RotationAxis(new Vector3(-f.AxisX, f.AxisY, f.AxisZ), f.Rotation);
      result *= Matrix.Scaling(scale, -scale, scale); //flip x axis
      result *= Matrix.Translation(f.X, f.Y, f.Z); //move object

      return result;
    }

    public Matrix GetLightMatrix(Light l) {
      return Matrix.Scaling(l.Intensity*2, l.Intensity*2, 1)*Matrix.Translation(l.X, l.Y, l.Z + l.ZOffset);
    }

    public Vector3 GetExactMapVectorByClick(Point click) {
      if (Program.ZONE == null)
        return Vector3.Empty;

      var current_intersect = new IntersectInformation();
      int current_x = 0;
      int current_y = 0;
      var rayStart = new Vector3(click.X, click.Y, 0.0f);

      for (int a = 0; a < 8; a++) {
        for (int b = 0; b < 8; b++) {
          Mesh sub = SUBZONES[a, b];

          if (sub == null) continue;

          rayStart.Z = 0.0f;
          Vector3 uRayNear =
            Vector3.Unproject(rayStart, DEVICE.Viewport, DEVICE.Transform.Projection,
                              DEVICE.Transform.View, GetSubMatrix(a, b));

          rayStart.Z = 1.0f;
          Vector3 uRayFar =
            Vector3.Unproject(rayStart, DEVICE.Viewport, DEVICE.Transform.Projection,
                              DEVICE.Transform.View, GetSubMatrix(a, b));

          Vector3 rayDir = Vector3.Subtract(uRayFar, uRayNear);

          IntersectInformation intersect;
          if (sub.Intersect(uRayNear, rayDir, out intersect)) {
            if (current_intersect.Dist == 0 || current_intersect.Dist > intersect.Dist) {
              current_intersect = intersect;
              current_x = a;
              current_y = b;
            }
          }
        }
      }

      if (current_intersect.Dist == 0) return Vector3.Empty; //not hit

      int subX = current_x*32;
      int subY = current_y*32;
      int num = current_intersect.FaceIndex/2;
      int triX = num%(current_x == 7 ? 31 : 32);
      int triY = num/(current_x == 7 ? 31 : 32);
      int add = current_intersect.FaceIndex%2;

      float u = current_intersect.U;
      float v = current_intersect.V;

      if (add == 1) //2nd triangle
      {
        float startU = u;
        float startV = v;
        u = 1 - startV;
        v = startU + startV;
      }

      int x = (subX + triX)*256 + (int) (256*u);
      int y = (subY + triY)*256 + (int) (256*v);

      return new Vector3(x, y, Program.ZONE.HeightMap[(int) Math.Round(x/256f), (int) Math.Round(y/256f)]);
    }

    public Vector3 GetGridLoc(Point click) {
      var current_intersect = new IntersectInformation();
      var rayStart = new Vector3(click.X, click.Y, 0.0f);

      Mesh sub = GRID;

      if (sub == null) return Vector3.Empty;

      rayStart.Z = 0.0f;
      Vector3 uRayNear =
        Vector3.Unproject(rayStart, DEVICE.Viewport, DEVICE.Transform.Projection,
                          DEVICE.Transform.View, GetGridMatrix());

      rayStart.Z = 1.0f;
      Vector3 uRayFar =
        Vector3.Unproject(rayStart, DEVICE.Viewport, DEVICE.Transform.Projection,
                          DEVICE.Transform.View, GetGridMatrix());

      Vector3 rayDir = Vector3.Subtract(uRayFar, uRayNear);

      IntersectInformation intersect;
      if (sub.Intersect(uRayNear, rayDir, out intersect)) {
        if (current_intersect.Dist == 0 || current_intersect.Dist > intersect.Dist)
          current_intersect = intersect;
      }


      if (current_intersect.Dist == 0) return Vector3.Empty; //not hit

      int s = Program.CONFIG.GridSize;
      int current_x = (int) CAMERA_TARGET.X/s*s - (s*128/2);
      int current_y = (int) CAMERA_TARGET.Y/s*s - (s*128/2);

      int num = current_intersect.FaceIndex/2;
      int triX = num%127;
      int triY = num/127;

      int q = Program.CONFIG.GridSize/2;
      int x = current_x + (triX*s);
      int y = current_y + (triY*s);
      x = (int) Math.Round(x/(float) q)*q;
      y = (int) Math.Round(y/(float) q)*q;

      return
        new Vector3(x, y,
                    (Program.FORM.CurrentFixture == null ? CAMERA_TARGET.Z : Program.FORM.CurrentFixture.Z));
    }

    public Vector3 GetRoundedVector(Vector3 exact) {
      exact.X = (int) Math.Round(exact.X/256);
      exact.Y = (int) Math.Round(exact.Y/256);
      return exact;
    }

    public int GetPolygonZ(Polygon p, Vector2 vec) {
      try {
        switch (p.Type) {
          default:
            return Program.ZONE.HeightMap[(int) (vec.X/256), (int) (vec.Y/256)];
          case ePolygon.Water:
            return p.WHeight;
        }
      }
      catch (Exception) {
        return 0;
      }
    }

    public Mesh GetShapeData(Shape sh, out Matrix mat) {
      Mesh m;
      int x = sh.X;
      int y = sh.Y;
      int sx;
      int sy;

      if (sh.Type == ShapeType.Circle) {
        m = SHAPE_CIRCLE;
        sx = sy = sh.Radius;
      }
      else {
        m = SHAPE_BOX;
        sx = sh.EndX - sh.X;
        sy = sh.EndY - sh.Y;
        x += sx/2;
        y += sy/2;
      }

      mat = Matrix.Scaling(sx, sy, 1)*Matrix.Translation(x, y, 0);
      return m;
    }

    public Shape GetSoundByClick(Point click) {
      if (SoundMgr.CurrentRegion == null)
        return null;

      var rayStart = new Vector3(click.X, click.Y, 0.0f); //y is our "far" plane
      float intDist = 0;
      Shape poly = null;

      var shapes = new List<Shape>();
      shapes.AddRange(SoundMgr.CurrentRegion.Shapes);
      shapes.AddRange(SoundMgr.CurrentRegion.xShapes);

      foreach (Shape g in shapes) {
        Matrix m;
        Mesh mesh = GetShapeData(g, out m);
        {
          rayStart.Z = 0.0f;
          Vector3 uRayNear =
            Vector3.Unproject(rayStart, DEVICE.Viewport, DEVICE.Transform.Projection,
                              DEVICE.Transform.View, m);

          rayStart.Z = 1.0f;
          Vector3 uRayFar =
            Vector3.Unproject(rayStart, DEVICE.Viewport, DEVICE.Transform.Projection,
                              DEVICE.Transform.View, m);

          Vector3 rayDir = Vector3.Subtract(uRayFar, uRayNear);

          IntersectInformation info;
          if (g != SoundMgr.CurrentShape && mesh.Intersect(uRayNear, rayDir, out info)) {
            if (poly == null || intDist > info.Dist) {
              intDist = info.Dist;
              poly = g;
            }
          }
        }
      }

      return poly;
    }

    public Polygon GetPolygonByClick(Point click, out int index) {
      var rayStart = new Vector3(click.X, click.Y, 0.0f); //y is our "far" plane
      float intDist = 0;
      Polygon poly = null;
      index = -1;

      foreach (Polygon g in Polygon.Polygons) {
        for (int i = 0; i < g.Points.Count; i++) {
          float x = g.Points[i].X;
          float y = g.Points[i].Y;
          int z = GetPolygonZ(g, g.Points[i]);
          Matrix m = Matrix.Scaling(2.0f, 2.0f, 2.0f)*Matrix.Translation(x, y, z);

          rayStart.Z = 0.0f;
          Vector3 uRayNear =
            Vector3.Unproject(rayStart, DEVICE.Viewport, DEVICE.Transform.Projection,
                              DEVICE.Transform.View, m);

          rayStart.Z = 1.0f;
          Vector3 uRayFar =
            Vector3.Unproject(rayStart, DEVICE.Viewport, DEVICE.Transform.Projection,
                              DEVICE.Transform.View, m);

          Vector3 rayDir = Vector3.Subtract(uRayFar, uRayNear);

          IntersectInformation info;
          if (POLYGON.Intersect(uRayNear, rayDir, out info)) {
            if (poly == null || intDist > info.Dist) {
              intDist = info.Dist;
              poly = g;
              index = i;
            }
          }
        }
      }

      return poly;
    }

    public Objects.Fixture GetFixtureByClick(Point click) {
      var rayStart = new Vector3(click.X, click.Y, 0.0f); //y is our "far" plane
      float intDist = 0;
      Objects.Fixture poly = null;

      foreach (Objects.Fixture g in Objects.Fixtures) {
        if (g.Hidden || g.WireFrame) {
          ///either hidden or locked
          continue;
        }

        Matrix world = GetFixtureMatrix(g, true);

        rayStart.Z = 0.0f;
        Vector3 uRayNear =
          Vector3.Unproject(rayStart, DEVICE.Viewport, DEVICE.Transform.Projection,
                            DEVICE.Transform.View, world);

        rayStart.Z = 1.0f;
        Vector3 uRayFar =
          Vector3.Unproject(rayStart, DEVICE.Viewport, DEVICE.Transform.Projection,
                            DEVICE.Transform.View, world);

        Vector3 rayDir = Vector3.Subtract(uRayFar, uRayNear);

        IntersectInformation info = new IntersectInformation() { Dist = float.MaxValue };
        if (g != Program.FORM.CurrentFixture /*&& OBJECT.Intersect(uRayNear, rayDir, out info)*/) { /* check collision with bounding */
          bool meshHit = false;
          if ((g.NIF.Model == null || Program.CONFIG.Objects.AlwaysShowBounding) && OBJECT.Intersect(uRayNear, rayDir, out info))
          {
            //no mesh, so we assume its hit.
            meshHit = true;
          }
          else if (g.NIF.Model != null) {
            world = GetFixtureMatrix(g, false);

            rayStart.Z = 0.0f;
            uRayNear =
              Vector3.Unproject(rayStart, DEVICE.Viewport, DEVICE.Transform.Projection,
                                DEVICE.Transform.View, world);

            rayStart.Z = 1.0f;
            uRayFar =
              Vector3.Unproject(rayStart, DEVICE.Viewport, DEVICE.Transform.Projection,
                                DEVICE.Transform.View, world);

            rayDir = Vector3.Subtract(uRayFar, uRayNear);
            if (g.NIF.Model.Intersect(uRayNear, rayDir)) meshHit = true;
          }

          if (meshHit && (poly == null || intDist > info.Dist)) {
            intDist = info.Dist;
            poly = g;
          }
        }
      }

      return poly;
    }

    public Light GetLightByClick(Point click) {
      var rayStart = new Vector3(click.X, click.Y, 0.0f); //y is our "far" plane
      float intDist = 0;
      Light poly = null;

      foreach (Light g in Light.Lights) {
        if (g == Program.FORM.CurrentLight)
          continue;

        Matrix world = GetLightMatrix(g);

        rayStart.Z = 0.0f;
        Vector3 uRayNear =
          Vector3.Unproject(rayStart, DEVICE.Viewport, DEVICE.Transform.Projection,
                            DEVICE.Transform.View, world);

        rayStart.Z = 1.0f;
        Vector3 uRayFar =
          Vector3.Unproject(rayStart, DEVICE.Viewport, DEVICE.Transform.Projection,
                            DEVICE.Transform.View, world);

        Vector3 rayDir = Vector3.Subtract(uRayFar, uRayNear);

        IntersectInformation info;
        if (LIGHT.Intersect(uRayNear, rayDir, out info)) {
          if (poly == null || intDist > info.Dist) {
            intDist = info.Dist;
            poly = g;
          }
        }
      }

      return poly;
    }

    public Zonejump GetZonejumpByClick(Point click, out int index) {
      float intDist = 0;
      Zonejump poly = null;
      index = 0;

      foreach (Zonejump g in Zonejump.Zonejumps) {
        IntersectInformation info;

        if (IsVectorHit(click, g.First, out info)) {
          if (poly == null || intDist > info.Dist) {
            intDist = info.Dist;
            poly = g;
            index = 1;
          }
        }
        else if (IsVectorHit(click, g.Second, out info)) {
          if (poly == null || intDist > info.Dist) {
            intDist = info.Dist;
            poly = g;
            index = 2;
          }
        }
      }

      return poly;
    }

    public bool IsVectorHit(Point click, Vector3 vec, out IntersectInformation info) {
      var rayStart = new Vector3(click.X, click.Y, 0.0f);
      Matrix world = Matrix.Translation(vec);

      rayStart.Z = 0.0f;
      Vector3 uRayNear =
        Vector3.Unproject(rayStart, DEVICE.Viewport, DEVICE.Transform.Projection,
                          DEVICE.Transform.View, world);

      rayStart.Z = 1.0f;
      Vector3 uRayFar =
        Vector3.Unproject(rayStart, DEVICE.Viewport, DEVICE.Transform.Projection,
                          DEVICE.Transform.View, world);

      Vector3 rayDir = Vector3.Subtract(uRayFar, uRayNear);

      if (POLYGON.Intersect(uRayNear, rayDir, out info)) return true;
      else return false;
    }
  }
}