namespace HMapEdit.Forms
{
    partial class SoundForm
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.delRegion = new System.Windows.Forms.Button();
            this.addRegion = new System.Windows.Forms.Button();
            this.regions = new System.Windows.Forms.ListBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.delSound = new System.Windows.Forms.Button();
            this.addSound = new System.Windows.Forms.Button();
            this.sounds = new System.Windows.Forms.ListBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.delRegion);
            this.groupBox1.Controls.Add(this.addRegion);
            this.groupBox1.Controls.Add(this.regions);
            this.groupBox1.Location = new System.Drawing.Point(0, 2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 170);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Regions";
            // 
            // delRegion
            // 
            this.delRegion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.delRegion.Location = new System.Drawing.Point(101, 142);
            this.delRegion.Name = "delRegion";
            this.delRegion.Size = new System.Drawing.Size(93, 23);
            this.delRegion.TabIndex = 2;
            this.delRegion.Text = "Del";
            this.delRegion.UseVisualStyleBackColor = true;
            this.delRegion.Click += new System.EventHandler(this.delRegion_Click);
            // 
            // addRegion
            // 
            this.addRegion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.addRegion.Location = new System.Drawing.Point(6, 142);
            this.addRegion.Name = "addRegion";
            this.addRegion.Size = new System.Drawing.Size(89, 23);
            this.addRegion.TabIndex = 1;
            this.addRegion.Text = "Add";
            this.addRegion.UseVisualStyleBackColor = true;
            this.addRegion.Click += new System.EventHandler(this.addRegion_Click);
            // 
            // regions
            // 
            this.regions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.regions.FormattingEnabled = true;
            this.regions.Location = new System.Drawing.Point(6, 15);
            this.regions.Name = "regions";
            this.regions.ScrollAlwaysVisible = true;
            this.regions.Size = new System.Drawing.Size(188, 121);
            this.regions.TabIndex = 0;
            this.regions.SelectedIndexChanged += new System.EventHandler(this.regions_SelectedIndexChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.delSound);
            this.groupBox2.Controls.Add(this.addSound);
            this.groupBox2.Controls.Add(this.sounds);
            this.groupBox2.Location = new System.Drawing.Point(0, 178);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(200, 183);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Sounds";
            // 
            // delSound
            // 
            this.delSound.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.delSound.Location = new System.Drawing.Point(101, 155);
            this.delSound.Name = "delSound";
            this.delSound.Size = new System.Drawing.Size(93, 23);
            this.delSound.TabIndex = 2;
            this.delSound.Text = "Del";
            this.delSound.UseVisualStyleBackColor = true;
            this.delSound.Click += new System.EventHandler(this.delSound_Click);
            // 
            // addSound
            // 
            this.addSound.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.addSound.Location = new System.Drawing.Point(6, 155);
            this.addSound.Name = "addSound";
            this.addSound.Size = new System.Drawing.Size(89, 23);
            this.addSound.TabIndex = 1;
            this.addSound.Text = "Add";
            this.addSound.UseVisualStyleBackColor = true;
            this.addSound.Click += new System.EventHandler(this.addSound_Click);
            // 
            // sounds
            // 
            this.sounds.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.sounds.FormattingEnabled = true;
            this.sounds.Location = new System.Drawing.Point(6, 15);
            this.sounds.Name = "sounds";
            this.sounds.ScrollAlwaysVisible = true;
            this.sounds.Size = new System.Drawing.Size(188, 134);
            this.sounds.TabIndex = 0;
            this.sounds.SelectedIndexChanged += new System.EventHandler(this.sounds_SelectedIndexChanged);
            // 
            // SoundForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(200, 361);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "SoundForm";
            this.Opacity = 0.9;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Sound Edit";
            this.TopMost = true;
            this.Shown += new System.EventHandler(this.SoundForm_Shown);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SoundForm_FormClosing);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button delRegion;
        private System.Windows.Forms.Button addRegion;
        private System.Windows.Forms.ListBox regions;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button delSound;
        private System.Windows.Forms.Button addSound;
        private System.Windows.Forms.ListBox sounds;
    }
}