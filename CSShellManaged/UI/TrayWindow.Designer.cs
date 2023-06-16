namespace CSShellManaged
{
    partial class Shell_TrayWnd
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
            components = new System.ComponentModel.Container();
            panel1 = new Panel();
            contextMenuStrip1 = new ContextMenuStrip(components);
            openTaskManagerToolStripMenuItem = new ToolStripMenuItem();
            taskbarSettingsToolStripMenuItem = new ToolStripMenuItem();
            contextMenuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.BackColor = Color.Red;
            panel1.Dock = DockStyle.Left;
            panel1.ForeColor = Color.White;
            panel1.Location = new Point(0, 0);
            panel1.Margin = new Padding(2);
            panel1.Name = "panel1";
            panel1.Size = new Size(68, 35);
            panel1.TabIndex = 0;
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.ImageScalingSize = new Size(28, 28);
            contextMenuStrip1.Items.AddRange(new ToolStripItem[] { openTaskManagerToolStripMenuItem, taskbarSettingsToolStripMenuItem });
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.RenderMode = ToolStripRenderMode.Professional;
            contextMenuStrip1.Size = new Size(181, 70);
            // 
            // openTaskManagerToolStripMenuItem
            // 
            openTaskManagerToolStripMenuItem.Name = "openTaskManagerToolStripMenuItem";
            openTaskManagerToolStripMenuItem.Size = new Size(180, 22);
            openTaskManagerToolStripMenuItem.Text = "Open Task Manager";
            openTaskManagerToolStripMenuItem.Click += openTaskManagerToolStripMenuItem_Click;
            // 
            // taskbarSettingsToolStripMenuItem
            // 
            taskbarSettingsToolStripMenuItem.Name = "taskbarSettingsToolStripMenuItem";
            taskbarSettingsToolStripMenuItem.Size = new Size(180, 22);
            taskbarSettingsToolStripMenuItem.Text = "Taskbar Settings";
            taskbarSettingsToolStripMenuItem.Click += taskbarSettingsToolStripMenuItem_Click;
            // 
            // Shell_TrayWnd
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.LawnGreen;
            ClientSize = new Size(467, 35);
            ContextMenuStrip = contextMenuStrip1;
            ControlBox = false;
            Controls.Add(panel1);
            FormBorderStyle = FormBorderStyle.None;
            Margin = new Padding(2);
            Name = "Shell_TrayWnd";
            ShowInTaskbar = false;
            Text = "TrayWindow";
            Load += Shell_TrayWnd_Load;
            contextMenuStrip1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Panel panel1;
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem openTaskManagerToolStripMenuItem;
        private ToolStripMenuItem taskbarSettingsToolStripMenuItem;
    }
}