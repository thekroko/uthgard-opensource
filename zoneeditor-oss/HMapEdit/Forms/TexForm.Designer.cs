namespace HMapEdit.Forms
{
    partial class TexForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.texCtrl1 = new HMapEdit.Forms.TexCtrl();
            this.SuspendLayout();
            // 
            // texCtrl1
            // 
            this.texCtrl1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.texCtrl1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.texCtrl1.Location = new System.Drawing.Point(-1, -1);
            this.texCtrl1.Name = "texCtrl1";
            this.texCtrl1.Size = new System.Drawing.Size(150, 484);
            this.texCtrl1.TabIndex = 0;
            // 
            // TexForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(148, 482);
            this.Controls.Add(this.texCtrl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "TexForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Patch";
            this.TopMost = true;
            this.VisibleChanged += new System.EventHandler(this.TexForm_VisibleChanged);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TexForm_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private TexCtrl texCtrl1;
    }
}