using RoverMe.Robot.Host;
using RoverMe.Shared.Network;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Gpio;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System.Threading;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace RoverMe.Robot.HostApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public Controller MainController { get; set; }
        public SocketServer Server { get; set; }

        public MainPage()
        {
            this.InitializeComponent();
            //TestMotors();
            Launch();
        }

        public void TestMotors()
        {
            MotorControl controller = new MotorControl();
            controller.RunFoward(10000);

            }

        public void Launch()
        {
            MainController = new Controller();
            MainController.StartCommandServer();
        }
    }
}
