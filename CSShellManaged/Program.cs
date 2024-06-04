using CSShellManaged.Win32;
using PlayHooky;
using System.ComponentModel;
using System.Runtime.InteropServices;
using static CSShellManaged.Win32.Win32Defs;

namespace CSShellManaged
{
    public class Program
    {
        static long ProgManPCRef = 0;
        static nint ProgManThreadRef;
        static uint ShellHook = 0;
        static Guid SID_ImmersiveShellHookService = new Guid("4624bd39-5fc3-44a8-a809-163a836e9031");
        static Guid ImmersiveShellHookServiceInterface = new Guid("914d9b3a-5e53-4e14-bbba-46062acb35a4");
        static IImmersiveShellHookService? HookService;
        public static IntPtr Progmanhwnd;
        public static int Main(IntPtr args, int sizeBytes)
        {
            try
            {
                Console.WriteLine("Hello from .NET");
                Application.SetCompatibleTextRenderingDefault(false);
                Application.EnableVisualStyles();

                DoExplorerInit();

                if (SHCreateThreadRef(ref ProgManPCRef, out ProgManThreadRef) >= 0)
                {
                    if (SHSetThreadRef(ProgManThreadRef) < 0)
                    {
                        Console.WriteLine("SHSetThreadRef failed");
                    }
                    else
                    {
                        SetProcessReference(ProgManThreadRef);
                    }
                }
                else
                {
                    Console.WriteLine("SHCreateThreadRef failed");
                }

                //We need to hook the NativeWindow.GetFullClassName() function as windows forms has a random class name
                WindowClassHook.Install();

                //create tray
                WindowClassHook.ClassName = "Shell_TrayWnd";
                var tray = new Shell_TrayWnd();
                tray.Show();
                WindowClassHook.ClassName = "unknown";

                //Create the desktop
                WindowClassHook.ClassName = "Progman";
                var desktop = new ProgManWindow();
                desktop.CreateControl();
               
                WindowClassHook.ClassName = "unknown23";

                //create taskman class (handles taskbar buttons)
                CreateTaskman();

                Application.DoEvents();
               

                StartExplorerHost();

                PostMessageW(tray.Handle, 1424, 1, 0); //?

                Application.Run(desktop);
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("exception: " + ex.ToString());
                Application.Run();
                return -1;
            }
        }
        private static void StartExplorerHost()
        {
            var host = (IExplorerHostCreator)new CExplorerHostCreator();
            var desktopexplorerhost = new Guid("682159d9-c321-47ca-b3f1-30e36b2ec8b9");
            if (host.CreateHost(ref desktopexplorerhost) != 0)
            {
                throw new Win32Exception();
            }
            //host.RunHost(); (doesn't seem to be called in explorer)
        }
        public static void StartImmersiveShell()
        {
            try
            {
                var builder = (IImmersiveShellBuilder)new CImmersiveShellBuilder();
                builder.CreateImmersiveShellController(out IImmersiveShellController controller);

                if (controller.Start() != 0)
                {
                    Console.WriteLine("!!!FAILED TO START IMMERSIVE SHELL!!!");
                }
            }
            catch { Console.WriteLine("!!!FAILED TO START IMMERSIVE SHELL!!!"); }
        }
        private static void CreateTaskman()
        {
            var taskmanclass = WNDCLASSEX.Build();
            taskmanclass.lpfnWndProc = Marshal.GetFunctionPointerForDelegate((Wndproc)TaskmanWndproc);
            taskmanclass.style = 8;
            taskmanclass.hInstance = GetModuleHandle(null);
            taskmanclass.lpszClassName = "TaskmanWndClass";
            taskmanclass.cbWndExtra = 8;
            taskmanclass.cbClsExtra = 0;
            taskmanclass.hbrBackground = new nint(2);
            taskmanclass.hCursor = LoadCursor(IntPtr.Zero, IDC_ARROW);

            if (RegisterClassExW(ref taskmanclass) == 0)
            {
                Console.WriteLine("[taskman] registerclassex failure: " + new Win32Exception(Marshal.GetLastWin32Error()).Message);
            }
            if (CreateWindowEx(128, "TaskmanWndClass", null, 0x82000000, GetSystemMetrics(SM_XVIRTUALSCREEN), GetSystemMetrics(SM_YVIRTUALSCREEN), GetSystemMetrics(SM_CXVIRTUALSCREEN), GetSystemMetrics(SM_CYVIRTUALSCREEN), 0, 0, taskmanclass.hInstance, 0) == 0)
            {
                Console.WriteLine("[taskman] createwindowex failure: " + new Win32Exception(Marshal.GetLastWin32Error()).Message);
            }
        }
        private static void CreateProgMan()
        {
            //create the program manager
            var progmanclass = WNDCLASSEX.Build();
            progmanclass.lpfnWndProc = Marshal.GetFunctionPointerForDelegate((Wndproc)ProgmanWndproc);
            progmanclass.style = 8;
            progmanclass.hInstance = GetModuleHandle(null);
            progmanclass.lpszClassName = "Progman";
            progmanclass.cbWndExtra = 8;
            progmanclass.cbClsExtra = 0;
            progmanclass.hbrBackground = new nint(2);
            progmanclass.hCursor = LoadCursor(IntPtr.Zero, IDC_ARROW);


            if (RegisterClassExW(ref progmanclass) == 0)
            {
                Console.WriteLine("[progman] registerclassex failure: " + new Win32Exception(Marshal.GetLastWin32Error()).Message);
            }
            Progmanhwnd = CreateWindowEx(128, "Progman", "Program Manager", 0x82000000, GetSystemMetrics(SM_XVIRTUALSCREEN), GetSystemMetrics(SM_YVIRTUALSCREEN), GetSystemMetrics(SM_CXVIRTUALSCREEN), GetSystemMetrics(SM_CYVIRTUALSCREEN), 0, 0, progmanclass.hInstance, 0);
            if (Progmanhwnd == 0)
            {
                Console.WriteLine("[progman] createwindowex failure: " + new Win32Exception(Marshal.GetLastWin32Error()).Message);
            }
        }
        public static IntPtr ProgmanWndproc(IntPtr hwnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            Console.WriteLine("[progman] msg " + msg);
            if (msg == WM_CREATE)
            {
                Console.WriteLine("[progman] created");
                if (SetShellWindow(hwnd) < 0)
                {
                    Console.WriteLine("SetShellWindow failed");
                }

                SetPropW(hwnd, "NonRudeHWND", new nint(1));
                SetPropW(hwnd, "AllowConsentToStealFocus", new nint(1));

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
            }
            else if (msg == WM_DESTROY)
            {
                RemovePropW(hwnd, "AllowConsentToStealFocus");
                RemovePropW(hwnd, "NonRudeHWND");
                SetShellWindow(0);
            }
            else if (msg == WM_SIZE)
            {
                ShowWindow(hwnd, ShowWindowCommands.Show);
            }
            else if (msg == WM_CLOSE)
            {
                return -1;
            }
            //else if (msg == WM_PAINT)
            //{
            //    PAINTSTRUCT Paint;
            //    RECT Rect;
            //    nint dc = BeginPaint(hwnd, out Paint);
            //    GetClientRect(hwnd, out Rect);
            //    FillRect(dc, ref Rect, CreateSolidBrush((uint)ColorTranslator.ToWin32(Color.Green)));
            //    EndPaint(hwnd, ref Paint);
            //    return 0;
            //}
            return DefWindowProc(hwnd, msg, wParam, lParam);
        }
        public static IntPtr TaskmanWndproc(IntPtr hwnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            if (msg == WM_CREATE)
            {
                Console.WriteLine("[taskman] created");
                ShellHook = RegisterWindowMessage("SHELLHOOK");
                if (ShellHook == 0)
                {
                    Console.WriteLine("Failed to create shellhook");
                }
                SetTaskmanWindow(hwnd);
                if (!RegisterShellHookWindow(hwnd))
                {
                    Console.WriteLine("failed to register shellhook window");
                }
            }
            else if (msg == WM_DESTROY)
            {
                if (GetTaskmanWindow() == hwnd)
                {
                    SetTaskmanWindow(0);
                }
                DeregisterShellHookWindow(hwnd);
            }
            else
            {
                if (msg != ShellHook && msg != WM_HOTKEY)
                {
                    return DefWindowProc(hwnd, msg, wParam, lParam);
                }

                if (HookService != null)
                {
                    bool handle = true;
                    if (wParam == 12)
                    {
                        Console.WriteLine("set window");
                        HookService.SetTargetWindowForSerialization(lParam);
                    }
                    else if (wParam == 0x32)
                    {
                        handle = false;
                    }
                    if (handle)
                    {
                        HookService.PostShellHookMessage(wParam, lParam);
                    }
                    return 0;
                }
                else
                {
                    try
                    {
                        var x = (CSShellManaged.Win32.IServiceProvider)new CImmersiveShell();
                        if (x.QueryService(ref SID_ImmersiveShellHookService, ref ImmersiveShellHookServiceInterface, out object shellhooksrv) < 0)
                        {
                            Console.WriteLine("failed to get the immersive shell hook service");
                        }
                        else
                        {
                            HookService = (IImmersiveShellHookService)shellhooksrv;
                        }
                    }
                    catch { }
                }


                return 0;
            }
            return 0;
        }
    }
}