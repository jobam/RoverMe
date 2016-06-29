using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.WiFi;
using Windows.Security.Credentials;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using ZXing.Mobile;

namespace WifiConnectLibrary
{
    public class WifiConnectManager
    {

        #region keys

        public class keys
        {
            public const string ssid = "ssid";
            public const string passphrase = "passphrase";
        }

        #endregion

        public async void ConnectFromQrCode()
        {
            try
            {
                MobileBarcodeScanner sc = new MobileBarcodeScanner();
                var scanResult = await sc.Scan();

                var results = JsonConvert.DeserializeObject<Dictionary<string, string>>(scanResult.Text);
                connectToNetwork(results[keys.ssid], results[keys.passphrase]);

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message, "Error");
            }
        }


        public WriteableBitmap GenerateQrCode(string ssid, string passphrase, int height, int width)
        {
            //serialize values into JSon
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
                    Height = height,
                    Width = width
                }
            };
            return writer.Write(json);
        }

        #region Helpers

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
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message, "Error");
            }
        }

        #endregion

    }
}
