namespace HMapEdit.Forms
{
    partial class TexCtrl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.list = new System.Windows.Forms.ListBox();
            this.l1 = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.l5 = new System.Windows.Forms.RadioButton();
            this.l4 = new System.Windows.Forms.RadioButton();
            this.l3 = new System.Windows.Forms.RadioButton();
            this.l2 = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.be = new System.Windows.Forms.RadioButton();
            this.lb = new System.Windows.Forms.Label();
            this.bb = new System.Windows.Forms.RadioButton();
            this.lg = new System.Windows.Forms.Label();
            this.bg = new System.Windows.Forms.RadioButton();
            this.lr = new System.Windows.Forms.Label();
            this.br = new System.Windows.Forms.RadioButton();
            this.image = new System.Windows.Forms.PictureBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.bChange = new System.Windows.Forms.Button();
            this.tiles = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.image)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // list
            // 
            this.list.BackColor = System.Drawing.Color.White;
            this.list.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.list.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.list.FormattingEnabled = true;
            this.list.ItemHeight = 12;
            this.list.Location = new System.Drawing.Point(3, 306);
            this.list.Name = "list";
            this.list.ScrollAlwaysVisible = true;
            this.list.Size = new System.Drawing.Size(142, 146);
            this.list.TabIndex = 4;
            this.list.SelectedIndexChanged += new System.EventHandler(this.list_SelectedIndexChanged);
            // 
            // l1
            // 
            this.l1.Appearance = System.Windows.Forms.Appearance.Button;
            this.l1.AutoSize = true;
            this.l1.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l1.Location = new System.Drawing.Point(6, 12);
            this.l1.Name = "l1";
            this.l1.Size = new System.Drawing.Size(21, 22);
            this.l1.TabIndex = 5;
            this.l1.TabStop = true;
            this.l1.Text = "1";
            this.l1.UseVisualStyleBackColor = true;
            this.l1.CheckedChanged += new System.EventHandler(this.l1_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.l5);
            this.groupBox1.Controls.Add(this.l4);
            this.groupBox1.Controls.Add(this.l3);
            this.groupBox1.Controls.Add(this.l2);
            this.groupBox1.Controls.Add(this.l1);
            this.groupBox1.Location = new System.Drawing.Point(3, -1);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(142, 38);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Layer";
            // 
            // l5
            // 
            this.l5.Appearance = System.Windows.Forms.Appearance.Button;
            this.l5.AutoSize = true;
            this.l5.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l5.Location = new System.Drawing.Point(114, 12);
            this.l5.Name = "l5";
            this.l5.Size = new System.Drawing.Size(21, 22);
            this.l5.TabIndex = 9;
            this.l5.TabStop = true;
            this.l5.Text = "5";
            this.l5.UseVisualStyleBackColor = true;
            this.l5.CheckedChanged += new System.EventHandler(this.l1_CheckedChanged);
            // 
            // l4
            // 
            this.l4.Appearance = System.Windows.Forms.Appearance.Button;
            this.l4.AutoSize = true;
            this.l4.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l4.Location = new System.Drawing.Point(87, 12);
            this.l4.Name = "l4";
            this.l4.Size = new System.Drawing.Size(21, 22);
            this.l4.TabIndex = 8;
            this.l4.TabStop = true;
            this.l4.Text = "4";
            this.l4.UseVisualStyleBackColor = true;
            this.l4.CheckedChanged += new System.EventHandler(this.l1_CheckedChanged);
            // 
            // l3
            // 
            this.l3.Appearance = System.Windows.Forms.Appearance.Button;
            this.l3.AutoSize = true;
            this.l3.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l3.Location = new System.Drawing.Point(60, 12);
            this.l3.Name = "l3";
            this.l3.Size = new System.Drawing.Size(21, 22);
            this.l3.TabIndex = 7;
            this.l3.TabStop = true;
            this.l3.Text = "3";
            this.l3.UseVisualStyleBackColor = true;
            this.l3.CheckedChanged += new System.EventHandler(this.l1_CheckedChanged);
            // 
            // l2
            // 
            this.l2.Appearance = System.Windows.Forms.Appearance.Button;
            this.l2.AutoSize = true;
            this.l2.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l2.Location = new System.Drawing.Point(33, 12);
            this.l2.Name = "l2";
            this.l2.Size = new System.Drawing.Size(21, 22);
            this.l2.TabIndex = 6;
            this.l2.TabStop = true;
            this.l2.Text = "2";
            this.l2.UseVisualStyleBackColor = true;
            this.l2.CheckedChanged += new System.EventHandler(this.l1_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.be);
            this.groupBox2.Controls.Add(this.lb);
            this.groupBox2.Controls.Add(this.bb);
            this.groupBox2.Controls.Add(this.lg);
            this.groupBox2.Controls.Add(this.bg);
            this.groupBox2.Controls.Add(this.lr);
            this.groupBox2.Controls.Add(this.br);
            this.groupBox2.Location = new System.Drawing.Point(3, 39);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(142, 108);
            this.groupBox2.TabIndex = 8;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Textures";
            // 
            // be
            // 
            this.be.AutoSize = true;
            this.be.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.be.Location = new System.Drawing.Point(5, 18);
            this.be.Name = "be";
            this.be.Size = new System.Drawing.Size(55, 16);
            this.be.TabIndex = 6;
            this.be.TabStop = true;
            this.be.Text = "Empty";
            this.be.UseVisualStyleBackColor = true;
            this.be.CheckedChanged += new System.EventHandler(this.br_CheckedChanged);
            // 
            // lb
            // 
            this.lb.AutoSize = true;
            this.lb.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb.Location = new System.Drawing.Point(37, 86);
            this.lb.Name = "lb";
            this.lb.Size = new System.Drawing.Size(89, 12);
            this.lb.TabIndex = 5;
            this.lb.Text = "XXXXXXXXXXX.dds";
            // 
            // bb
            // 
            this.bb.AutoSize = true;
            this.bb.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bb.Location = new System.Drawing.Point(6, 84);
            this.bb.Name = "bb";
            this.bb.Size = new System.Drawing.Size(30, 16);
            this.bb.TabIndex = 4;
            this.bb.TabStop = true;
            this.bb.Text = "B";
            this.bb.UseVisualStyleBackColor = true;
            this.bb.CheckedChanged += new System.EventHandler(this.br_CheckedChanged);
            // 
            // lg
            // 
            this.lg.AutoSize = true;
            this.lg.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lg.Location = new System.Drawing.Point(37, 64);
            this.lg.Name = "lg";
            this.lg.Size = new System.Drawing.Size(89, 12);
            this.lg.TabIndex = 3;
            this.lg.Text = "XXXXXXXXXXX.dds";
            // 
            // bg
            // 
            this.bg.AutoSize = true;
            this.bg.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bg.Location = new System.Drawing.Point(6, 62);
            this.bg.Name = "bg";
            this.bg.Size = new System.Drawing.Size(31, 16);
            this.bg.TabIndex = 2;
            this.bg.TabStop = true;
            this.bg.Text = "G";
            this.bg.UseVisualStyleBackColor = true;
            this.bg.CheckedChanged += new System.EventHandler(this.br_CheckedChanged);
            // 
            // lr
            // 
            this.lr.AutoSize = true;
            this.lr.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lr.Location = new System.Drawing.Point(37, 42);
            this.lr.Name = "lr";
            this.lr.Size = new System.Drawing.Size(89, 12);
            this.lr.TabIndex = 1;
            this.lr.Text = "XXXXXXXXXXX.dds";
            // 
            // br
            // 
            this.br.AutoSize = true;
            this.br.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.br.Location = new System.Drawing.Point(6, 40);
            this.br.Name = "br";
            this.br.Size = new System.Drawing.Size(31, 16);
            this.br.TabIndex = 0;
            this.br.TabStop = true;
            this.br.Text = "R";
            this.br.UseVisualStyleBackColor = true;
            this.br.CheckedChanged += new System.EventHandler(this.br_CheckedChanged);
            // 
            // image
            // 
            this.image.BackColor = System.Drawing.Color.Black;
            this.image.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.image.Location = new System.Drawing.Point(5, 13);
            this.image.Name = "image";
            this.image.Size = new System.Drawing.Size(128, 128);
            this.image.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.image.TabIndex = 9;
            this.image.TabStop = false;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.image);
            this.groupBox3.Location = new System.Drawing.Point(3, 153);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(142, 147);
            this.groupBox3.TabIndex = 10;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Image";
            // 
            // bChange
            // 
            this.bChange.Location = new System.Drawing.Point(3, 458);
            this.bChange.Name = "bChange";
            this.bChange.Size = new System.Drawing.Size(75, 23);
            this.bChange.TabIndex = 11;
            this.bChange.Text = "Change";
            this.bChange.UseVisualStyleBackColor = true;
            this.bChange.Click += new System.EventHandler(this.bChange_Click);
            // 
            // tiles
            // 
            this.tiles.Location = new System.Drawing.Point(84, 459);
            this.tiles.Name = "tiles";
            this.tiles.Size = new System.Drawing.Size(61, 20);
            this.tiles.TabIndex = 12;
            // 
            // TexCtrl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.tiles);
            this.Controls.Add(this.bChange);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.list);
            this.Name = "TexCtrl";
            this.Size = new System.Drawing.Size(150, 484);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.image)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox list;
        private System.Windows.Forms.RadioButton l1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton br;
        private System.Windows.Forms.Label lb;
        private System.Windows.Forms.RadioButton bb;
        private System.Windows.Forms.Label lg;
        private System.Windows.Forms.RadioButton bg;
        private System.Windows.Forms.Label lr;
        private System.Windows.Forms.PictureBox image;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button bChange;
        private System.Windows.Forms.RadioButton be;
        private System.Windows.Forms.TextBox tiles;
        private System.Windows.Forms.RadioButton l5;
        private System.Windows.Forms.RadioButton l4;
        private System.Windows.Forms.RadioButton l3;
        private System.Windows.Forms.RadioButton l2;
    }
}
