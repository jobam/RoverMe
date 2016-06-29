using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.WiFi;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Security.Credentials;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using ZXing.Mobile;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace WifiConnectLibrary
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WifiConnectPage : Page
    {
        WifiConnectManager manager;

        public WifiConnectPage()
        {
            this.InitializeComponent();
            manager = new WifiConnectManager();
        }

        private void btnQrCode_Click(object sender, RoutedEventArgs e)
        {
            manager.ConnectFromQrCode();
        }

        private void btnGenerateQrCode_Click(object sender, RoutedEventArgs e)
        {
            imgQrCode.Source = manager.GenerateQrCode(txtSSID.Text, txtPassphrase.Text, 
                Int32.Parse(imgQrCode.Height.ToString()), 
                Int32.Parse(imgQrCode.Width.ToString()));
        }

        #region Open Methods

        

        #endregion

    }
}
