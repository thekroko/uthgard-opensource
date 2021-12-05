using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace HMapEdit.Engine {
    //; **************************************************************************************
    //; Dark Age Of Camelot : zone audio control file : "sounds.dat" - zone 050 : Brad Derrick
    //; **************************************************************************************

    /// <summary>
    /// SoundRegion
    /// </summary>
    public class SoundRegion {
        //; =======================================================================================
        //; SOUNDS.DAT:	TERRAIN TEMPLATE - GRASSLAND
        //; UPDATED:		11-27-2002
        //;
        //; HEADER FORMAT
        //; [SoundRegion##] : 0-63
        //; name : any character string
        //- instant_start || 1 @sounds
        //; fade_on_mp3 : (1/0) do/don't fade for mp3 playback || 1 @ music
        //; fade_time : (int) fade in/out time for all soundregion sounds || 5
        //; random_sound_spacing : (int) minimum time between "random" sounds (!)
        //; ignore_random_offset : (0/1) do/don't randomly offset start times; if absent, assume "0" 
        //; zone_wide_sounds : (0/1) do/don't check soundregion shapes; if "1", no "shape" values needed
        //; sound_type : (1/2/3) ambient sfx/ambient music/sound effect; default: 1
        //; version : (1/2) above/underwater SoundRegion; if absent, assume "1"
        //;

        /// <summary>
        /// Shapes
        /// </summary>
        public List<Shape> Shapes = new List<Shape>();

        /// <summary>
        /// Sounds
        /// </summary>
        public List<Sound> Sounds = new List<Sound>();

        /// <summary>
        /// xShapes
        /// </summary>
        public List<Shape> xShapes = new List<Shape>();

        /// <summary>
        /// Constructor
        /// </summary>
        public SoundRegion() {
            Name = "Unnamed";
            Type = SoundType.AmbientSound;
            Spacing = 0;
            Version = SoundVersion.Standard;
            FadeTime = 6;
        }

        /// <summary>
        /// Name
        /// </summary>
        [Description("Name")]
        [Category("Sound Region")]
        public string Name { get; set; }

        /// <summary>
        /// Sound Type
        /// </summary>
        [Description("Type")]
        [Category("Sound Region")]
        public SoundType Type { get; set; }

        /// <summary>
        /// Delay in seconds (?)
        /// </summary>
        [Description("Delay in seconds between soundstarts")]
        [Category("Sound Region")]
        public int Spacing { get; set; }

        [Description("Fade In/Out Time in sec")]
        [Category("Sound Region")]
        public int FadeTime { get; set; }

        /// <summary>
        /// Version
        /// </summary>
        [Description("Environment")]
        [Category("Sound Region")]
        public SoundVersion Version { get; set; }

        /// <summary>
        /// Global (Zonewide)
        /// </summary>
        [Description("Zonewide sound?")]
        [Category("Sound Region")]
        public bool Global { get; set; }

        public override string ToString() {
            string str = "[";

            str += Type;
            str += ":" + Name;

            return str + "]";
        }
    }

    /// <summary>
    /// Sounds
    /// </summary>
    public class Sound {
        /// <summary>
        /// Constructor
        /// </summary>
        public Sound() {
            ID = "######";
            Interval = "0";
            Chance = 100;
            Volume = 64;
            StartTime = 0;
            EndTime = 0;
            Version = SoundVersion.Standard;
        }

        //; SOUND FORMAT
        //; Type, Sound, Period, Chance, Volume, Start Time, End Time, Weather, Environment Type

        /// <summary>
        /// Sound ID/Name
        /// </summary>
        [Description("String ID or Number")]
        [Category("Sound")]
        public string ID { get; set; }

        /// <summary>
        /// Interval (a-b)
        /// </summary>
        [Description("a-b sec; e.g. 3-9")]
        [Category("Sound")]
        public string Interval { get; set; }

        /// <summary>
        /// Chance
        /// </summary>
        [Description("Chance (0-100)")]
        [Category("Sound")]
        public int Chance { get; set; }

        /// <summary>
        /// Volume (0-100)
        /// </summary>
        [Description("Volume (0-100)")]
        [Category("Sound")]
        public int Volume { get; set; }

        /// <summary>
        /// StartTime in hour-minutes (hhmm)
        /// </summary>
        [Description("Start Time in hhmm")]
        [Category("Sound")]
        public int StartTime { get; set; }

        /// <summary>
        /// EndTime in hour-minutes (hhmm)
        /// </summary>
        [Description("End Time in hhmm")]
        [Category("Sound")]
        public int EndTime { get; set; }

        /// <summary>
        /// Version
        /// </summary>
        [Description("Environment")]
        [Category("Sound")]
        public SoundVersion Version { get; set; }

        public override string ToString() {
            string str = "[";

            str += ID;

            return str + "]";
        }
    }

    /// <summary>
    /// Shape
    /// </summary>
    public class Shape {
        //; SHAPE FORMAT
        //; Shape (0 = RECT, 1 = CIRCLE)
        //; (Rect [0]) Left, Top, Right, Bottom
        //; (Circ [1]) X, Y, Radius
        //; xShape : defines region of silence

        /// <summary>
        /// Constructor
        /// </summary>
        public Shape() {
            X = 2048;
            Y = 2048;
            Radius = 512;
            Type = ShapeType.Circle;
        }

        /// <summary>
        /// Type
        /// </summary>
        [Description("Type")]
        [Category("Shape")]
        public ShapeType Type { get; set; }

        /// <summary>
        /// X
        /// </summary>
        [Description("X")]
        [Category("Shape")]
        public int X { get; set; }

        /// <summary>
        /// Y
        /// </summary>
        [Description("Y")]
        [Category("Shape")]
        public int Y { get; set; }

        /// <summary>
        /// Radius
        /// </summary>
        [Description("Radius")]
        [Category("Circle")]
        public int Radius { get; set; }

        /// <summary>
        /// Width
        /// </summary>
        [Description("End X")]
        [Category("Rectangle")]
        public int EndX { get; set; }

        /// <summary>
        /// Height
        /// </summary>
        [Description("End Y")]
        [Category("Rectangle")]
        public int EndY { get; set; }

        public override string ToString() {
            string str = "[";

            str += Type;
            str += ":" + X;
            str += ":" + Y;

            return str + "]";
        }
    }

    /// <summary>
    /// Shape Type
    /// </summary>
    public enum ShapeType {
        Rectangle = 0,
        Circle = 1,
    }

    /// <summary>
    /// Sound Type
    /// </summary>
    public enum SoundType {
        AmbientSound = 1,
        AmbientMusic = 2,
        SoundEffect = 3,
    }

    /// <summary>
    /// Sound Version
    /// </summary>
    public enum SoundVersion {
        Standard = 1,
        Underwater = 2,
    }

    /// <summary>
    /// Sound Mgr
    /// </summary>
    public class SoundMgr {
        /// <summary>
        /// Sound Regions
        /// </summary>
        public static List<SoundRegion> SoundRegions = new List<SoundRegion>();

        /// <summary>
        /// Current Region
        /// </summary>
        public static SoundRegion CurrentRegion { get; set; }

        /// <summary>
        /// Current Shape
        /// </summary>
        public static Shape CurrentShape { get; set; }

        /// <summary>
        /// Touched?
        /// </summary>
        public static bool Touched { get; set; }

        /// <summary>
        /// Load
        /// </summary>
        public static void Load() {
            string file = Program.ZONE.PATH + @"\sounds.dat";
            StreamReader r = new StreamReader(file);

            SoundRegion reg = null;

            while (!r.EndOfStream) {
                string line = r.ReadLine();

                int last = line.LastIndexOf(';');

                if (last > 0)
                    line = line.Substring(0, last);

                if (string.IsNullOrEmpty(line) || line.StartsWith(";"))
                    continue; //empty

                if (line.StartsWith("[")) {
                    //new region
                    reg = new SoundRegion();
                    SoundRegions.Add(reg);
                }
                else if (reg != null) {
                    //properties
                    string[] split = line.Split('=');
                    string key = split[0];
                    string val = split[1];

                    switch (key) {
                            //Region properties
                        case "name":
                            reg.Name = val;
                            break;
                        case "sound_type":
                            reg.Type = (SoundType) int.Parse(val);
                            break;
                        case "random_sound_spacing":
                            reg.Spacing = int.Parse(val);
                            break;
                        case "fade_time":
                            reg.FadeTime = int.Parse(val);
                            break;
                        case "version":
                            reg.Version = (SoundVersion) int.Parse(val);
                            break;
                        case "zone_wide_sounds":
                            reg.Global = val == "1";
                            break;

                        default:
                            {
                                string[] parts = val.Split(',');
                                if (key.StartsWith("Sound")) {
                                    Sound snd = new Sound();
                                    reg.Sounds.Add(snd);

                                    snd.ID = parts[1].Trim();
                                    snd.Interval = parts[2].Trim();
                                    snd.Chance = int.Parse(parts[3]);
                                    snd.Volume = int.Parse(parts[4]);
                                    snd.StartTime = int.Parse(parts[5]);
                                    snd.EndTime = int.Parse(parts[6]);

                                    if (parts.Length > 8)
                                        snd.Version = (SoundVersion) int.Parse(parts[8]);
                                }
                                else if (key.StartsWith("Shape") || key.StartsWith("xShape")) {
                                    Shape sh = new Shape();

                                    sh.Type = (ShapeType) int.Parse(parts[0]);

                                    sh.X = int.Parse(parts[1]);
                                    sh.Y = int.Parse(parts[2]);

                                    if (sh.Type == ShapeType.Circle) sh.Radius = int.Parse(parts[3]);
                                    else if (sh.Type == ShapeType.Rectangle) {
                                        sh.EndX = int.Parse(parts[3]);
                                        sh.EndY = int.Parse(parts[4]);
                                    }

                                    if (key.StartsWith("xShape"))
                                        reg.xShapes.Add(sh);
                                    else
                                        reg.Shapes.Add(sh);
                                }
                            }
                            break;
                    }
                }
            }

            r.Close();

            if (SoundRegions.Count > 0)
                CurrentRegion = SoundRegions[0];
        }

        /// <summary>
        /// Save
        /// </summary>
        public static void Save(string path) {
            if (!Touched) return;

            string file = path + @"\sounds.dat";

            using (StreamWriter w = new StreamWriter(file, false)) {
                w.WriteLine(";".PadRight(50, '='));
                w.WriteLine("; Sound Data File (Created by DAoC Zone Editor)");
                w.WriteLine(";".PadRight(50, '='));
                w.WriteLine();

                int rn = 0;
                foreach (SoundRegion r in SoundRegions) {
                    w.WriteLine(("[SoundRegion" + (rn++).ToString("D2") + "];").PadRight(40, '-') + r);

                    w.WriteLine("name=" + r.Name);
                    w.WriteLine("sound_type=" + (int) r.Type);
                    w.WriteLine("random_sound_spacing=" + r.Spacing);
                    w.WriteLine("ignore_random_offset=0");
                    if (r.Type != SoundType.AmbientMusic)
                        w.WriteLine("instant_start=1");

                    //int fadetime = (r.Type == SoundType.AmbientMusic ? 6 : 1);
                    int fadetime = r.FadeTime;

                    w.WriteLine("fade_on_mp3=" + (r.Type == SoundType.AmbientMusic ? 1 : 0));
                    w.WriteLine("fade_on_mp3_time=" + fadetime);
                    w.WriteLine("fade_time=" + fadetime);
                    w.WriteLine("fade_on_weather_time=1");
                    w.WriteLine("fade_on_underwater_time=6");
                    w.WriteLine("zone_wide_sounds=" + (r.Global ? 1 : 0));
                    if ((int) r.Version > 1)
                        w.WriteLine("version=" + (int) r.Version);
                    w.WriteLine();

                    w.WriteLine("; SOUNDS");
                    int sn = 0;
                    //Sound00=2,    s_Bed_Prairie, 0,   100,    24,     0,          0,        0 
                    //        Type, Sound,      Period, Chance, Volume, Start Time, End Time, Weather, Environment Type
                    foreach (Sound s in r.Sounds) {
                        int t = 2; //more secure?

                        if (s.ID.StartsWith("g_"))
                            t = 1;
                        if (s.Interval != "0")
                            t = 1; // 1 = random interval, 2 = always?

                        w.WriteLine("Sound" + (sn++).ToString("D2") + "=" + t + ", " + s.ID + ", " + s.Interval + ", " + s.Chance + ", " + s.Volume +
                                    ", " + s.StartTime + ", " + s.EndTime + ", 0" + ((int) s.Version > 1 ? ", " + (int) s.Version : ""));
                    }
                    w.WriteLine();

                    //; (Rect [0]) Left, Top, Right, Bottom
                    //; (Circ [1]) X, Y, Radius
                    w.WriteLine("; SHAPES");
                    int sh = 0;
                    foreach (Shape s in r.Shapes) {
                        w.WriteLine("Shape" + (sh++).ToString("D2") + "=" + (int) s.Type + ", " + s.X + ", " + s.Y + ", " +
                                    (s.Type == ShapeType.Rectangle ? s.EndX + ", " + s.EndY : s.Radius.ToString()));
                    }
                    w.WriteLine();

                    w.WriteLine("; SHAPES");
                    int xsh = 0;
                    foreach (Shape s in r.xShapes) {
                        w.WriteLine("xShape" + (xsh++).ToString("D2") + "=" + (int) s.Type + ", " + s.X + ", " + s.Y + ", " +
                                    (s.Type == ShapeType.Rectangle ? s.EndX + ", " + s.EndY : s.Radius.ToString()));
                    }
                    w.WriteLine();
                }

                w.Flush();
            }
        }
    }
}