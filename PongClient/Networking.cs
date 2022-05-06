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
            //Text should appear in the format of ID:MOVEMENT
            //ID is determining whether it is our BAT, the opponents BAT or the ball
            //Movement is the new position, or the new velocities
            int ID = int.Parse(text.Split(':')[0]);
            int newypos = 0;
            switch (ID)
            {
                case 1: //Is it our bat?
                    newypos = int.Parse(text.Split(':')[1]);
                    var localbat = Bat.localBat;
                    localbat.location.Y = newypos;
                    localbat.ModifyLocation(newypos);
                    break;
                case 2: //Is it the opponents bat
                    newypos = int.Parse(text.Split(':')[1]);
                    var remoteBat = Bat.remoteBat;
                    remoteBat.location.Y = newypos;
                    remoteBat.ModifyLocation(newypos);
                    break;
                case 3: //IS it a new velocity for the ball
                    //WILL NOT BE IMPLEMENTED UNTIL NEXT COMMIT
                    //TODO: implement ball velocities
                    break;
            }

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
                        _clientSocket.Connect(IPAddress.Parse("192.168.1.3"), 9999);
                    }
                    else
                    {
                        _clientSocket.Connect(IPAddress.Parse("172.20.17.91"), 9999);
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
