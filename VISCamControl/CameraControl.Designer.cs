namespace VISCamControl
{
    partial class CameraControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CameraControl));
            this.CamView = new System.Windows.Forms.PictureBox();
            this.ToolStripMenu = new System.Windows.Forms.ToolStrip();
            this.CmbCameraList = new System.Windows.Forms.ToolStripComboBox();
            this.BtnStart = new System.Windows.Forms.ToolStripButton();
            this.BtnStop = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.BtnSettings = new System.Windows.Forms.ToolStripButton();
            this.BtnCross = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.LblError = new System.Windows.Forms.ToolStripLabel();
            ((System.ComponentModel.ISupportInitialize)(this.CamView)).BeginInit();
            this.ToolStripMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // CamView
            // 
            this.CamView.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.CamView.Location = new System.Drawing.Point(0, 28);
            this.CamView.Name = "CamView";
            this.CamView.Size = new System.Drawing.Size(562, 436);
            this.CamView.TabIndex = 0;
            this.CamView.TabStop = false;
            this.CamView.Paint += new System.Windows.Forms.PaintEventHandler(this.CamView_Paint);
            // 
            // ToolStripMenu
            // 
            this.ToolStripMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.CmbCameraList,
            this.BtnStart,
            this.BtnStop,
            this.toolStripSeparator1,
            this.BtnSettings,
            this.BtnCross,
            this.toolStripSeparator2,
            this.LblError});
            this.ToolStripMenu.Location = new System.Drawing.Point(0, 0);
            this.ToolStripMenu.Name = "ToolStripMenu";
            this.ToolStripMenu.Size = new System.Drawing.Size(562, 25);
            this.ToolStripMenu.TabIndex = 9;
            this.ToolStripMenu.Text = "toolStrip1";
            // 
            // CmbCameraList
            // 
            this.CmbCameraList.Name = "CmbCameraList";
            this.CmbCameraList.Size = new System.Drawing.Size(121, 25);
            this.CmbCameraList.SelectedIndexChanged += new System.EventHandler(this.CmbCameraList_SelectedIndexChanged);
            // 
            // BtnStart
            // 
            this.BtnStart.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.BtnStart.Image = ((System.Drawing.Image)(resources.GetObject("BtnStart.Image")));
            this.BtnStart.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.BtnStart.Name = "BtnStart";
            this.BtnStart.Size = new System.Drawing.Size(23, 22);
            this.BtnStart.Text = "Play";
            this.BtnStart.ToolTipText = "Start";
            this.BtnStart.Click += new System.EventHandler(this.BtnStart_Click);
            // 
            // BtnStop
            // 
            this.BtnStop.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.BtnStop.Image = ((System.Drawing.Image)(resources.GetObject("BtnStop.Image")));
            this.BtnStop.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.BtnStop.Name = "BtnStop";
            this.BtnStop.Size = new System.Drawing.Size(23, 22);
            this.BtnStop.Text = "Stop";
            this.BtnStop.Click += new System.EventHandler(this.BtnStop_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // BtnSettings
            // 
            this.BtnSettings.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.BtnSettings.Image = ((System.Drawing.Image)(resources.GetObject("BtnSettings.Image")));
            this.BtnSettings.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.BtnSettings.Name = "BtnSettings";
            this.BtnSettings.Size = new System.Drawing.Size(23, 22);
            this.BtnSettings.Text = "Controls";
            this.BtnSettings.ToolTipText = "Settings";
            this.BtnSettings.Click += new System.EventHandler(this.BtnSettings_Click);
            // 
            // BtnCross
            // 
            this.BtnCross.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.BtnCross.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.BtnCross.Image = ((System.Drawing.Image)(resources.GetObject("BtnCross.Image")));
            this.BtnCross.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.BtnCross.Name = "BtnCross";
            this.BtnCross.Size = new System.Drawing.Size(23, 22);
            this.BtnCross.Text = "toolStripButton1";
            this.BtnCross.ToolTipText = "Cross marker";
            this.BtnCross.Click += new System.EventHandler(this.BtnCross_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // LblError
            // 
            this.LblError.Name = "LblError";
            this.LblError.Size = new System.Drawing.Size(0, 22);
            // 
            // CameraControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ToolStripMenu);
            this.Controls.Add(this.CamView);
            this.Name = "CameraControl";
            this.Size = new System.Drawing.Size(562, 464);
            this.SizeChanged += new System.EventHandler(this.CameraControl_SizeChanged);
            ((System.ComponentModel.ISupportInitialize)(this.CamView)).EndInit();
            this.ToolStripMenu.ResumeLayout(false);
            this.ToolStripMenu.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox CamView;
        private System.Windows.Forms.ToolStrip ToolStripMenu;
        private System.Windows.Forms.ToolStripButton BtnStart;
        private System.Windows.Forms.ToolStripButton BtnStop;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton BtnSettings;
        private System.Windows.Forms.ToolStripButton BtnCross;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripLabel LblError;
        private System.Windows.Forms.ToolStripComboBox CmbCameraList;
    }
}
