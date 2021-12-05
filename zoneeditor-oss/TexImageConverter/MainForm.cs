using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.DirectX.Direct3D;

namespace ImageConverter
{
    public partial class MainForm : Form
    {
		public MainForm(string[] args)
			: this()
		{
			if (args.Length >= 2)
			{
				src.Text = args[0];
				target.Text = args[1];
			}
		}

        public MainForm()
        {
            InitializeComponent();
        }

        private void buttonsrc_Click(object sender, EventArgs e)
        {
            if (open.ShowDialog() == DialogResult.OK)
            {
                src.Text = open.FileName;
            }
        }

        private void buttontarget_Click(object sender, EventArgs e)
        {
            if (save.ShowDialog() == DialogResult.OK)
            {
                target.Text = save.FileName;
            }
        }

        private void convert_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(src.Text) || string.IsNullOrEmpty(target.Text))
                return;

            Texture t = Images.LoadFromFile(src.Text, "");
            Images.SaveToFile(t, target.Text,
                              (ImageFileFormat)Enum.Parse(typeof (ImageFileFormat),
                                         target.Text.Substring(target.Text.LastIndexOf('.') + 1), true));
        }
    }
}