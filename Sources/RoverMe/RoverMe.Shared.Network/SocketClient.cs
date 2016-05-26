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
    public class SocketClient : IDisposable
    {
        #region Attributes and Properties
        private static readonly SocketClient instance = new SocketClient("localhost", "80");
        public static SocketClient Instance
        {
            get
            {
                return instance;
            }
        }
        public string Host { get; set; }
        public string Port { get; set; }
        public StreamSocket Socket { get; set; }
        public DataWriter Writer { get; set; }
        public DataReader Reader { get; set; }

        #endregion

        #region cycle

        private SocketClient(string hostname, string port)
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

        public async Task Connect()
        {
            Socket = new StreamSocket();
            Socket.Control.KeepAlive = true;

            HostName hostname = new HostName(Host);

            try
            {
                Debug.WriteLine("Connecting to Host: " + Host);

                await Socket.ConnectAsync(hostname, Port);

                Reader = new DataReader(Socket.InputStream);
                Reader.InputStreamOptions = InputStreamOptions.Partial;

                Writer = new DataWriter(Socket.OutputStream);

            }catch (Exception e)
            {
                Debug.WriteLine("Error While connecting to host: " + Host + " " + e.Message, "Error");
                throw e;
            }
        }

        public async Task SendString(string message)
        {
            Writer.WriteString(message);
           await  Writer.StoreAsync();
        }

        #endregion
    }
}
