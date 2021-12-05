using System;
using System.Windows.Forms;
using HMapEdit.Engine;

namespace HMapEdit.Forms {
    public partial class SoundForm : Form {
        public SoundForm() {
            InitializeComponent();
        }

        private void SoundForm_FormClosing(object sender, FormClosingEventArgs e) {
            Program.FORM.m_SoundForm = null;
        }

        private void SoundForm_Shown(object sender, EventArgs e) {
            UpdateData();
        }

        private void UpdateData() {
            regions.Items.Clear();


            //Regions
            foreach (SoundRegion r in SoundMgr.SoundRegions)
                regions.Items.Add(r.ToString());

            //Sounds
            if (SoundMgr.CurrentRegion != null) regions.SelectedIndex = SoundMgr.SoundRegions.IndexOf(SoundMgr.CurrentRegion);
        }

        private void UpdateSounds() {
            sounds.Items.Clear();
            //Sounds
            if (SoundMgr.CurrentRegion != null) {
                foreach (Sound s in SoundMgr.CurrentRegion.Sounds)
                    sounds.Items.Add(s.ToString());
            }
        }

        private void addRegion_Click(object sender, EventArgs e) {
            SoundMgr.SoundRegions.Add(new SoundRegion());
            UpdateData();
            regions.SelectedIndex = regions.Items.Count - 1;
        }

        private void delRegion_Click(object sender, EventArgs e) {
            if (regions.SelectedItem == null)
                return;
            SoundMgr.SoundRegions.RemoveAt(regions.SelectedIndex);
            Program.FORM.grid.SelectedObject = null;
            UpdateData();
        }

        private void addSound_Click(object sender, EventArgs e) {
            if (SoundMgr.CurrentRegion == null)
                return;
            SoundMgr.CurrentRegion.Sounds.Add(new Sound());
            UpdateSounds();
            sounds.SelectedIndex = sounds.Items.Count - 1;
        }

        private void delSound_Click(object sender, EventArgs e) {
            if (SoundMgr.CurrentRegion == null)
                return;
            if (sounds.SelectedItem == null)
                return;
            SoundMgr.CurrentRegion.Sounds.RemoveAt(sounds.SelectedIndex);
            Program.FORM.grid.SelectedObject = SoundMgr.CurrentRegion;
            UpdateSounds();
        }

        private void regions_SelectedIndexChanged(object sender, EventArgs e) {
            if (regions.SelectedItem != null) {
                SoundMgr.CurrentRegion = SoundMgr.SoundRegions[regions.SelectedIndex];
                Program.FORM.renderControl1.Render();
                Program.FORM.grid.SelectedObject = SoundMgr.CurrentRegion;
                UpdateSounds();
            }
        }

        private void sounds_SelectedIndexChanged(object sender, EventArgs e) {
            if (sounds.SelectedItem != null) Program.FORM.grid.SelectedObject = SoundMgr.CurrentRegion.Sounds[sounds.SelectedIndex];
        }
    }
}