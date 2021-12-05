using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace ImageConverter
{
    public static class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            MainForm f = new MainForm(args);
            Application.Run(f);
        }
    }
}
