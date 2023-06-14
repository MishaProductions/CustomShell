using System.ComponentModel;
using System.Runtime.InteropServices;
using static CSShellManaged.Win32;

namespace CSShellManaged
{
    public class Program
    {
        static long ProgManPCRef = 0;
        static nint ProgManThreadRef;
        static uint ShellHook = 0;
        static Guid SID_ImmersiveShellHookService = new Guid("4624bd39-5fc3-44a8-a809-163a836e9031");
        static Guid ImmersiveShellHookServiceInterface = new Guid("914d9b3a-5e53-4e14-bbba-46062acb35a4");
        static IImmersiveShellHookService HookService;
        public static int Main(IntPtr args, int sizeBytes)
        {
            try
            {
                Console.WriteLine("Hello from .NET");
                Application.SetCompatibleTextRenderingDefault(false);
                Application.EnableVisualStyles();

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
                if (CreateWindowEx(128, "Progman", "Program Manager", 0x82000000, GetSystemMetrics(SM_XVIRTUALSCREEN), GetSystemMetrics(SM_YVIRTUALSCREEN), GetSystemMetrics(SM_CXVIRTUALSCREEN), GetSystemMetrics(SM_CYVIRTUALSCREEN), 0, 0, progmanclass.hInstance, 0) == 0)
                {
                    Console.WriteLine("[progman] createwindowex failure: " + new Win32Exception(Marshal.GetLastWin32Error()).Message);
                }

                // create taskman class (handles taskbar buttons)
                //create the program manager
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
                if (CreateWindowEx(128, "TaskmanWndClass", null, 0x82000000, GetSystemMetrics(SM_XVIRTUALSCREEN), GetSystemMetrics(SM_YVIRTUALSCREEN), GetSystemMetrics(SM_CXVIRTUALSCREEN), GetSystemMetrics(SM_CYVIRTUALSCREEN), 0, 0, progmanclass.hInstance, 0) == 0)
                {
                    Console.WriteLine("[taskman] createwindowex failure: " + new Win32Exception(Marshal.GetLastWin32Error()).Message);
                }

                var builder = (IImmersiveShellBuilder)new CImmersiveShellBuilder();
                builder.CreateImmersiveShellController(out IImmersiveShellController controller);

                if (controller.Start() != 0)
                {
                    Console.WriteLine("!!!FAILED TO START IMMERSIVE SHELL!!!");
                }

                Application.Run();
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("exception: " + ex.ToString());
                return -1;
            }
        }

        public static IntPtr ProgmanWndproc(IntPtr hwnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            Console.WriteLine("[progman] msg");
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
            else if (msg == WM_PAINT)
            {
                PAINTSTRUCT Paint;
                RECT Rect;
                nint dc = BeginPaint(hwnd, out Paint);
                GetClientRect(hwnd, out Rect);
                FillRect(dc, ref Rect, CreateSolidBrush((uint)ColorTranslator.ToWin32(Color.Green)));
                EndPaint(hwnd, ref Paint);
                return 0;
            }
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
                    if(wParam == 12)
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
                    var x = (IServiceProvider)new CImmersiveShell();
                    if (x.QueryService(ref SID_ImmersiveShellHookService, ref ImmersiveShellHookServiceInterface, out object shellhooksrv) < 0)
                    {
                        Console.WriteLine("failed to get the immersive shell hook service");
                    }
                    else
                    {
                        HookService = (IImmersiveShellHookService)shellhooksrv;
                    }
                }
                

                return 0;
            }
            return 0;
        }
    }
    [ComImport]
    [Guid("914d9b3a-5e53-4e14-bbba-46062acb35a4")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IImmersiveShellHookService
    {
        int Register(); //TODO args
        int Unregister(uint cookie);
        int PostShellHookMessage(IntPtr wparam, IntPtr lparam);
        int SetTargetWindowForSerialization(IntPtr hwnd);
        int PostShellHookMessageWithSerialization();//todo: args
        int UpdateWindowApplicationId(IntPtr hwnd, string pszAppid);
        int HandleWindowReplacement(IntPtr hwndOld, IntPtr hwndNew);
        bool IsExecutionOnSerializedThread();
    }
    [ComImport]
    [Guid("6d5140c1-7436-11ce-8034-00aa006009fa")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IServiceProvider
    {
        int QueryService(ref Guid guidService, ref Guid riid,
                   [MarshalAs(UnmanagedType.Interface)]  out object ppvObject);
    }
    [ComImport]
    [Guid("c2f03a33-21f5-47fa-b4bb-156362a2f239")]
    [ClassInterface(ClassInterfaceType.None)]
    public class CImmersiveShell
    {

    }

    [ComImport]
    [Guid("d6948331-eaf5-4365-86ac-d8d1d03b4600")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IImmersiveShellController
    {
        public int Start();
        public int Stop(IntPtr arg);
        public int SetCreationBehavior(IntPtr arg);
    }

    [ComImport]
    [Guid("1c56b3e4-e6ea-4ced-8a74-73b72c6bd435")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IImmersiveShellBuilder
    {
        int CreateImmersiveShellController(out IImmersiveShellController controller);
    }
    [ComImport]
    [Guid("c71c41f1-ddad-42dc-a8fc-f5bfc61df957")]
    [ClassInterface(ClassInterfaceType.None)]
    public class CImmersiveShellBuilder
    {

    }
}