namespace ImageConverter
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.buttonsrc = new System.Windows.Forms.Button();
            this.src = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.buttontarget = new System.Windows.Forms.Button();
            this.target = new System.Windows.Forms.TextBox();
            this.convert = new System.Windows.Forms.Button();
            this.open = new System.Windows.Forms.OpenFileDialog();
            this.save = new System.Windows.Forms.SaveFileDialog();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.buttonsrc);
            this.groupBox1.Controls.Add(this.src);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(409, 44);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Source";
            // 
            // buttonsrc
            // 
            this.buttonsrc.Location = new System.Drawing.Point(384, 17);
            this.buttonsrc.Name = "buttonsrc";
            this.buttonsrc.Size = new System.Drawing.Size(19, 20);
            this.buttonsrc.TabIndex = 1;
            this.buttonsrc.UseVisualStyleBackColor = true;
            this.buttonsrc.Click += new System.EventHandler(this.buttonsrc_Click);
            // 
            // src
            // 
            this.src.Location = new System.Drawing.Point(9, 17);
            this.src.Name = "src";
            this.src.ReadOnly = true;
            this.src.Size = new System.Drawing.Size(369, 20);
            this.src.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.buttontarget);
            this.groupBox2.Controls.Add(this.target);
            this.groupBox2.Location = new System.Drawing.Point(3, 53);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(409, 44);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Target";
            // 
            // buttontarget
            // 
            this.buttontarget.Location = new System.Drawing.Point(384, 17);
            this.buttontarget.Name = "buttontarget";
            this.buttontarget.Size = new System.Drawing.Size(19, 20);
            this.buttontarget.TabIndex = 1;
            this.buttontarget.UseVisualStyleBackColor = true;
            this.buttontarget.Click += new System.EventHandler(this.buttontarget_Click);
            // 
            // target
            // 
            this.target.Location = new System.Drawing.Point(9, 17);
            this.target.Name = "target";
            this.target.ReadOnly = true;
            this.target.Size = new System.Drawing.Size(369, 20);
            this.target.TabIndex = 0;
            // 
            // convert
            // 
            this.convert.Location = new System.Drawing.Point(337, 103);
            this.convert.Name = "convert";
            this.convert.Size = new System.Drawing.Size(75, 23);
            this.convert.TabIndex = 3;
            this.convert.Text = "Convert";
            this.convert.UseVisualStyleBackColor = true;
            this.convert.Click += new System.EventHandler(this.convert_Click);
            // 
            // open
            // 
            this.open.Filter = "Images|*.bmp;*.dds;*.dib,*.hdr;*.jpg;*.pfm;*.png;*.ppm,*.tga";
            this.open.RestoreDirectory = true;
            // 
            // save
            // 
            this.save.Filter = "Bitmap (bmp)|*.bmp|DirectX (dds)|*.dds|DIB (dib)|*.dib|HDR (hdr)|*.hdr|JPEG (jpg)" +
                "|*.jpg|PFM (pfm)|*.pfm|PNG (png)|*.png|PPM (ppm)|*.ppm|Targa (tga)|*.tga";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(417, 133);
            this.Controls.Add(this.convert);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.ShowIcon = false;
            this.Text = "Image Converter";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox src;
        private System.Windows.Forms.Button buttonsrc;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button buttontarget;
        private System.Windows.Forms.TextBox target;
        private System.Windows.Forms.Button convert;
        private System.Windows.Forms.OpenFileDialog open;
        private System.Windows.Forms.SaveFileDialog save;
    }
}