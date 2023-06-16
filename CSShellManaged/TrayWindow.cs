using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Interop;
using static CSShellManaged.Win32;

namespace CSShellManaged
{
    public partial class Shell_TrayWnd : Form
    {
        private bool fBarRegistered = false;
        private int uCallBack;

        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                //cp.ExStyle = 384;
                //cp.X = 0;
                //cp.Y = 0;
                //cp.Width = 0;
                //cp.Height = 0;
                //  cp.Style = unchecked((int)0x82000000);
                ///cp.Height = 48;
                return cp;
            }
        }
        public Shell_TrayWnd()
        {
            WindowClassHook.ClassName = "unknown3";
            InitializeComponent();

        }
        private void RegisterBar()
        {
            APPBARDATA abd = new APPBARDATA();
            abd.cbSize = Marshal.SizeOf(abd);
            abd.hWnd = this.Handle;
            if (!fBarRegistered)
            {
                uCallBack = (int)RegisterWindowMessage("AppBarMessage");
                abd.uCallbackMessage = uCallBack;

                uint ret = SHAppBarMessage((int)ABMsg.ABM_NEW, ref abd);
                fBarRegistered = true;

                ABSetPos();
            }
            else
            {
                SHAppBarMessage((int)ABMsg.ABM_REMOVE, ref abd);
                fBarRegistered = false;
            }
        }
        private void ABSetPos()
        {
            int height = 50;
            APPBARDATA abd = new APPBARDATA();
            abd.cbSize = Marshal.SizeOf(abd);
            abd.hWnd = this.Handle;
            abd.uEdge = (int)ABEdge.ABE_BOTTOM;

            if (abd.uEdge == (int)ABEdge.ABE_LEFT || abd.uEdge == (int)ABEdge.ABE_RIGHT)
            {
                abd.rc.top = 0;
                abd.rc.bottom = SystemInformation.PrimaryMonitorSize.Height;
                if (abd.uEdge == (int)ABEdge.ABE_LEFT)
                {
                    abd.rc.left = 0;
                    abd.rc.right = Size.Width;
                }
                else
                {
                    abd.rc.right = SystemInformation.PrimaryMonitorSize.Width;
                    abd.rc.left = abd.rc.right - Size.Width;
                }

            }
            else
            {
                abd.rc.left = 0;
                abd.rc.right = SystemInformation.PrimaryMonitorSize.Width;
                if (abd.uEdge == (int)ABEdge.ABE_TOP)
                {
                    abd.rc.top = 0;
                    abd.rc.bottom = height;
                }
                else
                {
                    abd.rc.bottom = SystemInformation.PrimaryMonitorSize.Height;
                    abd.rc.top = abd.rc.bottom - height;
                }
            }

            SHAppBarMessage((int)ABMsg.ABM_QUERYPOS, ref abd);

            switch (abd.uEdge)
            {
                case (int)ABEdge.ABE_LEFT:
                    abd.rc.right = abd.rc.left + Size.Width;
                    break;
                case (int)ABEdge.ABE_RIGHT:
                    abd.rc.left = abd.rc.right - Size.Width;
                    break;
                case (int)ABEdge.ABE_TOP:
                    abd.rc.bottom = abd.rc.top + height;
                    break;
                case (int)ABEdge.ABE_BOTTOM:
                    abd.rc.top = abd.rc.bottom - height;
                    break;
            }

            SHAppBarMessage((int)ABMsg.ABM_SETPOS, ref abd);
            MoveWindow(abd.hWnd, abd.rc.left, abd.rc.top,
                    abd.rc.right - abd.rc.left, abd.rc.bottom - abd.rc.top, true);
        }

        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            Console.WriteLine("trayproc " + m.Msg);
            if (m.Msg == uCallBack)
            {
                switch (m.WParam.ToInt32())
                {
                    case (int)ABNotify.ABN_POSCHANGED:
                        ABSetPos();
                        break;
                }
            }
            else if (m.Msg == 22)
            {

            }
            else if (m.Msg == 254 || m.Msg == 255)
            {
                AudioHidProcessMessage(m.Msg, m.WParam, m.LParam);
            }
            else if (m.Msg == 1369) //Handle boot stuff
            {
                PostMessageW(m.HWnd, 1396, 0, 1); //1 is idk
                var eventt = CreateEvent(0, true, true, "ShellReadyEvent");
                SetEvent(eventt);
            }
            else if (m.Msg == 1396)
            {
                //PostMessageW(Program.Progmanhwnd, 1116, 3, 0);

            
                                                                                                              //PostMessageW(Program.Progmanhwnd, 1118, 3, 0);



                //SendMessageW(Program.Progmanhwnd, 5, 0, 0);


                //SendMessageW(Program.Progmanhwnd, 1116, 3, 0);


                ////some time later
                //PostMessageW(Program.Progmanhwnd, 1118, 0, 0x10002); //_StartDesktopApiSurface

                //PostMessageW(Program.Progmanhwnd, 1101, 0, 0);
                //PostMessageW(Program.Progmanhwnd, 1101, 0, 0);

                //PostMessageW(Program.Progmanhwnd, 1116, 3, 5); //_StartWaitForDesktopVisuals

                //PostMessageW(Program.Progmanhwnd, 1115, 0, 0x10000); //_StartDesktopFinalTasks
            }
            else if (m.Msg == shellhook)
            {
                HandleShellHook(m);
            }

            base.WndProc(ref m);
        }

        private void HandleShellHook(Message m)
        {
            Console.WriteLine("shellhook: " + m.WParam);
            if (m.WParam == 12)
            {
                //app command
                var app = (uint)((m.LParam >> 16) & 0xFFFF);
                if (AudioHIDProcessAppCommand(app))
                {
                    return;
                }
                Console.WriteLine(app + " is not handled by sndvolsso");
            }
        }

        uint shellhook;
        private void Shell_TrayWnd_Load(object sender, EventArgs e)
        {
            RegisterBar();
            EnableBlur();
            if (!AudioHIDInitialize(Handle))
            {
                Console.WriteLine("failed to init audio hid");
            }
            MINIMIZEDMETRICS pvParam = new MINIMIZEDMETRICS();
            pvParam.size = (uint)Marshal.SizeOf<MINIMIZEDMETRICS>();
            pvParam.iWidth = 0;
            SystemParametersInfo(SPI.SPI_GETMINIMIZEDMETRICS, 0x14, ref pvParam, SPIF.None);
            pvParam.iArrange |= 8;
            SystemParametersInfo(SPI.SPI_SETMINIMIZEDMETRICS, 0x14, ref pvParam, SPIF.None);


            shellhook = RegisterWindowMessage("SHELLHOOK");
            RegisterShellHookWindow(Handle);

            WindowClassHook.ClassName = "Start";
            panel1.Controls.Add(new StartButton() { Dock = DockStyle.Fill });
            WindowClassHook.ClassName = "unknown2";
        }

        private void openTaskManagerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start(new ProcessStartInfo() { UseShellExecute = true, FileName = "taskmgr.exe" });
        }

        internal void EnableBlur()
        {
            var accent = new AccentPolicy();
            accent.AccentState = AccentState.ACCENT_ENABLE_ACRYLICBLURBEHIND;
            accent.GradientColor = (50 << 24) | (0x990000 & 0xFFFFFF);

            var accentStructSize = Marshal.SizeOf(accent);

            var accentPtr = Marshal.AllocHGlobal(accentStructSize);
            Marshal.StructureToPtr(accent, accentPtr, false);

            var data = new WindowCompositionAttributeData();
            data.Attribute = WindowCompositionAttribute.WCA_ACCENT_POLICY;
            data.SizeOfData = accentStructSize;
            data.Data = accentPtr;
            int hr = SetWindowCompositionAttribute(Handle, ref data);
            if (hr != 1)
            {
                throw new Win32Exception(hr);
            }

            Marshal.FreeHGlobal(accentPtr);
        }
    }
}
