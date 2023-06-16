using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace CSShellManaged.Win32
{
    public static class Win32Defs
    {
        public const int SM_XVIRTUALSCREEN = 76;
        public const int SM_YVIRTUALSCREEN = 77;
        public const int SM_CXVIRTUALSCREEN = 78;
        public const int SM_CYVIRTUALSCREEN = 79;

        public const int WM_CREATE = 1;
        public const int WM_DESTROY = 2;
        public const int WM_SIZE = 5;
        public const int WM_CLOSE = 0x10;
        public const int WM_PAINT = 0xF;
        public const int WM_QUIT = 0x0012;
        public const int WM_TIMER = 0x0113;

        public const int WM_HOTKEY = 0x0312;

        private const int SPIF_SENDWININICHANGE = 2;
        private const int SPIF_UPDATEINIFILE = 1;
        private const int SPIF_change = SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE;
        private const int SPI_SETWORKAREA = 47;

        public const int
   IDC_ARROW = 32512,
   IDC_IBEAM = 32513,
   IDC_WAIT = 32514,
   IDC_CROSS = 32515,
   IDC_UPARROW = 32516,
   IDC_SIZE = 32640,
   IDC_ICON = 32641,
   IDC_SIZENWSE = 32642,
   IDC_SIZENESW = 32643,
   IDC_SIZEWE = 32644,
   IDC_SIZENS = 32645,
   IDC_SIZEALL = 32646,
   IDC_NO = 32648,
   IDC_HAND = 32649,
   IDC_APPSTARTING = 32650,
   IDC_HELP = 32651;
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SetPropW(nint hWnd, string lpString, nint hData);
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.U2)]
        public static extern short RegisterClassExW([In] ref WNDCLASSEX lpwcx);
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern nint GetModuleHandle([MarshalAs(UnmanagedType.LPWStr)] string? lpModuleName);
        public delegate nint Wndproc(nint hwnd, uint msg, nint wParam, nint lParam);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern nint CreateWindowEx(uint dwExStyle, string lpClassName,
   string? lpWindowName, uint dwStyle, int x, int y, int nWidth, int nHeight,
   nint hWndParent, nint hMenu, nint hInstance, nint lpParam);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetSystemMetrics(int index);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ShowWindow(nint hWnd, ShowWindowCommands nCmdShow);

        [DllImport("user32.dll")]
        public static extern nint BeginPaint(nint hwnd, out PAINTSTRUCT lpPaint);

        [DllImport("user32.dll")]
        public static extern nint DefWindowProc(nint hWnd, uint uMsg, nint wParam, nint lParam);

        [DllImport("user32.dll")]
        public static extern bool GetClientRect(nint hWnd, out RECT lpRect);

        [DllImport("user32.dll")]
        public static extern int DrawText(nint hDC, string lpString, int nCount, ref RECT lpRect, uint uFormat);

        [DllImport("user32.dll")]
        public static extern bool EndPaint(nint hWnd, [In] ref PAINTSTRUCT lpPaint);

        [DllImport("user32.dll")]
        public static extern void PostQuitMessage(int nExitCode);

        [DllImport("user32.dll")]
        public static extern nint LoadIcon(nint hInstance, string lpIconName);

        [DllImport("user32.dll")]
        public static extern nint LoadIcon(nint hInstance, nint lpIConName);



        [DllImport("user32.dll")]
        public static extern MessageBoxResult MessageBox(nint hWnd, string text, string caption, int options);

        [DllImport("user32.dll")]
        public static extern bool UpdateWindow(nint hWnd);

        [DllImport("user32.dll")]
        public static extern nint LoadCursor(nint hInstance, int lpCursorName);

        [DllImport("user32.dll")]
        public static extern int SetShellWindow(nint handle);
        [DllImport("user32.dll")]
        public static extern nint GetShellWindow();
        [DllImport("user32.dll")]
        public static extern int SetTaskmanWindow(nint handle);
        [DllImport("user32.dll")]
        public static extern nint GetTaskmanWindow();
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern nint RemovePropW(nint hWnd, string lpString);

        [DllImport("Shlwapi.dll")]
        public static extern int SHCreateThreadRef(ref long pcRef, out nint iUnknown);
        [DllImport("Shlwapi.dll")]
        public static extern int SHSetThreadRef(nint iUnknown);
        [DllImport("Api-ms-win-shcore-thread-L1-1-0.dll")]
        public static extern int SetProcessReference(nint iUnknown);
        [DllImport("user32.dll")]
        public static extern int FillRect(nint hDC, [In] ref RECT lprc, nint hbr);
        [DllImport("gdi32.dll")]
        public static extern nint CreateSolidBrush(uint crColor);
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern uint RegisterWindowMessage(string lpString);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool RegisterShellHookWindow(nint hWnd);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool DeregisterShellHookWindow(nint hWnd);
        [DllImport("shell32.dll", SetLastError = true)]
        public static extern void SetCurrentProcessExplicitAppUserModelID([MarshalAs(UnmanagedType.LPWStr)] string AppID);
        [DllImport("kernel32.dll")]
        public static extern ErrorModes SetErrorMode(ErrorModes uMode);
        [DllImport("shell32.dll", SetLastError = true, EntryPoint = "#200")]
        public static extern nint ShCreateDesktop(nint impl);
        [DllImport("shell32.dll", SetLastError = true, EntryPoint = "#201")]
        public static extern nint SHDesktopMessageLoop(nint impl);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern nint SendMessageW(nint hWnd, uint Msg, nuint wParam, nint lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern nint PostMessageW(nint hWnd, uint Msg, nuint wParam, nint lParam);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern nint FindWindowW(string lpClassName, string lpWindowName);

        [Flags]
        public enum ErrorModes : uint
        {
            SYSTEM_DEFAULT = 0x0,
            SEM_FAILCRITICALERRORS = 0x0001,
            SEM_NOALIGNMENTFAULTEXCEPT = 0x0004,
            SEM_NOGPFAULTERRORBOX = 0x0002,
            SEM_NOOPENFILEERRORBOX = 0x8000
        }
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool SetPriorityClass(nint handle, PriorityClass priorityClass);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern nint GetCurrentProcess();
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool EnableMouseInPointer([MarshalAs(UnmanagedType.Bool)] bool fEnable);
        public enum PriorityClass : uint
        {
            ABOVE_NORMAL_PRIORITY_CLASS = 0x8000,
            BELOW_NORMAL_PRIORITY_CLASS = 0x4000,
            HIGH_PRIORITY_CLASS = 0x80,
            IDLE_PRIORITY_CLASS = 0x40,
            NORMAL_PRIORITY_CLASS = 0x20,
            PROCESS_MODE_BACKGROUND_BEGIN = 0x100000,// 'Windows Vista/2008 and higher
            PROCESS_MODE_BACKGROUND_END = 0x200000,//   'Windows Vista/2008 and higher
            REALTIME_PRIORITY_CLASS = 0x100
        }
        [DllImport("shell32.dll", EntryPoint = "#899")]
        public static extern bool SetExplorerServerMode(int mode);
        [DllImport("shell32.dll", EntryPoint = "#188")]
        public static extern bool ShellDDEInit(bool init);
        [DllImport("kernel32.dll")]
        public static extern bool SetProcessShutdownParameters(uint level, uint flags);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool PeekMessage(out NativeMessage lpMsg, nint hWnd, uint wMsgFilterMin,
   uint wMsgFilterMax, uint wRemoveMsg);
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UnregisterHotKey(nint hWnd, int id);
        [DllImport("SHELL32", CallingConvention = CallingConvention.StdCall)]
        public static extern uint SHAppBarMessage(int dwMessage, ref APPBARDATA pData);
        [DllImport("User32.dll", ExactSpelling = true,
            CharSet = CharSet.Auto)]
        public static extern bool MoveWindow
            (nint hWnd, int x, int y, int cx, int cy, bool repaint);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SystemParametersInfo(SPI uiAction, uint uiParam, ref MINIMIZEDMETRICS pvParam, SPIF fWinIni);
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SystemParametersInfo(SPI uiAction, uint uiParam, ref RECT pvParam, SPIF fWinIni);

        [DllImport("sndvolsso.dll", SetLastError = true, EntryPoint = "#1")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool AudioHIDInitialize(nint handle);
        [DllImport("sndvolsso.dll", SetLastError = true, EntryPoint = "#2")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool AudioHIDShutdown();
        [DllImport("sndvolsso.dll", SetLastError = true, EntryPoint = "#3")]
        public static extern void AudioHidProcessMessage(int msg, nint wParam, nint lParam);
        [DllImport("sndvolsso.dll", SetLastError = true, EntryPoint = "#4")]
        public static extern bool AudioHIDProcessAppCommand(uint msg);
        [DllImport("kernel32.dll")]
        public static extern nint CreateEvent(nint lpEventAttributes, bool bManualReset, bool bInitialState, string lpName);

        [DllImport("kernel32.dll")]
        public static extern bool SetEvent(nint hEvent);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern nint SetParent(nint hWndChild, nint hWndNewParent);
        [DllImport("user32.dll")]
        internal static extern int SetWindowCompositionAttribute(nint hwnd, ref WindowCompositionAttributeData data);
        [DllImport("user32.dll")]
        static extern bool EnumDisplayMonitors(nint hdc, nint lprcClip, EnumMonitorsDelegate lpfnEnum, nint dwData);

        delegate bool EnumMonitorsDelegate(nint hMonitor, nint hdcMonitor, ref Rect lprcMonitor, nint dwData);
        internal static void DoExplorerInit()
        {
            SetCurrentProcessExplicitAppUserModelID("Microsoft.Windows.Explorer");
            SetErrorMode((ErrorModes)((int)ErrorModes.SEM_FAILCRITICALERRORS | 0x4000));
            SetPriorityClass(GetCurrentProcess(), PriorityClass.HIGH_PRIORITY_CLASS);
            EnableMouseInPointer(false);
            // Create Explorer regkey (actually needed when you create the profile for the first time and log in)
            SetExplorerServerMode(3);
            SetPriorityClass(GetCurrentProcess(), PriorityClass.NORMAL_PRIORITY_CLASS);
            ShellDDEInit(true);
            SetProcessShutdownParameters(0x4FF, 1);//SHUTDOWN_NORETRY

            NativeMessage msg;
            PeekMessage(out msg, 0, WM_QUIT, WM_QUIT, 0); // fixes a bug

            var cache = (IStartMenuItemsCache)new CStartMenuItemsCache();
            try
            {
                cache.RegisterForNotifications();
            }
            catch {
                Console.WriteLine("incorrect process filename");
            }
        }
        [DllImport("User32.dll")]
        static extern bool GetMonitorInfoW(nint hMonitor, [In, Out] ref MonitorInfoEx lpmi);
        public static void UpdateWorkspace()
        {
            EnumDisplayMonitors(0, 0, delegate (nint hMonitor, nint hdcMonitor, ref Rect lprcMonitor, nint dwData)
            {
                var x = new MonitorInfoEx();
                x.Init();

                if (!GetMonitorInfoW(hMonitor, ref x))
                {
                    Console.WriteLine("failed to get monitor informations");
                }
                else
                {
                    Console.WriteLine(x.Monitor.ToString());
                    Console.WriteLine("workarea:" + x.WorkArea.ToString());
                    x.Monitor.bottom += 70;
                    SetWorkspace(x.Monitor);
                }
                return true;
            }, 0);
        }
        private static bool SetWorkspace(RECT rect)
        {
            // Since you've declared the P/Invoke function correctly, you don't need to
            // do the marshaling yourself manually. The .NET FW will take care of it.

            bool result = SystemParametersInfo(SPI.SPI_SETWORKAREA,
                                               1,
                                               ref rect,
                                               SPIF.SPIF_UPDATEINIFILE | SPIF.SPIF_SENDWININICHANGE);
            if (!result)
            {
                Console.WriteLine("Failed to set " + new Win32Exception().Message);
            }

            return result;
        }
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct SECURITY_DESCRIPTOR
    {
        public byte revision;
        public byte size;
        public short control;
        public nint owner;
        public nint group;
        public nint sacl;
        public nint dacl;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct NativeMessage
    {
        public nint handle;
        public uint msg;
        public nint wParam;
        public nint lParam;
        public uint time;
        public System.Drawing.Point p;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct WNDCLASSEX
    {
        [MarshalAs(UnmanagedType.U4)]
        public int cbSize;
        [MarshalAs(UnmanagedType.U4)]
        public int style;
        public nint lpfnWndProc; // not WndProc
        public int cbClsExtra;
        public int cbWndExtra;
        public nint hInstance;
        public nint hIcon;
        public nint hCursor;
        public nint hbrBackground;
        public string lpszMenuName;
        public string lpszClassName;
        public nint hIconSm;


        public static WNDCLASSEX Build()
        {
            var nw = new WNDCLASSEX();
            nw.cbSize = Marshal.SizeOf(typeof(WNDCLASSEX));
            return nw;
        }
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int left;
        public int top;
        public int right;
        public int bottom;


        public override string ToString()
        {
            int width = right - left;
            int height = bottom - top;
            return $"{width}x{height}";
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MINIMIZEDMETRICS
    {
        public uint size;
        public int iWidth;
        public int iHorzGap;
        public int iVertGap;
        public int iArrange;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PAINTSTRUCT
    {
        public nint hdc;
        public bool fErase;
        public RECT rcPaint;
        public bool fRestore;
        public bool fIncUpdate;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] rgbReserved;
    }
    [Flags]
    public enum SPIF
    {
        None = 0x00,
        /// <summary>Writes the new system-wide parameter setting to the user profile.</summary>
        SPIF_UPDATEINIFILE = 0x01,
        /// <summary>Broadcasts the WM_SETTINGCHANGE message after updating the user profile.</summary>
        SPIF_SENDCHANGE = 0x02,
        /// <summary>Same as SPIF_SENDCHANGE.</summary>
        SPIF_SENDWININICHANGE = 0x02
    }

    // http://www.pinvoke.net/default.aspx/Enums/ShowWindowCommand.html  
    public enum ShowWindowCommands : int
    {
        /// <summary>  
        /// Hides the window and activates another window.  
        /// </summary>  
        Hide = 0,
        /// <summary>  
        /// Activates and displays a window. If the window is minimized or   
        /// maximized, the system restores it to its original size and position.  
        /// An application should specify this flag when displaying the window   
        /// for the first time.  
        /// </summary>  
        Normal = 1,
        /// <summary>  
        /// Activates the window and displays it as a minimized window.  
        /// </summary>  
        ShowMinimized = 2,
        /// <summary>  
        /// Maximizes the specified window.  
        /// </summary>  
        Maximize = 3, // is this the right value?  
        /// <summary>  
        /// Activates the window and displays it as a maximized window.  
        /// </summary>         
        ShowMaximized = 3,
        /// <summary>  
        /// Displays a window in its most recent size and position. This value   
        /// is similar to <see cref="Win32.ShowWindowCommand.Normal"/>, except   
        /// the window is not activated.  
        /// </summary>  
        ShowNoActivate = 4,
        /// <summary>  
        /// Activates the window and displays it in its current size and position.   
        /// </summary>  
        Show = 5,
        /// <summary>  
        /// Minimizes the specified window and activates the next top-level   
        /// window in the Z order.  
        /// </summary>  
        Minimize = 6,
        /// <summary>  
        /// Displays the window as a minimized window. This value is similar to  
        /// <see cref="Win32.ShowWindowCommand.ShowMinimized"/>, except the   
        /// window is not activated.  
        /// </summary>  
        ShowMinNoActive = 7,
        /// <summary>  
        /// Displays the window in its current size and position. This value is   
        /// similar to <see cref="Win32.ShowWindowCommand.Show"/>, except the   
        /// window is not activated.  
        /// </summary>  
        ShowNA = 8,
        /// <summary>  
        /// Activates and displays the window. If the window is minimized or   
        /// maximized, the system restores it to its original size and position.   
        /// An application should specify this flag when restoring a minimized window.  
        /// </summary>  
        Restore = 9,
        /// <summary>  
        /// Sets the show state based on the SW_* value specified in the   
        /// STARTUPINFO structure passed to the CreateProcess function by the   
        /// program that started the application.  
        /// </summary>  
        ShowDefault = 10,
        /// <summary>  
        ///  <b>Windows 2000/XP:</b> Minimizes a window, even if the thread   
        /// that owns the window is not responding. This flag should only be   
        /// used when minimizing windows from a different thread.  
        /// </summary>  
        ForceMinimize = 11
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct APPBARDATA
    {
        public int cbSize;
        public nint hWnd;
        public int uCallbackMessage;
        public int uEdge;
        public RECT rc;
        public nint lParam;
    }
    public enum ABMsg : int
    {
        ABM_NEW = 0,
        ABM_REMOVE,
        ABM_QUERYPOS,
        ABM_SETPOS,
        ABM_GETSTATE,
        ABM_GETTASKBARPOS,
        ABM_ACTIVATE,
        ABM_GETAUTOHIDEBAR,
        ABM_SETAUTOHIDEBAR,
        ABM_WINDOWPOSCHANGED,
        ABM_SETSTATE
    }
    enum ABNotify : int
    {
        ABN_STATECHANGE = 0,
        ABN_POSCHANGED,
        ABN_FULLSCREENAPP,
        ABN_WINDOWARRANGE
    }
    enum ABEdge : int
    {
        ABE_LEFT = 0,
        ABE_TOP,
        ABE_RIGHT,
        ABE_BOTTOM
    }
    #region SPI
    /// <summary>
    /// SPI_ System-wide parameter - Used in SystemParametersInfo function
    /// </summary>
    [Description("SPI_(System-wide parameter - Used in SystemParametersInfo function )")]
    public enum SPI : uint
    {
        /// <summary>
        /// Determines whether the warning beeper is on.
        /// The pvParam parameter must point to a BOOL variable that receives TRUE if the beeper is on, or FALSE if it is off.
        /// </summary>
        SPI_GETBEEP = 0x0001,

        /// <summary>
        /// Turns the warning beeper on or off. The uiParam parameter specifies TRUE for on, or FALSE for off.
        /// </summary>
        SPI_SETBEEP = 0x0002,

        /// <summary>
        /// Retrieves the two mouse threshold values and the mouse speed.
        /// </summary>
        SPI_GETMOUSE = 0x0003,

        /// <summary>
        /// Sets the two mouse threshold values and the mouse speed.
        /// </summary>
        SPI_SETMOUSE = 0x0004,

        /// <summary>
        /// Retrieves the border multiplier factor that determines the width of a window's sizing border.
        /// The pvParam parameter must point to an integer variable that receives this value.
        /// </summary>
        SPI_GETBORDER = 0x0005,

        /// <summary>
        /// Sets the border multiplier factor that determines the width of a window's sizing border.
        /// The uiParam parameter specifies the new value.
        /// </summary>
        SPI_SETBORDER = 0x0006,

        /// <summary>
        /// Retrieves the keyboard repeat-speed setting, which is a value in the range from 0 (approximately 2.5 repetitions per second)
        /// through 31 (approximately 30 repetitions per second). The actual repeat rates are hardware-dependent and may vary from
        /// a linear scale by as much as 20%. The pvParam parameter must point to a DWORD variable that receives the setting
        /// </summary>
        SPI_GETKEYBOARDSPEED = 0x000A,

        /// <summary>
        /// Sets the keyboard repeat-speed setting. The uiParam parameter must specify a value in the range from 0
        /// (approximately 2.5 repetitions per second) through 31 (approximately 30 repetitions per second).
        /// The actual repeat rates are hardware-dependent and may vary from a linear scale by as much as 20%.
        /// If uiParam is greater than 31, the parameter is set to 31.
        /// </summary>
        SPI_SETKEYBOARDSPEED = 0x000B,

        /// <summary>
        /// Not implemented.
        /// </summary>
        SPI_LANGDRIVER = 0x000C,

        /// <summary>
        /// Sets or retrieves the width, in pixels, of an icon cell. The system uses this rectangle to arrange icons in large icon view.
        /// To set this value, set uiParam to the new value and set pvParam to null. You cannot set this value to less than SM_CXICON.
        /// To retrieve this value, pvParam must point to an integer that receives the current value.
        /// </summary>
        SPI_ICONHORIZONTALSPACING = 0x000D,

        /// <summary>
        /// Retrieves the screen saver time-out value, in seconds. The pvParam parameter must point to an integer variable that receives the value.
        /// </summary>
        SPI_GETSCREENSAVETIMEOUT = 0x000E,

        /// <summary>
        /// Sets the screen saver time-out value to the value of the uiParam parameter. This value is the amount of time, in seconds,
        /// that the system must be idle before the screen saver activates.
        /// </summary>
        SPI_SETSCREENSAVETIMEOUT = 0x000F,

        /// <summary>
        /// Determines whether screen saving is enabled. The pvParam parameter must point to a bool variable that receives TRUE
        /// if screen saving is enabled, or FALSE otherwise.
        /// </summary>
        SPI_GETSCREENSAVEACTIVE = 0x0010,

        /// <summary>
        /// Sets the state of the screen saver. The uiParam parameter specifies TRUE to activate screen saving, or FALSE to deactivate it.
        /// </summary>
        SPI_SETSCREENSAVEACTIVE = 0x0011,

        /// <summary>
        /// Retrieves the current granularity value of the desktop sizing grid. The pvParam parameter must point to an integer variable
        /// that receives the granularity.
        /// </summary>
        SPI_GETGRIDGRANULARITY = 0x0012,

        /// <summary>
        /// Sets the granularity of the desktop sizing grid to the value of the uiParam parameter.
        /// </summary>
        SPI_SETGRIDGRANULARITY = 0x0013,

        /// <summary>
        /// Sets the desktop wallpaper. The value of the pvParam parameter determines the new wallpaper. To specify a wallpaper bitmap,
        /// set pvParam to point to a null-terminated string containing the name of a bitmap file. Setting pvParam to "" removes the wallpaper.
        /// Setting pvParam to SETWALLPAPER_DEFAULT or null reverts to the default wallpaper.
        /// </summary>
        SPI_SETDESKWALLPAPER = 0x0014,

        /// <summary>
        /// Sets the current desktop pattern by causing Windows to read the Pattern= setting from the WIN.INI file.
        /// </summary>
        SPI_SETDESKPATTERN = 0x0015,

        /// <summary>
        /// Retrieves the keyboard repeat-delay setting, which is a value in the range from 0 (approximately 250 ms delay) through 3
        /// (approximately 1 second delay). The actual delay associated with each value may vary depending on the hardware. The pvParam parameter must point to an integer variable that receives the setting.
        /// </summary>
        SPI_GETKEYBOARDDELAY = 0x0016,

        /// <summary>
        /// Sets the keyboard repeat-delay setting. The uiParam parameter must specify 0, 1, 2, or 3, where zero sets the shortest delay
        /// (approximately 250 ms) and 3 sets the longest delay (approximately 1 second). The actual delay associated with each value may
        /// vary depending on the hardware.
        /// </summary>
        SPI_SETKEYBOARDDELAY = 0x0017,

        /// <summary>
        /// Sets or retrieves the height, in pixels, of an icon cell.
        /// To set this value, set uiParam to the new value and set pvParam to null. You cannot set this value to less than SM_CYICON.
        /// To retrieve this value, pvParam must point to an integer that receives the current value.
        /// </summary>
        SPI_ICONVERTICALSPACING = 0x0018,

        /// <summary>
        /// Determines whether icon-title wrapping is enabled. The pvParam parameter must point to a bool variable that receives TRUE
        /// if enabled, or FALSE otherwise.
        /// </summary>
        SPI_GETICONTITLEWRAP = 0x0019,

        /// <summary>
        /// Turns icon-title wrapping on or off. The uiParam parameter specifies TRUE for on, or FALSE for off.
        /// </summary>
        SPI_SETICONTITLEWRAP = 0x001A,

        /// <summary>
        /// Determines whether pop-up menus are left-aligned or right-aligned, relative to the corresponding menu-bar item.
        /// The pvParam parameter must point to a bool variable that receives TRUE if left-aligned, or FALSE otherwise.
        /// </summary>
        SPI_GETMENUDROPALIGNMENT = 0x001B,

        /// <summary>
        /// Sets the alignment value of pop-up menus. The uiParam parameter specifies TRUE for right alignment, or FALSE for left alignment.
        /// </summary>
        SPI_SETMENUDROPALIGNMENT = 0x001C,

        /// <summary>
        /// Sets the width of the double-click rectangle to the value of the uiParam parameter.
        /// The double-click rectangle is the rectangle within which the second click of a double-click must fall for it to be registered
        /// as a double-click.
        /// To retrieve the width of the double-click rectangle, call GetSystemMetrics with the SM_CXDOUBLECLK flag.
        /// </summary>
        SPI_SETDOUBLECLKWIDTH = 0x001D,

        /// <summary>
        /// Sets the height of the double-click rectangle to the value of the uiParam parameter.
        /// The double-click rectangle is the rectangle within which the second click of a double-click must fall for it to be registered
        /// as a double-click.
        /// To retrieve the height of the double-click rectangle, call GetSystemMetrics with the SM_CYDOUBLECLK flag.
        /// </summary>
        SPI_SETDOUBLECLKHEIGHT = 0x001E,

        /// <summary>
        /// Retrieves the logical font information for the current icon-title font. The uiParam parameter specifies the size of a LOGFONT structure,
        /// and the pvParam parameter must point to the LOGFONT structure to fill in.
        /// </summary>
        SPI_GETICONTITLELOGFONT = 0x001F,

        /// <summary>
        /// Sets the double-click time for the mouse to the value of the uiParam parameter. The double-click time is the maximum number
        /// of milliseconds that can occur between the first and second clicks of a double-click. You can also call the SetDoubleClickTime
        /// function to set the double-click time. To get the current double-click time, call the GetDoubleClickTime function.
        /// </summary>
        SPI_SETDOUBLECLICKTIME = 0x0020,

        /// <summary>
        /// Swaps or restores the meaning of the left and right mouse buttons. The uiParam parameter specifies TRUE to swap the meanings
        /// of the buttons, or FALSE to restore their original meanings.
        /// </summary>
        SPI_SETMOUSEBUTTONSWAP = 0x0021,

        /// <summary>
        /// Sets the font that is used for icon titles. The uiParam parameter specifies the size of a LOGFONT structure,
        /// and the pvParam parameter must point to a LOGFONT structure.
        /// </summary>
        SPI_SETICONTITLELOGFONT = 0x0022,

        /// <summary>
        /// This flag is obsolete. Previous versions of the system use this flag to determine whether ALT+TAB fast task switching is enabled.
        /// For Windows 95, Windows 98, and Windows NT version 4.0 and later, fast task switching is always enabled.
        /// </summary>
        SPI_GETFASTTASKSWITCH = 0x0023,

        /// <summary>
        /// This flag is obsolete. Previous versions of the system use this flag to enable or disable ALT+TAB fast task switching.
        /// For Windows 95, Windows 98, and Windows NT version 4.0 and later, fast task switching is always enabled.
        /// </summary>
        SPI_SETFASTTASKSWITCH = 0x0024,

        //#if(WINVER >= 0x0400)
        /// <summary>
        /// Sets dragging of full windows either on or off. The uiParam parameter specifies TRUE for on, or FALSE for off.
        /// Windows 95:  This flag is supported only if Windows Plus! is installed. See SPI_GETWINDOWSEXTENSION.
        /// </summary>
        SPI_SETDRAGFULLWINDOWS = 0x0025,

        /// <summary>
        /// Determines whether dragging of full windows is enabled. The pvParam parameter must point to a BOOL variable that receives TRUE
        /// if enabled, or FALSE otherwise.
        /// Windows 95:  This flag is supported only if Windows Plus! is installed. See SPI_GETWINDOWSEXTENSION.
        /// </summary>
        SPI_GETDRAGFULLWINDOWS = 0x0026,

        /// <summary>
        /// Retrieves the metrics associated with the nonclient area of nonminimized windows. The pvParam parameter must point
        /// to a NONCLIENTMETRICS structure that receives the information. Set the cbSize member of this structure and the uiParam parameter
        /// to sizeof(NONCLIENTMETRICS).
        /// </summary>
        SPI_GETNONCLIENTMETRICS = 0x0029,

        /// <summary>
        /// Sets the metrics associated with the nonclient area of nonminimized windows. The pvParam parameter must point
        /// to a NONCLIENTMETRICS structure that contains the new parameters. Set the cbSize member of this structure
        /// and the uiParam parameter to sizeof(NONCLIENTMETRICS). Also, the lfHeight member of the LOGFONT structure must be a negative value.
        /// </summary>
        SPI_SETNONCLIENTMETRICS = 0x002A,

        /// <summary>
        /// Retrieves the metrics associated with minimized windows. The pvParam parameter must point to a MINIMIZEDMETRICS structure
        /// that receives the information. Set the cbSize member of this structure and the uiParam parameter to sizeof(MINIMIZEDMETRICS).
        /// </summary>
        SPI_GETMINIMIZEDMETRICS = 0x002B,

        /// <summary>
        /// Sets the metrics associated with minimized windows. The pvParam parameter must point to a MINIMIZEDMETRICS structure
        /// that contains the new parameters. Set the cbSize member of this structure and the uiParam parameter to sizeof(MINIMIZEDMETRICS).
        /// </summary>
        SPI_SETMINIMIZEDMETRICS = 0x002C,

        /// <summary>
        /// Retrieves the metrics associated with icons. The pvParam parameter must point to an ICONMETRICS structure that receives
        /// the information. Set the cbSize member of this structure and the uiParam parameter to sizeof(ICONMETRICS).
        /// </summary>
        SPI_GETICONMETRICS = 0x002D,

        /// <summary>
        /// Sets the metrics associated with icons. The pvParam parameter must point to an ICONMETRICS structure that contains
        /// the new parameters. Set the cbSize member of this structure and the uiParam parameter to sizeof(ICONMETRICS).
        /// </summary>
        SPI_SETICONMETRICS = 0x002E,

        /// <summary>
        /// Sets the size of the work area. The work area is the portion of the screen not obscured by the system taskbar
        /// or by application desktop toolbars. The pvParam parameter is a pointer to a RECT structure that specifies the new work area rectangle,
        /// expressed in virtual screen coordinates. In a system with multiple display monitors, the function sets the work area
        /// of the monitor that contains the specified rectangle.
        /// </summary>
        SPI_SETWORKAREA = 0x002F,

        /// <summary>
        /// Retrieves the size of the work area on the primary display monitor. The work area is the portion of the screen not obscured
        /// by the system taskbar or by application desktop toolbars. The pvParam parameter must point to a RECT structure that receives
        /// the coordinates of the work area, expressed in virtual screen coordinates.
        /// To get the work area of a monitor other than the primary display monitor, call the GetMonitorInfo function.
        /// </summary>
        SPI_GETWORKAREA = 0x0030,

        /// <summary>
        /// Windows Me/98/95:  Pen windows is being loaded or unloaded. The uiParam parameter is TRUE when loading and FALSE
        /// when unloading pen windows. The pvParam parameter is null.
        /// </summary>
        SPI_SETPENWINDOWS = 0x0031,

        /// <summary>
        /// Retrieves information about the HighContrast accessibility feature. The pvParam parameter must point to a HIGHCONTRAST structure
        /// that receives the information. Set the cbSize member of this structure and the uiParam parameter to sizeof(HIGHCONTRAST).
        /// For a general discussion, see remarks.
        /// Windows NT:  This value is not supported.
        /// </summary>
        /// <remarks>
        /// There is a difference between the High Contrast color scheme and the High Contrast Mode. The High Contrast color scheme changes
        /// the system colors to colors that have obvious contrast; you switch to this color scheme by using the Display Options in the control panel.
        /// The High Contrast Mode, which uses SPI_GETHIGHCONTRAST and SPI_SETHIGHCONTRAST, advises applications to modify their appearance
        /// for visually-impaired users. It involves such things as audible warning to users and customized color scheme
        /// (using the Accessibility Options in the control panel). For more information, see HIGHCONTRAST on MSDN.
        /// For more information on general accessibility features, see Accessibility on MSDN.
        /// </remarks>
        SPI_GETHIGHCONTRAST = 0x0042,

        /// <summary>
        /// Sets the parameters of the HighContrast accessibility feature. The pvParam parameter must point to a HIGHCONTRAST structure
        /// that contains the new parameters. Set the cbSize member of this structure and the uiParam parameter to sizeof(HIGHCONTRAST).
        /// Windows NT:  This value is not supported.
        /// </summary>
        SPI_SETHIGHCONTRAST = 0x0043,

        /// <summary>
        /// Determines whether the user relies on the keyboard instead of the mouse, and wants applications to display keyboard interfaces
        /// that would otherwise be hidden. The pvParam parameter must point to a BOOL variable that receives TRUE
        /// if the user relies on the keyboard; or FALSE otherwise.
        /// Windows NT:  This value is not supported.
        /// </summary>
        SPI_GETKEYBOARDPREF = 0x0044,

        /// <summary>
        /// Sets the keyboard preference. The uiParam parameter specifies TRUE if the user relies on the keyboard instead of the mouse,
        /// and wants applications to display keyboard interfaces that would otherwise be hidden; uiParam is FALSE otherwise.
        /// Windows NT:  This value is not supported.
        /// </summary>
        SPI_SETKEYBOARDPREF = 0x0045,

        /// <summary>
        /// Determines whether a screen reviewer utility is running. A screen reviewer utility directs textual information to an output device,
        /// such as a speech synthesizer or Braille display. When this flag is set, an application should provide textual information
        /// in situations where it would otherwise present the information graphically.
        /// The pvParam parameter is a pointer to a BOOL variable that receives TRUE if a screen reviewer utility is running, or FALSE otherwise.
        /// Windows NT:  This value is not supported.
        /// </summary>
        SPI_GETSCREENREADER = 0x0046,

        /// <summary>
        /// Determines whether a screen review utility is running. The uiParam parameter specifies TRUE for on, or FALSE for off.
        /// Windows NT:  This value is not supported.
        /// </summary>
        SPI_SETSCREENREADER = 0x0047,

        /// <summary>
        /// Retrieves the animation effects associated with user actions. The pvParam parameter must point to an ANIMATIONINFO structure
        /// that receives the information. Set the cbSize member of this structure and the uiParam parameter to sizeof(ANIMATIONINFO).
        /// </summary>
        SPI_GETANIMATION = 0x0048,

        /// <summary>
        /// Sets the animation effects associated with user actions. The pvParam parameter must point to an ANIMATIONINFO structure
        /// that contains the new parameters. Set the cbSize member of this structure and the uiParam parameter to sizeof(ANIMATIONINFO).
        /// </summary>
        SPI_SETANIMATION = 0x0049,

        /// <summary>
        /// Determines whether the font smoothing feature is enabled. This feature uses font antialiasing to make font curves appear smoother
        /// by painting pixels at different gray levels.
        /// The pvParam parameter must point to a BOOL variable that receives TRUE if the feature is enabled, or FALSE if it is not.
        /// Windows 95:  This flag is supported only if Windows Plus! is installed. See SPI_GETWINDOWSEXTENSION.
        /// </summary>
        SPI_GETFONTSMOOTHING = 0x004A,

        /// <summary>
        /// Enables or disables the font smoothing feature, which uses font antialiasing to make font curves appear smoother
        /// by painting pixels at different gray levels.
        /// To enable the feature, set the uiParam parameter to TRUE. To disable the feature, set uiParam to FALSE.
        /// Windows 95:  This flag is supported only if Windows Plus! is installed. See SPI_GETWINDOWSEXTENSION.
        /// </summary>
        SPI_SETFONTSMOOTHING = 0x004B,

        /// <summary>
        /// Sets the width, in pixels, of the rectangle used to detect the start of a drag operation. Set uiParam to the new value.
        /// To retrieve the drag width, call GetSystemMetrics with the SM_CXDRAG flag.
        /// </summary>
        SPI_SETDRAGWIDTH = 0x004C,

        /// <summary>
        /// Sets the height, in pixels, of the rectangle used to detect the start of a drag operation. Set uiParam to the new value.
        /// To retrieve the drag height, call GetSystemMetrics with the SM_CYDRAG flag.
        /// </summary>
        SPI_SETDRAGHEIGHT = 0x004D,

        /// <summary>
        /// Used internally; applications should not use this value.
        /// </summary>
        SPI_SETHANDHELD = 0x004E,

        /// <summary>
        /// Retrieves the time-out value for the low-power phase of screen saving. The pvParam parameter must point to an integer variable
        /// that receives the value. This flag is supported for 32-bit applications only.
        /// Windows NT, Windows Me/98:  This flag is supported for 16-bit and 32-bit applications.
        /// Windows 95:  This flag is supported for 16-bit applications only.
        /// </summary>
        SPI_GETLOWPOWERTIMEOUT = 0x004F,

        /// <summary>
        /// Retrieves the time-out value for the power-off phase of screen saving. The pvParam parameter must point to an integer variable
        /// that receives the value. This flag is supported for 32-bit applications only.
        /// Windows NT, Windows Me/98:  This flag is supported for 16-bit and 32-bit applications.
        /// Windows 95:  This flag is supported for 16-bit applications only.
        /// </summary>
        SPI_GETPOWEROFFTIMEOUT = 0x0050,

        /// <summary>
        /// Sets the time-out value, in seconds, for the low-power phase of screen saving. The uiParam parameter specifies the new value.
        /// The pvParam parameter must be null. This flag is supported for 32-bit applications only.
        /// Windows NT, Windows Me/98:  This flag is supported for 16-bit and 32-bit applications.
        /// Windows 95:  This flag is supported for 16-bit applications only.
        /// </summary>
        SPI_SETLOWPOWERTIMEOUT = 0x0051,

        /// <summary>
        /// Sets the time-out value, in seconds, for the power-off phase of screen saving. The uiParam parameter specifies the new value.
        /// The pvParam parameter must be null. This flag is supported for 32-bit applications only.
        /// Windows NT, Windows Me/98:  This flag is supported for 16-bit and 32-bit applications.
        /// Windows 95:  This flag is supported for 16-bit applications only.
        /// </summary>
        SPI_SETPOWEROFFTIMEOUT = 0x0052,

        /// <summary>
        /// Determines whether the low-power phase of screen saving is enabled. The pvParam parameter must point to a BOOL variable
        /// that receives TRUE if enabled, or FALSE if disabled. This flag is supported for 32-bit applications only.
        /// Windows NT, Windows Me/98:  This flag is supported for 16-bit and 32-bit applications.
        /// Windows 95:  This flag is supported for 16-bit applications only.
        /// </summary>
        SPI_GETLOWPOWERACTIVE = 0x0053,

        /// <summary>
        /// Determines whether the power-off phase of screen saving is enabled. The pvParam parameter must point to a BOOL variable
        /// that receives TRUE if enabled, or FALSE if disabled. This flag is supported for 32-bit applications only.
        /// Windows NT, Windows Me/98:  This flag is supported for 16-bit and 32-bit applications.
        /// Windows 95:  This flag is supported for 16-bit applications only.
        /// </summary>
        SPI_GETPOWEROFFACTIVE = 0x0054,

        /// <summary>
        /// Activates or deactivates the low-power phase of screen saving. Set uiParam to 1 to activate, or zero to deactivate.
        /// The pvParam parameter must be null. This flag is supported for 32-bit applications only.
        /// Windows NT, Windows Me/98:  This flag is supported for 16-bit and 32-bit applications.
        /// Windows 95:  This flag is supported for 16-bit applications only.
        /// </summary>
        SPI_SETLOWPOWERACTIVE = 0x0055,

        /// <summary>
        /// Activates or deactivates the power-off phase of screen saving. Set uiParam to 1 to activate, or zero to deactivate.
        /// The pvParam parameter must be null. This flag is supported for 32-bit applications only.
        /// Windows NT, Windows Me/98:  This flag is supported for 16-bit and 32-bit applications.
        /// Windows 95:  This flag is supported for 16-bit applications only.
        /// </summary>
        SPI_SETPOWEROFFACTIVE = 0x0056,

        /// <summary>
        /// Reloads the system cursors. Set the uiParam parameter to zero and the pvParam parameter to null.
        /// </summary>
        SPI_SETCURSORS = 0x0057,

        /// <summary>
        /// Reloads the system icons. Set the uiParam parameter to zero and the pvParam parameter to null.
        /// </summary>
        SPI_SETICONS = 0x0058,

        /// <summary>
        /// Retrieves the input locale identifier for the system default input language. The pvParam parameter must point
        /// to an HKL variable that receives this value. For more information, see Languages, Locales, and Keyboard Layouts on MSDN.
        /// </summary>
        SPI_GETDEFAULTINPUTLANG = 0x0059,

        /// <summary>
        /// Sets the default input language for the system shell and applications. The specified language must be displayable
        /// using the current system character set. The pvParam parameter must point to an HKL variable that contains
        /// the input locale identifier for the default language. For more information, see Languages, Locales, and Keyboard Layouts on MSDN.
        /// </summary>
        SPI_SETDEFAULTINPUTLANG = 0x005A,

        /// <summary>
        /// Sets the hot key set for switching between input languages. The uiParam and pvParam parameters are not used.
        /// The value sets the shortcut keys in the keyboard property sheets by reading the registry again. The registry must be set before this flag is used. the path in the registry is \HKEY_CURRENT_USER\keyboard layout\toggle. Valid values are "1" = ALT+SHIFT, "2" = CTRL+SHIFT, and "3" = none.
        /// </summary>
        SPI_SETLANGTOGGLE = 0x005B,

        /// <summary>
        /// Windows 95:  Determines whether the Windows extension, Windows Plus!, is installed. Set the uiParam parameter to 1.
        /// The pvParam parameter is not used. The function returns TRUE if the extension is installed, or FALSE if it is not.
        /// </summary>
        SPI_GETWINDOWSEXTENSION = 0x005C,

        /// <summary>
        /// Enables or disables the Mouse Trails feature, which improves the visibility of mouse cursor movements by briefly showing
        /// a trail of cursors and quickly erasing them.
        /// To disable the feature, set the uiParam parameter to zero or 1. To enable the feature, set uiParam to a value greater than 1
        /// to indicate the number of cursors drawn in the trail.
        /// Windows 2000/NT:  This value is not supported.
        /// </summary>
        SPI_SETMOUSETRAILS = 0x005D,

        /// <summary>
        /// Determines whether the Mouse Trails feature is enabled. This feature improves the visibility of mouse cursor movements
        /// by briefly showing a trail of cursors and quickly erasing them.
        /// The pvParam parameter must point to an integer variable that receives a value. If the value is zero or 1, the feature is disabled.
        /// If the value is greater than 1, the feature is enabled and the value indicates the number of cursors drawn in the trail.
        /// The uiParam parameter is not used.
        /// Windows 2000/NT:  This value is not supported.
        /// </summary>
        SPI_GETMOUSETRAILS = 0x005E,

        /// <summary>
        /// Windows Me/98:  Used internally; applications should not use this flag.
        /// </summary>
        SPI_SETSCREENSAVERRUNNING = 0x0061,

        /// <summary>
        /// Same as SPI_SETSCREENSAVERRUNNING.
        /// </summary>
        SPI_SCREENSAVERRUNNING = SPI_SETSCREENSAVERRUNNING,
        //#endif /* WINVER >= 0x0400 */

        /// <summary>
        /// Retrieves information about the FilterKeys accessibility feature. The pvParam parameter must point to a FILTERKEYS structure
        /// that receives the information. Set the cbSize member of this structure and the uiParam parameter to sizeof(FILTERKEYS).
        /// </summary>
        SPI_GETFILTERKEYS = 0x0032,

        /// <summary>
        /// Sets the parameters of the FilterKeys accessibility feature. The pvParam parameter must point to a FILTERKEYS structure
        /// that contains the new parameters. Set the cbSize member of this structure and the uiParam parameter to sizeof(FILTERKEYS).
        /// </summary>
        SPI_SETFILTERKEYS = 0x0033,

        /// <summary>
        /// Retrieves information about the ToggleKeys accessibility feature. The pvParam parameter must point to a TOGGLEKEYS structure
        /// that receives the information. Set the cbSize member of this structure and the uiParam parameter to sizeof(TOGGLEKEYS).
        /// </summary>
        SPI_GETTOGGLEKEYS = 0x0034,

        /// <summary>
        /// Sets the parameters of the ToggleKeys accessibility feature. The pvParam parameter must point to a TOGGLEKEYS structure
        /// that contains the new parameters. Set the cbSize member of this structure and the uiParam parameter to sizeof(TOGGLEKEYS).
        /// </summary>
        SPI_SETTOGGLEKEYS = 0x0035,

        /// <summary>
        /// Retrieves information about the MouseKeys accessibility feature. The pvParam parameter must point to a MOUSEKEYS structure
        /// that receives the information. Set the cbSize member of this structure and the uiParam parameter to sizeof(MOUSEKEYS).
        /// </summary>
        SPI_GETMOUSEKEYS = 0x0036,

        /// <summary>
        /// Sets the parameters of the MouseKeys accessibility feature. The pvParam parameter must point to a MOUSEKEYS structure
        /// that contains the new parameters. Set the cbSize member of this structure and the uiParam parameter to sizeof(MOUSEKEYS).
        /// </summary>
        SPI_SETMOUSEKEYS = 0x0037,

        /// <summary>
        /// Determines whether the Show Sounds accessibility flag is on or off. If it is on, the user requires an application
        /// to present information visually in situations where it would otherwise present the information only in audible form.
        /// The pvParam parameter must point to a BOOL variable that receives TRUE if the feature is on, or FALSE if it is off.
        /// Using this value is equivalent to calling GetSystemMetrics (SM_SHOWSOUNDS). That is the recommended call.
        /// </summary>
        SPI_GETSHOWSOUNDS = 0x0038,

        /// <summary>
        /// Sets the parameters of the SoundSentry accessibility feature. The pvParam parameter must point to a SOUNDSENTRY structure
        /// that contains the new parameters. Set the cbSize member of this structure and the uiParam parameter to sizeof(SOUNDSENTRY).
        /// </summary>
        SPI_SETSHOWSOUNDS = 0x0039,

        /// <summary>
        /// Retrieves information about the StickyKeys accessibility feature. The pvParam parameter must point to a STICKYKEYS structure
        /// that receives the information. Set the cbSize member of this structure and the uiParam parameter to sizeof(STICKYKEYS).
        /// </summary>
        SPI_GETSTICKYKEYS = 0x003A,

        /// <summary>
        /// Sets the parameters of the StickyKeys accessibility feature. The pvParam parameter must point to a STICKYKEYS structure
        /// that contains the new parameters. Set the cbSize member of this structure and the uiParam parameter to sizeof(STICKYKEYS).
        /// </summary>
        SPI_SETSTICKYKEYS = 0x003B,

        /// <summary>
        /// Retrieves information about the time-out period associated with the accessibility features. The pvParam parameter must point
        /// to an ACCESSTIMEOUT structure that receives the information. Set the cbSize member of this structure and the uiParam parameter
        /// to sizeof(ACCESSTIMEOUT).
        /// </summary>
        SPI_GETACCESSTIMEOUT = 0x003C,

        /// <summary>
        /// Sets the time-out period associated with the accessibility features. The pvParam parameter must point to an ACCESSTIMEOUT
        /// structure that contains the new parameters. Set the cbSize member of this structure and the uiParam parameter to sizeof(ACCESSTIMEOUT).
        /// </summary>
        SPI_SETACCESSTIMEOUT = 0x003D,

        //#if(WINVER >= 0x0400)
        /// <summary>
        /// Windows Me/98/95:  Retrieves information about the SerialKeys accessibility feature. The pvParam parameter must point
        /// to a SERIALKEYS structure that receives the information. Set the cbSize member of this structure and the uiParam parameter
        /// to sizeof(SERIALKEYS).
        /// Windows Server 2003, Windows XP/2000/NT:  Not supported. The user controls this feature through the control panel.
        /// </summary>
        SPI_GETSERIALKEYS = 0x003E,

        /// <summary>
        /// Windows Me/98/95:  Sets the parameters of the SerialKeys accessibility feature. The pvParam parameter must point
        /// to a SERIALKEYS structure that contains the new parameters. Set the cbSize member of this structure and the uiParam parameter
        /// to sizeof(SERIALKEYS).
        /// Windows Server 2003, Windows XP/2000/NT:  Not supported. The user controls this feature through the control panel.
        /// </summary>
        SPI_SETSERIALKEYS = 0x003F,
        //#endif /* WINVER >= 0x0400 */

        /// <summary>
        /// Retrieves information about the SoundSentry accessibility feature. The pvParam parameter must point to a SOUNDSENTRY structure
        /// that receives the information. Set the cbSize member of this structure and the uiParam parameter to sizeof(SOUNDSENTRY).
        /// </summary>
        SPI_GETSOUNDSENTRY = 0x0040,

        /// <summary>
        /// Sets the parameters of the SoundSentry accessibility feature. The pvParam parameter must point to a SOUNDSENTRY structure
        /// that contains the new parameters. Set the cbSize member of this structure and the uiParam parameter to sizeof(SOUNDSENTRY).
        /// </summary>
        SPI_SETSOUNDSENTRY = 0x0041,

        //#if(_WIN32_WINNT >= 0x0400)
        /// <summary>
        /// Determines whether the snap-to-default-button feature is enabled. If enabled, the mouse cursor automatically moves
        /// to the default button, such as OK or Apply, of a dialog box. The pvParam parameter must point to a BOOL variable
        /// that receives TRUE if the feature is on, or FALSE if it is off.
        /// Windows 95:  Not supported.
        /// </summary>
        SPI_GETSNAPTODEFBUTTON = 0x005F,

        /// <summary>
        /// Enables or disables the snap-to-default-button feature. If enabled, the mouse cursor automatically moves to the default button,
        /// such as OK or Apply, of a dialog box. Set the uiParam parameter to TRUE to enable the feature, or FALSE to disable it.
        /// Applications should use the ShowWindow function when displaying a dialog box so the dialog manager can position the mouse cursor.
        /// Windows 95:  Not supported.
        /// </summary>
        SPI_SETSNAPTODEFBUTTON = 0x0060,
        //#endif /* _WIN32_WINNT >= 0x0400 */

        //#if (_WIN32_WINNT >= 0x0400) || (_WIN32_WINDOWS > 0x0400)
        /// <summary>
        /// Retrieves the width, in pixels, of the rectangle within which the mouse pointer has to stay for TrackMouseEvent
        /// to generate a WM_MOUSEHOVER message. The pvParam parameter must point to a UINT variable that receives the width.
        /// Windows 95:  Not supported.
        /// </summary>
        SPI_GETMOUSEHOVERWIDTH = 0x0062,

        /// <summary>
        /// Retrieves the width, in pixels, of the rectangle within which the mouse pointer has to stay for TrackMouseEvent
        /// to generate a WM_MOUSEHOVER message. The pvParam parameter must point to a UINT variable that receives the width.
        /// Windows 95:  Not supported.
        /// </summary>
        SPI_SETMOUSEHOVERWIDTH = 0x0063,

        /// <summary>
        /// Retrieves the height, in pixels, of the rectangle within which the mouse pointer has to stay for TrackMouseEvent
        /// to generate a WM_MOUSEHOVER message. The pvParam parameter must point to a UINT variable that receives the height.
        /// Windows 95:  Not supported.
        /// </summary>
        SPI_GETMOUSEHOVERHEIGHT = 0x0064,

        /// <summary>
        /// Sets the height, in pixels, of the rectangle within which the mouse pointer has to stay for TrackMouseEvent
        /// to generate a WM_MOUSEHOVER message. Set the uiParam parameter to the new height.
        /// Windows 95:  Not supported.
        /// </summary>
        SPI_SETMOUSEHOVERHEIGHT = 0x0065,

        /// <summary>
        /// Retrieves the time, in milliseconds, that the mouse pointer has to stay in the hover rectangle for TrackMouseEvent
        /// to generate a WM_MOUSEHOVER message. The pvParam parameter must point to a UINT variable that receives the time.
        /// Windows 95:  Not supported.
        /// </summary>
        SPI_GETMOUSEHOVERTIME = 0x0066,

        /// <summary>
        /// Sets the time, in milliseconds, that the mouse pointer has to stay in the hover rectangle for TrackMouseEvent
        /// to generate a WM_MOUSEHOVER message. This is used only if you pass HOVER_DEFAULT in the dwHoverTime parameter in the call to TrackMouseEvent. Set the uiParam parameter to the new time.
        /// Windows 95:  Not supported.
        /// </summary>
        SPI_SETMOUSEHOVERTIME = 0x0067,

        /// <summary>
        /// Retrieves the number of lines to scroll when the mouse wheel is rotated. The pvParam parameter must point
        /// to a UINT variable that receives the number of lines. The default value is 3.
        /// Windows 95:  Not supported.
        /// </summary>
        SPI_GETWHEELSCROLLLINES = 0x0068,

        /// <summary>
        /// Sets the number of lines to scroll when the mouse wheel is rotated. The number of lines is set from the uiParam parameter.
        /// The number of lines is the suggested number of lines to scroll when the mouse wheel is rolled without using modifier keys.
        /// If the number is 0, then no scrolling should occur. If the number of lines to scroll is greater than the number of lines viewable,
        /// and in particular if it is WHEEL_PAGESCROLL (#defined as UINT_MAX), the scroll operation should be interpreted
        /// as clicking once in the page down or page up regions of the scroll bar.
        /// Windows 95:  Not supported.
        /// </summary>
        SPI_SETWHEELSCROLLLINES = 0x0069,

        /// <summary>
        /// Retrieves the time, in milliseconds, that the system waits before displaying a shortcut menu when the mouse cursor is
        /// over a submenu item. The pvParam parameter must point to a DWORD variable that receives the time of the delay.
        /// Windows 95:  Not supported.
        /// </summary>
        SPI_GETMENUSHOWDELAY = 0x006A,

        /// <summary>
        /// Sets uiParam to the time, in milliseconds, that the system waits before displaying a shortcut menu when the mouse cursor is
        /// over a submenu item.
        /// Windows 95:  Not supported.
        /// </summary>
        SPI_SETMENUSHOWDELAY = 0x006B,

        /// <summary>
        /// Determines whether the IME status window is visible (on a per-user basis). The pvParam parameter must point to a BOOL variable
        /// that receives TRUE if the status window is visible, or FALSE if it is not.
        /// Windows NT, Windows 95:  This value is not supported.
        /// </summary>
        SPI_GETSHOWIMEUI = 0x006E,

        /// <summary>
        /// Sets whether the IME status window is visible or not on a per-user basis. The uiParam parameter specifies TRUE for on or FALSE for off.
        /// Windows NT, Windows 95:  This value is not supported.
        /// </summary>
        SPI_SETSHOWIMEUI = 0x006F,
        //#endif

        //#if(WINVER >= 0x0500)
        /// <summary>
        /// Retrieves the current mouse speed. The mouse speed determines how far the pointer will move based on the distance the mouse moves.
        /// The pvParam parameter must point to an integer that receives a value which ranges between 1 (slowest) and 20 (fastest).
        /// A value of 10 is the default. The value can be set by an end user using the mouse control panel application or
        /// by an application using SPI_SETMOUSESPEED.
        /// Windows NT, Windows 95:  This value is not supported.
        /// </summary>
        SPI_GETMOUSESPEED = 0x0070,

        /// <summary>
        /// Sets the current mouse speed. The pvParam parameter is an integer between 1 (slowest) and 20 (fastest). A value of 10 is the default.
        /// This value is typically set using the mouse control panel application.
        /// Windows NT, Windows 95:  This value is not supported.
        /// </summary>
        SPI_SETMOUSESPEED = 0x0071,

        /// <summary>
        /// Determines whether a screen saver is currently running on the window station of the calling process.
        /// The pvParam parameter must point to a BOOL variable that receives TRUE if a screen saver is currently running, or FALSE otherwise.
        /// Note that only the interactive window station, "WinSta0", can have a screen saver running.
        /// Windows NT, Windows 95:  This value is not supported.
        /// </summary>
        SPI_GETSCREENSAVERRUNNING = 0x0072,

        /// <summary>
        /// Retrieves the full path of the bitmap file for the desktop wallpaper. The pvParam parameter must point to a buffer
        /// that receives a null-terminated path string. Set the uiParam parameter to the size, in characters, of the pvParam buffer. The returned string will not exceed MAX_PATH characters. If there is no desktop wallpaper, the returned string is empty.
        /// Windows NT, Windows Me/98/95:  This value is not supported.
        /// </summary>
        SPI_GETDESKWALLPAPER = 0x0073,
        //#endif /* WINVER >= 0x0500 */

        //#if(WINVER >= 0x0500)
        /// <summary>
        /// Determines whether active window tracking (activating the window the mouse is on) is on or off. The pvParam parameter must point
        /// to a BOOL variable that receives TRUE for on, or FALSE for off.
        /// Windows NT, Windows 95:  This value is not supported.
        /// </summary>
        SPI_GETACTIVEWINDOWTRACKING = 0x1000,

        /// <summary>
        /// Sets active window tracking (activating the window the mouse is on) either on or off. Set pvParam to TRUE for on or FALSE for off.
        /// Windows NT, Windows 95:  This value is not supported.
        /// </summary>
        SPI_SETACTIVEWINDOWTRACKING = 0x1001,

        /// <summary>
        /// Determines whether the menu animation feature is enabled. This master switch must be on to enable menu animation effects.
        /// The pvParam parameter must point to a BOOL variable that receives TRUE if animation is enabled and FALSE if it is disabled.
        /// If animation is enabled, SPI_GETMENUFADE indicates whether menus use fade or slide animation.
        /// Windows NT, Windows 95:  This value is not supported.
        /// </summary>
        SPI_GETMENUANIMATION = 0x1002,

        /// <summary>
        /// Enables or disables menu animation. This master switch must be on for any menu animation to occur.
        /// The pvParam parameter is a BOOL variable; set pvParam to TRUE to enable animation and FALSE to disable animation.
        /// If animation is enabled, SPI_GETMENUFADE indicates whether menus use fade or slide animation.
        /// Windows NT, Windows 95:  This value is not supported.
        /// </summary>
        SPI_SETMENUANIMATION = 0x1003,

        /// <summary>
        /// Determines whether the slide-open effect for combo boxes is enabled. The pvParam parameter must point to a BOOL variable
        /// that receives TRUE for enabled, or FALSE for disabled.
        /// Windows NT, Windows 95:  This value is not supported.
        /// </summary>
        SPI_GETCOMBOBOXANIMATION = 0x1004,

        /// <summary>
        /// Enables or disables the slide-open effect for combo boxes. Set the pvParam parameter to TRUE to enable the gradient effect,
        /// or FALSE to disable it.
        /// Windows NT, Windows 95:  This value is not supported.
        /// </summary>
        SPI_SETCOMBOBOXANIMATION = 0x1005,

        /// <summary>
        /// Determines whether the smooth-scrolling effect for list boxes is enabled. The pvParam parameter must point to a BOOL variable
        /// that receives TRUE for enabled, or FALSE for disabled.
        /// Windows NT, Windows 95:  This value is not supported.
        /// </summary>
        SPI_GETLISTBOXSMOOTHSCROLLING = 0x1006,

        /// <summary>
        /// Enables or disables the smooth-scrolling effect for list boxes. Set the pvParam parameter to TRUE to enable the smooth-scrolling effect,
        /// or FALSE to disable it.
        /// Windows NT, Windows 95:  This value is not supported.
        /// </summary>
        SPI_SETLISTBOXSMOOTHSCROLLING = 0x1007,

        /// <summary>
        /// Determines whether the gradient effect for window title bars is enabled. The pvParam parameter must point to a BOOL variable
        /// that receives TRUE for enabled, or FALSE for disabled. For more information about the gradient effect, see the GetSysColor function.
        /// Windows NT, Windows 95:  This value is not supported.
        /// </summary>
        SPI_GETGRADIENTCAPTIONS = 0x1008,

        /// <summary>
        /// Enables or disables the gradient effect for window title bars. Set the pvParam parameter to TRUE to enable it, or FALSE to disable it.
        /// The gradient effect is possible only if the system has a color depth of more than 256 colors. For more information about
        /// the gradient effect, see the GetSysColor function.
        /// Windows NT, Windows 95:  This value is not supported.
        /// </summary>
        SPI_SETGRADIENTCAPTIONS = 0x1009,

        /// <summary>
        /// Determines whether menu access keys are always underlined. The pvParam parameter must point to a BOOL variable that receives TRUE
        /// if menu access keys are always underlined, and FALSE if they are underlined only when the menu is activated by the keyboard.
        /// Windows NT, Windows 95:  This value is not supported.
        /// </summary>
        SPI_GETKEYBOARDCUES = 0x100A,

        /// <summary>
        /// Sets the underlining of menu access key letters. The pvParam parameter is a BOOL variable. Set pvParam to TRUE to always underline menu
        /// access keys, or FALSE to underline menu access keys only when the menu is activated from the keyboard.
        /// Windows NT, Windows 95:  This value is not supported.
        /// </summary>
        SPI_SETKEYBOARDCUES = 0x100B,

        /// <summary>
        /// Same as SPI_GETKEYBOARDCUES.
        /// </summary>
        SPI_GETMENUUNDERLINES = SPI_GETKEYBOARDCUES,

        /// <summary>
        /// Same as SPI_SETKEYBOARDCUES.
        /// </summary>
        SPI_SETMENUUNDERLINES = SPI_SETKEYBOARDCUES,

        /// <summary>
        /// Determines whether windows activated through active window tracking will be brought to the top. The pvParam parameter must point
        /// to a BOOL variable that receives TRUE for on, or FALSE for off.
        /// Windows NT, Windows 95:  This value is not supported.
        /// </summary>
        SPI_GETACTIVEWNDTRKZORDER = 0x100C,

        /// <summary>
        /// Determines whether or not windows activated through active window tracking should be brought to the top. Set pvParam to TRUE
        /// for on or FALSE for off.
        /// Windows NT, Windows 95:  This value is not supported.
        /// </summary>
        SPI_SETACTIVEWNDTRKZORDER = 0x100D,

        /// <summary>
        /// Determines whether hot tracking of user-interface elements, such as menu names on menu bars, is enabled. The pvParam parameter
        /// must point to a BOOL variable that receives TRUE for enabled, or FALSE for disabled.
        /// Hot tracking means that when the cursor moves over an item, it is highlighted but not selected. You can query this value to decide
        /// whether to use hot tracking in the user interface of your application.
        /// Windows NT, Windows 95:  This value is not supported.
        /// </summary>
        SPI_GETHOTTRACKING = 0x100E,

        /// <summary>
        /// Enables or disables hot tracking of user-interface elements such as menu names on menu bars. Set the pvParam parameter to TRUE
        /// to enable it, or FALSE to disable it.
        /// Hot-tracking means that when the cursor moves over an item, it is highlighted but not selected.
        /// Windows NT, Windows 95:  This value is not supported.
        /// </summary>
        SPI_SETHOTTRACKING = 0x100F,

        /// <summary>
        /// Determines whether menu fade animation is enabled. The pvParam parameter must point to a BOOL variable that receives TRUE
        /// when fade animation is enabled and FALSE when it is disabled. If fade animation is disabled, menus use slide animation.
        /// This flag is ignored unless menu animation is enabled, which you can do using the SPI_SETMENUANIMATION flag.
        /// For more information, see AnimateWindow.
        /// Windows NT, Windows Me/98/95:  This value is not supported.
        /// </summary>
        SPI_GETMENUFADE = 0x1012,

        /// <summary>
        /// Enables or disables menu fade animation. Set pvParam to TRUE to enable the menu fade effect or FALSE to disable it.
        /// If fade animation is disabled, menus use slide animation. he The menu fade effect is possible only if the system
        /// has a color depth of more than 256 colors. This flag is ignored unless SPI_MENUANIMATION is also set. For more information,
        /// see AnimateWindow.
        /// Windows NT, Windows Me/98/95:  This value is not supported.
        /// </summary>
        SPI_SETMENUFADE = 0x1013,

        /// <summary>
        /// Determines whether the selection fade effect is enabled. The pvParam parameter must point to a BOOL variable that receives TRUE
        /// if enabled or FALSE if disabled.
        /// The selection fade effect causes the menu item selected by the user to remain on the screen briefly while fading out
        /// after the menu is dismissed.
        /// Windows NT, Windows Me/98/95:  This value is not supported.
        /// </summary>
        SPI_GETSELECTIONFADE = 0x1014,

        /// <summary>
        /// Set pvParam to TRUE to enable the selection fade effect or FALSE to disable it.
        /// The selection fade effect causes the menu item selected by the user to remain on the screen briefly while fading out
        /// after the menu is dismissed. The selection fade effect is possible only if the system has a color depth of more than 256 colors.
        /// Windows NT, Windows Me/98/95:  This value is not supported.
        /// </summary>
        SPI_SETSELECTIONFADE = 0x1015,

        /// <summary>
        /// Determines whether ToolTip animation is enabled. The pvParam parameter must point to a BOOL variable that receives TRUE
        /// if enabled or FALSE if disabled. If ToolTip animation is enabled, SPI_GETTOOLTIPFADE indicates whether ToolTips use fade or slide animation.
        /// Windows NT, Windows Me/98/95:  This value is not supported.
        /// </summary>
        SPI_GETTOOLTIPANIMATION = 0x1016,

        /// <summary>
        /// Set pvParam to TRUE to enable ToolTip animation or FALSE to disable it. If enabled, you can use SPI_SETTOOLTIPFADE
        /// to specify fade or slide animation.
        /// Windows NT, Windows Me/98/95:  This value is not supported.
        /// </summary>
        SPI_SETTOOLTIPANIMATION = 0x1017,

        /// <summary>
        /// If SPI_SETTOOLTIPANIMATION is enabled, SPI_GETTOOLTIPFADE indicates whether ToolTip animation uses a fade effect or a slide effect.
        ///  The pvParam parameter must point to a BOOL variable that receives TRUE for fade animation or FALSE for slide animation.
        ///  For more information on slide and fade effects, see AnimateWindow.
        /// Windows NT, Windows Me/98/95:  This value is not supported.
        /// </summary>
        SPI_GETTOOLTIPFADE = 0x1018,

        /// <summary>
        /// If the SPI_SETTOOLTIPANIMATION flag is enabled, use SPI_SETTOOLTIPFADE to indicate whether ToolTip animation uses a fade effect
        /// or a slide effect. Set pvParam to TRUE for fade animation or FALSE for slide animation. The tooltip fade effect is possible only
        /// if the system has a color depth of more than 256 colors. For more information on the slide and fade effects,
        /// see the AnimateWindow function.
        /// Windows NT, Windows Me/98/95:  This value is not supported.
        /// </summary>
        SPI_SETTOOLTIPFADE = 0x1019,

        /// <summary>
        /// Determines whether the cursor has a shadow around it. The pvParam parameter must point to a BOOL variable that receives TRUE
        /// if the shadow is enabled, FALSE if it is disabled. This effect appears only if the system has a color depth of more than 256 colors.
        /// Windows NT, Windows Me/98/95:  This value is not supported.
        /// </summary>
        SPI_GETCURSORSHADOW = 0x101A,

        /// <summary>
        /// Enables or disables a shadow around the cursor. The pvParam parameter is a BOOL variable. Set pvParam to TRUE to enable the shadow
        /// or FALSE to disable the shadow. This effect appears only if the system has a color depth of more than 256 colors.
        /// Windows NT, Windows Me/98/95:  This value is not supported.
        /// </summary>
        SPI_SETCURSORSHADOW = 0x101B,

        //#if(_WIN32_WINNT >= 0x0501)
        /// <summary>
        /// Retrieves the state of the Mouse Sonar feature. The pvParam parameter must point to a BOOL variable that receives TRUE
        /// if enabled or FALSE otherwise. For more information, see About Mouse Input on MSDN.
        /// Windows 2000/NT, Windows 98/95:  This value is not supported.
        /// </summary>
        SPI_GETMOUSESONAR = 0x101C,

        /// <summary>
        /// Turns the Sonar accessibility feature on or off. This feature briefly shows several concentric circles around the mouse pointer
        /// when the user presses and releases the CTRL key. The pvParam parameter specifies TRUE for on and FALSE for off. The default is off.
        /// For more information, see About Mouse Input.
        /// Windows 2000/NT, Windows 98/95:  This value is not supported.
        /// </summary>
        SPI_SETMOUSESONAR = 0x101D,

        /// <summary>
        /// Retrieves the state of the Mouse ClickLock feature. The pvParam parameter must point to a BOOL variable that receives TRUE
        /// if enabled, or FALSE otherwise. For more information, see About Mouse Input.
        /// Windows 2000/NT, Windows 98/95:  This value is not supported.
        /// </summary>
        SPI_GETMOUSECLICKLOCK = 0x101E,

        /// <summary>
        /// Turns the Mouse ClickLock accessibility feature on or off. This feature temporarily locks down the primary mouse button
        /// when that button is clicked and held down for the time specified by SPI_SETMOUSECLICKLOCKTIME. The uiParam parameter specifies
        /// TRUE for on,
        /// or FALSE for off. The default is off. For more information, see Remarks and About Mouse Input on MSDN.
        /// Windows 2000/NT, Windows 98/95:  This value is not supported.
        /// </summary>
        SPI_SETMOUSECLICKLOCK = 0x101F,

        /// <summary>
        /// Retrieves the state of the Mouse Vanish feature. The pvParam parameter must point to a BOOL variable that receives TRUE
        /// if enabled or FALSE otherwise. For more information, see About Mouse Input on MSDN.
        /// Windows 2000/NT, Windows 98/95:  This value is not supported.
        /// </summary>
        SPI_GETMOUSEVANISH = 0x1020,

        /// <summary>
        /// Turns the Vanish feature on or off. This feature hides the mouse pointer when the user types; the pointer reappears
        /// when the user moves the mouse. The pvParam parameter specifies TRUE for on and FALSE for off. The default is off.
        /// For more information, see About Mouse Input on MSDN.
        /// Windows 2000/NT, Windows 98/95:  This value is not supported.
        /// </summary>
        SPI_SETMOUSEVANISH = 0x1021,

        /// <summary>
        /// Determines whether native User menus have flat menu appearance. The pvParam parameter must point to a BOOL variable
        /// that returns TRUE if the flat menu appearance is set, or FALSE otherwise.
        /// Windows 2000/NT, Windows Me/98/95:  This value is not supported.
        /// </summary>
        SPI_GETFLATMENU = 0x1022,

        /// <summary>
        /// Enables or disables flat menu appearance for native User menus. Set pvParam to TRUE to enable flat menu appearance
        /// or FALSE to disable it.
        /// When enabled, the menu bar uses COLOR_MENUBAR for the menubar background, COLOR_MENU for the menu-popup background, COLOR_MENUHILIGHT
        /// for the fill of the current menu selection, and COLOR_HILIGHT for the outline of the current menu selection.
        /// If disabled, menus are drawn using the same metrics and colors as in Windows 2000 and earlier.
        /// Windows 2000/NT, Windows Me/98/95:  This value is not supported.
        /// </summary>
        SPI_SETFLATMENU = 0x1023,

        /// <summary>
        /// Determines whether the drop shadow effect is enabled. The pvParam parameter must point to a BOOL variable that returns TRUE
        /// if enabled or FALSE if disabled.
        /// Windows 2000/NT, Windows Me/98/95:  This value is not supported.
        /// </summary>
        SPI_GETDROPSHADOW = 0x1024,

        /// <summary>
        /// Enables or disables the drop shadow effect. Set pvParam to TRUE to enable the drop shadow effect or FALSE to disable it.
        /// You must also have CS_DROPSHADOW in the window class style.
        /// Windows 2000/NT, Windows Me/98/95:  This value is not supported.
        /// </summary>
        SPI_SETDROPSHADOW = 0x1025,

        /// <summary>
        /// Retrieves a BOOL indicating whether an application can reset the screensaver's timer by calling the SendInput function
        /// to simulate keyboard or mouse input. The pvParam parameter must point to a BOOL variable that receives TRUE
        /// if the simulated input will be blocked, or FALSE otherwise.
        /// </summary>
        SPI_GETBLOCKSENDINPUTRESETS = 0x1026,

        /// <summary>
        /// Determines whether an application can reset the screensaver's timer by calling the SendInput function to simulate keyboard
        /// or mouse input. The uiParam parameter specifies TRUE if the screensaver will not be deactivated by simulated input,
        /// or FALSE if the screensaver will be deactivated by simulated input.
        /// </summary>
        SPI_SETBLOCKSENDINPUTRESETS = 0x1027,
        //#endif /* _WIN32_WINNT >= 0x0501 */

        /// <summary>
        /// Determines whether UI effects are enabled or disabled. The pvParam parameter must point to a BOOL variable that receives TRUE
        /// if all UI effects are enabled, or FALSE if they are disabled.
        /// Windows NT, Windows Me/98/95:  This value is not supported.
        /// </summary>
        SPI_GETUIEFFECTS = 0x103E,

        /// <summary>
        /// Enables or disables UI effects. Set the pvParam parameter to TRUE to enable all UI effects or FALSE to disable all UI effects.
        /// Windows NT, Windows Me/98/95:  This value is not supported.
        /// </summary>
        SPI_SETUIEFFECTS = 0x103F,

        /// <summary>
        /// Retrieves the amount of time following user input, in milliseconds, during which the system will not allow applications
        /// to force themselves into the foreground. The pvParam parameter must point to a DWORD variable that receives the time.
        /// Windows NT, Windows 95:  This value is not supported.
        /// </summary>
        SPI_GETFOREGROUNDLOCKTIMEOUT = 0x2000,

        /// <summary>
        /// Sets the amount of time following user input, in milliseconds, during which the system does not allow applications
        /// to force themselves into the foreground. Set pvParam to the new timeout value.
        /// The calling thread must be able to change the foreground window, otherwise the call fails.
        /// Windows NT, Windows 95:  This value is not supported.
        /// </summary>
        SPI_SETFOREGROUNDLOCKTIMEOUT = 0x2001,

        /// <summary>
        /// Retrieves the active window tracking delay, in milliseconds. The pvParam parameter must point to a DWORD variable
        /// that receives the time.
        /// Windows NT, Windows 95:  This value is not supported.
        /// </summary>
        SPI_GETACTIVEWNDTRKTIMEOUT = 0x2002,

        /// <summary>
        /// Sets the active window tracking delay. Set pvParam to the number of milliseconds to delay before activating the window
        /// under the mouse pointer.
        /// Windows NT, Windows 95:  This value is not supported.
        /// </summary>
        SPI_SETACTIVEWNDTRKTIMEOUT = 0x2003,

        /// <summary>
        /// Retrieves the number of times SetForegroundWindow will flash the taskbar button when rejecting a foreground switch request.
        /// The pvParam parameter must point to a DWORD variable that receives the value.
        /// Windows NT, Windows 95:  This value is not supported.
        /// </summary>
        SPI_GETFOREGROUNDFLASHCOUNT = 0x2004,

        /// <summary>
        /// Sets the number of times SetForegroundWindow will flash the taskbar button when rejecting a foreground switch request.
        /// Set pvParam to the number of times to flash.
        /// Windows NT, Windows 95:  This value is not supported.
        /// </summary>
        SPI_SETFOREGROUNDFLASHCOUNT = 0x2005,

        /// <summary>
        /// Retrieves the caret width in edit controls, in pixels. The pvParam parameter must point to a DWORD that receives this value.
        /// Windows NT, Windows Me/98/95:  This value is not supported.
        /// </summary>
        SPI_GETCARETWIDTH = 0x2006,

        /// <summary>
        /// Sets the caret width in edit controls. Set pvParam to the desired width, in pixels. The default and minimum value is 1.
        /// Windows NT, Windows Me/98/95:  This value is not supported.
        /// </summary>
        SPI_SETCARETWIDTH = 0x2007,

        //#if(_WIN32_WINNT >= 0x0501)
        /// <summary>
        /// Retrieves the time delay before the primary mouse button is locked. The pvParam parameter must point to DWORD that receives
        /// the time delay. This is only enabled if SPI_SETMOUSECLICKLOCK is set to TRUE. For more information, see About Mouse Input on MSDN.
        /// Windows 2000/NT, Windows 98/95:  This value is not supported.
        /// </summary>
        SPI_GETMOUSECLICKLOCKTIME = 0x2008,

        /// <summary>
        /// Turns the Mouse ClickLock accessibility feature on or off. This feature temporarily locks down the primary mouse button
        /// when that button is clicked and held down for the time specified by SPI_SETMOUSECLICKLOCKTIME. The uiParam parameter
        /// specifies TRUE for on, or FALSE for off. The default is off. For more information, see Remarks and About Mouse Input on MSDN.
        /// Windows 2000/NT, Windows 98/95:  This value is not supported.
        /// </summary>
        SPI_SETMOUSECLICKLOCKTIME = 0x2009,

        /// <summary>
        /// Retrieves the type of font smoothing. The pvParam parameter must point to a UINT that receives the information.
        /// Windows 2000/NT, Windows Me/98/95:  This value is not supported.
        /// </summary>
        SPI_GETFONTSMOOTHINGTYPE = 0x200A,

        /// <summary>
        /// Sets the font smoothing type. The pvParam parameter points to a UINT that contains either FE_FONTSMOOTHINGSTANDARD,
        /// if standard anti-aliasing is used, or FE_FONTSMOOTHINGCLEARTYPE, if ClearType is used. The default is FE_FONTSMOOTHINGSTANDARD.
        /// When using this option, the fWinIni parameter must be set to SPIF_SENDWININICHANGE | SPIF_UPDATEINIFILE; otherwise,
        /// SystemParametersInfo fails.
        /// </summary>
        SPI_SETFONTSMOOTHINGTYPE = 0x200B,

        /// <summary>
        /// Retrieves a contrast value that is used in ClearType™ smoothing. The pvParam parameter must point to a UINT
        /// that receives the information.
        /// Windows 2000/NT, Windows Me/98/95:  This value is not supported.
        /// </summary>
        SPI_GETFONTSMOOTHINGCONTRAST = 0x200C,

        /// <summary>
        /// Sets the contrast value used in ClearType smoothing. The pvParam parameter points to a UINT that holds the contrast value.
        /// Valid contrast values are from 1000 to 2200. The default value is 1400.
        /// When using this option, the fWinIni parameter must be set to SPIF_SENDWININICHANGE | SPIF_UPDATEINIFILE; otherwise,
        /// SystemParametersInfo fails.
        /// SPI_SETFONTSMOOTHINGTYPE must also be set to FE_FONTSMOOTHINGCLEARTYPE.
        /// Windows 2000/NT, Windows Me/98/95:  This value is not supported.
        /// </summary>
        SPI_SETFONTSMOOTHINGCONTRAST = 0x200D,

        /// <summary>
        /// Retrieves the width, in pixels, of the left and right edges of the focus rectangle drawn with DrawFocusRect.
        /// The pvParam parameter must point to a UINT.
        /// Windows 2000/NT, Windows Me/98/95:  This value is not supported.
        /// </summary>
        SPI_GETFOCUSBORDERWIDTH = 0x200E,

        /// <summary>
        /// Sets the height of the left and right edges of the focus rectangle drawn with DrawFocusRect to the value of the pvParam parameter.
        /// Windows 2000/NT, Windows Me/98/95:  This value is not supported.
        /// </summary>
        SPI_SETFOCUSBORDERWIDTH = 0x200F,

        /// <summary>
        /// Retrieves the height, in pixels, of the top and bottom edges of the focus rectangle drawn with DrawFocusRect.
        /// The pvParam parameter must point to a UINT.
        /// Windows 2000/NT, Windows Me/98/95:  This value is not supported.
        /// </summary>
        SPI_GETFOCUSBORDERHEIGHT = 0x2010,

        /// <summary>
        /// Sets the height of the top and bottom edges of the focus rectangle drawn with DrawFocusRect to the value of the pvParam parameter.
        /// Windows 2000/NT, Windows Me/98/95:  This value is not supported.
        /// </summary>
        SPI_SETFOCUSBORDERHEIGHT = 0x2011,

        /// <summary>
        /// Not implemented.
        /// </summary>
        SPI_GETFONTSMOOTHINGORIENTATION = 0x2012,

        /// <summary>
        /// Not implemented.
        /// </summary>
        SPI_SETFONTSMOOTHINGORIENTATION = 0x2013,
    }
    internal enum AccentState
    {
        ACCENT_DISABLED = 0,
        ACCENT_ENABLE_GRADIENT = 1,
        ACCENT_ENABLE_TRANSPARENTGRADIENT = 2,
        ACCENT_ENABLE_BLURBEHIND = 3,
        ACCENT_ENABLE_ACRYLICBLURBEHIND = 4,
        ACCENT_INVALID_STATE = 5
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct AccentPolicy
    {
        public AccentState AccentState;
        public uint AccentFlags;
        public uint GradientColor;
        public uint AnimationId;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct WindowCompositionAttributeData
    {
        public WindowCompositionAttribute Attribute;
        public nint Data;
        public int SizeOfData;
    }

    internal enum WindowCompositionAttribute
    {
        // ...
        WCA_ACCENT_POLICY = 19
        // ...
    }
    /// <summary>
    /// The MONITORINFOEX structure contains information about a display monitor.
    /// The GetMonitorInfo function stores information into a MONITORINFOEX structure or a MONITORINFO structure.
    /// The MONITORINFOEX structure is a superset of the MONITORINFO structure. The MONITORINFOEX structure adds a string member to contain a name
    /// for the display monitor.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    internal struct MonitorInfoEx
    {

        /// <summary>
        /// The size, in bytes, of the structure. Set this member to sizeof(MONITORINFOEX) (72) before calling the GetMonitorInfo function.
        /// Doing so lets the function determine the type of structure you are passing to it.
        /// </summary>
        public int Size;

        /// <summary>
        /// A RECT structure that specifies the display monitor rectangle, expressed in virtual-screen coordinates.
        /// Note that if the monitor is not the primary display monitor, some of the rectangle's coordinates may be negative values.
        /// </summary>
        public RECT Monitor;

        /// <summary>
        /// A RECT structure that specifies the work area rectangle of the display monitor that can be used by applications,
        /// expressed in virtual-screen coordinates. Windows uses this rectangle to maximize an application on the monitor.
        /// The rest of the area in rcMonitor contains system windows such as the task bar and side bars.
        /// Note that if the monitor is not the primary display monitor, some of the rectangle's coordinates may be negative values.
        /// </summary>
        public RECT WorkArea;

        /// <summary>
        /// The attributes of the display monitor.
        ///
        /// This member can be the following value:
        ///   1 : MONITORINFOF_PRIMARY
        /// </summary>
        public uint Flags;

        /// <summary>
        /// A string that specifies the device name of the monitor being used. Most applications have no use for a display monitor name,
        /// and so can save some bytes by using a MONITORINFO structure.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string DeviceName;

        public void Init()
        {
            Size = 40 + 2 * 32;
            DeviceName = string.Empty;
        }
    }
    #endregion // SPI
}
