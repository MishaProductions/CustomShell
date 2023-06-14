using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CSShellManaged
{
    public static class Win32
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
        public const int WM_TIMER = 0x0113;
        public const int WM_HOTKEY = 0x0312;
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
        public static extern bool SetPropW(IntPtr hWnd, string lpString, IntPtr hData);
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.U2)]
        public static extern short RegisterClassExW([In] ref WNDCLASSEX lpwcx);
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr GetModuleHandle([MarshalAs(UnmanagedType.LPWStr)] string? lpModuleName);
        public delegate IntPtr Wndproc(IntPtr hwnd, uint msg, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr CreateWindowEx(uint dwExStyle, string lpClassName,
   string? lpWindowName, uint dwStyle, int x, int y, int nWidth, int nHeight,
   IntPtr hWndParent, IntPtr hMenu, IntPtr hInstance, IntPtr lpParam);
        [DllImport("user32.dll",SetLastError = true)]
        public static extern int GetSystemMetrics(int index);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ShowWindow(IntPtr hWnd, ShowWindowCommands nCmdShow);

        [DllImport("user32.dll")]
        public static extern IntPtr BeginPaint(IntPtr hwnd, out PAINTSTRUCT lpPaint);

        [DllImport("user32.dll")]
        public static extern IntPtr DefWindowProc(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll")]
        public static extern int DrawText(IntPtr hDC, string lpString, int nCount, ref RECT lpRect, uint uFormat);

        [DllImport("user32.dll")]
        public static extern bool EndPaint(IntPtr hWnd, [In] ref PAINTSTRUCT lpPaint);

        [DllImport("user32.dll")]
        public static extern void PostQuitMessage(int nExitCode);

        [DllImport("user32.dll")]
        public static extern IntPtr LoadIcon(IntPtr hInstance, string lpIconName);

        [DllImport("user32.dll")]
        public static extern IntPtr LoadIcon(IntPtr hInstance, IntPtr lpIConName);

     

        [DllImport("user32.dll")]
        public static extern MessageBoxResult MessageBox(IntPtr hWnd, string text, string caption, int options);

        [DllImport("user32.dll")]
        public static extern bool UpdateWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern IntPtr LoadCursor(IntPtr hInstance, int lpCursorName);

        [DllImport("user32.dll")]
        public static extern int SetShellWindow(IntPtr handle);
        [DllImport("user32.dll")]
        public static extern IntPtr GetShellWindow();
        [DllImport("user32.dll")]
        public static extern int SetTaskmanWindow(IntPtr handle);
        [DllImport("user32.dll")]
        public static extern IntPtr GetTaskmanWindow();
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr RemovePropW(IntPtr hWnd, string lpString);

        [DllImport("Shlwapi.dll")]
        public static extern int SHCreateThreadRef(ref long pcRef, out IntPtr iUnknown);
        [DllImport("Shlwapi.dll")]
        public static extern int SHSetThreadRef(IntPtr iUnknown);
        [DllImport("user32.dll")]
        public static extern int FillRect(IntPtr hDC, [In] ref RECT lprc, IntPtr hbr);
        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateSolidBrush(uint crColor);
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern uint RegisterWindowMessage(string lpString);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool RegisterShellHookWindow(IntPtr hWnd);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool DeregisterShellHookWindow(IntPtr hWnd);
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct WNDCLASSEX
    {
        [MarshalAs(UnmanagedType.U4)]
        public int cbSize;
        [MarshalAs(UnmanagedType.U4)]
        public int style;
        public IntPtr lpfnWndProc; // not WndProc
        public int cbClsExtra;
        public int cbWndExtra;
        public IntPtr hInstance;
        public IntPtr hIcon;
        public IntPtr hCursor;
        public IntPtr hbrBackground;
        public string lpszMenuName;
        public string lpszClassName;
        public IntPtr hIconSm;

        //Use this function to make a new one with cbSize already filled in.
        //For example:
        //var WndClss = WNDCLASSEX.Build()
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
        public int Left, Top, Right, Bottom;

        public RECT(int left, int top, int right, int bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        public RECT(System.Drawing.Rectangle r) : this(r.Left, r.Top, r.Right, r.Bottom) { }

        public int X
        {
            get { return Left; }
            set { Right -= (Left - value); Left = value; }
        }

        public int Y
        {
            get { return Top; }
            set { Bottom -= (Top - value); Top = value; }
        }

        public int Height
        {
            get { return Bottom - Top; }
            set { Bottom = value + Top; }
        }

        public int Width
        {
            get { return Right - Left; }
            set { Right = value + Left; }
        }

        public System.Drawing.Point Location
        {
            get { return new System.Drawing.Point(Left, Top); }
            set { X = value.X; Y = value.Y; }
        }

        public System.Drawing.Size Size
        {
            get { return new System.Drawing.Size(Width, Height); }
            set { Width = value.Width; Height = value.Height; }
        }

        public static implicit operator System.Drawing.Rectangle(RECT r)
        {
            return new System.Drawing.Rectangle(r.Left, r.Top, r.Width, r.Height);
        }

        public static implicit operator RECT(System.Drawing.Rectangle r)
        {
            return new RECT(r);
        }

        public static bool operator ==(RECT r1, RECT r2)
        {
            return r1.Equals(r2);
        }

        public static bool operator !=(RECT r1, RECT r2)
        {
            return !r1.Equals(r2);
        }

        public bool Equals(RECT r)
        {
            return r.Left == Left && r.Top == Top && r.Right == Right && r.Bottom == Bottom;
        }

        public override bool Equals(object obj)
        {
            if (obj is RECT)
                return Equals((RECT)obj);
            else if (obj is System.Drawing.Rectangle)
                return Equals(new RECT((System.Drawing.Rectangle)obj));
            return false;
        }

        public override int GetHashCode()
        {
            return ((System.Drawing.Rectangle)this).GetHashCode();
        }

        public override string ToString()
        {
            return string.Format(System.Globalization.CultureInfo.CurrentCulture, "{{Left={0},Top={1},Right={2},Bottom={3}}}", Left, Top, Right, Bottom);
        }
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct PAINTSTRUCT
    {
        public IntPtr hdc;
        public bool fErase;
        public RECT rcPaint;
        public bool fRestore;
        public bool fIncUpdate;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] rgbReserved;
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

}
