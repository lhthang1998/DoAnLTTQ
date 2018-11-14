using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace Client
{
    public partial class SignUp : Form
    {
        Login login;
        private TcpClient client;
        const int PORT_NUM = 2018;
        const int READ_BUFFER_SIZE = 255;
        private byte[] readBuffer = new byte[READ_BUFFER_SIZE];
        public SignUp(Login f1)
        {
            InitializeComponent();
            login = f1;
        }

        private void SignUp_Load(object sender, EventArgs e)
        {
            Connect();
        }

        void Connect()
        {
            try
            {
                client = new TcpClient(login.textBox2.Text, PORT_NUM);
                // Start an asynchronous read invoking DoRead to avoid lagging the user
                // interface.
                client.GetStream().BeginRead(readBuffer, 0, READ_BUFFER_SIZE, new AsyncCallback(Receive), null);
                // Make sure the window is showing before popping up connection dialog.
            }
            catch (Exception Ex)
            {
                MessageBox.Show("Không thể kết nối với máy chủ. Vui lòng thực hiện lại việc đăng nhập.",
                       this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                this.Dispose();
            }
        }
        public void SendData(string data)
        {
            StreamWriter writer = new StreamWriter(client.GetStream());
            writer.Write(data + (char)13);
            writer.Flush();
        }

        private void Receive(IAsyncResult ar)
        {
            int BytesRead;
            string strMessage;

            try
            {
                // Finish asynchronous read into readBuffer and return number of bytes read.
                BytesRead = client.GetStream().EndRead(ar);
                if (BytesRead < 1)
                {
                    // if no bytes were read server has close.  Disable input window.
                    //MarkAsDisconnected();
                    return;
                }
                // Convert the byte array the message was saved into, minus two for the
                // Chr(13) and Chr(10)
                strMessage = Encoding.UTF8.GetString(readBuffer, 0, BytesRead - 2);
                ProcessCommands(strMessage);
                // Start a new asynchronous read into readBuffer.
                client.GetStream().BeginRead(readBuffer, 0, READ_BUFFER_SIZE, new AsyncCallback(Receive), null);

            }
            catch (Exception e)
            {
                Application.Exit();

            }
        }

        private void ProcessCommands(string strMessage)
        {
            string[] cmd;
            // Message parts are divided by "|"  Break the string into an array accordingly.
            cmd = strMessage.Split('|');
            // dataArray(0) is the command.
            switch (cmd[0])
            {
                case "EXISTNAME":
                    MessageBox.Show("Đã tồn tại tên tài khoản này!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    SendData("DISCONNECT|");
                    this.Dispose();
                    break;
                case "SUCCESS":
                    MessageBox.Show("Đăng ký thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    SendData("DISCONNECT|");
                    this.Dispose();
                    break;
                default:
                    break;
            }
            
        }

        private void SignUp_FormClosing(object sender, FormClosingEventArgs e)
        {
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SendData("SIGNUP|" + textBox1.Text +"|"+ textBox2.Text);
        }
    }
}
