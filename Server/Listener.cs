using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace Server
{
    class Listener
    {
        #region Thuoctinh
        private Socket socket;
        #endregion
        public delegate void SocketAcceptedHandler(Socket e);
        public event SocketAcceptedHandler SocketAccepted;
        public bool Listening { get; private set; }
        public int Port { get; private set; }

        public Listener(int port)
        {
            Port = port;
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void Start()
        {
            if (Listening) return;
            socket.Bind(new IPEndPoint(0, Port));
            socket.Listen(0);
            socket.BeginAccept(Callback, null);
            Listening = true;
        }

        public void Stop()
        {
            if (!Listening) return;
            if (socket.Connected)
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        }

        private void Callback(IAsyncResult ar)
        {
            try
            {
                var s = socket.EndAccept(ar);
                if (SocketAccepted != null) SocketAccepted(s);
                socket.BeginAccept(Callback, null);
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

    }
}
