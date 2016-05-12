using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace RoverMe.Shared.Network
{
    class SocketClient : IDisposable
    {
        #region Attributes and Properties

        public string Host { get; set; }
        public string Port { get; set; }
        public StreamSocket Socket { get; set; }
        public DataWriter Writer { get; set; }
        public DataReader Reader { get; set; }

        #endregion

        #region cycle

        public SocketClient(string hostname, string port)
        {
            Host = hostname;
            Port = port;
        }

        public void Dispose()
        {
            //lvl 2
            if (Reader != null)
                Reader.Dispose();
            if (Writer != null)
                Writer.Dispose();
            //lvl 1
            if (Socket != null)
                Socket.Dispose();
        }

        #endregion

        #region Methods

        public async void Connect()
        {
            Socket = new StreamSocket();
            Socket.Control.KeepAlive = true;

            HostName hostname = new HostName(Host);

            try
            {
                Debug.WriteLine("Connecting to Host: " + Host);

                await Socket.ConnectAsync(hostname, Port);

                Reader = new DataReader(Socket.InputStream);
                Writer = new DataWriter(Socket.OutputStream);

            }catch (Exception e)
            {
                Debug.WriteLine("Error While connecting to host: " + Host + " " + e.Message, "Error");
                throw e;
            }
        }

        #endregion
    }
}
