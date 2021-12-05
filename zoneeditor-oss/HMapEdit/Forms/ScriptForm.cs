using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using HMapEdit.Forms;

namespace HMapEdit {
    public partial class ScriptForm : Form {
        private static readonly ScriptForm FORM = new ScriptForm();

        public static ScriptConsole csl = new ScriptConsole();

        private string curScript = "";

        public ScriptForm() {
            InitializeComponent();
        }

        public static void ShowForm() {
            if (FORM.Visible)
                return;

            FORM.Show();
        }

        public static void HideForm() {
            if (!FORM.Visible)
                return;

            FORM.Hide();
        }

        public static void Console(object o) {
            if (!csl.Visible)
                csl.Show();

            csl.Write(o.ToString());
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e) {
            StreamReader r = new StreamReader("scripts\\" + comboBox1.SelectedItem);
            curScript = r.ReadToEnd();
            r.Close();

            input.Text = curScript;
        }

        private void button1_Click(object sender, EventArgs e) {
            try {
                RunCode();
                Program.FORM.renderControl1.Render();
            }
            catch (Exception ex) {
                MessageBox.Show(ex.ToString(), "Script Error");
            }
        }

        private void ScriptForm_Load(object sender, EventArgs e) {
            if (!Directory.Exists("scripts")) Directory.CreateDirectory("scripts");

            int r = Path.GetFullPath("scripts").Length + 1;
            foreach (string f in Directory.GetFiles("scripts\\", "*.txt", SearchOption.AllDirectories))
                comboBox1.Items.Add(Path.GetFullPath(f).Substring(r));

            comboBox1.SelectedItem = "empty.txt";
        }

        private void RunCode() {
            CompilerParameters cp = new CompilerParameters();
            cp.GenerateExecutable = false;
            cp.GenerateInMemory = true;
            //foreach (AssemblyName a in Assembly.GetExecutingAssembly().GetReferencedAssemblies())
            //	cp.ReferencedAssemblies.Add(a.Name);

            cp.ReferencedAssemblies.Add(Assembly.GetCallingAssembly().Location);
            cp.ReferencedAssemblies.Add("System.dll");
            cp.ReferencedAssemblies.Add("System.Windows.Forms.dll");

            StringBuilder s = new StringBuilder();
            s.Append(
                "using System; using System.Text; using System.Collections; using HMapEdit; using System.Collections.Generic; using System.Windows.Forms;");
            s.Append("using Fixture = HMapEdit.Objects.Fixture; using obj = HMapEdit.Objects.Fixture; using nif = HMapEdit.Objects.NIF;");
            s.Append("public static class Script {");
            s.Append(
                "public static System.Collections.Generic.List<HMapEdit.Objects.Fixture> objects { get { return HMapEdit.Objects.Fixtures; } }");
            s.Append(
                "public static System.Collections.Generic.Dictionary<int,HMapEdit.Objects.NIF>.ValueCollection nifs { get { return HMapEdit.Objects.NIFs.Values; } }");
            s.AppendLine("public static void log(object o) { HMapEdit.ScriptForm.Console(o); }");
            s.Append(input.Text);
            s.AppendLine("}");

            CompilerResults r = CodeDomProvider.CreateProvider("c#").CompileAssemblyFromSource(cp, s.ToString());

            if (r.Errors.Count > 0) throw new ArgumentException("Compiler Error: \r\n" + r.Errors[0]);

            r.CompiledAssembly.GetType("Script").InvokeMember("Run",
                                                              BindingFlags.InvokeMethod,
                                                              null, null, null);
        }

        private void ScriptForm_FormClosing(object sender, FormClosingEventArgs e) {
            e.Cancel = true;
            HideForm();
        }
    }
}