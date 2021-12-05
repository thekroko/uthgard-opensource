using System;
using System.Windows.Forms;

namespace HMapEdit.Forms {
    public partial class TexForm : Form {
        public TexForm() {
            InitializeComponent();
        }

        public void UpdateData() {
            texCtrl1.UpdateData();
        }

        private void TexForm_VisibleChanged(object sender, EventArgs e) {
            texCtrl1.UpdateData();
        }

        private void TexForm_FormClosing(object sender, FormClosingEventArgs e) {
            Program.FORM.m_TexCtrl = null;
        }
    }
}