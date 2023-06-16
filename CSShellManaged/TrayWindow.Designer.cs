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
            label1 = new Label();
            contextMenuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Dock = DockStyle.Left;
            panel1.ForeColor = Color.White;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(117, 70);
            panel1.TabIndex = 0;
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.ImageScalingSize = new Size(28, 28);
            contextMenuStrip1.Items.AddRange(new ToolStripItem[] { openTaskManagerToolStripMenuItem });
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new Size(272, 40);
            // 
            // openTaskManagerToolStripMenuItem
            // 
            openTaskManagerToolStripMenuItem.Name = "openTaskManagerToolStripMenuItem";
            openTaskManagerToolStripMenuItem.Size = new Size(271, 36);
            openTaskManagerToolStripMenuItem.Text = "Open Task Manager";
            openTaskManagerToolStripMenuItem.Click += openTaskManagerToolStripMenuItem_Click;
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            label1.ForeColor = Color.White;
            label1.Location = new Point(706, 9);
            label1.Name = "label1";
            label1.Size = new Size(119, 72);
            label1.TabIndex = 1;
            label1.Text = "7:00 AM";
            // 
            // Shell_TrayWnd
            // 
            AutoScaleDimensions = new SizeF(12F, 30F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Coral;
            ClientSize = new Size(800, 70);
            ContextMenuStrip = contextMenuStrip1;
            ControlBox = false;
            Controls.Add(label1);
            Controls.Add(panel1);
            FormBorderStyle = FormBorderStyle.None;
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
        private Label label1;
    }
}