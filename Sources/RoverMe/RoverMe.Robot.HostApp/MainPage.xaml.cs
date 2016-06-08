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
        public MainPage()
        {
            this.InitializeComponent();
            TestMotors();
        }

        public async void TestMotors()
        {
            //MotorControl.MotorsInit();
            //MotorControl.MoveMotorsForTime(3000);

            GpioController gpio = GpioController.GetDefault();
            var pin1 = gpio.OpenPin(23);
            var pin2 = gpio.OpenPin(24);
            var pin3 = gpio.OpenPin(25);

            var pin21 = gpio.OpenPin(4);// 4
            var pin22 = gpio.OpenPin(5);// 5
            var pin23 = gpio.OpenPin(6);// 6

            while (true)
            {
                pin1.Write(GpioPinValue.High);
                pin2.Write(GpioPinValue.Low);
                pin3.Write(GpioPinValue.High);
                pin21.Write(GpioPinValue.High);
                pin22.Write(GpioPinValue.Low);
                pin23.Write(GpioPinValue.High);

                pin1.SetDriveMode(GpioPinDriveMode.Output);
                pin2.SetDriveMode(GpioPinDriveMode.Output);
                pin3.SetDriveMode(GpioPinDriveMode.Output);
                pin21.SetDriveMode(GpioPinDriveMode.Output);
                pin22.SetDriveMode(GpioPinDriveMode.Output);
                pin23.SetDriveMode(GpioPinDriveMode.Output);
                await Task.Delay(1000);
            }

            //while (true)
            //{
            //    await Task.Delay(1000);
            //    pin.Write(GpioPinValue.High);
            //    await Task.Delay(50);
            //    pin.Write(GpioPinValue.Low);
            //}

        }
    }
}
