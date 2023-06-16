using CSShellManaged.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CSShellManaged.UI.XAML
{
    /// <summary>
    /// Interaction logic for StartButton.xaml
    /// </summary>
    public partial class StartButtonUI : System.Windows.Controls.UserControl
    {
        private IImmersiveMonitorManager MonitorManager;
        private IImmersiveLauncher Launcher;
        private bool inited = false;
        private IntPtr _handle;
        public StartButtonUI(IntPtr handle)
        {
            InitializeComponent();
            _handle = handle;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!inited)
            {

                try
                {
                    //Get the monitor manager service
                    var x = (CSShellManaged.Win32.IServiceProvider)new CImmersiveShell();
                    Guid MonitorManagerClass = new Guid("47094e3a-0cf2-430f-806f-cf9e4f0f12dd");
                    Guid MonitorManagerInterface = new Guid("4d4c1e64-e410-4faa-bafa-59ca069bfec2");
                    x.QueryService(ref MonitorManagerClass, ref MonitorManagerInterface, out object monitormang);

                    MonitorManager = (IImmersiveMonitorManager)monitormang;

                    //Get the immersive launcher
                    Guid ImmersiveLauncherClass = new Guid("6f86e01c-c649-4d61-be23-f1322ddeca9d");
                    Guid ImmersiveLauncherInterface = new Guid("d8d60399-a0f1-f987-5551-321fd1b49864");
                    x.QueryService(ref ImmersiveLauncherClass, ref ImmersiveLauncherInterface, out object launcherobj);
                    Launcher = (IImmersiveLauncher)launcherobj;
                    inited = true;
                }
                catch
                {
                    return;
                }
            }
            Guid a = new Guid("880b26f8-9197-43d0-8045-8702d0d72000");
            Guid b = new Guid("880b26f8-9197-43d0-8045-8702d0d72000");
            MonitorManager.QueryServiceFromWindow(_handle, ref a, ref b, out object monitorObj);

            var monitor = (IImmersiveMonitor)monitorObj;
            if (-1 < Launcher.ConnectToMonitor(monitor))
            {
                if (-1 < Launcher.OnStartButtonPressed(IMMERSIVELAUNCHERSHOWMETHOD.ILSM_STARTBUTTON, IMMERSIVELAUNCHERDISMISSMETHOD.ILDM_STARTTIP))
                {

                }
                else
                {
                    Console.WriteLine("failed to press the start button");
                }
            }
            else
            {
                Console.WriteLine("failed to connect to monitor");
            }
        }
    }
}
