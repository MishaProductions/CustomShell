using System.Runtime.InteropServices;

namespace CSShellManaged
{
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
        public int RegisterARNotify(IntPtr str);
        public int RefreshCache(int flags);
        public int ReleaseGlobalCacheObject();
        public int IsCacheMatchingLanguage(IntPtr ptr);
    }

    [ComImport]
    [Guid("660b90c8-73a9-4b58-8cae-355b7f55341b")]
    public class CStartMenuItemsCache
    {
    }
}
