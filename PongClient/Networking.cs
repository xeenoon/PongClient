using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace PongClient
{
    public static class Networking
    {
        private static Socket _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private static byte[] _buffer = new byte[10];
        public static void SetupClient()
        {
            ConnectLoop();
            _clientSocket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(RecieveCallback), _clientSocket);
        }
        private static void RecieveCallback(IAsyncResult AR)
        {
            Socket socket = (Socket)AR.AsyncState;
            int recieved = socket.EndReceive(AR);
            byte[] dataBuf = new byte[recieved];
            Array.Copy(_buffer, dataBuf, recieved);

            string text = Encoding.ASCII.GetString(dataBuf);

            socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(RecieveCallback), socket);
        }
        private static void ConnectLoop()
        {
            //Port for all recievers should always be 101
            //For the purposes of testing, this will be variable
            int attempts = 0;
            while (!_clientSocket.Connected)
            {
                try
                {
                    ++attempts;
                    if (DateTime.Now.Hour >= 15)
                    {
                        _clientSocket.Connect(IPAddress.Parse("192.168.1.3"), 100);
                    }
                    else
                    {
                        _clientSocket.Connect(IPAddress.Parse("172.20.17.91"), 100);
                    }
                }
                catch (Exception e)
                {
                }
            }
        }
        public static void Send(byte[] data)
        {
            _clientSocket.Send(data);
        }
    }
}
