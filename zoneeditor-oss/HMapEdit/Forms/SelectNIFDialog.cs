using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using HMapEdit.Tools;

namespace HMapEdit {
    public partial class SelectNIFDialog : Form {
        public static bool reload = true;

        public SelectNIFDialog() {
            InitializeComponent();
        }

        public Objects.NIF SelectedNIF {
            get {
                if (list.SelectedItems.Count == 0) return null;

                ListViewItem sel = list.SelectedItems[0];
                return Objects.NIFs[int.Parse(sel.Name)];
            }
        }

        public void LoadGroups() {
            group.Items.Clear();
            group.Items.Add("");
            foreach (Objects.NIF n in Objects.NIFs.Values) {
                if (!group.Items.Contains(n.Group.ToLower()))
                    group.Items.Add(n.Group.ToLower());
            }
        }

        private void list_SelectedIndexChanged(object sender, EventArgs e) {
            accept.Enabled = list.SelectedItems.Count > 0;
        }

        public void RefreshData() {
            string grp = group.Text.ToLower();
            Loading.ShowLoading();
            Loading.Update("Loading Images...");

            List<ListViewItem> l = new List<ListViewItem>();
            ImageList imgs = new ImageList();
            imgs.ImageSize = new Size(130, 100);

            foreach (Objects.NIF n in Objects.NIFs.Values) {
                if (!string.IsNullOrEmpty(grp) && n.Group.ToLower() != grp)
                    continue;

                
                //Image img = ImageMgr.Get(n.FileName);
              Image img = null;
                int index = -1;

                if (img != null) {
                    imgs.Images.Add(img);
                    index = imgs.Images.Count - 1;
                }

                ListViewItem item = new ListViewItem();
                item.Text = n.FileName;
                item.Name = n.ID.ToString();
                item.ImageIndex = index;
                l.Add(item);
            }

            list.Items.Clear();
            list.LargeImageList = imgs;
            list.Items.AddRange(l.ToArray());

            Loading.CloseLoading();
        }

        private void SelectNIFDialog_Load(object sender, EventArgs e) {
            if (reload) {
                reload = false;
                RefreshData();
            }
        }

        private void button1_Click(object sender, EventArgs e) {
            LoadGroups();
            RefreshData();
        }

        private void group_SelectedIndexChanged(object sender, EventArgs e) {
            RefreshData();
        }

        private void SelectNIFDialog_Shown(object sender, EventArgs e) {
            LoadGroups();
        }
    }
}