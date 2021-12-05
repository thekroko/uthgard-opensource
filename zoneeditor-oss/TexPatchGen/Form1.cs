using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using ImageConverter;
using Microsoft.DirectX.Direct3D;
using PatchMap;

namespace PatchGen
{
    public partial class MainForm : Form
    {
        private Random m_Random = new Random();
        private PatchField[,] m_Fields = new PatchField[8,8];
        private class PatchInfo {
            public PatchInfo() {
                Layers = new List<string>();
            }

            public int X { get; set; }
            public int Y { get; set; }
            public string BaseTex { get; set; }
            public string Patchmap { get; set;}
            public List<string> Layers { get; private set; }
        }
        private List<PatchInfo> m_RemainingPatches = new List<PatchInfo>();

        /// <summary>
        /// Construct
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
            Height += 8*32+ 8;

            for (int i = 0; i < m_Fields.GetLength(0); i++) {
                for (int j = 0; j < m_Fields.GetLength(1); j++) {
                    PatchField f = new PatchField();
                    f.Location = new Point(bar.Location.X + i * f.Width,
                                           bar.Location.Y + bar.Height + 8 + j * f.Height);
                    f.Status = Status.Inexistent;
                    Controls.Add(f);
                    m_Fields[i, j] = f;
                }
            }
        }

        /// <summary>
        /// Update Status
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="status"></param>
        public void UpdateStatus(int x, int y, Status status) {
            Invoke(new EventHandler(delegate
                                        {
                                            m_Fields[x, y].Status = status;

                                            if (status == Status.Done)
                                                bar.Value++;
                                        }));
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            ///////// Start progress ////////
            // 1. Load textures.csv
            using (StreamReader rs = new StreamReader(Program.ter+Path.DirectorySeparatorChar+"textures.csv")) {
                rs.ReadLine();

                PatchInfo cur = null;
                while (!rs.EndOfStream) {
                    string line = rs.ReadLine();

                    if (string.IsNullOrEmpty(line))
                        continue;

                    string[] split = line.Split(',');

                    if (split.Length < 10)
                        continue; //empty/invalid

                    int x = int.Parse(split[0]);
                    int y = int.Parse(split[1]);

                    if (cur == null || cur.X != x || cur.Y != y) {
                        cur = new PatchInfo();
                        cur.X = x;
                        cur.Y = y;
                        cur.Patchmap = Program.ter + Path.DirectorySeparatorChar + "patch{0:D2}{1:D2}-{2:D2}.dds";
                        cur.BaseTex = Program.tex + Path.DirectorySeparatorChar + "tex{0:D2}-{1:D2}.bmp";
                        UpdateStatus(x,y, Status.ToDo);
                        m_RemainingPatches.Add(cur);
                    }

                    cur.Layers.Add(line);
                }

                rs.Close();
            }

            if (!Directory.Exists(Program.output))
                Directory.CreateDirectory(Program.output);

            // 2. Start workers
            int workers = Program.workers;

            if (workers <= 0)
                workers = Environment.ProcessorCount;

            for (int i = 0; i < workers; i++) {
                Thread worker = new Thread(RunWorker);
                worker.IsBackground = true;
                worker.Start();
            }
            /////////////////////////////////
        }

        /// <summary>
        /// Worker Thread
        /// </summary>
        private void RunWorker() {
            while (m_RemainingPatches.Count > 0)
            {
            	//1. Get Quad
            	PatchInfo info;
            	lock (m_RemainingPatches)
            	{
            		int ind = m_Random.Next(m_RemainingPatches.Count);
            		info = m_RemainingPatches[ind];
            		m_RemainingPatches.RemoveAt(ind);
            	}

            	//2. Set Status
            	UpdateStatus(info.X, info.Y, Status.Working);

            	//3. Do Work
            	try {
            		RunWork(info);
            		GC.Collect();
					}
					catch (Exception e) {
                    MessageBox.Show(e.Message, "Worker Error");
                    UpdateStatus(info.X, info.Y, Status.ToDo);
                    lock (m_RemainingPatches) {
                        m_RemainingPatches.Add(info);
                    }
                    Thread.Sleep(1000);
                }

                //4. Update Status
                UpdateStatus(info.X, info.Y, Status.Done);
            }
        }

