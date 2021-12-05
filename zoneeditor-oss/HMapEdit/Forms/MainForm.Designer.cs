namespace HMapEdit
{
    partial class MainForm
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
          System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
          this.menuStrip1 = new System.Windows.Forms.MenuStrip();
          this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
          this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
          this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
          this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
          this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
          this.reloadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
          this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
          this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
          this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
          this.undoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
          this.nIFSToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
          this.cUSTOMToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
          this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
          this.clearUndolistToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
          this.unfreezeAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
          this.unhideAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
          this.setCameraHeightToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
          this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
          this.switchToOrtoLHMatrixToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
          this.switchAlphamodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
          this.resetDeviceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
          this.smoothEdgesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
          this.resetBusyFlagDEBUGToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
          this.gridToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
          this.editorSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
          this.zoneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
          this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
          this.helpToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
          this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
          this.shortcutsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
          this.folder = new System.Windows.Forms.FolderBrowserDialog();
          this.grid = new System.Windows.Forms.PropertyGrid();
          this.toolStrip1 = new System.Windows.Forms.ToolStrip();
          this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
          this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
          this.toolStripButton3 = new System.Windows.Forms.ToolStripButton();
          this.toolStripButton4 = new System.Windows.Forms.ToolStripButton();
          this.toolStripButton5 = new System.Windows.Forms.ToolStripButton();
          this.toolStripButton6 = new System.Windows.Forms.ToolStripButton();
          this.toolStripButton7 = new System.Windows.Forms.ToolStripButton();
          this.toolStripButton8 = new System.Windows.Forms.ToolStripButton();
          this.toolStripButton9 = new System.Windows.Forms.ToolStripButton();
          this.toolStripButton10 = new System.Windows.Forms.ToolStripButton();
          this.toolStripButton11 = new System.Windows.Forms.ToolStripButton();
          this.statusStrip1 = new System.Windows.Forms.StatusStrip();
          this.status_text = new System.Windows.Forms.ToolStripStatusLabel();
          this.toolUnsaved = new System.Windows.Forms.ToolStripStatusLabel();
          this.status_obj = new System.Windows.Forms.ToolStripStatusLabel();
          this.status_coords = new System.Windows.Forms.ToolStripStatusLabel();
          this.renderControl1 = new HMapEdit.RenderControl();
          this.dbgCmdDEBUGToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
          this.menuStrip1.SuspendLayout();
          this.toolStrip1.SuspendLayout();
          this.statusStrip1.SuspendLayout();
          this.SuspendLayout();
          // 
          // menuStrip1
          // 
          this.menuStrip1.ImageScalingSize = new System.Drawing.Size(0, 0);
          this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.gridToolStripMenuItem,
            this.helpToolStripMenuItem});
          this.menuStrip1.Location = new System.Drawing.Point(0, 0);
          this.menuStrip1.Name = "menuStrip1";
          this.menuStrip1.Size = new System.Drawing.Size(834, 24);
          this.menuStrip1.TabIndex = 1;
          this.menuStrip1.Text = "menuStrip1";
          // 
          // fileToolStripMenuItem
          // 
          this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.toolStripMenuItem2,
            this.closeToolStripMenuItem,
            this.reloadToolStripMenuItem,
            this.toolStripMenuItem3,
            this.exitToolStripMenuItem});
          this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
          this.fileToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
          this.fileToolStripMenuItem.Text = "File";
          // 
          // loadToolStripMenuItem
          // 
          this.loadToolStripMenuItem.Name = "loadToolStripMenuItem";
          this.loadToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
          this.loadToolStripMenuItem.Text = "Load";
          this.loadToolStripMenuItem.Click += new System.EventHandler(this.loadToolStripMenuItem_Click);
          // 
          // saveToolStripMenuItem
          // 
          this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
          this.saveToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
          this.saveToolStripMenuItem.Text = "Save";
          this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
          // 
          // toolStripMenuItem2
          // 
          this.toolStripMenuItem2.Name = "toolStripMenuItem2";
          this.toolStripMenuItem2.Size = new System.Drawing.Size(104, 6);
          // 
          // closeToolStripMenuItem
          // 
          this.closeToolStripMenuItem.Enabled = false;
          this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
          this.closeToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
          this.closeToolStripMenuItem.Text = "Unload";
          this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
          // 
          // reloadToolStripMenuItem
          // 
          this.reloadToolStripMenuItem.Enabled = false;
          this.reloadToolStripMenuItem.Name = "reloadToolStripMenuItem";
          this.reloadToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
          this.reloadToolStripMenuItem.Text = "Reload";
          this.reloadToolStripMenuItem.Click += new System.EventHandler(this.reloadToolStripMenuItem_Click);
          // 
          // toolStripMenuItem3
          // 
          this.toolStripMenuItem3.Name = "toolStripMenuItem3";
          this.toolStripMenuItem3.Size = new System.Drawing.Size(104, 6);
          // 
          // exitToolStripMenuItem
          // 
          this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
          this.exitToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
          this.exitToolStripMenuItem.Text = "Exit";
          this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
          // 
          // editToolStripMenuItem
          // 
          this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.undoToolStripMenuItem,
            this.nIFSToolStripMenuItem,
            this.cUSTOMToolStripMenuItem,
            this.toolStripMenuItem1,
            this.clearUndolistToolStripMenuItem,
            this.unfreezeAllToolStripMenuItem,
            this.unhideAllToolStripMenuItem,
            this.setCameraHeightToolStripMenuItem,
            this.toolStripMenuItem4,
            this.switchToOrtoLHMatrixToolStripMenuItem,
            this.switchAlphamodeToolStripMenuItem,
            this.resetDeviceToolStripMenuItem,
            this.smoothEdgesToolStripMenuItem,
            this.resetBusyFlagDEBUGToolStripMenuItem,
            this.dbgCmdDEBUGToolStripMenuItem});
          this.editToolStripMenuItem.Name = "editToolStripMenuItem";
          this.editToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
          this.editToolStripMenuItem.Text = "Edit";
          // 
          // undoToolStripMenuItem
          // 
          this.undoToolStripMenuItem.Name = "undoToolStripMenuItem";
          this.undoToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
          this.undoToolStripMenuItem.Size = new System.Drawing.Size(205, 22);
          this.undoToolStripMenuItem.Text = "Undo";
          this.undoToolStripMenuItem.Click += new System.EventHandler(this.undoToolStripMenuItem_Click);
          // 
          // nIFSToolStripMenuItem
          // 
          this.nIFSToolStripMenuItem.Name = "nIFSToolStripMenuItem";
          this.nIFSToolStripMenuItem.Size = new System.Drawing.Size(205, 22);
          this.nIFSToolStripMenuItem.Text = "NIFS..";
          this.nIFSToolStripMenuItem.Click += new System.EventHandler(this.nIFSToolStripMenuItem_Click);
          // 
          // cUSTOMToolStripMenuItem
          // 
          this.cUSTOMToolStripMenuItem.Name = "cUSTOMToolStripMenuItem";
          this.cUSTOMToolStripMenuItem.Size = new System.Drawing.Size(205, 22);
          this.cUSTOMToolStripMenuItem.Text = "Scripts...";
          this.cUSTOMToolStripMenuItem.Click += new System.EventHandler(this.cUSTOMToolStripMenuItem_Click);
          // 
          // toolStripMenuItem1
          // 
          this.toolStripMenuItem1.Name = "toolStripMenuItem1";
          this.toolStripMenuItem1.Size = new System.Drawing.Size(202, 6);
          // 
          // clearUndolistToolStripMenuItem
          // 
          this.clearUndolistToolStripMenuItem.Name = "clearUndolistToolStripMenuItem";
          this.clearUndolistToolStripMenuItem.Size = new System.Drawing.Size(205, 22);
          this.clearUndolistToolStripMenuItem.Text = "Clear Undolist";
          this.clearUndolistToolStripMenuItem.Click += new System.EventHandler(this.clearUndolistToolStripMenuItem_Click);
          // 
          // unfreezeAllToolStripMenuItem
          // 
          this.unfreezeAllToolStripMenuItem.Name = "unfreezeAllToolStripMenuItem";
          this.unfreezeAllToolStripMenuItem.Size = new System.Drawing.Size(205, 22);
          this.unfreezeAllToolStripMenuItem.Text = "Unfreeze all";
          this.unfreezeAllToolStripMenuItem.Click += new System.EventHandler(this.unfreezeAllToolStripMenuItem_Click);
          // 
          // unhideAllToolStripMenuItem
          // 
          this.unhideAllToolStripMenuItem.Name = "unhideAllToolStripMenuItem";
          this.unhideAllToolStripMenuItem.Size = new System.Drawing.Size(205, 22);
          this.unhideAllToolStripMenuItem.Text = "Unhide all";
          this.unhideAllToolStripMenuItem.Click += new System.EventHandler(this.unhideAllToolStripMenuItem_Click);
          // 
          // setCameraHeightToolStripMenuItem
          // 
          this.setCameraHeightToolStripMenuItem.Name = "setCameraHeightToolStripMenuItem";
          this.setCameraHeightToolStripMenuItem.Size = new System.Drawing.Size(205, 22);
          this.setCameraHeightToolStripMenuItem.Text = "Set Camera Height..";
          this.setCameraHeightToolStripMenuItem.Click += new System.EventHandler(this.setCameraHeightToolStripMenuItem_Click);
          // 
          // toolStripMenuItem4
          // 
          this.toolStripMenuItem4.Name = "toolStripMenuItem4";
          this.toolStripMenuItem4.Size = new System.Drawing.Size(202, 6);
          // 
          // switchToOrtoLHMatrixToolStripMenuItem
          // 
          this.switchToOrtoLHMatrixToolStripMenuItem.Name = "switchToOrtoLHMatrixToolStripMenuItem";
          this.switchToOrtoLHMatrixToolStripMenuItem.Size = new System.Drawing.Size(205, 22);
          this.switchToOrtoLHMatrixToolStripMenuItem.Text = "Switch to OrtoLH Matrix";
          this.switchToOrtoLHMatrixToolStripMenuItem.Click += new System.EventHandler(this.switchToOrtoLHMatrixToolStripMenuItem_Click);
          // 
          // switchAlphamodeToolStripMenuItem
          // 
          this.switchAlphamodeToolStripMenuItem.Name = "switchAlphamodeToolStripMenuItem";
          this.switchAlphamodeToolStripMenuItem.Size = new System.Drawing.Size(205, 22);
          this.switchAlphamodeToolStripMenuItem.Text = "Switch Alphamode (DEBUG)";
          this.switchAlphamodeToolStripMenuItem.Click += new System.EventHandler(this.switchAlphamodeToolStripMenuItem_Click);
          // 
          // resetDeviceToolStripMenuItem
          // 
          this.resetDeviceToolStripMenuItem.Name = "resetDeviceToolStripMenuItem";
          this.resetDeviceToolStripMenuItem.Size = new System.Drawing.Size(205, 22);
          this.resetDeviceToolStripMenuItem.Text = "Reset Device (DEBUG)";
          this.resetDeviceToolStripMenuItem.Click += new System.EventHandler(this.resetDeviceToolStripMenuItem_Click);
          // 
          // smoothEdgesToolStripMenuItem
          // 
          this.smoothEdgesToolStripMenuItem.Name = "smoothEdgesToolStripMenuItem";
          this.smoothEdgesToolStripMenuItem.Size = new System.Drawing.Size(205, 22);
          this.smoothEdgesToolStripMenuItem.Text = "Smooth Map (DEBUG)";
          this.smoothEdgesToolStripMenuItem.Click += new System.EventHandler(this.smoothEdgesToolStripMenuItem_Click);
          // 
          // resetBusyFlagDEBUGToolStripMenuItem
          // 
          this.resetBusyFlagDEBUGToolStripMenuItem.Name = "resetBusyFlagDEBUGToolStripMenuItem";
          this.resetBusyFlagDEBUGToolStripMenuItem.Size = new System.Drawing.Size(205, 22);
          this.resetBusyFlagDEBUGToolStripMenuItem.Text = "Reset Busy-Flag (DEBUG)";
          this.resetBusyFlagDEBUGToolStripMenuItem.Click += new System.EventHandler(this.resetBusyFlagDEBUGToolStripMenuItem_Click);
          // 
          // gridToolStripMenuItem
          // 
          this.gridToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editorSettingsToolStripMenuItem,
            this.zoneToolStripMenuItem});
          this.gridToolStripMenuItem.Name = "gridToolStripMenuItem";
          this.gridToolStripMenuItem.Size = new System.Drawing.Size(38, 20);
          this.gridToolStripMenuItem.Text = "Grid";
          // 
          // editorSettingsToolStripMenuItem
          // 
          this.editorSettingsToolStripMenuItem.Name = "editorSettingsToolStripMenuItem";
          this.editorSettingsToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
          this.editorSettingsToolStripMenuItem.Text = "Editor Settings";
          this.editorSettingsToolStripMenuItem.Click += new System.EventHandler(this.editorSettingsToolStripMenuItem_Click);
          // 
          // zoneToolStripMenuItem
          // 
          this.zoneToolStripMenuItem.Name = "zoneToolStripMenuItem";
          this.zoneToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
          this.zoneToolStripMenuItem.Text = "Zone";
          this.zoneToolStripMenuItem.Click += new System.EventHandler(this.zoneToolStripMenuItem_Click);
          // 
          // helpToolStripMenuItem
          // 
          this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.helpToolStripMenuItem1,
            this.aboutToolStripMenuItem,
            this.shortcutsToolStripMenuItem});
          this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
          this.helpToolStripMenuItem.Size = new System.Drawing.Size(24, 20);
          this.helpToolStripMenuItem.Text = "?";
          // 
          // helpToolStripMenuItem1
          // 
          this.helpToolStripMenuItem1.Name = "helpToolStripMenuItem1";
          this.helpToolStripMenuItem1.Size = new System.Drawing.Size(120, 22);
          this.helpToolStripMenuItem1.Text = "Help..";
          this.helpToolStripMenuItem1.Click += new System.EventHandler(this.helpToolStripMenuItem1_Click);
          // 
          // aboutToolStripMenuItem
          // 
          this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
          this.aboutToolStripMenuItem.Size = new System.Drawing.Size(120, 22);
          this.aboutToolStripMenuItem.Text = "About";
          this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
          // 
          // shortcutsToolStripMenuItem
          // 
          this.shortcutsToolStripMenuItem.Name = "shortcutsToolStripMenuItem";
          this.shortcutsToolStripMenuItem.Size = new System.Drawing.Size(120, 22);
          this.shortcutsToolStripMenuItem.Text = "Shortcuts";
          this.shortcutsToolStripMenuItem.Click += new System.EventHandler(this.shortcutsToolStripMenuItem_Click);
          // 
          // folder
          // 
          this.folder.RootFolder = System.Environment.SpecialFolder.MyComputer;
          // 
          // grid
          // 
          this.grid.Dock = System.Windows.Forms.DockStyle.Right;
          this.grid.Location = new System.Drawing.Point(631, 24);
          this.grid.Name = "grid";
          this.grid.Size = new System.Drawing.Size(203, 544);
          this.grid.TabIndex = 2;
          this.grid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.grid_PropertyValueChanged);
          this.grid.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.renderControl1_MouseWheel);
          // 
          // toolStrip1
          // 
          this.toolStrip1.Dock = System.Windows.Forms.DockStyle.Bottom;
          this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
          this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1,
            this.toolStripButton2,
            this.toolStripButton3,
            this.toolStripButton4,
            this.toolStripButton5,
            this.toolStripButton6,
            this.toolStripButton7,
            this.toolStripButton8,
            this.toolStripButton9,
            this.toolStripButton10,
            this.toolStripButton11});
          this.toolStrip1.Location = new System.Drawing.Point(0, 543);
          this.toolStrip1.Name = "toolStrip1";
          this.toolStrip1.Size = new System.Drawing.Size(631, 25);
          this.toolStrip1.TabIndex = 3;
          this.toolStrip1.Text = "toolStrip1";
          // 
          // toolStripButton1
          // 
          this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
          this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
          this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
          this.toolStripButton1.Name = "toolStripButton1";
          this.toolStripButton1.Size = new System.Drawing.Size(23, 22);
          this.toolStripButton1.Text = "Cursor";
          this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
          // 
          // toolStripButton2
          // 
          this.toolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
          this.toolStripButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton2.Image")));
          this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
          this.toolStripButton2.Name = "toolStripButton2";
          this.toolStripButton2.Size = new System.Drawing.Size(23, 22);
          this.toolStripButton2.Text = "Edit Heightmap";
          this.toolStripButton2.Click += new System.EventHandler(this.toolStripButton2_Click);
          // 
          // toolStripButton3
          // 
          this.toolStripButton3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
          this.toolStripButton3.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton3.Image")));
          this.toolStripButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
          this.toolStripButton3.Name = "toolStripButton3";
          this.toolStripButton3.Size = new System.Drawing.Size(23, 22);
          this.toolStripButton3.Text = "Smooth";
          this.toolStripButton3.Click += new System.EventHandler(this.toolStripButton3_Click);
          // 
          // toolStripButton4
          // 
          this.toolStripButton4.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
          this.toolStripButton4.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton4.Image")));
          this.toolStripButton4.ImageTransparentColor = System.Drawing.Color.Magenta;
          this.toolStripButton4.Name = "toolStripButton4";
          this.toolStripButton4.Size = new System.Drawing.Size(23, 22);
          this.toolStripButton4.Text = "Polygons";
          this.toolStripButton4.Click += new System.EventHandler(this.toolStripButton4_Click);
          // 
          // toolStripButton5
          // 
          this.toolStripButton5.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
          this.toolStripButton5.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton5.Image")));
          this.toolStripButton5.ImageTransparentColor = System.Drawing.Color.Magenta;
          this.toolStripButton5.Name = "toolStripButton5";
          this.toolStripButton5.Size = new System.Drawing.Size(23, 22);
          this.toolStripButton5.Text = "Edit Objects";
          this.toolStripButton5.Click += new System.EventHandler(this.toolStripButton5_Click);
          // 
          // toolStripButton6
          // 
          this.toolStripButton6.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
          this.toolStripButton6.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton6.Image")));
          this.toolStripButton6.ImageTransparentColor = System.Drawing.Color.Magenta;
          this.toolStripButton6.Name = "toolStripButton6";
          this.toolStripButton6.Size = new System.Drawing.Size(23, 22);
          this.toolStripButton6.Text = "Edit Zonejumps";
          this.toolStripButton6.Click += new System.EventHandler(this.toolStripButton6_Click);
          // 
          // toolStripButton7
          // 
          this.toolStripButton7.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
          this.toolStripButton7.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton7.Image")));
          this.toolStripButton7.ImageTransparentColor = System.Drawing.Color.Magenta;
          this.toolStripButton7.Name = "toolStripButton7";
          this.toolStripButton7.Size = new System.Drawing.Size(23, 22);
          this.toolStripButton7.Text = "Ruler";
          this.toolStripButton7.Click += new System.EventHandler(this.toolStripButton7_Click);
          // 
          // toolStripButton8
          // 
          this.toolStripButton8.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
          this.toolStripButton8.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton8.Image")));
          this.toolStripButton8.ImageTransparentColor = System.Drawing.Color.Magenta;
          this.toolStripButton8.Name = "toolStripButton8";
          this.toolStripButton8.Size = new System.Drawing.Size(23, 22);
          this.toolStripButton8.Text = "Lights";
          this.toolStripButton8.Click += new System.EventHandler(this.toolStripButton8_Click);
          // 
          // toolStripButton9
          // 
          this.toolStripButton9.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
          this.toolStripButton9.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton9.Image")));
          this.toolStripButton9.ImageTransparentColor = System.Drawing.Color.Magenta;
          this.toolStripButton9.Name = "toolStripButton9";
          this.toolStripButton9.Size = new System.Drawing.Size(23, 22);
          this.toolStripButton9.Text = "Scripting";
          this.toolStripButton9.Click += new System.EventHandler(this.toolStripButton9_Click);
          // 
          // toolStripButton10
          // 
          this.toolStripButton10.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
          this.toolStripButton10.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton10.Image")));
          this.toolStripButton10.ImageTransparentColor = System.Drawing.Color.Magenta;
          this.toolStripButton10.Name = "toolStripButton10";
          this.toolStripButton10.Size = new System.Drawing.Size(23, 22);
          this.toolStripButton10.Text = "Texturing";
          this.toolStripButton10.Click += new System.EventHandler(this.toolStripButton10_Click);
          // 
          // toolStripButton11
          // 
          this.toolStripButton11.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
          this.toolStripButton11.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton11.Image")));
          this.toolStripButton11.ImageTransparentColor = System.Drawing.Color.Magenta;
          this.toolStripButton11.Name = "toolStripButton11";
          this.toolStripButton11.Size = new System.Drawing.Size(23, 22);
          this.toolStripButton11.Text = "Sound Edit";
          this.toolStripButton11.Click += new System.EventHandler(this.toolStripButton11_Click);
          // 
          // statusStrip1
          // 
          this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.status_text,
            this.toolUnsaved,
            this.status_obj,
            this.status_coords});
          this.statusStrip1.Location = new System.Drawing.Point(0, 521);
          this.statusStrip1.Name = "statusStrip1";
          this.statusStrip1.Size = new System.Drawing.Size(631, 22);
          this.statusStrip1.TabIndex = 4;
          this.statusStrip1.Text = "statusStrip1";
          // 
          // status_text
          // 
          this.status_text.Name = "status_text";
          this.status_text.Size = new System.Drawing.Size(524, 17);
          this.status_text.Spring = true;
          this.status_text.Text = "Done.";
          this.status_text.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
          // 
          // toolUnsaved
          // 
          this.toolUnsaved.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
          this.toolUnsaved.Name = "toolUnsaved";
          this.toolUnsaved.Size = new System.Drawing.Size(4, 17);
          // 
          // status_obj
          // 
          this.status_obj.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
          this.status_obj.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
          this.status_obj.Name = "status_obj";
          this.status_obj.Size = new System.Drawing.Size(44, 17);
          this.status_obj.Text = "(x,y,z)";
          // 
          // status_coords
          // 
          this.status_coords.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
          this.status_coords.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
          this.status_coords.Name = "status_coords";
          this.status_coords.Size = new System.Drawing.Size(44, 17);
          this.status_coords.Text = "(x,y,z)";
          // 
          // renderControl1
          // 
          this.renderControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                      | System.Windows.Forms.AnchorStyles.Left)
                      | System.Windows.Forms.AnchorStyles.Right)));
          this.renderControl1.BackColor = System.Drawing.Color.Silver;
          this.renderControl1.Location = new System.Drawing.Point(0, 24);
          this.renderControl1.Name = "renderControl1";
          this.renderControl1.Size = new System.Drawing.Size(629, 497);
          this.renderControl1.TabIndex = 0;
          this.renderControl1.Text = "Edit Heightmap";
          this.renderControl1.Click += new System.EventHandler(this.renderControl1_Click);
          this.renderControl1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.renderControl1_KeyDown);
          this.renderControl1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.renderControl1_KeyPress);
          this.renderControl1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.renderControl1_KeyUp);
          this.renderControl1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.renderControl1_MouseDown);
          this.renderControl1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.renderControl1_MouseMove);
          this.renderControl1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.renderControl1_MouseUp);
          this.renderControl1.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.renderControl1_MouseWheel);
          this.renderControl1.Resize += new System.EventHandler(this.renderControl1_Resize);
          // 
          // dbgCmdDEBUGToolStripMenuItem
          // 
          this.dbgCmdDEBUGToolStripMenuItem.Name = "dbgCmdDEBUGToolStripMenuItem";
          this.dbgCmdDEBUGToolStripMenuItem.Size = new System.Drawing.Size(205, 22);
          this.dbgCmdDEBUGToolStripMenuItem.Text = "dbgCmd (DEBUG)";
          this.dbgCmdDEBUGToolStripMenuItem.Click += new System.EventHandler(this.dbgCmdDEBUGToolStripMenuItem_Click);
          // 
          // MainForm
          // 
          this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
          this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
          this.ClientSize = new System.Drawing.Size(834, 568);
          this.Controls.Add(this.statusStrip1);
          this.Controls.Add(this.toolStrip1);
          this.Controls.Add(this.grid);
          this.Controls.Add(this.renderControl1);
          this.Controls.Add(this.menuStrip1);
          this.DoubleBuffered = true;
          this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
          this.MainMenuStrip = this.menuStrip1;
          this.MinimizeBox = false;
          this.Name = "MainForm";
          this.Text = "DAoC Zone Editor v1.1";
          this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
          this.Load += new System.EventHandler(this.MainForm_Load);
          this.Shown += new System.EventHandler(this.MainForm_Shown);
          this.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.renderControl1_MouseWheel);
          this.menuStrip1.ResumeLayout(false);
          this.menuStrip1.PerformLayout();
          this.toolStrip1.ResumeLayout(false);
          this.toolStrip1.PerformLayout();
          this.statusStrip1.ResumeLayout(false);
          this.statusStrip1.PerformLayout();
          this.ResumeLayout(false);
          this.PerformLayout();

        }

        #endregion

		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem loadToolStripMenuItem;
		private System.Windows.Forms.FolderBrowserDialog folder;
        public RenderControl renderControl1;
		private System.Windows.Forms.ToolStripMenuItem gridToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem editorSettingsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem zoneToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
		private System.Windows.Forms.ToolStrip toolStrip1;
		private System.Windows.Forms.ToolStripButton toolStripButton1;
		private System.Windows.Forms.ToolStripButton toolStripButton2;
		private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem undoToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.ToolStripStatusLabel status_coords;
		private System.Windows.Forms.ToolStripStatusLabel status_text;
		private System.Windows.Forms.ToolStripMenuItem clearUndolistToolStripMenuItem;
		private System.Windows.Forms.ToolStripButton toolStripButton3;
		private System.Windows.Forms.ToolStripButton toolStripButton4;
		private System.Windows.Forms.ToolStripButton toolStripButton5;
		private System.Windows.Forms.ToolStripMenuItem smoothEdgesToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
		private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem nIFSToolStripMenuItem;
		private System.Windows.Forms.ToolStripButton toolStripButton6;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
		private System.Windows.Forms.ToolStripButton toolStripButton7;
		private System.Windows.Forms.ToolStripButton toolStripButton8;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem4;
		private System.Windows.Forms.ToolStripMenuItem cUSTOMToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem unhideAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem switchToOrtoLHMatrixToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem resetDeviceToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem switchAlphamodeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setCameraHeightToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem unfreezeAllToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem1;
		private System.Windows.Forms.ToolStripStatusLabel status_obj;
        private System.Windows.Forms.ToolStripMenuItem shortcutsToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton toolStripButton9;
        private System.Windows.Forms.ToolStripMenuItem reloadToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripButton toolStripButton10;
        private System.Windows.Forms.ToolStripMenuItem resetBusyFlagDEBUGToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel toolUnsaved;
        private System.Windows.Forms.ToolStripButton toolStripButton11;
        public System.Windows.Forms.PropertyGrid grid;
        private System.Windows.Forms.ToolStripMenuItem dbgCmdDEBUGToolStripMenuItem;
    }
}

