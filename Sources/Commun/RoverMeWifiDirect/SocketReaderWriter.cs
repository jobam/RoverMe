//*********************************************************
//
// Made by Abraham Jonathan
//      04/02/2016
//
//*********************************************************

using System;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Windows.Devices.Enumeration;
using System.ComponentModel;
using Windows.Devices.WiFiDirect;
using System.Threading.Tasks;
using System.Diagnostics;

namespace RoverMeWifiDirect
{
    public static class Globals
    {
        public static readonly byte[] CustomOui = { 0xAA, 0xBB, 0xCC };
        public static readonly byte CustomOuiType = 0xDD;
        public static readonly byte[] WfaOui = { 0x50, 0x6F, 0x9A };
        public static readonly byte[] MsftOui = { 0x00, 0x50, 0xF2 };
        public static readonly string strServerPort = "4242";
    }

    public class SocketReaderWriter : IDisposable
    {
        #region Events to Subscribe

        public delegate void ReceivedMessageDelegate(string receivedMessage);
        public event ReceivedMessageDelegate ReceivedMessageEvent;

        #endregion
       
        #region Properties

        DataReader _dataReader;
        DataWriter _dataWriter;
        StreamSocket _streamSocket;
        string _currentMessage;

        #endregion

        #region Cycle

        public SocketReaderWriter(StreamSocket socket)
        {
            _dataReader = new DataReader(socket.InputStream);
            _dataReader.UnicodeEncoding = UnicodeEncoding.Utf8;
            _dataReader.ByteOrder = ByteOrder.LittleEndian;

            _dataWriter = new DataWriter(socket.OutputStream);
            _dataWriter.UnicodeEncoding = UnicodeEncoding.Utf8;
            _dataWriter.ByteOrder = ByteOrder.LittleEndian;

            _streamSocket = socket;
            _currentMessage = null;
        }

        public void Dispose()
        {
            _dataReader.Dispose();
            _dataWriter.Dispose();
            _streamSocket.Dispose();
        }

        #endregion

        #region Methods

        public async Task<bool> WriteMessage(string message)
        {
            try
            {
                _dataWriter.WriteUInt32(_dataWriter.MeasureString(message));
                _dataWriter.WriteString(message);
                await _dataWriter.StoreAsync();
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Socket closed: " + ex.Message);
                return false;
            }
        }

        public async void ReadMessage()
        {
            try
            {
                UInt32 bytesRead = await _dataReader.LoadAsync(sizeof(UInt32));
                if (bytesRead > 0)
                {
                    // Determine how long the string is.
                    UInt32 messageLength = _dataReader.ReadUInt32();
                    bytesRead = await _dataReader.LoadAsync(messageLength);
                    if (bytesRead > 0)
                    {
                        // Decode the string.
                        _currentMessage = _dataReader.ReadString(messageLength);

                        // raise event to tell message received
                        ReceivedMessageEvent.Invoke(_currentMessage);

                        ReadMessage();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Socket closed: " + ex.Message);
            }
        }

        public string GetCurrentMessage()
        {
            return _currentMessage;
        }
    }

    #endregion

}
