namespace PatchGen
{
    partial class MainForm
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
            this.bar = new System.Windows.Forms.ProgressBar();
            this.SuspendLayout();
            // 
            // bar
            // 
            this.bar.Location = new System.Drawing.Point(2, 3);
            this.bar.Maximum = 64;
            this.bar.Name = "bar";
            this.bar.Size = new System.Drawing.Size(256, 23);
            this.bar.TabIndex = 0;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(260, 27);
            this.Controls.Add(this.bar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "MainForm";
            this.Text = "Patchmap Generator";
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ProgressBar bar;
    }
}

