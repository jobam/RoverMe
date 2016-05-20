using RoverMe.Shared.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Gaming.Input;
using Windows.UI.Core;

namespace RoverMe.Shared.Commands
{
    public class ControllerCommand : IRoverMeCommand
    {
        private const double SENSIBILITY_Y = 0.18;
        private const double SENSIBILITY_X = 0.1;
        private CoreDispatcher dispatcher;
        private GamepadReading previousState;
        private Boolean isPadConnected;
        private SocketClient socketClient;

        public ControllerCommand(SocketClient socketClient)
        {
            dispatcher = CoreWindow.GetForCurrentThread().Dispatcher;
            //statusBlock.Text = "No gamepad connected";
            this.socketClient = socketClient;

            Gamepad.GamepadAdded += Gamepad_GamepadAdded;
            Gamepad.GamepadRemoved += Gamepad_GamepadRemoved;
        }
        private void Gamepad_GamepadRemoved(object sender, Gamepad e)
        {
            isPadConnected = false;
            //dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => setStatusText("removed"));
        }

        private void Gamepad_GamepadAdded(object sender, Gamepad e)
        {
            isPadConnected = true;
            //dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => setStatusText("added"));

            Task loopGamepad = new Task(() =>
            {
                while (isPadConnected)
                {
                    if (e.GetCurrentReading().Buttons != previousState.Buttons || e.GetCurrentReading().LeftThumbstickX != previousState.LeftThumbstickX)
                    {
                        previousState = e.GetCurrentReading();
                        /*
                         * Buttons
                         */
                        if (e.GetCurrentReading().Buttons == GamepadButtons.A)
                        {
                            //dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => setStatusText("A PRESS"));
                        }
                        if (e.GetCurrentReading().Buttons == GamepadButtons.B)
                        {
                            //dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => setStatusText("B PRESS"));
                        }
                        /*
                         * Directional Pad
                         */
                        if (e.GetCurrentReading().Buttons == GamepadButtons.DPadDown)
                        {
                            //dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => BackwardCommand(true));
                        }
                        if (e.GetCurrentReading().Buttons == GamepadButtons.DPadUp)
                        {
                            //dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => FowardCommand(true));
                        }
                        if (e.GetCurrentReading().Buttons == GamepadButtons.DPadLeft)
                        {
                            //dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => setStatusText("Left Pad"));
                        }
                        if (e.GetCurrentReading().Buttons == GamepadButtons.DPadRight)
                        {
                            //dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => setStatusText("Right Pad"));
                        }
                        /*
                         * Triggers Left/Right (gachettes)
                         */
                        if (e.GetCurrentReading().RightTrigger > 0.3)
                        {
                            //dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => setStatusText("Right Trigger"));
                        }
                        if (e.GetCurrentReading().LeftTrigger > 0.3)
                        {
                            //dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => setStatusText("Left Trigger"));
                        }
                        /*
                         * Joysticks
                         */
                        if (e.GetCurrentReading().LeftThumbstickX != 0)
                        {
                            if (e.GetCurrentReading().LeftThumbstickX > 0-SENSIBILITY_X && e.GetCurrentReading().LeftThumbstickX < SENSIBILITY_X && e.GetCurrentReading().LeftThumbstickY > SENSIBILITY_Y)
                            {
                                //Foward
                                dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => FowardCommand(null));
                            }
                            if (e.GetCurrentReading().LeftThumbstickX < 0 - SENSIBILITY_X && e.GetCurrentReading().LeftThumbstickY > SENSIBILITY_Y)
                            {
                                //Foward Left
                                dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => FowardLeftCommand(new[] {e.GetCurrentReading().LeftThumbstickX.ToString(), e.GetCurrentReading().LeftThumbstickY.ToString()}));
                            }
                            if(e.GetCurrentReading().LeftThumbstickX > SENSIBILITY_X && e.GetCurrentReading().LeftThumbstickY > SENSIBILITY_Y)
                            {
                                //Foward Right
                                dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => FowardRightCommand(new[] { e.GetCurrentReading().LeftThumbstickX.ToString(), e.GetCurrentReading().LeftThumbstickY.ToString() }));
                            }
                            if (e.GetCurrentReading().LeftThumbstickX > 0 - SENSIBILITY_X && e.GetCurrentReading().LeftThumbstickX < SENSIBILITY_X && e.GetCurrentReading().LeftThumbstickY < 0-SENSIBILITY_Y)
                            {
                                //Backward
                                dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => BackwardCommand(new[] { e.GetCurrentReading().LeftThumbstickX.ToString(), e.GetCurrentReading().LeftThumbstickY.ToString() }));
                            }
                            if (e.GetCurrentReading().LeftThumbstickX < 0-SENSIBILITY_X && e.GetCurrentReading().LeftThumbstickY < 0-SENSIBILITY_Y)
                            {
                                //Backward Left
                                dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => BackwardLeftCommand(new[] { e.GetCurrentReading().LeftThumbstickX.ToString(), e.GetCurrentReading().LeftThumbstickY.ToString() }));
                            }
                            if(e.GetCurrentReading().LeftThumbstickX > SENSIBILITY_X && e.GetCurrentReading().LeftThumbstickY < 0-SENSIBILITY_Y)
                            {
                                //Backward Right
                                dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => BackwardRightCommand(new[] { e.GetCurrentReading().LeftThumbstickX.ToString(), e.GetCurrentReading().LeftThumbstickY.ToString() }));
                            }
                        }
                        if (e.GetCurrentReading().LeftThumbstickY != 0)
                        {
                            //dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => setJoystickLeftText("X: " + e.GetCurrentReading().LeftThumbstickX + "Y: " + e.GetCurrentReading().LeftThumbstickY));
                            if (e.GetCurrentReading().LeftThumbstickX < SENSIBILITY_X && e.GetCurrentReading().LeftThumbstickY > SENSIBILITY_Y)
                            {
                                //Foward Left
                            }
                            if (e.GetCurrentReading().LeftThumbstickX > SENSIBILITY_X && e.GetCurrentReading().LeftThumbstickY > SENSIBILITY_Y)
                            {
                                //Foward Right
                            }
                            if (e.GetCurrentReading().LeftThumbstickX < SENSIBILITY_X && e.GetCurrentReading().LeftThumbstickY < SENSIBILITY_Y)
                            {
                                //Backward Left
                            }
                            if (e.GetCurrentReading().LeftThumbstickX > SENSIBILITY_X && e.GetCurrentReading().LeftThumbstickY < SENSIBILITY_Y)
                            {
                                //Backward Right
                            }
                        }
                        if (e.GetCurrentReading().RightThumbstickX != 0)
                        {
                            //dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => setJoystickRightText("X: " + e.GetCurrentReading().RightThumbstickX + "Y: " + e.GetCurrentReading().RightThumbstickY));
                        }
                        if (e.GetCurrentReading().RightThumbstickY != 0)
                        {
                            //dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => setJoystickRightText("X: " + e.GetCurrentReading().RightThumbstickX + "Y: " + e.GetCurrentReading().RightThumbstickY));
                        }
                    }
                }
            }, TaskCreationOptions.LongRunning);
            loopGamepad.Start();
        }

        public override bool BackwardCommand(string[] args)
        {
            var message = IRoverMeCommand.RCommand.Backward.ToString();

            try
            {
                Task.WaitAll(new[] { socketClient.SendString(message) });
                return true;
            }
            catch
            {
                return false;
            }
        }

        public override bool BackwardLeftCommand(string[] args)
        {
            var message = IRoverMeCommand.RCommand.BackwardLeft.ToString();

            try
            {
                Task.WaitAll(new[] { socketClient.SendString(message) });
                return true;
            }
            catch
            {
                return false;
            }
        }

        public override bool BackwardRightCommand(string[] args)
        {
            var message = IRoverMeCommand.RCommand.BackwardRight.ToString();

            try
            {
                Task.WaitAll(new[] { socketClient.SendString(message) });
                return true;
            }
            catch
            {
                return false;
            }
        }

        public override bool CameraLeftCommand(string[] args)
        {
            throw new NotImplementedException();
        }

        public override bool CameraRightCommand(string[] args)
        {
            throw new NotImplementedException();
        }

        public override bool FowardCommand(string[] args)
        {
            var message = IRoverMeCommand.RCommand.Foward.ToString();

            try
            {
                Task.WaitAll(new[] { socketClient.SendString(message) });
                return true;
            }
            catch
            {
                return false;
            }
            
        }

        public override bool FowardLeftCommand(string[] args)
        {
            var message = IRoverMeCommand.RCommand.FowardLeft.ToString();

            try
            {
                Task.WaitAll(new[] { socketClient.SendString(message) });
                return true;
            }
            catch
            {
                return false;
            }
        }

        public override bool FowardRightCommand(string[] args)
        {
            var message = IRoverMeCommand.RCommand.FowardRight.ToString();

            try
            {
                Task.WaitAll(new[] { socketClient.SendString(message) });
                return true;
            }
            catch
            {
                return false;
            }
        }

        public override bool LeftCommand(string[] args)
        {
            throw new NotImplementedException();
        }

        public override bool RightCommand(string[] args)
        {
            throw new NotImplementedException();
        }

        public override bool SoundMessageCommand(string[] args)
        {
            throw new NotImplementedException();
        }

        public override bool StopCommand(string[] args)
        {
            throw new NotImplementedException();
        }
    }
}
