using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Connectivity;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace RoverMe.Shared.Network
{
    public class SocketServer : IDisposable
    {
        #region Attributes and Properties

        public StreamSocketListener Listener { get; private set; }
        public string Port { get; set; }
        public DataReader Reader { get; private set; }
        public DataWriter Writer { get; set; }
        public StreamSocket ClientSocket { get; set; }
        public bool IsConnected { get; set; }


        public string Hostname
        {
            get { return NetworkInformation.GetHostNames().FirstOrDefault().CanonicalName; }
        }


        #endregion

        #region Events

        public event Action<DataReader, DataWriter> ClientConnected;
        public event Action<string> IncommingCommand;

        #endregion

        #region cycle
        public SocketServer(string port)
        {
            Port = port;
        }

        public async void Start()
        {
            Debug.WriteLine("Starting server", "Info");

            StreamSocketListener listener = new StreamSocketListener();
            listener.Control.KeepAlive = true;

            listener.ConnectionReceived += OnConnection;

            //starting server
            try
            {
                await listener.BindServiceNameAsync(Port);
                Debug.WriteLine("Server started on port: " + Port, "Info");

            }
            catch (Exception e)
            {
                Debug.WriteLine("Error when starting server: " + e.Message, "Error");
                throw e;
            }
        }

        public void Dispose()
        {
            //lvl 2
            if (Reader != null)
                Reader.Dispose();
            if (Writer != null)
                Writer.Dispose();
            //lvl 1
            if (ClientSocket != null)
                ClientSocket.Dispose();
            if (Listener != null)
                Listener.Dispose();
        }

        #endregion

        #region Event Catch

        private void OnConnection(
           StreamSocketListener sender,
           StreamSocketListenerConnectionReceivedEventArgs args)
        {
            Debug.WriteLine("Connection recevied", "Info");

            Reader = new DataReader(args.Socket.InputStream);
            Writer = new DataWriter(args.Socket.OutputStream);
            ClientSocket = args.Socket;
            IsConnected = true;

            ClientConnected?.Invoke(Reader, Writer);
        }

        #endregion

        #region Methods

        public async void StartListeningCommands()
        {
            Debug.WriteLine("Starting listening incomming commands");

            await Task.Run(() =>
           {
               while (IsConnected)
               {
                   var datas = Reader.ReadString(sizeof(uint));
                   if (datas != null)
                       IncommingCommand?.Invoke(datas);
               }
           });
        }

        #endregion
    }
}
