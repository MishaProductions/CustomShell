using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CSShellManaged.Win32
{
    [ComVisible(true)]
    [Guid("546A0315-5645-4810-A39D-8496EA24E1B4")]
    public unsafe class DeskTrayImpl : IDeskTray
    {
        private nint trayHandle;
        public nint desktop;
        public DeskTrayImpl(nint handle)
        {
            trayHandle = handle;
        }

        public uint AppBarGetState()
        {
            Debugger.Break();
            Console.WriteLine("AppBarGetState");
            return 0;
        }
        [return: MarshalAs(UnmanagedType.Error)]
        public void GetTrayWindow(ref nint tray)
        {
            Console.WriteLine("GetTrayWindow");
            Debugger.Break();
            tray = trayHandle;
            Console.WriteLine("GetTrayWindow end");
            //  return 0;
        }

        public int SetDesktopWindow(nint desktop)
        {
            Console.WriteLine("SetDesktopWindow end");
            this.desktop = desktop;
            return 0;
        }

        public int SetVar(int a, ulong b)
        {
            //  Debugger.Break();
            Console.WriteLine("SetVar");
            return 0;
        }

    }
}
