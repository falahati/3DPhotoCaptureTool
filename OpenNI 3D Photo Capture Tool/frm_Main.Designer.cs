namespace OpenNI_3D_Photo_Capture_Tool
{
    partial class frm_Main
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frm_Main));
            this.p_depth = new System.Windows.Forms.PictureBox();
            this.p_image = new System.Windows.Forms.PictureBox();
            this.p_depth3d = new System.Windows.Forms.PictureBox();
            this.p_image3d = new System.Windows.Forms.PictureBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cb_hd = new System.Windows.Forms.CheckBox();
            this.cb_devices = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.nud_maxdisp = new System.Windows.Forms.NumericUpDown();
            this.cb_smoothing = new System.Windows.Forms.CheckBox();
            this.cb_swap = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.but_stereo = new System.Windows.Forms.Button();
            this.but_anag = new System.Windows.Forms.Button();
            this.l_website = new System.Windows.Forms.LinkLabel();
            this.l_save = new System.Windows.Forms.LinkLabel();
            this.SaveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.but_saveall = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.p_depth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.p_image)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.p_depth3d)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.p_image3d)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nud_maxdisp)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // p_depth
            // 
            this.p_depth.BackColor = System.Drawing.SystemColors.ControlDark;
            this.p_depth.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.p_depth.Location = new System.Drawing.Point(12, 12);
            this.p_depth.Name = "p_depth";
            this.p_depth.Size = new System.Drawing.Size(160, 120);
            this.p_depth.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.p_depth.TabIndex = 0;
            this.p_depth.TabStop = false;
            // 
            // p_image
            // 
            this.p_image.BackColor = System.Drawing.SystemColors.ControlDark;
            this.p_image.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.p_image.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.p_image.Location = new System.Drawing.Point(178, 12);
            this.p_image.Name = "p_image";
            this.p_image.Size = new System.Drawing.Size(160, 120);
            this.p_image.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.p_image.TabIndex = 0;
            this.p_image.TabStop = false;
            // 
            // p_depth3d
            // 
            this.p_depth3d.BackColor = System.Drawing.SystemColors.ControlDark;
            this.p_depth3d.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.p_depth3d.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.p_depth3d.Location = new System.Drawing.Point(344, 12);
            this.p_depth3d.Name = "p_depth3d";
            this.p_depth3d.Size = new System.Drawing.Size(160, 120);
            this.p_depth3d.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.p_depth3d.TabIndex = 0;
            this.p_depth3d.TabStop = false;
            // 
            // p_image3d
            // 
            this.p_image3d.BackColor = System.Drawing.SystemColors.ControlDark;
            this.p_image3d.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.p_image3d.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.p_image3d.Location = new System.Drawing.Point(12, 138);
            this.p_image3d.Name = "p_image3d";
            this.p_image3d.Size = new System.Drawing.Size(492, 369);
            this.p_image3d.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.p_image3d.TabIndex = 0;
            this.p_image3d.TabStop = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cb_hd);
            this.groupBox1.Controls.Add(this.cb_devices);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(510, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(257, 80);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Device and Sources";
            // 
            // cb_hd
            // 
            this.cb_hd.AutoSize = true;
            this.cb_hd.Enabled = false;
            this.cb_hd.Location = new System.Drawing.Point(111, 52);
            this.cb_hd.Name = "cb_hd";
            this.cb_hd.Size = new System.Drawing.Size(135, 17);
            this.cb_hd.TabIndex = 2;
            this.cb_hd.Text = "Use 1.3 MP Resolution";
            this.cb_hd.UseVisualStyleBackColor = true;
            this.cb_hd.CheckedChanged += new System.EventHandler(this.cb_hd_CheckedChanged);
            // 
            // cb_devices
            // 
            this.cb_devices.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_devices.FormattingEnabled = true;
            this.cb_devices.Location = new System.Drawing.Point(111, 19);
            this.cb_devices.Name = "cb_devices";
            this.cb_devices.Size = new System.Drawing.Size(140, 21);
            this.cb_devices.TabIndex = 1;
            this.cb_devices.SelectedIndexChanged += new System.EventHandler(this.cb_devices_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Use this device:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.nud_maxdisp);
            this.groupBox2.Controls.Add(this.cb_smoothing);
            this.groupBox2.Controls.Add(this.cb_swap);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Location = new System.Drawing.Point(510, 98);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(257, 82);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "3D Constructor";
            // 
            // nud_maxdisp
            // 
            this.nud_maxdisp.Location = new System.Drawing.Point(111, 19);
            this.nud_maxdisp.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nud_maxdisp.Name = "nud_maxdisp";
            this.nud_maxdisp.Size = new System.Drawing.Size(140, 20);
            this.nud_maxdisp.TabIndex = 5;
            this.nud_maxdisp.Value = new decimal(new int[] {
            6,
            0,
            0,
            0});
            // 
            // cb_smoothing
            // 
            this.cb_smoothing.AutoSize = true;
            this.cb_smoothing.Checked = true;
            this.cb_smoothing.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_smoothing.Location = new System.Drawing.Point(9, 51);
            this.cb_smoothing.Name = "cb_smoothing";
            this.cb_smoothing.Size = new System.Drawing.Size(76, 17);
            this.cb_smoothing.TabIndex = 6;
            this.cb_smoothing.Text = "Smoothing";
            this.cb_smoothing.UseVisualStyleBackColor = true;
            // 
            // cb_swap
            // 
            this.cb_swap.AutoSize = true;
            this.cb_swap.Location = new System.Drawing.Point(133, 51);
            this.cb_swap.Name = "cb_swap";
            this.cb_swap.Size = new System.Drawing.Size(102, 17);
            this.cb_swap.TabIndex = 7;
            this.cb_swap.Text = "Swap Right Left";
            this.cb_swap.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 21);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(97, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Max Displacement:";
            // 
            // but_stereo
            // 
            this.but_stereo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.but_stereo.Enabled = false;
            this.but_stereo.Location = new System.Drawing.Point(510, 186);
            this.but_stereo.Name = "but_stereo";
            this.but_stereo.Size = new System.Drawing.Size(124, 23);
            this.but_stereo.TabIndex = 7;
            this.but_stereo.Text = "Generate Stereoscopic";
            this.but_stereo.UseVisualStyleBackColor = true;
            this.but_stereo.Click += new System.EventHandler(this.but_stereo_Click);
            // 
            // but_anag
            // 
            this.but_anag.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.but_anag.Enabled = false;
            this.but_anag.Location = new System.Drawing.Point(643, 186);
            this.but_anag.Name = "but_anag";
            this.but_anag.Size = new System.Drawing.Size(124, 23);
            this.but_anag.TabIndex = 8;
            this.but_anag.Text = "Generate Anaglyph";
            this.but_anag.UseVisualStyleBackColor = true;
            this.but_anag.Click += new System.EventHandler(this.but_anag_Click);
            // 
            // l_website
            // 
            this.l_website.AutoSize = true;
            this.l_website.Location = new System.Drawing.Point(516, 494);
            this.l_website.Name = "l_website";
            this.l_website.Size = new System.Drawing.Size(165, 13);
            this.l_website.TabIndex = 9;
            this.l_website.TabStop = true;
            this.l_website.Text = "By Soroush Falahati (Falahati.net)";
            this.l_website.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.l_website_LinkClicked);
            // 
            // l_save
            // 
            this.l_save.BackColor = System.Drawing.Color.White;
            this.l_save.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.l_save.Enabled = false;
            this.l_save.ForeColor = System.Drawing.SystemColors.ControlText;
            this.l_save.Location = new System.Drawing.Point(22, 478);
            this.l_save.Name = "l_save";
            this.l_save.Size = new System.Drawing.Size(54, 20);
            this.l_save.TabIndex = 10;
            this.l_save.TabStop = true;
            this.l_save.Text = "Save";
            this.l_save.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.l_save.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.l_save_LinkClicked);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(519, 300);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(248, 207);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 12;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Visible = false;
            // 
            // but_saveall
            // 
            this.but_saveall.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.but_saveall.Enabled = false;
            this.but_saveall.Location = new System.Drawing.Point(510, 215);
            this.but_saveall.Name = "but_saveall";
            this.but_saveall.Size = new System.Drawing.Size(257, 23);
            this.but_saveall.TabIndex = 13;
            this.but_saveall.Text = "Save Original Image and Depth";
            this.but_saveall.UseVisualStyleBackColor = true;
            this.but_saveall.Click += new System.EventHandler(this.but_saveall_Click);
            // 
            // frm_Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(779, 520);
            this.Controls.Add(this.but_saveall);
            this.Controls.Add(this.l_save);
            this.Controls.Add(this.l_website);
            this.Controls.Add(this.but_stereo);
            this.Controls.Add(this.but_anag);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.p_image3d);
            this.Controls.Add(this.p_depth3d);
            this.Controls.Add(this.p_image);
            this.Controls.Add(this.p_depth);
            this.Controls.Add(this.pictureBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frm_Main";
            this.Text = "OpenNI 3D Photo Capture Tool";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_Main_FormClosing);
            this.Load += new System.EventHandler(this.frm_Main_Load);
            ((System.ComponentModel.ISupportInitialize)(this.p_depth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.p_image)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.p_depth3d)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.p_image3d)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nud_maxdisp)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox p_depth;
        private System.Windows.Forms.PictureBox p_image;
        private System.Windows.Forms.PictureBox p_depth3d;
        private System.Windows.Forms.PictureBox p_image3d;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox cb_devices;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox cb_hd;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.NumericUpDown nud_maxdisp;
        private System.Windows.Forms.CheckBox cb_smoothing;
        private System.Windows.Forms.CheckBox cb_swap;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button but_stereo;
        private System.Windows.Forms.Button but_anag;
        private System.Windows.Forms.LinkLabel l_website;
        private System.Windows.Forms.LinkLabel l_save;
        private System.Windows.Forms.SaveFileDialog SaveFileDialog;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button but_saveall;
    }
}

