using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PatchGen
{
    public enum Status {
        Inexistent,
        ToDo,
        Working,
        WorkingR,
        WorkingG,
        WorkingB,
        Done
    }
    public partial class PatchField : Control
    {
        private Status m_Status = Status.Inexistent;

        public Status Status { 
            get {
                return m_Status;
            }
            set {
                m_Status = value;
                Refresh();
            }
        }

        public PatchField()
        {
            InitializeComponent();
            Size = new Size(32, 32);
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);

            Graphics g = pe.Graphics;
            g.Clear(Color.Black);

            Color bg = Color.Magenta;

            switch (Status) {
                case Status.Inexistent:
                    bg = Color.Black;
                    break;
                case Status.ToDo:
                    bg = Color.White;
                    break;
                case Status.Working:
                    bg = Color.Yellow;
                    break;
                case Status.WorkingR:
                    bg = Color.Red;
                    break;
                case Status.WorkingG:
                    bg = Color.Green;
                    break;
                case Status.WorkingB:
                    bg = Color.Blue;
                    break;
                case Status.Done:
                    bg = Color.DimGray;
                    break;
            }

            g.FillRectangle(new SolidBrush(bg), 1, 1, Width-2, Height-2);
        }
    }
}
