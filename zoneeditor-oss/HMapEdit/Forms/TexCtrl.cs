using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using HMapEdit.Engine;
using HMapEdit.Tools;
using Microsoft.DirectX.Direct3D;

namespace HMapEdit.Forms {
    public partial class TexCtrl : UserControl {
        public TexCtrl() {
            InitializeComponent();

          list.Items.AddRange(GameData.FindAllTerrainTex().Select(Path.GetFileName).Select(x => (object)x).ToArray());
        }

        public void UpdateData() {
            switch (PatchMap.SelectedLayer) {
                case 1:
                    if (!l1.Checked)
                      l1.Checked = true;
                    break;
                case 2:
                    if (!l2.Checked)
                      l2.Checked = true;
                    break;
                case 3:
                    if (!l3.Checked)
                      l3.Checked = true;
                    break;
                case 4:
                    if (!l3.Checked)
                      l4.Checked = true;
                    break;
                case 5:
                    if (!l5.Checked)
                      l5.Checked = true;
                    break;
            }

            Color c = PatchMap.SelectedColor;

            if (c == PatchMap.RED) br.Checked = true;
            if (c == PatchMap.GREEN) bg.Checked = true;
            if (c == PatchMap.BLUE) bb.Checked = true;
            if (c == PatchMap.BLACK) be.Checked = true;

            l1_CheckedChanged(this, EventArgs.Empty);
        }

        private void l1_CheckedChanged(object sender, EventArgs e) {
            //Selected layer?
            int l = 1;

            if (l1.Checked) l = 1;
            else if (l2.Checked) l = 2;
            else if (l3.Checked) l = 3;
            else if (l4.Checked) l = 4;
            else if (l5.Checked) l = 5;

            PatchMap.SelectedLayer = l;

            //Black
            if (l == 1) {
                be.Checked = false;
                be.Enabled = false;
                be.Visible = false;
                br.Checked = true;
            }
            else {
                be.Enabled = true;
                be.Visible = true;
                be.Checked = true;
            }

            lr.Text = TerrainTex.Get(PatchMap.CurrentInfo.R[l - 1]);
            lg.Text = TerrainTex.Get(PatchMap.CurrentInfo.G[l - 1]);
            lb.Text = TerrainTex.Get(PatchMap.CurrentInfo.B[l - 1]);
        }

        /// <summary>
        /// Update Image
        /// </summary>
        public void UpdateImage(Texture t) {
            if (t == null) {
                if (br.Checked) t = PatchMap.CurrentInfo.R[PatchMap.SelectedLayer - 1];
                else if (bg.Checked) t = PatchMap.CurrentInfo.G[PatchMap.SelectedLayer - 1];
                else if (bb.Checked) t = PatchMap.CurrentInfo.B[PatchMap.SelectedLayer - 1];
            }

            if (t == null) {
                image.Image = null;
                return;
            }

            Stream s = TextureLoader.SaveToStream(ImageFileFormat.Jpg, t);
            s.Position = 0;
            image.Image = Image.FromStream(s);
            s.Dispose();
        }

        private void br_CheckedChanged(object sender, EventArgs e) {
            if (be.Checked) {
                PatchMap.SelectedColor = PatchMap.BLACK;
                return;
            }

            if (br.Checked) {
                PatchMap.SelectedColor = PatchMap.RED;
                tiles.Text = PatchMap.CurrentInfo.RTiles[PatchMap.SelectedLayer - 1].ToString();
                list.SelectedItem = TerrainTex.Get(PatchMap.CurrentInfo.R[PatchMap.SelectedLayer - 1]);
            }
            if (bg.Checked) {
                PatchMap.SelectedColor = PatchMap.GREEN;
                tiles.Text = PatchMap.CurrentInfo.GTiles[PatchMap.SelectedLayer - 1].ToString();
                list.SelectedItem = TerrainTex.Get(PatchMap.CurrentInfo.G[PatchMap.SelectedLayer - 1]);
            }
            if (bb.Checked) {
                PatchMap.SelectedColor = PatchMap.BLUE;
                tiles.Text = PatchMap.CurrentInfo.BTiles[PatchMap.SelectedLayer - 1].ToString();
                list.SelectedItem = TerrainTex.Get(PatchMap.CurrentInfo.B[PatchMap.SelectedLayer - 1]);
            }
            UpdateImage(null);
        }

        private void bChange_Click(object sender, EventArgs e) {
            if (list.SelectedItem == null)
                return;

            string file = list.SelectedItem.ToString();
            Texture t = TerrainTex.Get(file);

            int val;

            if (!int.TryParse(tiles.Text, out val) || val <= 0)
                val = 256;
            tiles.Text = val.ToString();

            if (br.Checked) {
                PatchMap.CurrentInfo.R[PatchMap.SelectedLayer - 1] = t;
                PatchMap.CurrentInfo.RTiles[PatchMap.SelectedLayer - 1] = val;
            }
            if (bg.Checked) {
                PatchMap.CurrentInfo.G[PatchMap.SelectedLayer - 1] = t;
                PatchMap.CurrentInfo.GTiles[PatchMap.SelectedLayer - 1] = val;
            }
            if (bb.Checked) {
                PatchMap.CurrentInfo.B[PatchMap.SelectedLayer - 1] = t;
                PatchMap.CurrentInfo.BTiles[PatchMap.SelectedLayer - 1] = val;
            }

            UpdateData();
            UpdateImage(null);
        }

        private void list_SelectedIndexChanged(object sender, EventArgs e) {
            if (list.SelectedItem != null)
                UpdateImage(TerrainTex.Get(list.SelectedItem.ToString()));
        }
    }
}