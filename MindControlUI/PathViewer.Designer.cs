namespace MindControlUI
{
    partial class PathViewer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PathViewer));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.LoadMapImageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.LoadPathToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearPathsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MapPictureBox = new System.Windows.Forms.PictureBox();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MapPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.ViewToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1023, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.LoadMapImageToolStripMenuItem,
            this.LoadPathToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // LoadMapImageToolStripMenuItem
            // 
            this.LoadMapImageToolStripMenuItem.Name = "LoadMapImageToolStripMenuItem";
            this.LoadMapImageToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.LoadMapImageToolStripMenuItem.Text = "Load map image";
            this.LoadMapImageToolStripMenuItem.Click += new System.EventHandler(this.LoadMapImageToolStripMenuItem_Click);
            // 
            // LoadPathToolStripMenuItem
            // 
            this.LoadPathToolStripMenuItem.Name = "LoadPathToolStripMenuItem";
            this.LoadPathToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.LoadPathToolStripMenuItem.Text = "Load path";
            this.LoadPathToolStripMenuItem.Click += new System.EventHandler(this.LoadPathToolStripMenuItem_Click);
            // 
            // ViewToolStripMenuItem
            // 
            this.ViewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.clearPathsToolStripMenuItem});
            this.ViewToolStripMenuItem.Name = "ViewToolStripMenuItem";
            this.ViewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.ViewToolStripMenuItem.Text = "View";
            // 
            // clearPathsToolStripMenuItem
            // 
            this.clearPathsToolStripMenuItem.Name = "clearPathsToolStripMenuItem";
            this.clearPathsToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.clearPathsToolStripMenuItem.Text = "Clear paths";
            this.clearPathsToolStripMenuItem.Click += new System.EventHandler(this.clearPathsToolStripMenuItem_Click);
            // 
            // MapPictureBox
            // 
            this.MapPictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.MapPictureBox.Location = new System.Drawing.Point(12, 31);
            this.MapPictureBox.Name = "MapPictureBox";
            this.MapPictureBox.Size = new System.Drawing.Size(1000, 700);
            this.MapPictureBox.TabIndex = 0;
            this.MapPictureBox.TabStop = false;
            this.MapPictureBox.Paint += new System.Windows.Forms.PaintEventHandler(this.MapPictureBox_Paint);
            // 
            // PathViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1023, 747);
            this.Controls.Add(this.MapPictureBox);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "PathViewer";
            this.Text = "PathViewer";
            this.TopMost = true;
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MapPictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox MapPictureBox;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem LoadMapImageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem LoadPathToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ViewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearPathsToolStripMenuItem;
    }
}