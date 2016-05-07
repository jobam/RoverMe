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

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace TestX360Pad
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public static CoreDispatcher dispatcher;
        public MainPage()
        {
            this.InitializeComponent();
            dispatcher = CoreWindow.GetForCurrentThread().Dispatcher;
            statusBlock.Text = "No gamepad";
            
            Gamepad.GamepadAdded += Gamepad_GamepadAdded;

        }

        private void Gamepad_GamepadAdded(object sender, Gamepad e)
        {
            dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => setStatusText("added"));
        }
        
        public void setStatusText(String status)
        {
            statusBlock.Text = status;
        }
    }
}