        /// <summary>
        /// Run Work
        /// </summary>
        /// <param name="info"></param>
        private void RunWork(PatchInfo info)
        {
            //1. Create Intermediatemap
            int size = Program.gensize;
            FastMap work = new FastMap(new Bitmap(size, size, PixelFormat.Format24bppRgb), "work");
            work.Lock();
            
            // Init the map with pink
            for (int dx = 0; dx < size; dx++)
              for (int dy = 0; dy < size; dy++)
                work.SetPixel(dx, dy, Color.Magenta);

            // Do all the work
            {
                //1.a) Layers
                FastMap patch = null;
                for (int layer = -1; layer < info.Layers.Count; layer++)
                //for (int layer = 0; layer < info.Layers.Count; layer++)  
                //for (int layer = 0; layer < Math.Min(info.Layers.Count, 1); layer++)
                {
                    //Update Status
                    if (layer >= 0) {
                        switch (layer%3) {
                            case 0: UpdateStatus(info.X, info.Y, Status.WorkingR); break;
                            case 1: UpdateStatus(info.X, info.Y, Status.WorkingG); break;
                            case 2: UpdateStatus(info.X, info.Y, Status.WorkingB); break;
                        }
                    }

                    //Load patch (and resize to working layer size)
                    if (layer % 3 == 0)
                    {
                        if (patch != null)
                            patch.Dispose();

                        Texture file = Images.LoadFromFile(string.Format(info.Patchmap, info.X, info.Y, layer / 3), "");
                        Bitmap imgSmall = Images.GetBitmap(file);
                        Bitmap img = resize(imgSmall, size,0);
                        imgSmall.Dispose();
                        patch = new FastMap(img, "patch");
                        file.Dispose();
                        patch.Lock();
                    }

                    //Load Texture
                    FastMap src;

                    if (layer == -1)
                    {
                        //Base Texture
                      Bitmap brush = null;
                      try
                      {
                        Bitmap Btex = (Bitmap) Image.FromFile(string.Format(info.BaseTex, info.X, info.Y));
                        brush = new Bitmap(Btex, size, size);
                        Btex.Dispose();
                      }
                      catch (Exception ex)
                      {
                        string path = string.Format(info.BaseTex, info.X, info.Y).Replace(".bmp", ".dds");
                        //Load Layer tex
                        Texture file = Images.LoadFromFile(path, "");
                        brush = Images.GetBitmap(file);
                        file.Dispose();
                      }
                      
                        src = new FastMap(brush, "base");
                    }
                    else
                    {
                        //Infos
                        string[] line = info.Layers[layer].Split(',');
                        string tex = line[2];
                        int count = int.Parse(line[9]);

                        string path = Program.terraintex + Path.DirectorySeparatorChar + tex + ".dds";
                        if (!File.Exists(path))
                            continue;

                        //Load Layer tex
                        Texture file = Images.LoadFromFile(path, "");
                        Bitmap brush = Images.GetBitmap(file);
                        file.Dispose();

                        //Tile (8 = 1:1 || < 8 = bigger|| > 8 = smaller
                        int brushsize = size * 8 / count;
                        int imagesize = Math.Min(brushsize, size);
                        Bitmap map = new Bitmap(imagesize, imagesize, PixelFormat.Format24bppRgb);

                        //Fill
                        using (Graphics g = Graphics.FromImage(map))
                        {
                            Rectangle Rsrc = new Rectangle((info.X * brush.Width / 8 / count),
                                                           (info.Y * brush.Height / 8 / count),
                                                           brush.Width,
                                                           brush.Height);
                            Rectangle Rdst = new Rectangle(0, 0, brushsize, brushsize);
                            g.DrawImage(brush, Rdst, Rsrc, GraphicsUnit.Pixel);
                        }
                        brush.Dispose();

                        src = new FastMap(map, "layer tex");
                    }

                    //Create Map
                    int srcW = src.Bitmap.Width;
                    int srcH = src.Bitmap.Height;
                    src.Lock();
                    for (int x = 0; x < size; x++)
                    {
                        for (int y = 0; y < size; y++)
                        {
                            //1. Texture Info
                          const int WEIRD_OFFSET = 0; // 8?
                            // rx, ry = real xy
                            int rx = Math.Min(Math.Max(x - WEIRD_OFFSET, 0), Program.gensize);
                            int ry = Math.Min(Math.Max(y - WEIRD_OFFSET, 0), Program.gensize);
                            // brush pos
                            int px = ((rx) % (srcW));
                            int py = ((ry) % (srcH));

                            //2. Color Mapping
                            Color col = work.GetPixel(x, y);
                            Color add = src.GetPixel(px, py);
                            int val = 0;

                            if (layer == -1)
                                val = byte.MaxValue;
                            else
                            {
                                Color pc = patch.GetPixel(rx,ry);
                                
                                switch (layer % 3)
                                {
                                    case 0: val = pc.R; break;
                                    case 1: val = pc.G; break;
                                    case 2: val = pc.B; break;
                                }

                                if (layer == 0)
                                    val = 255; //R = start
                            }

                            col = lerp(col, add, val);

                            work.SetPixel(x, y, col);
                        }
                    }
                    src.Release();
                    src.Dispose();
                }
            }
            work.Release();

            //2. Create Results
            //=> Resultmap
            {
                using (Bitmap res = resize(work.Bitmap, Program.targetsize, 0)) {
                    res.Save(Program.output + Path.DirectorySeparatorChar + string.Format("gen{0:D2}-{1:D2}.png", info.X, info.Y), ImageFormat.Png);
                }
            }
            //Lod Map & Tex Map
            {
                using (Bitmap res = resize(work.Bitmap, 256, 0))
                {
                    res.Save(Program.output + Path.DirectorySeparatorChar + string.Format("tex{0:D2}-{1:D2}.bmp", info.X, info.Y), ImageFormat.Bmp);
                    Texture t = Images.GetTexture(res);
                    Images.SaveToFile(t, Program.output + Path.DirectorySeparatorChar + string.Format("lod{0:D2}-{1:D2}.dds", info.X, info.Y), ImageFileFormat.Dds);
                    t.Dispose();
                }
            }
            work.Dispose();
        }

        private static Color lerp(Color a, Color b, int mix) {
            float valB = mix/255f;
            float valA = (1.0f - valB);

            return Color.FromArgb((byte) (a.R*valA + b.R*valB),
                                  (byte) (a.G*valA + b.G*valB),
                                  (byte) (a.B*valA + b.B*valB));
        }

        private static Bitmap resize(Bitmap src, int size, int cut = 0) {
            Bitmap newImage = new Bitmap(size, size);
            using (Graphics g = Graphics.FromImage(newImage))
            {
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(src, new Rectangle(0, 0, size, size), new Rectangle(cut, cut, src.Width-2*cut, src.Height-2*cut), GraphicsUnit.Pixel);
            }
            return newImage;

        }
    }
}
