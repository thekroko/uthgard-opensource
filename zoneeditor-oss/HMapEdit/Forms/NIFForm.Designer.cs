namespace HMapEdit
{
	partial class NIFForm
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
            this.loaded = new System.Windows.Forms.ListView();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.grid = new System.Windows.Forms.PropertyGrid();
            this.button3 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.nonmodel = new System.Windows.Forms.CheckBox();
            this.button2 = new System.Windows.Forms.Button();
            this.tv = new System.Windows.Forms.TreeView();
            this.cur = new System.Windows.Forms.ListView();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // loaded
            // 
            this.loaded.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.loaded.Location = new System.Drawing.Point(3, 17);
            this.loaded.Name = "loaded";
            this.loaded.Size = new System.Drawing.Size(236, 342);
            this.loaded.TabIndex = 0;
            this.loaded.TileSize = new System.Drawing.Size(130, 120);
            this.loaded.UseCompatibleStateImageBehavior = false;
            this.loaded.SelectedIndexChanged += new System.EventHandler(this.loaded_SelectedIndexChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox1.Controls.Add(this.grid);
            this.groupBox1.Controls.Add(this.button3);
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Controls.Add(this.loaded);
            this.groupBox1.Location = new System.Drawing.Point(5, 1);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(242, 582);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Loaded NIFs";
            // 
            // grid
            // 
            this.grid.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.grid.HelpVisible = false;
            this.grid.Location = new System.Drawing.Point(6, 365);
            this.grid.Name = "grid";
            this.grid.Size = new System.Drawing.Size(233, 185);
            this.grid.TabIndex = 5;
            this.grid.ToolbarVisible = false;
            // 
            // button3
            // 
            this.button3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.button3.Enabled = false;
            this.button3.Location = new System.Drawing.Point(6, 555);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(108, 23);
            this.button3.TabIndex = 2;
            this.button3.Text = "Remove";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(123, 555);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(110, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "Remove Unused";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.nonmodel);
            this.groupBox2.Controls.Add(this.button2);
            this.groupBox2.Controls.Add(this.tv);
            this.groupBox2.Controls.Add(this.cur);
            this.groupBox2.Location = new System.Drawing.Point(250, 1);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(616, 582);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Load NIFS..";
            // 
            // nonmodel
            // 
            this.nonmodel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.nonmodel.AutoSize = true;
            this.nonmodel.Location = new System.Drawing.Point(22, 552);
            this.nonmodel.Name = "nonmodel";
            this.nonmodel.Size = new System.Drawing.Size(108, 17);
            this.nonmodel.TabIndex = 5;
            this.nonmodel.Text = "Show Non-Model";
            this.nonmodel.UseVisualStyleBackColor = true;
            this.nonmodel.CheckedChanged += new System.EventHandler(this.nonmodel_CheckedChanged);
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.Enabled = false;
            this.button2.Location = new System.Drawing.Point(158, 548);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(452, 23);
            this.button2.TabIndex = 4;
            this.button2.Text = "Load";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // tv
            // 
            this.tv.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.tv.Location = new System.Drawing.Point(3, 17);
            this.tv.Name = "tv";
            this.tv.Size = new System.Drawing.Size(138, 525);
            this.tv.TabIndex = 3;
            this.tv.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tv_AfterSelect);
            // 
            // cur
            // 
            this.cur.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cur.Location = new System.Drawing.Point(142, 17);
            this.cur.Name = "cur";
            this.cur.Size = new System.Drawing.Size(471, 525);
            this.cur.TabIndex = 0;
            this.cur.TileSize = new System.Drawing.Size(140, 110);
            this.cur.UseCompatibleStateImageBehavior = false;
            this.cur.SelectedIndexChanged += new System.EventHandler(this.cur_SelectedIndexChanged);
            // 
            // NIFForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.ClientSize = new System.Drawing.Size(867, 586);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "NIFForm";
            this.ShowIcon = false;
            this.Text = "NIFs";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.NIFForm_Load);
            this.Shown += new System.EventHandler(this.NIFForm_Shown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ListView loaded;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.ListView cur;
		private System.Windows.Forms.TreeView tv;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.Button button3;
		private System.Windows.Forms.PropertyGrid grid;
		private System.Windows.Forms.CheckBox nonmodel;
	}
}