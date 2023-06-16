using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CSShellManaged
{
    public partial class StartButton : UserControl
    {
        private IImmersiveMonitorManager MonitorManager;
        private IImmersiveLauncher Launcher;
        private bool inited = false;
        public StartButton()
        {
            InitializeComponent();



        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenStartMenu();
        }
        
        private void OpenStartMenu()
        {
            if(!inited)
            {

                try
                {
                    //Get the monitor manager service
                    var x = (IServiceProvider)new CImmersiveShell();
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
            MonitorManager.QueryServiceFromWindow(Handle, ref a, ref b, out object monitorObj);

            var monitor = (IImmersiveMonitor)monitorObj;
            if (-1 < Launcher.ConnectToMonitor(monitor))
            {
               if (-1<Launcher.OnStartButtonPressed(IMMERSIVELAUNCHERSHOWMETHOD.ILSM_STARTBUTTON, IMMERSIVELAUNCHERDISMISSMETHOD.ILDM_STARTTIP))
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

        protected override void WndProc(ref Message m)
        {
            Console.WriteLine("[startbtn] msg: " + m.Msg);
            base.WndProc(ref m);
        }
    }
}
