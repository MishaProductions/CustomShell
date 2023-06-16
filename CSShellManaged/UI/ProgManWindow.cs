using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static CSShellManaged.Win32.Win32Defs;

namespace CSShellManaged
{
    public partial class ProgManWindow : Form
    {
        private long ProgManPCRef = 0;
        private nint ProgManThreadRef;
        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.ExStyle = 128;
                cp.Style = unchecked((int)0x82000000);

                return cp;
            }
        }
        public ProgManWindow()
        {
            InitializeComponent();

            this.Location = new Point(GetSystemMetrics(SM_XVIRTUALSCREEN), GetSystemMetrics(SM_YVIRTUALSCREEN));
            this.Width = GetSystemMetrics(SM_CXVIRTUALSCREEN);
            this.Height = GetSystemMetrics(SM_CYVIRTUALSCREEN);

            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
        }

        protected override void WndProc(ref Message m)
        {

            if (m.Msg == WM_SIZE)
            {
                Console.WriteLine("!! WND PROC OF PROGMAN !!");
                Show();
            }
            base.WndProc(ref m);
        }

        private void ProgManWindow_Load(object sender, EventArgs e)
        {
            Console.WriteLine("[progman] created");
            if (SetShellWindow(Handle) < 0)
            {
                Console.WriteLine("SetShellWindow failed");
            }

            SetPropW(Handle, "NonRudeHWND", new nint(1));
            SetPropW(Handle, "AllowConsentToStealFocus", new nint(1));

            if (SHCreateThreadRef(ref ProgManPCRef, out ProgManThreadRef) >= 0)
            {
                if (SHSetThreadRef(ProgManThreadRef) < 0)
                {
                    Console.WriteLine("SHSetThreadRef failed");
                }
            }
            else
            {
                Console.WriteLine("SHCreateThreadRef failed");
            }
            UnregisterHotKey(0, 11);


            Program.StartImmersiveShell();
            //load wallpaper
            var desktop = Registry.CurrentUser.OpenSubKey("Control Panel\\Desktop");
            if (desktop != null)
            {
                var key = (string)desktop.GetValue("Wallpaper");
                if (key != null)
                {
                    BackgroundImage = Image.FromFile(key);
                }
            }
        }
    }
}
