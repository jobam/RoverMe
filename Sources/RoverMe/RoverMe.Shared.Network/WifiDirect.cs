//*********************************************************
//
// Made by Abraham Jonathan
//      10/03/2016
//
//*********************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Networking.Proximity;
using Windows.Networking.Sockets;


/**
 ** Give Verbose Action to the constructor to print State Messages
 ** 1) Register to ConnectedToDevice Event to get the stream when connected
 **
 ** ******************CLIENT****************************
 ** 2)  Call the FindPeerAsync() to get the Peers List
 ** 3) Perform a Connect()
 **
 ** ******************SERVER*****************************
 ** 2) Just wait for events ;)
 **
 **  Be careful ... To print messages in The UI Thread,
 **  call in your verbose(string message) method:
 ** this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { [YOUR LABEL] = message; });
**/

namespace RoverMe.Shared.Network
{
    public class WifiDirect
    {

        #region Events to register

        public event EventHandler<ConnectionRequestedEventArgs> ConnectionRequested;
        public event Action<StreamSocket> ConnectedToDevice;

        #endregion

        #region Attributes

        static readonly uint BLOCK_SIZE = 1024;
        Action<string> _verboseCb;
        public List<PeerInformation> _peerInformationList { get; private set; }

        public StreamSocket Stream { get; private set; }
        public PeerInformation ConnectedDevice { get; set; }

        #endregion

        #region Cycle

        public WifiDirect(Action<string> verboseCb = null)
        {
            _verboseCb = verboseCb;
        }

        public void Start()
        {
            PeerFinder.Role = PeerRole.Peer;
            PeerFinder.ConnectionRequested += PeerFinder_ConnectionRequested;
            PeerFinder.Start();
        }

        public void Dispose()
        {
            PeerFinder.ConnectionRequested -= PeerFinder_ConnectionRequested;
            PeerFinder.Stop();
        }

        #endregion

        #region Catch

        // This gets called when we receive a connect request from a Peer
        private void PeerFinder_ConnectionRequested(object sender, ConnectionRequestedEventArgs args)
        {
            Verbose(string.Format("Device Connecting : " + args.PeerInformation.DisplayName));
            this.ConnectedDevice = args.PeerInformation;

            if (this.ConnectedDevice != null)
            {
                if (ConnectionRequested != null)
                    ConnectionRequested(this, args);
                Connect(ConnectedDevice);
            }
        }

        #endregion

        #region Methods

        public async Task<IEnumerable<PeerInformation>> FindPeersAsync()
        {
            _peerInformationList = null;

            if ((PeerFinder.SupportedDiscoveryTypes & PeerDiscoveryTypes.Browse) ==
                                      PeerDiscoveryTypes.Browse)
            {
                if (PeerFinder.AllowWiFiDirect)
                {
                    // Find all discoverable peers with compatible roles
                    _peerInformationList = (await PeerFinder.FindAllPeersAsync()).ToList();
                    if (_peerInformationList == null)
                    {
                        Verbose("Found no peer");
                    }
                    else
                    {
                        Verbose(string.Format("I found {0} devices(s)", _peerInformationList.Count()));
                    }
                }
                else
                {
                    Verbose("WIFI direct not available");
                }
            }
            else
            {
                Verbose("Browse not available");
            }

            return _peerInformationList;
        }

        public async Task Connect(PeerInformation selectedPeer)
        {
            try
            {
                this.Stream = await PeerFinder.ConnectAsync(selectedPeer);
                if (Stream != null)
                {
                    Verbose("Stream Is Openned with device: " + selectedPeer.DisplayName);
                    if (ConnectedToDevice != null)
                        ConnectedToDevice(Stream);
                }
            }
            catch (Exception e)
            {

            }
        }

        private void Verbose(string message)
        {
            if (_verboseCb != null)
            {
                _verboseCb(message);
            }
        }

        #endregion
    }
}
