//*********************************************************
//
// Made by Abraham Jonathan
//      04/02/2016
//
//*********************************************************

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.Devices.WiFiDirect;
using Windows.Networking.Sockets;
using Windows.UI.Core;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.Devices.Enumeration;
using Windows.UI.Popups;
using System.Collections.Generic;
using System;
using System.Diagnostics;
using Windows.Devices.WiFiDirect.Services;

namespace RoverMeWifiDirect
{
    /**
     ** To listen to comming messages,
     ** Subscribe to the _connectedDevices[X].socketRW.ReceivedMessageEvent Event
    **/

    public class WifiDirectServer
    {
        #region Events to Subscribe

        public delegate void ConnectedDeviceDelegte(string deviceName);
        public event ConnectedDeviceDelegte ConnectedDeviceEvent;

        #endregion

        #region Attributes
        public ObservableCollection<ConnectedDevice> _connectedDevices
        {
            get;
            private set;
        }
        WiFiDirectAdvertisementPublisher _publisher;
        WiFiDirectConnectionListener _listener;
        StreamSocketListener _listenerSocket;
        public Page CallingPage { get; set; }

        #endregion

        #region Cycle

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
                    WiFiDirectConfigurationMethod conf = WiFiDirectConfigurationMethod.ProvidePin;
                    _publisher = new WiFiDirectAdvertisementPublisher();
                    _publisher.Advertisement.SupportedConfigurationMethods.Add(conf);
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
                Debug.WriteLine("Error starting Advertisement: " + ex.Message, "Error");
            }
        }

        #endregion

        #region Catch

        private async void OnConnectionRequested(WiFiDirectConnectionListener sender, WiFiDirectConnectionRequestedEventArgs connectionEventArgs)
        {
            try
            {
                var connectionRequest = connectionEventArgs.GetConnectionRequest();

                var tcsWiFiDirectDevice = new TaskCompletionSource<WiFiDirectDevice>();
                var wfdDeviceTask = tcsWiFiDirectDevice.Task;

                //setting task actions
                await CallingPage.Dispatcher.RunAsync(CoreDispatcherPriority.High, async () =>
                {
                    try
                {
                        Debug.WriteLine("Connecting to " + connectionRequest.DeviceInformation.Name + "...");

                        WiFiDirectConnectionParameters connectionParams = new WiFiDirectConnectionParameters();
                        connectionParams.GroupOwnerIntent = Convert.ToInt16("0"); // must be set if mutiple devices ?

                        // IMPORTANT: FromIdAsync needs to be called from the UI thread
                        tcsWiFiDirectDevice.SetResult(
                            await WiFiDirectDevice.FromIdAsync(connectionRequest.DeviceInformation.Id, connectionParams));
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("FromIdAsync task threw an exception: " + ex.Message, "Error");
                    throw;
                }
                });

                // getting device
                WiFiDirectDevice wfdDevice = await wfdDeviceTask;

                // Register for the ConnectionStatusChanged event handler
                //wfdDevice.ConnectionStatusChanged += OnConnectionStatusChanged;

                ConnectedDevice connectedDevice = new ConnectedDevice("Waiting for client to connect...", wfdDevice, null);
                _connectedDevices.Add(connectedDevice);

                var endpointPairs = wfdDevice.GetConnectionEndpointPairs();

                _listenerSocket = null;
                _listenerSocket = new StreamSocketListener();
                _listenerSocket.ConnectionReceived += OnSocketConnectionReceived;
                await _listenerSocket.BindEndpointAsync(endpointPairs[0].LocalHostName, Globals.strServerPort);

                Debug.WriteLine("Devices connected on L2, listening on IP Address: " + endpointPairs[0].LocalHostName.ToString() +
                                        " Port: " + Globals.strServerPort);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Connect operation threw an exception: " + ex.Message, "Error");
            }
        }

        private async void OnSocketConnectionReceived(StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args)
        {
            Debug.WriteLine("Connecting to remote side on L4 layer...");
            StreamSocket serverSocket = args.Socket;

            try
            {
                SocketReaderWriter socketRw = new SocketReaderWriter(serverSocket);

                //Run reading messages instance. Don't forget it
                socketRw.ReadMessage();

                while (true)
                {
                    string sessionId = socketRw.GetCurrentMessage();
                    //waiting for message, when getting message breaking loop
                    if (sessionId != null)
                    {
                        Debug.WriteLine("Connected with remote side on L4 layer");


                        for (int idx = 0; idx < _connectedDevices.Count; idx++)
                        {
                            if (_connectedDevices[idx].DisplayName.Equals("Waiting for client to connect...") == true)
                            {
                                ConnectedDevice connectedDevice = _connectedDevices[idx];
                                _connectedDevices.RemoveAt(idx);

                                connectedDevice.DisplayName = sessionId;
                                connectedDevice.SocketRW = socketRw;

                                _connectedDevices.Add(connectedDevice);
                                Debug.WriteLine("Connected with client : " + connectedDevice.DisplayName);
                                break;
                            }
                        }
                        break;
                    }
                    await Task.Delay(100);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Connection failed: " + ex.Message, "Error");
            }
        }

        #endregion

    }
}
