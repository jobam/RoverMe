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
using RoverMe.Shared.Commands;
using RoverMe.Shared.Network;

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace RoverMeClient
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private ControllerCommand controllerCommand;
        private SocketClient socketClient;
        public MainPage()
        {
            this.InitializeComponent();

        }

        private void forwardButton(System.Object sender, RoutedEventArgs e)
        {
            controllerCommand.FowardCommand(new[] {"1","1"});
        }

        private void rightButton(object sender, RoutedEventArgs e)
        {
            controllerCommand.RightCommand(new[] { "1", "1" });
        }

        private void leftButton(object sender, RoutedEventArgs e)
        {
            controllerCommand.LeftCommand(new[] { "1", "1" });
        }

        private void backwardButton(object sender, RoutedEventArgs e)
        {
            controllerCommand.BackwardCommand(new[] { "1", "1" });
        }

        private void forwardRightButton(object sender, RoutedEventArgs e)
        {
            controllerCommand.FowardRightCommand(new[] { "1", "1" });
        }

        private void forwardLeftButton(object sender, RoutedEventArgs e)
        {
            controllerCommand.FowardLeftCommand(new[] { "1", "1" });
        }

        private void backwardRightButton(object sender, RoutedEventArgs e)
        {
            controllerCommand.BackwardRightCommand(new[] { "1", "1" });
        }

        private void backwardLeftButton(object sender, RoutedEventArgs e)
        {
            controllerCommand.BackwardLeftCommand(new[] { "1", "1" });
        }

        private void connectionButton(object sender, RoutedEventArgs e)
        {
            socketClient = SocketClient.Instance;
            
            //controllerCommand = socketClient.Connect.controllerCommand;
        }

        private void commandInstanciation(SocketClient socketCl)
        {
            controllerCommand = new ControllerCommand(socketCl);
            button1.Content = "Connecté au robot";
        }
    }
}
