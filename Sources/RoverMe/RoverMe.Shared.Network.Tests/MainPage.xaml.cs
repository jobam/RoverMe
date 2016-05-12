using RoverMe.Shared.Network;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking.Proximity;
using Windows.Networking.Sockets;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Storage.Streams;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace RoverMe.Shared.Network.Tests
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        SocketServer server = new SocketServer("8080");

        public MainPage()
        {
            InitializeComponent();
        }

        private void btnServer_Click(object sender, RoutedEventArgs e)
        {
            server.ClientConnected += ClientConnected;
            server.IncommingCommand += commandrecevied;
            server.Start();
            var hostname = server.Hostname;
        }

        private async void ClientConnected(DataReader arg1, DataWriter arg2)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {
                textBlock.Text = "Client connected ! ";
                });
           await server.StartListeningCommands();
        }

        private async void commandrecevied(string command)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                textBlock.Text = command;
            });
        }

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {

        }


    }
}
