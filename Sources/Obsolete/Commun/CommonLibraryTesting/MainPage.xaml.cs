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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace RoverMe.Shared.Networks.Tests
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public List<PeerInformation> Peers { get; set; }
        public WifiDirect WD { get; set; }
        public MainPage()
        {
            this.InitializeComponent();
            this.comboBox.DataContext = Peers;
            this.Peers = new List<PeerInformation>();
            WD = new WifiDirect(Write);
            WD.ConnectedToDevice += ConnectedToDevice;
            WD.Start();
        }

        private void BtnStartServer_OnClick(object sender, RoutedEventArgs e)
        {
        }

        public async void Write(string message)
        {
            await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { txtStatus.Text = message; });
            Debug.WriteLine(message);

        }

        private async void ConnectButton_OnClick(object sender, RoutedEventArgs e)
        {
            var peer = comboBox.SelectedItem as PeerInformation;
            if (peer != null)
            {
                try
                {
                    await WD.Connect(peer);
                }
                catch (Exception ex)
                {

                }
            }
        }

        private async void serverButton_Click(object sender, RoutedEventArgs e)
        {

            Peers = (await WD.FindPeersAsync()).ToList();
            this.comboBox.DataContext = Peers;
        }

        void ConnectedToDevice(StreamSocket stream)
        {
            Write("I Got The Stream !");
        }

    }
}
