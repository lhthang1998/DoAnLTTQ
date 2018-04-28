using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    class Client
    {
        #region Thuoc tinh
        public Socket socket;
        public IPEndPoint ip { get; private set; }
        public delegate void ClientReceivedHandler(Client sender, byte[] data);
        public delegate void ClientDisconnectedHandler(Client sender);
        public event ClientReceivedHandler Received;
        public event ClientDisconnectedHandler Disconnected;
        #endregion

        public Client(Socket accept)
        {
            socket = accept;
            ip = (IPEndPoint)socket.RemoteEndPoint;
            socket.BeginReceive(new byte[] { 0 }, 0, 0, 0, Callback, null);
        }

        void Callback(IAsyncResult ar)
        {
            try
            {
                socket.EndReceive(ar);
                var buffer = new byte[socket.ReceiveBufferSize];
                var rec = socket.Receive(buffer, buffer.Length, 0);
                if(rec<buffer.Length)
                {
                    Array.Resize(ref buffer, rec);
                }
                if(Received!=null)
                {
                    Received(this, buffer);
                }
                socket.BeginReceive(new byte[] { 0 }, 0, 0, 0, Callback, null);
            }
            catch(Exception e)
            {
                Close();
                if(Disconnected!=null)
                {
                    Disconnected(this);
                }

            }
        }
        void Close()
        {
            socket.Dispose();
            socket.Close();
        }
        public void SendData(string s)
        {
            var buffer = Encoding.ASCII.GetBytes(s); // chuyen thanh []byte
            socket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, ar => socket.EndSend(ar), buffer);

        }
        
    }
}
