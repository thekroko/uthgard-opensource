using System.Windows.Forms;

namespace HMapEdit {
    public partial class InputForm : Form {
        public InputForm(string title) : this() {
            Text = title;
        }

        public InputForm() {
            InitializeComponent();
        }

        public string Input {
            get { return textBox1.Text; }
        }
    }
}