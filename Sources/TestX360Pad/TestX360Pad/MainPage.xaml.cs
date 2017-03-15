using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Gaming.Input;
using Windows.UI.Core;
using System.Threading;
using System.Threading.Tasks;

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace TestX360Pad
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private CoreDispatcher dispatcher;
        private GamepadReading previousState;
        private Boolean isPadConnected;

        public MainPage()
        {
            this.InitializeComponent();
            dispatcher = CoreWindow.GetForCurrentThread().Dispatcher;
            statusBlock.Text = "No gamepad connected";
            
            Gamepad.GamepadAdded += Gamepad_GamepadAdded;
            Gamepad.GamepadRemoved += Gamepad_GamepadRemoved;
        }

        private void Gamepad_GamepadRemoved(object sender, Gamepad e)
        {
            isPadConnected = false;
            dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => setStatusText("removed"));
        }

        private void Gamepad_GamepadAdded(object sender, Gamepad e)
        {
            isPadConnected = true;
            dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => setStatusText("added"));

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
                            dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => setStatusText("A PRESS"));
                        }
                        if (e.GetCurrentReading().Buttons == GamepadButtons.B)
                        {
                            dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => setStatusText("B PRESS"));
                        }
                        /*
                         * Directional Pad
                         */
                        if (e.GetCurrentReading().Buttons == GamepadButtons.DPadDown)
                        {
                            dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => setStatusText("Down Pad"));
                        }
                        if (e.GetCurrentReading().Buttons == GamepadButtons.DPadUp)
                        {
                            dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => setStatusText("Up Pad"));
                        }
                        if (e.GetCurrentReading().Buttons == GamepadButtons.DPadLeft)
                        {
                            dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => setStatusText("Left Pad"));
                        }
                        if (e.GetCurrentReading().Buttons == GamepadButtons.DPadRight)
                        {
                            dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => setStatusText("Right Pad"));
                        }
                        /*
                         * Triggers Left/Right (gachettes)
                         */
                        if (e.GetCurrentReading().RightTrigger > 0.3)
                        {
                            dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => setStatusText("Right Trigger"));
                        }
                        if (e.GetCurrentReading().LeftTrigger > 0.3)
                        {
                            dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => setStatusText("Left Trigger"));
                        }
                        /*
                         * Joysticks
                         */
                        if (e.GetCurrentReading().LeftThumbstickX != 0)
                        {
                            dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => setJoystickLeftText("X: "+e.GetCurrentReading().LeftThumbstickX+"Y: "+e.GetCurrentReading().LeftThumbstickY));
                        }
                        if (e.GetCurrentReading().LeftThumbstickY != 0)
                        {
                            dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => setJoystickLeftText("X: " + e.GetCurrentReading().LeftThumbstickX + "Y: " + e.GetCurrentReading().LeftThumbstickY));
                        }
                        if (e.GetCurrentReading().RightThumbstickX != 0)
                        {
                            dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => setJoystickRightText("X: " + e.GetCurrentReading().RightThumbstickX + "Y: " + e.GetCurrentReading().RightThumbstickY));
                        }
                        if (e.GetCurrentReading().RightThumbstickY != 0)
                        {
                            dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => setJoystickRightText("X: " + e.GetCurrentReading().RightThumbstickX + "Y: " + e.GetCurrentReading().RightThumbstickY));
                        }
                    }
                 }
            }, TaskCreationOptions.LongRunning);
            loopGamepad.Start();            
        }

        public void setStatusText(String status)
        {
            statusBlock.Text = status;
        }
        public void setJoystickLeftText(String status)
        {
            statusJoystickLBlock.Text = status;
        }
        public void setJoystickRightText(String status)
        {
            statusJoystickRBlock.Text = status;
        }
    }
}
