using PlayHooky;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSShellManaged
{
    public class WindowClassHook
    {
        public static void Install()
        {
            HookManager manager = new HookManager();

            //find the method
            var type = typeof(NativeWindow);
            Type WindowClassType = null;
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type2 in asm.GetTypes())
                {
                    if (type2.Name == "WindowClass")
                    {
                        WindowClassType = type2;
                        break;
                    }
                }
            }
            if (WindowClassType == null)
            {
                throw new Exception("failed to find the NativeWindow.WindowClass class");
            }

            var GetFullClassNameFunc = WindowClassType.GetMethod("GetFullClassName", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            if (GetFullClassNameFunc == null)
            {
                throw new Exception("failed to find the NativeWindow.GetFullClassName(string) method");
            }

            manager.Hook(GetFullClassNameFunc, typeof(WindowClassHook).GetMethod("GetFullClassNameHook"));
        }
        public static string ClassName { get; set; } = "UNKNOWN";
        public static string GetFullClassNameHook(object instance)
        {
            string x = ClassName;
            ClassName += "__";
            return x;
        }
    }
}
