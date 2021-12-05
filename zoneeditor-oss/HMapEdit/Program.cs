using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Google.Apis.Samples.Helper;
using HMapEdit.Engine;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace HMapEdit {
  internal static class Program {
    /// <summary>
    /// Command Line Arguments
    /// </summary>
    public class CommandLineArguments
    {
      [Argument("--aa", Description = "Enables or disables antialiasing")]
      public bool AntiAlias { get; set; }

      [Argument("--game", ShortName = "g", Description = "Path to the DAoC game directory")]
      public string GameDirectory { get; set; }

      [Argument("--zone", ShortName = "z", Description = "Zone to edit (zones/zoneXXX/ dir)")]
      public string ZoneDirectory { get; set; }

      [Argument("--nif", ShortName = "n", Description = "Selects which nif-layer to show (collidee/visible/..)")]
      public string NifLayer { get; set; }
    }

    /// <summary>
    /// Command Line Arguments
    /// </summary>
    public static CommandLineArguments Arguments { get; private set; }

    /// <summary>
    /// Positional Arguments
    /// </summary>
    public static string[] PositionalArguments { get; private set; }

    /// <summary>
    /// Default arguments
    /// </summary>
    private static CommandLineArguments DefaultArguments {
      get {
        return new CommandLineArguments() {AntiAlias = true, GameDirectory = null, ZoneDirectory = null, NifLayer = "visible"};
      }
    }

    #region eMode enum

    public enum eMode {
      Cursor,
      Heightmap,
      Smooth,
      Polygon,
      Object,
      Zonejump,
      Ruler,
      Lights,
      Texturing,
      SoundEdit,
    }

    #endregion

    #region eSmooth enum

    public enum eSmooth {
      RadiusAVG,
      NearbyAVG,
    }

    #endregion

    public static EditorConfig CONFIG = new EditorConfig();
    public static MainForm FORM;
    public static eMode MODE = eMode.Cursor;
    public static HeightmapTool TOOL_HMAP = new HeightmapTool();
    public static RulerTool TOOL_RULER = new RulerTool();
    public static SmoothTool TOOL_SMOOTH = new SmoothTool();
    public static TexturTool TOOL_TEXTURE = new TexturTool();
    public static Zone ZONE;
    public static bool NEW_RENDERING = true; /* deprecated */

    /// <summary>
    /// Der Haupteinstiegspunkt für die Anwendung.
    /// </summary>
    [STAThread]
    private static void Main() {
      Control.CheckForIllegalCrossThreadCalls = false;
      //Application.EnableVisualStyles();
      //Application.SetCompatibleTextRenderingDefault(false);

#if !DEBUG
			try
			{
#endif

      // Parse Arguments
      Arguments = DefaultArguments;
      PositionalArguments = CommandLineFlags.ParseArguments(Arguments, Environment.GetCommandLineArgs().Skip(1).ToArray());

      if (string.IsNullOrEmpty(Arguments.ZoneDirectory)) {
        MessageBox.Show(String.Join(Environment.NewLine, CommandLineFlags.GenerateCommandLineHelp(Arguments)), "Syntax");
      }

      var backup = new BackgroundWorker();
      backup.DoWork += delegate {
                         while (true) {
                           Thread.Sleep(1000*60*3);
                           if (ZONE != null) {
                             ZONE.SaveZone(true);
                           }
                         }
                       };
      backup.RunWorkerAsync();

      Application.Run((FORM = new MainForm()));
#if !DEBUG
			}
			catch (Exception e)
			{
				CrashDialog.Show(e);
			}
#endif
    }

    #region Nested type: EditorConfig

    /// <summary>
    /// Editor Config
    /// </summary>
    public class EditorConfig {
      private readonly ObjectConfig m_Objects = new ObjectConfig() { AlwaysWireframe = true, AlwaysShowBounding = false};
      protected bool m_AdaptiveDegeneration;
      private Color m_Background = Color.Black;
      private int m_Clipping = 256*256*2;
      private FillMode m_FillMode = FillMode.Solid;
      private Color m_GridColor = Color.Yellow;
      private int m_GridSize = 1;
      private float m_MouseWheelScroll = 5;
      private int m_ObjectRenderDistance = 256*212;
      protected bool m_Rotate = true;
      private bool m_ShowCursor = true;
      private bool m_ShowFilledPolygons = true;
      private bool m_ShowHeightmap = true;
      private bool m_ShowObjects = true;

      private bool m_ShowPolygons = true;
      private bool m_ShowZonejumps = true;
      protected int m_TextureRenderDistance = 256*64;
      protected bool m_Textures = true;
      protected bool m_UseNewRendering;
      private bool m_UsePatchmaps;

      [Category("Visibility")]
      public bool ShowHeightmap {
        get { return m_ShowHeightmap; }
        set { m_ShowHeightmap = value; }
      }

      [Category("Visibility")]
      public bool ShowPolygons {
        get { return m_ShowPolygons; }
        set { m_ShowPolygons = value; }
      }

      [Category("Visibility")]
      public bool ShowFilledPolygons {
        get { return m_ShowFilledPolygons; }
        set { m_ShowFilledPolygons = value; }
      }

      [Category("Visibility")]
      public bool ShowObjects {
        get { return m_ShowObjects; }
        set { m_ShowObjects = value; }
      }

      [Category("Visibility")]
      [TypeConverter(typeof (ExpandableObjectConverter))]
      public ObjectConfig Objects {
        get { return m_Objects; }
      }

      [Category("Visibility")]
      public bool ShowCursor {
        get { return m_ShowCursor; }
        set { m_ShowCursor = value; }
      }

      [Category("Visibility")]
      public bool ShowZonejumps {
        get { return m_ShowZonejumps; }
        set { m_ShowZonejumps = value; }
      }

      [Category("Visibility")]
      public Color Background {
        get { return m_Background; }
        set { m_Background = value; }
      }

      [Category("Visibility")]
      public bool ShowGrid { get; set; }

      [Category("Visibility")]
      public int GridSize {
        get { return m_GridSize; }
        set {
          m_GridSize = value;
          FORM.renderControl1.RebuildGrid();
        }
      }

      [Category("Visibility")]
      public Color GridColor {
        get { return m_GridColor; }
        set {
          m_GridColor = value;
          FORM.renderControl1.RebuildGrid();
        }
      }


      [Category("Camera")]
      public float MouseWheelScroll {
        get { return m_MouseWheelScroll; }
        set { m_MouseWheelScroll = value; }
      }

      [Category("Rendering")]
      public FillMode FillMode {
        get { return m_FillMode; }
        set {
          m_FillMode = value;
          FORM.renderControl1.DEVICE.RenderState.FillMode = value;
        }
      }

      [Category("Rendering")]
      public int Clipping {
        get { return m_Clipping; }
        set { m_Clipping = value; }
      }

      [Category("Rendering")]
      public int ObjectRenderDistance {
        get { return m_ObjectRenderDistance; }
        set { m_ObjectRenderDistance = value; }
      }

      [Category("Rendering")]
      public bool AdaptiveDegeneration {
        get { return m_AdaptiveDegeneration; }
        set { m_AdaptiveDegeneration = value; }
      }

      /// <summary>
      /// Object Wire Color
      /// </summary>
      [Category("Rendering")]
      public bool ObjectWireColor { get; set; }

      ///<summary>
      /// Uses the new patchmaps
      ///</summary>
      [Category("Rendering")]
      public bool UsePatchmaps {
        get { return m_UsePatchmaps; }
        set {
          m_UsePatchmaps = value;
          if (value) PatchMap.Init();
        }
      }

      ///<summary>
      /// Rotate?
      ///</summary>
      [Category("Object Placement")]
      public bool Rotate {
        get { return m_Rotate; }
        set { m_Rotate = value; }
      }

      #region Nested type: ObjectConfig

      [TypeConverter(typeof (ExpandableObjectConverter))]
      [Category("Visibility")]
      public class ObjectConfig {
        private bool m_ShowModelSolid = true;

        private bool m_ShowModelWire = true;

        public bool ShowModelSolid {
          get { return m_ShowModelSolid; }
          set { m_ShowModelSolid = value; }
        }

        public bool ShowModelWire {
          get { return m_ShowModelWire; }
          set { m_ShowModelWire = value; }
        }

        public bool AlwaysShowBounding { get; set; }

        public bool AlwaysWireframe { get; set; }
      }

      #endregion
    }

    #endregion

    #region Nested type: HeightmapTool

    public class HeightmapTool {
      private int m_Radius = 5;
      private float m_Smooth = 0.5f;

      private float m_Strength = 32.0f;

      [Category("Editing Heightmaps")]
      public int Radius {
        get { return m_Radius; }
        set { m_Radius = value; }
      }

      [Category("Editing Heightmaps")]
      public float Strength {
        get { return m_Strength; }
        set { m_Strength = value; }
      }

      [Category("Editing Heightmaps")]
      public float Smooth {
        get { return m_Smooth; }
        set { m_Smooth = value; }
      }

      [Category("Editing Heightmaps")]
      public bool Flatten { get; set; }
    }

    #endregion

    #region Nested type: RulerTool

    public class RulerTool {
      public Vector3 Destination;
      public Vector3 Source;

      public RulerTool() {
        Dist2D = true;
      }

      [Category("Ruler")]
      public int SourceX {
        get { return (int) Source.X; }
      }

      [Category("Ruler")]
      public int SourceY {
        get { return (int) Source.Y; }
      }

      [Category("Ruler")]
      public int SourceZ {
        get { return (int) Source.Z; }
      }

      [Category("Ruler")]
      public int DestX {
        get { return (int) Destination.X; }
      }

      [Category("Ruler")]
      public int DestY {
        get { return (int) Destination.Y; }
      }

      [Category("Ruler")]
      public int DestZ {
        get { return (int) Destination.Z; }
      }

      [Category("Ruler")]
      public int Distance {
        get {
          double dist = (Dist2D
                           ? Utils.Get2DDistance(Source, Destination)
                           : Utils.GetDistance(Source, Destination));
          return (int) Math.Round(dist);
        }
        set { }
      }

      [Category("Ruler")]
      public bool Dist2D { get; set; }
    }

    #endregion

    #region Nested type: SmoothTool

    public class SmoothTool {
      private float m_Factor = 0.8f;
      private eSmooth m_Method = eSmooth.NearbyAVG;
      private int m_Radius = 4;

      [Category("Smoothing")]
      public int Radius {
        get { return m_Radius; }
        set { m_Radius = value; }
      }

      [Category("Smoothing")]
      public float Factor {
        get { return m_Factor; }
        set { m_Factor = value; }
      }

      [Category("Smoothing")]
      public eSmooth Method {
        get { return m_Method; }
        set { m_Method = value; }
      }
    }

    #endregion

    #region Nested type: TexturTool

    /// <summary>
    /// TexturTool
    /// </summary>
    public class TexturTool {
      #region eGradient enum

      /// <summary>
      /// eGradient
      /// </summary>
      public enum eGradient {
        None = 0,
        Linear = 1,
        Cubic = 2,
      }

      #endregion

      public TexturTool() {
        Radius = 200;
        Strength = 255;
        PaintMode = eGradient.Linear;
        LimitSubzones = false;
        IntelligentLayers = true;
        ShowGrid = false;
      }

      [Category("Texturing")]
      public int Radius { get; set; }

      [Category("Texturing")]
      public eGradient PaintMode { get; set; }

      [Category("Texturing")]
      public int Strength { get; set; }

      [Category("Texturing")]
      public bool LimitSubzones { get; set; }

      [Category("Texturing")]
      public bool IntelligentLayers { get; set; }

      [Category("Texturing")]
      public bool ShowGrid { get; set; }
    }

    #endregion
  }
}