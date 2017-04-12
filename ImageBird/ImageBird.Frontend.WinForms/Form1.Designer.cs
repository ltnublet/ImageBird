namespace ImageBird.Frontend.WinForms
{
    partial class mainWindow
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
            this.menuBar = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tempToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolPanel = new System.Windows.Forms.Panel();
            this.mainWindowContent = new System.Windows.Forms.Panel();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.directoryTree1 = new ImageBird.Frontend.WinForms.DirectoryTree();
            this.splitter2 = new System.Windows.Forms.Splitter();
            this.categoryView1 = new ImageBird.Frontend.WinForms.CategoryView();
            this.imagePane1 = new ImageBird.Frontend.WinForms.ImagePane();
            this.menuBar.SuspendLayout();
            this.toolPanel.SuspendLayout();
            this.mainWindowContent.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuBar
            // 
            this.menuBar.AccessibleDescription = "Menu bar";
            this.menuBar.AccessibleName = "Menu bar";
            this.menuBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuBar.Location = new System.Drawing.Point(0, 0);
            this.menuBar.Name = "menuBar";
            this.menuBar.Size = new System.Drawing.Size(784, 24);
            this.menuBar.TabIndex = 0;
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tempToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // tempToolStripMenuItem
            // 
            this.tempToolStripMenuItem.Name = "tempToolStripMenuItem";
            this.tempToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.tempToolStripMenuItem.Text = "Temp";
            // 
            // toolPanel
            // 
            this.toolPanel.BackColor = System.Drawing.Color.OrangeRed;
            this.toolPanel.Controls.Add(this.categoryView1);
            this.toolPanel.Controls.Add(this.splitter2);
            this.toolPanel.Controls.Add(this.directoryTree1);
            this.toolPanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.toolPanel.Location = new System.Drawing.Point(0, 0);
            this.toolPanel.Name = "toolPanel";
            this.toolPanel.Size = new System.Drawing.Size(286, 537);
            this.toolPanel.TabIndex = 0;
            // 
            // mainWindowContent
            // 
            this.mainWindowContent.BackColor = System.Drawing.SystemColors.Control;
            this.mainWindowContent.Controls.Add(this.imagePane1);
            this.mainWindowContent.Controls.Add(this.splitter1);
            this.mainWindowContent.Controls.Add(this.toolPanel);
            this.mainWindowContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainWindowContent.Location = new System.Drawing.Point(0, 24);
            this.mainWindowContent.Name = "mainWindowContent";
            this.mainWindowContent.Size = new System.Drawing.Size(784, 537);
            this.mainWindowContent.TabIndex = 1;
            // 
            // splitter1
            // 
            this.splitter1.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.splitter1.Location = new System.Drawing.Point(286, 0);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 537);
            this.splitter1.TabIndex = 1;
            this.splitter1.TabStop = false;
            // 
            // directoryTree1
            // 
            this.directoryTree1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.directoryTree1.Dock = System.Windows.Forms.DockStyle.Left;
            this.directoryTree1.Location = new System.Drawing.Point(0, 0);
            this.directoryTree1.MinimumSize = new System.Drawing.Size(200, 0);
            this.directoryTree1.Name = "directoryTree1";
            this.directoryTree1.Size = new System.Drawing.Size(200, 537);
            this.directoryTree1.TabIndex = 0;
            // 
            // splitter2
            // 
            this.splitter2.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.splitter2.Location = new System.Drawing.Point(200, 0);
            this.splitter2.Name = "splitter2";
            this.splitter2.Size = new System.Drawing.Size(3, 537);
            this.splitter2.TabIndex = 1;
            this.splitter2.TabStop = false;
            // 
            // categoryView1
            // 
            this.categoryView1.BackColor = System.Drawing.Color.Green;
            this.categoryView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.categoryView1.Location = new System.Drawing.Point(203, 0);
            this.categoryView1.Name = "categoryView1";
            this.categoryView1.Size = new System.Drawing.Size(83, 537);
            this.categoryView1.TabIndex = 2;
            // 
            // imagePane1
            // 
            this.imagePane1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.imagePane1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.imagePane1.Location = new System.Drawing.Point(289, 0);
            this.imagePane1.Name = "imagePane1";
            this.imagePane1.Size = new System.Drawing.Size(495, 537);
            this.imagePane1.TabIndex = 2;
            // 
            // mainWindow
            // 
            this.AccessibleDescription = "ImageBird main window";
            this.AccessibleName = "ImageBird main window";
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.mainWindowContent);
            this.Controls.Add(this.menuBar);
            this.MainMenuStrip = this.menuBar;
            this.MinimumSize = new System.Drawing.Size(800, 600);
            this.Name = "mainWindow";
            this.Text = "ImageBird";
            this.menuBar.ResumeLayout(false);
            this.menuBar.PerformLayout();
            this.toolPanel.ResumeLayout(false);
            this.mainWindowContent.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuBar;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tempToolStripMenuItem;
        private System.Windows.Forms.Panel toolPanel;
        private System.Windows.Forms.Panel mainWindowContent;
        private System.Windows.Forms.Splitter splitter1;
        private DirectoryTree directoryTree1;
        private System.Windows.Forms.Splitter splitter2;
        private CategoryView categoryView1;
        private ImagePane imagePane1;
    }
}

