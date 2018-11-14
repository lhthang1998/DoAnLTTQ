using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    public delegate void LineReceive(User sender, string Data);
    public class User
    {
        #region Thuoc tinh
        private TcpClient client;
        private byte[] readBuffer = new byte[READ_BUFFER_SIZE];
        private string strName;
        const int READ_BUFFER_SIZE = 255;
        // Thuộc tính tên xác định người dùng kết nối
        public string Name
        {
            get
            {
                return strName;
            }
            set
            {
                strName = value;
            }
        }
        
        public event LineReceive LineReceived;
        #endregion
        //Hàm khởi tạo thread đọc dữ liệu
        public User(TcpClient client)
        {
            this.client = client;
            //Bắt đầu thread đọc bất đồng bộ và dữ liệu sẽ được lưu xuống readbuffer
            this.client.GetStream().BeginRead(readBuffer, 0, READ_BUFFER_SIZE, new AsyncCallback(StreamReceiver), null);
        }

        
        //sử dụng StreamReader và StreamWriter để gửi nhận dữ liệu mà không cần bước chuyển đổi qua lại mảng byte
        public void SendData(string Data)
        {
            // đảm bảo không còn luồng nào khác cố gắng sử dụng stream tại cùng thời điểm
            lock (client.GetStream())
            {
                StreamWriter writer = new StreamWriter(client.GetStream());
                writer.Write(Data + (char)13 + (char)10);
                //Đảm bảo dữ liệu được gửi đi
                //Giải phóng tài nguyên
                writer.Flush();
            }
        }

        //Hàm gọi lại cho TcpClient.GetStream.Begin.
        //Bắt đầu đọc không đồng bộ từ stream
        private void StreamReceiver(IAsyncResult ar)
        {
            int BytesRead;
            string strMessage;

            try
            {
                // Ensure that no other threads try to use the stream at the same time.
                lock (client.GetStream())
                {
                    // Finish asynchronous read into readBuffer and get number of bytes read.
                    BytesRead = client.GetStream().EndRead(ar);
                }
                // Convert the byte array the message was saved into, minus one for the
                // Chr(13).
                strMessage = Encoding.UTF8.GetString(readBuffer, 0, BytesRead - 1);
                LineReceived(this, strMessage);
                // Ensure that no other threads try to use the stream at the same time.
                lock (client.GetStream())
                {
                    // Start a new asynchronous read into readBuffer.
                    client.GetStream().BeginRead(readBuffer, 0, READ_BUFFER_SIZE, new AsyncCallback(StreamReceiver), null);
                }
            }
            catch (Exception e)
            {
            }
        }
    }
}
