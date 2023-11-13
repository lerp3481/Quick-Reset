
namespace Quick_Reset
{
    partial class Form1
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openPDFToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveDrillToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openDrillToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.appendFromPDFToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(800, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openPDFToolStripMenuItem,
            this.appendFromPDFToolStripMenuItem,
            this.openDrillToolStripMenuItem,
            this.saveDrillToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openPDFToolStripMenuItem
            // 
            this.openPDFToolStripMenuItem.Name = "openPDFToolStripMenuItem";
            this.openPDFToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.openPDFToolStripMenuItem.Text = "Open PDF";
            this.openPDFToolStripMenuItem.Click += new System.EventHandler(this.openPDFToolStripMenuItem_Click);
            // 
            // saveDrillToolStripMenuItem
            // 
            this.saveDrillToolStripMenuItem.Name = "saveDrillToolStripMenuItem";
            this.saveDrillToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.saveDrillToolStripMenuItem.Text = "Save Drill";
            this.saveDrillToolStripMenuItem.Click += new System.EventHandler(this.saveDrillToolStripMenuItem_Click);
            // 
            // openDrillToolStripMenuItem
            // 
            this.openDrillToolStripMenuItem.Name = "openDrillToolStripMenuItem";
            this.openDrillToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.openDrillToolStripMenuItem.Text = "Open Drill";
            this.openDrillToolStripMenuItem.Click += new System.EventHandler(this.openDrillToolStripMenuItem_Click);
            // 
            // appendFromPDFToolStripMenuItem
            // 
            this.appendFromPDFToolStripMenuItem.Name = "appendFromPDFToolStripMenuItem";
            this.appendFromPDFToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.appendFromPDFToolStripMenuItem.Text = "Append from PDF";
            this.appendFromPDFToolStripMenuItem.Click += new System.EventHandler(this.appendFromPDFToolStripMenuItem_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openPDFToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveDrillToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openDrillToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem appendFromPDFToolStripMenuItem;
    }
}

