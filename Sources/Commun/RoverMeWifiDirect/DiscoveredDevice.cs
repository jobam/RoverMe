using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;

namespace RoverMeWifiDirect
{
    public class DiscoveredDevice : INotifyPropertyChanged
    {
        private DeviceInformation deviceInfo;

        public DiscoveredDevice(DeviceInformation deviceInfoIn)
        {
            deviceInfo = deviceInfoIn;
        }

        public DeviceInformation DeviceInfo
        {
            get
            {
                return deviceInfo;
            }
        }

        public override string ToString()
        {
            return deviceInfo.Name;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}