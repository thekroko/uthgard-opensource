namespace HMapEdit.Forms
{
    partial class ScriptConsole
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
            this.box = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // box
            // 
            this.box.BackColor = System.Drawing.Color.Black;
            this.box.Dock = System.Windows.Forms.DockStyle.Fill;
            this.box.Font = new System.Drawing.Font("Lucida Console", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.box.ForeColor = System.Drawing.Color.Lime;
            this.box.Location = new System.Drawing.Point(0, 0);
            this.box.Multiline = true;
            this.box.Name = "box";
            this.box.ReadOnly = true;
            this.box.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.box.Size = new System.Drawing.Size(394, 313);
            this.box.TabIndex = 0;
            // 
            // ScriptConsole
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(394, 313);
            this.Controls.Add(this.box);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MinimizeBox = false;
            this.Name = "ScriptConsole";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "ScriptConsole";
            this.TopMost = true;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ScriptConsole_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox box;
    }
}