//*********************************************************
//
// Made by Abraham Jonathan
//      04/02/2016
//
//*********************************************************

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.WiFiDirect;

namespace RoverMeWifiDirect
{
    /**
    ** To listen to comming messages,
    ** Subscribe to the socketRW.ReceivedMessageEvent Event
    */

    public class ConnectedDevice : INotifyPropertyChanged
    {
        private SocketReaderWriter socketRW;
        private WiFiDirectDevice wfdDevice;
        private string displayName = "";

        public ConnectedDevice(string displayName, WiFiDirectDevice wfdDevice, SocketReaderWriter socketRW)
        {
            this.socketRW = socketRW;
            this.wfdDevice = wfdDevice;
            this.displayName = displayName;
        }

        private ConnectedDevice() { }

        public SocketReaderWriter SocketRW
        {
            get
            {
                return socketRW;
            }

            set
            {
                socketRW = value;
            }
        }

        public WiFiDirectDevice WfdDevice
        {
            get
            {
                return wfdDevice;
            }

            set
            {
                wfdDevice = value;
            }
        }

        public override string ToString()
        {
            return displayName;
        }

        public string DisplayName
        {
            get
            {
                return displayName;
            }

            set
            {
                displayName = value;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
