using System;
using System.Windows.Forms;

namespace HMapEdit {
    public partial class CrashDialog : Form {
        public CrashDialog(Exception ex) : this() {
            textBox1.Text = ex.ToString();
        }

        public CrashDialog() {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e) {
            Close();
        }

        private void button2_Click(object sender, EventArgs e) {
            Close();
            new MainForm().ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e) {
            try {
                Program.ZONE.SaveZone(true);
                MessageBox.Show("Saved Successfully!");
            }
            catch (Exception ex) {
                MessageBox.Show("Could not save data: \r\n" + ex);
            }
        }

        public static void Show(Exception e) {
            new CrashDialog(e).ShowDialog();
        }
    }
}