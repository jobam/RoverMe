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
        private CoreDispatcher loopDispatcher;
        private List<Gamepad> gamepadList = new List<Gamepad>();
        private GamepadReading previousState;
        //EventWaitHandle ewh = new AutoResetEvent(false);

        public MainPage()
        {
            this.InitializeComponent();
            dispatcher = CoreWindow.GetForCurrentThread().Dispatcher;
            statusBlock.Text = "No gamepad";
            
            Gamepad.GamepadAdded += Gamepad_GamepadAdded;
            Gamepad.GamepadRemoved += Gamepad_GamepadRemoved;
        }

        private void Gamepad_GamepadRemoved(object sender, Gamepad e)
        {
            gamepadList.Clear(); //should not be a problem if we want to use only one gamepad.
            dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => setStatusText("removed"));
        }

        private void Gamepad_GamepadAdded(object sender, Gamepad e)
        {
            gamepadList.Add(e);
            
            dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => setStatusText("added"));
            //practically infinite loop

            Task loopGamepad = new Task(() =>
            {
                while (true)
                {
                    if (e.GetCurrentReading().Buttons != previousState.Buttons || e.GetCurrentReading().LeftThumbstickX != previousState.LeftThumbstickX)
                    {
                        previousState = e.GetCurrentReading();
                        if (e.GetCurrentReading().Buttons == GamepadButtons.A)
                        {
                            dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => setStatusText("APRESS"));
                        }
                        if (e.GetCurrentReading().Buttons == GamepadButtons.B)
                        {
                            dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => setStatusText("BPRESS"));
                        }
                    }
                }
            }, TaskCreationOptions.LongRunning);
            loopGamepad.Start();
            
            //System.Diagnostics.Debug.WriteLine("Gamepad unplugged end loop finish correctly.");
            
        }

        private void Gamepad_GamepadRemoved1(object sender, Gamepad e)
        {
            throw new NotImplementedException();
        }

        public void setStatusText(String status)
        {
            statusBlock.Text = status;
        }
    }
}
