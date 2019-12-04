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
            this.MapPictureBox = new System.Windows.Forms.PictureBox();
            this.PathViewerMenu = new System.Windows.Forms.MenuStrip();
            this.FileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.LoadPathToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ClearPathsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.MapPictureBox)).BeginInit();
            this.PathViewerMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // MapPictureBox
            // 
            this.MapPictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.MapPictureBox.Location = new System.Drawing.Point(12, 27);
            this.MapPictureBox.Name = "MapPictureBox";
            this.MapPictureBox.Size = new System.Drawing.Size(1000, 700);
            this.MapPictureBox.TabIndex = 0;
            this.MapPictureBox.TabStop = false;
            this.MapPictureBox.Paint += new System.Windows.Forms.PaintEventHandler(this.MapPictureBox_Paint);
            // 
            // PathViewerMenu
            // 
            this.PathViewerMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.FileToolStripMenuItem,
            this.ViewToolStripMenuItem});
            this.PathViewerMenu.Location = new System.Drawing.Point(0, 0);
            this.PathViewerMenu.Name = "PathViewerMenu";
            this.PathViewerMenu.Size = new System.Drawing.Size(1023, 24);
            this.PathViewerMenu.TabIndex = 1;
            this.PathViewerMenu.Text = "menuStrip1";
            // 
            // FileToolStripMenuItem
            // 
            this.FileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.LoadPathToolStripMenuItem});
            this.FileToolStripMenuItem.Name = "FileToolStripMenuItem";
            this.FileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.FileToolStripMenuItem.Text = "File";
            // 
            // LoadPathToolStripMenuItem
            // 
            this.LoadPathToolStripMenuItem.Name = "LoadPathToolStripMenuItem";
            this.LoadPathToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.LoadPathToolStripMenuItem.Text = "Load path";
            this.LoadPathToolStripMenuItem.Click += new System.EventHandler(this.LoadPathToolStripMenuItem_Click);
            // 
            // ViewToolStripMenuItem
            // 
            this.ViewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ClearPathsToolStripMenuItem});
            this.ViewToolStripMenuItem.Name = "ViewToolStripMenuItem";
            this.ViewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.ViewToolStripMenuItem.Text = "View";
            // 
            // ClearPathsToolStripMenuItem
            // 
            this.ClearPathsToolStripMenuItem.Name = "ClearPathsToolStripMenuItem";
            this.ClearPathsToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.ClearPathsToolStripMenuItem.Text = "Clear paths";
            this.ClearPathsToolStripMenuItem.Click += new System.EventHandler(this.ClearPathsToolStripMenuItem_Click);
            // 
            // PathViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1023, 735);
            this.Controls.Add(this.MapPictureBox);
            this.Controls.Add(this.PathViewerMenu);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.PathViewerMenu;
            this.MaximizeBox = false;
            this.Name = "PathViewer";
            this.Text = "PathViewer";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.MapPictureBox)).EndInit();
            this.PathViewerMenu.ResumeLayout(false);
            this.PathViewerMenu.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox MapPictureBox;
        private System.Windows.Forms.MenuStrip PathViewerMenu;
        private System.Windows.Forms.ToolStripMenuItem FileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem LoadPathToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ViewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ClearPathsToolStripMenuItem;
    }
}