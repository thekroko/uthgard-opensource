using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using HMapEdit.Tools;

namespace HMapEdit {
  public partial class NIFForm : Form {
    public NIFForm() {
      InitializeComponent();
    }

    private void NIFForm_Load(object sender, EventArgs e) {}

    private void LoadLoaded() {
      Loading.ShowLoading();
      Loading.Update("Loading Current Images...");
      var ilist = new ImageList();
      ilist.ImageSize = new Size(130, 100);
      loaded.Items.Clear();
      var items = new List<ListViewItem>();
      foreach (Objects.NIF n in Objects.NIFs.Values) {
        Image img = null; // TODO: fix me
        int index = -1;

        if (img != null) {
          ilist.Images.Add(img);
          index = ilist.Images.Count - 1;
        }

        items.Add(new ListViewItem(n.FileName, index));
      }
      loaded.LargeImageList = ilist;
      loaded.Items.AddRange(items.ToArray());
      Loading.CloseLoading();
    }

    private void AddFolder(string folder, TreeNode node) {
      var n = new TreeNode(Path.GetFileName(folder));
      n.Name = folder;

      if (!Directory.Exists(folder))
        return;

      foreach (string dir in Directory.GetDirectories(folder)) {
        if (dir.Contains(".svn"))
          continue;

        AddFolder(dir, n);
      }

      if (node != null) node.Nodes.Add(n);
      else tv.Nodes.Add(n);
    }

    private void tv_AfterSelect(object sender, TreeViewEventArgs e) {
      string folder = tv.SelectedNode.Name;
      BrowseFolder(folder);
    }

    private void BrowseFolder(string folder) {
      Loading.ShowLoading();
      Loading.Update("Loading Folder Images...");
      var items = new List<ListViewItem>();
      var imgs = new ImageList();
      cur.Items.Clear();
      imgs.ImageSize = new Size(130, 100);

      var used = new List<string>();
      foreach (Objects.NIF n in Objects.NIFs.Values)
        used.Add(n.FileName);

      foreach (string file in Directory.GetFiles(folder, "*.jpg", SearchOption.TopDirectoryOnly)) {
        string nif = Path.GetFileNameWithoutExtension(file);

        if (used.Contains(nif))
          continue; //already in use

        if (nonmodel.Checked || Directory.GetFiles(folder, nif + ".obj", SearchOption.AllDirectories).Length > 0 ||
            Directory.GetFiles(folder, nif + ".bb", SearchOption.AllDirectories).Length > 0) {
          imgs.Images.Add(Image.FromFile(file));
          items.Add(new ListViewItem(nif, imgs.Images.Count - 1));
        }
      }

      cur.LargeImageList = imgs;
      cur.Items.AddRange(items.ToArray());
      Loading.CloseLoading();
    }

    private void cur_SelectedIndexChanged(object sender, EventArgs e) {
      button2.Enabled = cur.SelectedItems.Count > 0;
    }

    private void button2_Click(object sender, EventArgs e) {
      foreach (ListViewItem i in cur.SelectedItems) {
        string nif = i.Text;

        //Check if already added
        bool already = false;
        foreach (Objects.NIF n in Objects.NIFs.Values) {
          if (n.FileName == nif) {
            already = true;
            break; //already added
          }
        }

        if (already) continue;

        if (Objects.NIFs.Count >= 200) {
          MessageBox.Show("Maximum NIF Count reached! (200)", "Error", MessageBoxButtons.OK,
                          MessageBoxIcon.Error);
          break;
        }

        //Add
        var nn = new Objects.NIF();
        nn.FileName = nif;
        nn.ID = ++Objects.NIFS_Max;
        nn.LoadData();
        Objects.NIFs.Add(nn.ID, nn);
        SelectNIFDialog.reload = true;
      }

      LoadLoaded();
    }

    private void button1_Click(object sender, EventArgs e) {
      var nifs = new List<Objects.NIF>();
      foreach (Objects.NIF n in Objects.NIFs.Values) nifs.Add(n);

      foreach (Objects.Fixture f in Objects.Fixtures) if (nifs.Contains(f.NIF)) nifs.Remove(f.NIF);

      foreach (Objects.NIF rem in nifs) Objects.NIFs.Remove(rem.ID);

      LoadLoaded();

      SelectNIFDialog.reload = true;
    }

    private void button3_Click(object sender, EventArgs e) {
      string nif = loaded.SelectedItems[0].Text;
      Objects.NIF obj = null;

      foreach (Objects.NIF n in Objects.NIFs.Values) {
        if (n.FileName == nif) {
          obj = n;
          break;
        }
      }

      if (obj == null) return;

      foreach (Objects.Fixture f in Objects.Fixtures) {
        if (f.NIF == obj) {
          MessageBox.Show("NIF is currently in use.");
          return;
        }
      }

      Objects.NIFs.Remove(obj.ID);
      LoadLoaded();

      SelectNIFDialog.reload = true;
    }

    private void loaded_SelectedIndexChanged(object sender, EventArgs e) {
      button3.Enabled = loaded.SelectedItems.Count > 0;

      if (loaded.SelectedItems.Count > 0) {
        Objects.NIF obj = null;
        string nif = loaded.SelectedItems[0].Text;

        foreach (Objects.NIF n in Objects.NIFs.Values) {
          if (n.FileName == nif) {
            obj = n;
            break;
          }
        }

        grid.SelectedObject = obj;
      }
    }

    private void nonmodel_CheckedChanged(object sender, EventArgs e) {
      string folder = tv.SelectedNode.Name;
      BrowseFolder(folder);
    }

    private void NIFForm_Shown(object sender, EventArgs e) {
      //Init loaded
      LoadLoaded();

      //Init treeview
      tv.Nodes.Clear();
      AddFolder(Objects.DIR_OBJECTS, null);
      tv.ExpandAll();
    }
  }
}