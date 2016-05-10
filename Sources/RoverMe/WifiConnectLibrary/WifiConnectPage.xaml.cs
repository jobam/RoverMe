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
        #region keys

        public class keys
        {
            public const string ssid = "ssid";
            public const string passphrase = "passphrase";
        }

        #endregion

        public WifiConnectPage()
        {
            this.InitializeComponent();
        }

        private async void btnQrCode_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MobileBarcodeScanner sc = new MobileBarcodeScanner();
                var scanResult = await sc.Scan();

                var results = JsonConvert.DeserializeObject<Dictionary<string, string>>(scanResult.Text);
                connectToNetwork(results[keys.ssid], results[keys.passphrase]);
            }catch(Exception ex)
            {
                Debug.WriteLine(ex.Message, "Error");
            }
        }

        private async void connectToNetwork(string ssid, string password)
        {
            try
            {
                var result = await Windows.Devices.Enumeration.DeviceInformation.FindAllAsync(WiFiAdapter.GetDeviceSelector());
                if (result.Count >= 1)
                {
                    var adapter = await WiFiAdapter.FromIdAsync(result[0].Id);
                    await adapter.ScanAsync();

                    var selectedNetwork = adapter.NetworkReport.AvailableNetworks.Where(x => x.Ssid == ssid).FirstOrDefault();
                    if (selectedNetwork != null)
                    {
                        PasswordCredential credentials = new PasswordCredential();
                        credentials.Password = password;
                        await adapter.ConnectAsync(selectedNetwork, WiFiReconnectionKind.Automatic, credentials);
                    }
                }
            }catch(Exception ex)
            {
                Debug.WriteLine(ex.Message, "Error");
            }
        }

        private void btnGenerateQrCode_Click(object sender, RoutedEventArgs e)
        {
            //serialize values into JSon
            string ssid = txtSSID.Text;
            string passphrase = txtPassphrase.Text;
            Dictionary<string, string> values = new Dictionary<string, string>();
            values.Add(keys.ssid, ssid);
            values.Add(keys.passphrase, passphrase);

            string json = JsonConvert.SerializeObject(values);

            // generate QR Code
            ZXing.Mobile.BarcodeWriter writer = new ZXing.Mobile.BarcodeWriter()
            {
                Format = ZXing.BarcodeFormat.QR_CODE,
                Options = new ZXing.Common.EncodingOptions
                {
                    Height = Int32.Parse(imgQrCode.Height.ToString()),
                    Width = Int32.Parse(imgQrCode.Width.ToString())
                }
            };
            var image = writer.Write(json);
            imgQrCode.Source = image;
        }
    }
}
