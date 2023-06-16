using CSShellManaged.UI.XAML;
using CSShellManaged.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Integration;

namespace CSShellManaged
{
    public partial class StartButton : UserControl
    {
        private StartButtonUI? ui;
        public StartButton()
        {
            InitializeComponent();
        }


        protected override void WndProc(ref Message m)
        {
            Console.WriteLine("[startbtn] msg: " + m.Msg);
            base.WndProc(ref m);
        }

        private void StartButton_Load(object sender, EventArgs e)
        {
            ui = new StartButtonUI(Handle);
            var host = new ElementHost();
            host.Child = ui;
            host.Dock = DockStyle.Fill;
            Controls.Add(host);
        }
    }
}
