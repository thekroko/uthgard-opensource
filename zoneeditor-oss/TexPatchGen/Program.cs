using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace PatchGen
{
    static class Program
    {
        public static string ter { get; private set; }
        public static string terraintex { get; private set; }
        public static string tex { get; private set; }
        public static string output { get; private set;}
        public static int gensize { get; private set; }
        public static int targetsize { get; private set; }
        public static int workers { get; private set; }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [MTAThread]
        static void Main(string[] args)
        {
            ter = args[0];
            terraintex = args[1];
            tex = args[2];
            output = args[3];
            gensize = int.Parse(args[4]);
            targetsize = int.Parse(args[5]);
            workers = int.Parse(args[6]);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
