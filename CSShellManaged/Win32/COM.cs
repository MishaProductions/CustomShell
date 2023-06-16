using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace CSShellManaged.Win32
{
    [ComImport]
    [Guid("914d9b3a-5e53-4e14-bbba-46062acb35a4")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IImmersiveShellHookService
    {
        int Register(); //TODO args
        int Unregister(uint cookie);
        int PostShellHookMessage(nint wparam, nint lparam);
        int SetTargetWindowForSerialization(nint hwnd);
        int PostShellHookMessageWithSerialization();//todo: args
        int UpdateWindowApplicationId(nint hwnd, string pszAppid);
        int HandleWindowReplacement(nint hwndOld, nint hwndNew);
        bool IsExecutionOnSerializedThread();
    }
    [ComImport]
    [Guid("6d5140c1-7436-11ce-8034-00aa006009fa")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IServiceProvider
    {
        int QueryService(ref Guid guidService, ref Guid riid,
                   [MarshalAs(UnmanagedType.Interface)] out object ppvObject);
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
        public int Stop(nint arg);
        public int SetCreationBehavior(nint arg);
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


    [ComImport]
    [Guid("ba5a92ae-bfd7-4916-854f-6b3a402b84a8")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IStartMenuItemsCache
    {
        public int OnChangeNotify(); //todo
        public int RegisterForNotifications();
        public int UnregisterForNotifications();
        public int PauseNotifications();
        public int ResumeNotifications();
        public int RegisterARNotify(nint str);
        public int RefreshCache(int flags);
        public int ReleaseGlobalCacheObject();
        public int IsCacheMatchingLanguage(nint ptr);
    }

    [ComImport]
    [Guid("660b90c8-73a9-4b58-8cae-355b7f55341b")]
    public class CStartMenuItemsCache
    {
    }

    public enum IMMERSIVE_MONITOR_FILTER_FLAGS
    {
        IMMERSIVE_MONITOR_FILTER_FLAGS_NONE = 0x0,
        IMMERSIVE_MONITOR_FILTER_FLAGS_DISABLE_TRAY = 0x1,
    }

    [ComImport]
    [Guid("880b26f8-9197-43d0-8045-8702d0d72000")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IImmersiveMonitor
    {
        public int GetIdentity(out uint pIdentity);
        public int Append(object unknown);
        public int GetHandle(nint phMonitor);
        public int IsConnected(out bool pfConnected);
        public int IsPrimary(out bool pfPrimary);
        public int GetTrustLevel(out uint level);
        public int GetDisplayRect(out RECT prcDisplayRect);
        public int GetOrientation(out uint pdwOrientation);
        public int GetWorkArea(out RECT prcWorkArea);
        public int IsEqual(IImmersiveMonitor pMonitor, out bool pfEqual);
        public int GetTrustLevel2(out uint level);
        public int GetEffectiveDpi(out uint dpiX, out uint dpiY);
        public int GetFilterFlags(out IMMERSIVE_MONITOR_FILTER_FLAGS flags);
    }
    public enum IMMERSIVE_MONITOR_MOVE_DIRECTION
    {
        IMMD_PREVIOUS = 0,
        IMMD_NEXT = 1
    }
    [ComImport]
    [Guid("4d4c1e64-e410-4faa-bafa-59ca069bfec2")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IImmersiveMonitorManager
    {
        public int GetCount(out uint pcMonitors);
        public int GetConnectedCount(out uint pcMonitors);
        public int GetAt(out uint idxMonitor, out IImmersiveMonitor monitor);
        public int GetFromHandle(nint monitor, out IImmersiveMonitor monitor2);
        public int GetFromIdentity(uint identity, out IImmersiveMonitor monitor);
        public int GetImmersiveProxyMonitor(out IImmersiveMonitor monitor);
        public int QueryService(nint monit, ref Guid guidService, ref Guid riid, [MarshalAs(UnmanagedType.IUnknown)] out object service);
        public int QueryServiceByIdentity(uint monit, ref Guid guidService, ref Guid riid, [MarshalAs(UnmanagedType.IUnknown)] out object service);
        public int QueryServiceFromWindow(nint hwnd, ref Guid guidService, ref Guid riid, [MarshalAs(UnmanagedType.IUnknown)] out object service);
        public int QueryServiceFromPoint(nint point, ref Guid guidService, ref Guid riid, [MarshalAs(UnmanagedType.IUnknown)] out object service);
        public int GetNextImmersiveMonitor(IMMERSIVE_MONITOR_MOVE_DIRECTION direction, IImmersiveMonitor monitor, out IImmersiveMonitor monitorout);
        public int GetMonitorArray(out object array);
        public int SetFilter(object filter);
    }
    public enum IMMERSIVELAUNCHERSHOWMETHOD
    {
        ILSM_INVALID = 0x0,
        ILSM_HSHELLTASKMAN = 0x1,
        ILSM_IMMERSIVEBACKGROUND = 0x4,
        ILSM_APPCLOSED = 0x6,
        ILSM_STARTBUTTON = 0xB,
        ILSM_RETAILDEMO_EDUCATIONAPP = 0xC,
        ILSM_BACK = 0xD,
        ILSM_SESSIONONUNLOCK = 0xE
    }
    public enum IMMERSIVELAUNCHERSHOWFLAGS
    {
        ILSF_NONE = 0x0,
        ILSF_IGNORE_SET_FOREGROUND_ERROR = 0x4,
    }
    public enum IMMERSIVELAUNCHERDISMISSMETHOD
    {
        ILDM_INVALID = 0x0,
        ILDM_HSHELLTASKMAN = 0x1,
        ILDM_STARTCHARM = 0x2,
        ILDM_BACKGESTURE = 0x3,
        ILDM_ESCAPEKEY = 0x4,
        ILDM_SHOWDESKTOP = 0x5,
        ILDM_STARTTIP = 0x6,
        ILDM_GENERIC_NONANIMATING = 0x7,
    }
    [ComImport]
    [Guid("d8d60399-a0f1-f987-5551-321fd1b49864")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IImmersiveLauncher
    {
        public int ShowStartView(IMMERSIVELAUNCHERSHOWMETHOD showMethod, IMMERSIVELAUNCHERSHOWFLAGS showFlags);
        public int Dismiss(IMMERSIVELAUNCHERDISMISSMETHOD dismissMethod);
        public int Dismiss2(IMMERSIVELAUNCHERDISMISSMETHOD dismissMethod);
        public int DismissSynchronouslyWithoutTransition();
        public int IsVisible(out bool p0);
        public int OnStartButtonPressed(IMMERSIVELAUNCHERSHOWMETHOD showMethod, IMMERSIVELAUNCHERDISMISSMETHOD dismissMethod);
        public int SetForeground();
        public int ConnectToMonitor(IImmersiveMonitor monitor);
        public int GetMonitor(out IImmersiveMonitor monitor);
        public int OnFirstSignAnimationFinished();
        public int Prelaunch();
    }

    [ComImport]
    [Guid("00000000-0000-0000-0000-000000000000")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe interface IDeskTray
    {
        //IDeskTray
        public uint AppBarGetState();
        public void GetTrayWindow(ref nint tray);
        public int SetDesktopWindow(nint desktop);
        public int SetVar(int a, ulong b);
    }

    [ComImport]
    [Guid("c4de032a-d902-450a-bc43-d9df6d0fd48c")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IExplorerHostCreator
    {
        //IDeskTray
        public int CreateHost(ref Guid guid);
        public int RunHost();
    }

    [ComImport]
    [Guid("ab0b37ec-56f6-4a0e-a8fd-7a8bf7c2da96")]
    public class CExplorerHostCreator
    {
    }
}
