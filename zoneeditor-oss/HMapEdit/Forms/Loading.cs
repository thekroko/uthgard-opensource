using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace HMapEdit {
    public partial class Loading : Form {
        private const int speed = 5;
        private static int counter;
        private static Loading cur;
        private static int dirX = 1;
        private static int dirY = 1;
        private static int x = 64;
        private static int y = 64;
        private static string operation = "Loading...";
        private bool shown = true;

        public Loading() {
            InitializeComponent();
            //SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            label1.Text = operation;
            label2.Text = "";
            ClientSize = new Size(pictureBox1.Width, pictureBox1.Height + label1.Height);
        }

        public static void ShowLoading() {
            ShowLoading(false);
        }
        public static void ShowLoading(bool mouse)
        {
            counter++;
            if (cur != null) return;

            Thread t = new Thread(new ThreadStart(delegate
                                                      {
                                                          cur = new Loading();

                                                          cur.Location = new Point(Screen.PrimaryScreen.WorkingArea.Right - cur.Width,
                                                                                   Screen.PrimaryScreen.WorkingArea.Bottom - cur.Height);

                                                          cur.Show();

                                                          Point l = cur.Location;
                                                          l.Offset(128, 128);

                                                          if (mouse)
                                                            Cursor.Position = l;

                                                          Stopwatch sw = new Stopwatch();
                                                          sw.Start();

                                                          x = cur.box.Location.X;

                                                          int last = 0;

                                                          while (cur.shown) {
                                                              if (x < 0 || x + 32 > cur.pictureBox1.Width) dirX = -dirX;
                                                              if (y < 0 || y + 32 > cur.pictureBox1.Height) dirY = -dirY;

                                                              x += dirX*speed;
                                                              y += dirY*speed;

                                                              cur.box.Location = new Point(x, y);

                                                              Thread.Sleep(20); //20 ms standard sleep

                                                              int secs = (int) sw.Elapsed.TotalSeconds;
                                                              if (cur.label1.Text != operation)
                                                                cur.label1.Text = operation;
                                                              if (last != secs) {
                                                                  last = secs;
                                                                  cur.label2.Text = secs + "s";
                                                              }

                                                              Application.DoEvents();
                                                          }


                                                          cur.Close();
                                                          cur = null;
                                                      }));
            t.IsBackground = true;
            t.Start();
            Application.DoEvents();
        }

        public static void CloseLoading() {
            counter--;
            Application.DoEvents();

            if (cur != null && counter <= 0) cur.shown = false;
            counter = Math.Max(0, counter);
        }

        public static void Update(string text) {
            operation = text;
            Application.DoEvents();

            //if (cur == null)
            //    return;

            //try {
            //    cur.Invoke(new EventHandler(delegate { cur.label1.Text = text; }));
            //    Application.DoEvents();
            //}
            //catch (Exception) {
            //    ;
            //}
        }

        private void Loading_FormClosed(object sender, FormClosedEventArgs e) {
            shown = false;
        }
    }
}