using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.WiFiDirect;
using Windows.Networking.Sockets;

namespace RoverMeWifiDirect
{
    public class WifiDirectServer
    {
        #region Attributes
        public ObservableCollection<ConnectedDevice> _connectedDevices
        {
            get;
            private set;
        }
        WiFiDirectAdvertisementPublisher _publisher;
        WiFiDirectConnectionListener _listener;
        StreamSocketListener _listenerSocket;

        #endregion

        public WifiDirectServer()
        {
            _connectedDevices = new ObservableCollection<ConnectedDevice>();
            _listenerSocket = null;
        }


        public void StartServer()
        {
            try
            {
                if (_publisher == null)
                {
                    _publisher = new WiFiDirectAdvertisementPublisher();
                }

                if (_listener == null)
                {
                    _listener = new WiFiDirectConnectionListener();
                }

                _listener.ConnectionRequested += OnConnectionRequested;

                // can be defined to Intensive
                _publisher.Advertisement.ListenStateDiscoverability = WiFiDirectAdvertisementListenStateDiscoverability.Normal;

                _publisher.Start();

                Debug.WriteLine("Advertisement started, waiting for StatusChanged callback...");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error starting Advertisement: " + ex.ToString());
            }
        }

        #region Helpers

        private async void OnConnectionRequested(WiFiDirectConnectionListener sender, WiFiDirectConnectionRequestedEventArgs connectionEventArgs)
        {
            try
            {
                var connectionRequest = connectionEventArgs.GetConnectionRequest();

                    var tcsWiFiDirectDevice = new TaskCompletionSource<WiFiDirectDevice>();
                    var wfdDeviceTask = tcsWiFiDirectDevice.Task;

                        try
                        {
                            Debug.WriteLine("Connecting to " + connectionRequest.DeviceInformation.Name + "...");

                            WiFiDirectConnectionParameters connectionParams = new WiFiDirectConnectionParameters();
                            connectionParams.GroupOwnerIntent = Convert.ToInt16("0"); // must be set if mutiple devices ?

                            // IMPORTANT: FromIdAsync needs to be called from the UI thread
                            tcsWiFiDirectDevice.SetResult(await WiFiDirectDevice.FromIdAsync(connectionRequest.DeviceInformation.Id, connectionParams));
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine("FromIdAsync task threw an exception: " + ex.ToString());
                        }

                    WiFiDirectDevice wfdDevice = await wfdDeviceTask;

                    // Register for the ConnectionStatusChanged event handler
                    wfdDevice.ConnectionStatusChanged += OnConnectionStatusChanged;

                    await Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
                    {
                        ConnectedDevice connectedDevice = new ConnectedDevice("Waiting for client to connect...", wfdDevice, null);
                        _connectedDevices.Add(connectedDevice);
                    });

                    var EndpointPairs = wfdDevice.GetConnectionEndpointPairs();

                    _listenerSocket = null;
                    _listenerSocket = new StreamSocketListener();
                    _listenerSocket.ConnectionReceived += OnSocketConnectionReceived;
                    await _listenerSocket.BindEndpointAsync(EndpointPairs[0].LocalHostName, Globals.strServerPort);

                    rootPage.NotifyUserFromBackground("Devices connected on L2, listening on IP Address: " + EndpointPairs[0].LocalHostName.ToString() +
                                            " Port: " + Globals.strServerPort, NotifyType.StatusMessage);
                }
                else
                {
                    // Decline the connection request
                    rootPage.NotifyUserFromBackground("Connection request from " + connectionRequest.DeviceInformation.Name + " was declined", NotifyType.ErrorMessage);
                    connectionRequest.Dispose();
                }
            }
            catch (Exception ex)
            {
                rootPage.NotifyUserFromBackground("Connect operation threw an exception: " + ex.Message, NotifyType.ErrorMessage);
            }
        }

        #endregion

    }
}
