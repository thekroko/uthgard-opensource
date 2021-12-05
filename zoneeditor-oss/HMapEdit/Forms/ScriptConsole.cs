using System;
using System.Windows.Forms;

namespace HMapEdit.Forms {
    public partial class ScriptConsole : Form {
        public ScriptConsole() {
            InitializeComponent();
        }

        public void Write(string txt) {
            Invoke(new EventHandler(delegate { box.Text += txt + "\r\n"; }));
        }

        private void ScriptConsole_FormClosed(object sender, FormClosedEventArgs e) {
            ScriptForm.csl = new ScriptConsole();
        }
    }
}