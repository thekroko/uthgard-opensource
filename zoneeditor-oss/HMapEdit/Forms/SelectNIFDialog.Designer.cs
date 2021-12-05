namespace HMapEdit
{
	partial class SelectNIFDialog
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
            this.list = new System.Windows.Forms.ListView();
            this.cancel = new System.Windows.Forms.Button();
            this.accept = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.group = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // list
            // 
            this.list.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.list.FullRowSelect = true;
            this.list.Location = new System.Drawing.Point(4, 28);
            this.list.MultiSelect = false;
            this.list.Name = "list";
            this.list.Size = new System.Drawing.Size(863, 569);
            this.list.TabIndex = 0;
            this.list.TileSize = new System.Drawing.Size(140, 110);
            this.list.UseCompatibleStateImageBehavior = false;
            this.list.SelectedIndexChanged += new System.EventHandler(this.list_SelectedIndexChanged);
            // 
            // cancel
            // 
            this.cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancel.Location = new System.Drawing.Point(711, 601);
            this.cancel.Name = "cancel";
            this.cancel.Size = new System.Drawing.Size(75, 23);
            this.cancel.TabIndex = 1;
            this.cancel.Text = "Cancel";
            this.cancel.UseVisualStyleBackColor = true;
            // 
            // accept
            // 
            this.accept.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.accept.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.accept.Enabled = false;
            this.accept.Location = new System.Drawing.Point(790, 601);
            this.accept.Name = "accept";
            this.accept.Size = new System.Drawing.Size(75, 23);
            this.accept.TabIndex = 2;
            this.accept.Text = "Accept";
            this.accept.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button1.Location = new System.Drawing.Point(5, 601);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 3;
            this.button1.Text = "Refresh";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // group
            // 
            this.group.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.group.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.group.FormattingEnabled = true;
            this.group.Location = new System.Drawing.Point(5, 5);
            this.group.Name = "group";
            this.group.Size = new System.Drawing.Size(862, 21);
            this.group.TabIndex = 4;
            this.group.SelectedIndexChanged += new System.EventHandler(this.group_SelectedIndexChanged);
            // 
            // SelectNIFDialog
            // 
            this.AcceptButton = this.accept;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancel;
            this.ClientSize = new System.Drawing.Size(871, 627);
            this.Controls.Add(this.group);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.accept);
            this.Controls.Add(this.cancel);
            this.Controls.Add(this.list);
            this.Name = "SelectNIFDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Select NIF";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.SelectNIFDialog_Load);
            this.Shown += new System.EventHandler(this.SelectNIFDialog_Shown);
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ListView list;
		private System.Windows.Forms.Button cancel;
		private System.Windows.Forms.Button accept;
		private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ComboBox group;
	}
}