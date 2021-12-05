using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace HMapEdit {
    internal partial class AboutForm : Form {
        public AboutForm() {
            InitializeComponent();

            //as a transparent label produces bullshit, we just modify image
            Graphics g = Graphics.FromImage(pictureBox1.Image);

            Font f = new Font("Times New Roman", 25, FontStyle.Bold, GraphicsUnit.Pixel);
            string txt = string.Format("Version {0}", AssemblyVersion);

            g.DrawString(txt, f, new SolidBrush(Color.Black), 12, 157);
            g.DrawString(txt, f, new SolidBrush(Color.White), 10, 155);

            g.Save();
        }

        #region Assemblyattributaccessoren

        public string AssemblyVersion {
            get { return Assembly.GetExecutingAssembly().GetName().Version.ToString(); }
        }

        #endregion
    }
}