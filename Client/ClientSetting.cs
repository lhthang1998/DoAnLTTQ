using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Client
{
    public class ClientSetting
    {
        #region Thuoc tinh
        readonly Socket socket;
        public event ReceivedEventHandler Received = delegate { };
        public delegate void ReceivedEventHandler(ClientSetting cs, string received);
        public event DisconnectedEventHandler Disconnected = delegate { };
        public delegate void DisconnectedEventHandler(ClientSetting cs);
        public event EventHandler Connected = delegate { };
        bool connect;
        #endregion

        public ClientSetting()
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void Connect(string ip,int port)
        {
            try
            {
                var ep = new IPEndPoint(IPAddress.Parse(ip), port);
                socket.BeginConnect(ep, ConnectCallback, socket);
            }
            catch
            {

            }
        }
        public void Close()
        {
            socket.Dispose();
            socket.Close();
        }
        void ConnectCallback(IAsyncResult ar)
        {
            socket.EndConnect(ar);
            connect = true;
            Connected(this, EventArgs.Empty);
            var buffer = new byte[socket.ReceiveBufferSize];
            socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, ReadCallback, buffer);
        }

        void ReadCallback(IAsyncResult ar)
        {
            var buffer = (byte[])ar.AsyncState;
            var rec = socket.EndReceive(ar);
            if (rec != 0)
            {
                var data = Encoding.ASCII.GetString(buffer, 0, rec);
                Received(this, data);
            }
            else
            {
                Disconnected(this);
                connect = false;
                Close();
                return;
            }
            socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, ReadCallback, buffer);
        }

        public void SendData(string s)
        {
            try
            {
                var buffer =Encoding.ASCII.GetBytes(s);
                socket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, SendCallback, buffer);
            }
            catch
            {
                Disconnected(this);
            }
        }

        void SendCallback(IAsyncResult ar)
        {
            socket.EndSend(ar);
        }
        

    }
}
