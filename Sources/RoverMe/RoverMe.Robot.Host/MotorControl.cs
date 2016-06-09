﻿using System;
using Windows.Foundation;
using System.Diagnostics;
using Windows.Devices.Gpio;
using System.Threading;
using Windows.System.Threading;
using System.Threading.Tasks;

namespace RoverMe.Robot.HostApp
{
    public class MotorControl
    {
        #region Pins

        public static int motorL1 = 4;
        public static int motorL2 = 5;
        public static int motorL3 = 6;

        public static int motorR1 = 23;
        public static int motorR2 = 24;
        public static int motorR3 = 25;

        #endregion

        #region Attributes and Properties

        GpioController controller = GpioController.GetDefault();

        private GpioPin pinL1;
        private GpioPin pinL2;
        private GpioPin pinL3;

        private GpioPin pinR1;
        private GpioPin pinR2;
        private GpioPin pinR3;

        #endregion

        #region Cycle

        public MotorControl()
        {
            Init();
        }

        public void Init()
        {
            pinL1 = controller.OpenPin(motorL1);
            pinL2 = controller.OpenPin(motorL2);
            pinL3 = controller.OpenPin(motorL3);
            pinL1.SetDriveMode(GpioPinDriveMode.Output);
            pinL2.SetDriveMode(GpioPinDriveMode.Output);
            pinL3.SetDriveMode(GpioPinDriveMode.Output);

            pinR1 = controller.OpenPin(motorR1);
            pinR2 = controller.OpenPin(motorR2);
            pinR3 = controller.OpenPin(motorR3);
            pinR1.SetDriveMode(GpioPinDriveMode.Output);
            pinR2.SetDriveMode(GpioPinDriveMode.Output);
            pinR2.SetDriveMode(GpioPinDriveMode.Output);
        }

        #endregion

        #region motors control

        public async void RunMotorLeft(int milliseconds)
        {
            pinL1.Write(GpioPinValue.High);
            pinL2.Write(GpioPinValue.Low);
            pinL3.Write(GpioPinValue.High);

            await Task.Delay(milliseconds);
            pinL1.Write(GpioPinValue.Low);
            pinL2.Write(GpioPinValue.Low);
            pinL3.Write(GpioPinValue.Low);
        }

        public async void RunMotorRight(int milliseconds)
        {
            pinR1.Write(GpioPinValue.High);
            pinR2.Write(GpioPinValue.Low);
            pinR3.Write(GpioPinValue.High);

            await Task.Delay(milliseconds);
            pinR1.Write(GpioPinValue.Low);
            pinR2.Write(GpioPinValue.Low);
            pinR3.Write(GpioPinValue.Low);
        }

        public void RunFoward(int milliseconds)
        {
            RunMotorLeft(milliseconds);
            RunMotorRight(milliseconds);
        }

        #endregion
    }
}
