using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace Client
{
    public partial class MainChat : Form
    {
        #region Thuoc tinh
        bool flag = true; //Cờ hiệu ngôn ngữ
        MultiLang ml = new MultiLang();
        private TcpClient client ;
        private PrivateChat pChat;
        private PrivateUser uChat;
        private GroupChat gChat;
        private List<GroupChat> listgChat = new List<GroupChat>();
        private List<PrivateUser> listuChat = new List<PrivateUser>();
        private List<PrivateChat> listpChat = new List<PrivateChat>();
        private bool checkuChat;//gán cờ nếu để kiểm tra xem uChat đã đóng hay chưa       
        private bool Check;//Gán cờ để nhận biết xem pChat đã đóng hay chưa
        const int PORT_NUM = 2018;
        const int READ_BUFFER_SIZE = 255;
        string ip="127.0.0.1";
        string user;
        private byte[] readBuffer = new byte[READ_BUFFER_SIZE];
        #endregion


        public MainChat()
        {
            InitializeComponent();
            label.Text = "Name";
            Icon = Properties.Resources.chat;
            //Mac dinh ngon ngu Tieng Viet
            this.ml.SetLanguage((int)eLanguage.TiengViet);
            this.ml.ChangeLanguage(this);
            gChat = new GroupChat(this);

        }

        private void MainChat_Load(object sender, EventArgs e)
        {
            menuStrip1.BackColor = Color.Transparent;
            Connect();
            
        }

        private void Connect()
        {

            
            try
            {
                this.Hide();
                Login frmLogin = new Login();
                frmLogin.ShowDialog(this);
                ip = frmLogin.IP();
                // The TcpClient is a subclass of Socket, providing higher level 
                // functionality like streaming.
                client = new TcpClient(ip,PORT_NUM);
                
                // Start an asynchronous read invoking DoRead to avoid lagging the user
                // interface.
                // Bắt đầu luồng Receive để tránh tình trạng lag giao diện người dùng
                client.GetStream().BeginRead(readBuffer, 0, READ_BUFFER_SIZE, new AsyncCallback(Receive), null);
                // Make sure the window is showing before popping up connection dialog.

                //Gửi tên tài khoản đăng nhập và mật khẩu đến server để kiểm tra xem có hợp lệ
                SendData("CONNECT|" + frmLogin.textBox1.Text + "|" + frmLogin.textBox3.Text);
                user = frmLogin.textBox1.Text;
                labelName.Text = " : "+user;
                frmLogin.Dispose();
                this.Show();
            }
            catch (Exception Ex)
            {
                MessageBox.Show("Không thể kết nối với máy chủ. Vui lòng thực hiện lại việc đăng nhập.",
                       this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                this.Dispose();
            }
        }


        //void AttemptLogin()
        //{
           
        //    frmLogin.textBox2.Text = ip;
        //    SendData("CONNECT|" + frmLogin.textBox1.Text);
        //    user = frmLogin.textBox1.Text;
        //    labelName.Text = "Name : " + user;
        //    frmLogin.Dispose();
        //    this.Show();

        //}

        //Gui Data
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
                //Kết thúc đọc bất đồng bộ vào readBuffer và trả về số byte đã đọc
                BytesRead = client.GetStream().EndRead(ar);
                if (BytesRead < 1)
                {
                    return;
                }
                // Convert the byte array the message was saved into, minus two for the
                // Chr(13) and Chr(10)
                strMessage = Encoding.UTF8.GetString(readBuffer, 0, BytesRead - 2);
                ProcessCommands(strMessage);
                //Bắt đầu đọc bất đồng bộ vào readBuffer
                client.GetStream().BeginRead(readBuffer, 0, READ_BUFFER_SIZE, new AsyncCallback(Receive), null);

            }
            catch (Exception e)
            {
                MessageBox.Show("Server đã đóng!");

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
                //Đăng nhập sai tên tài khoản hoặc mật khẩu
                case "WRONG":
                    MessageBox.Show("Sai tên tài khoản hoặc mật khẩu!");
                    Application.Exit();
                    break;
                //Server chấp nhận kết nối
                case "JOIN":
                    // Server acknowledged login.
                    DisplayText(user + " vừa vào phòng chat");
                    break;
                //Tài khoản này đang được đăng nhập nên server từ chối
                case "REFUSE":
                    MessageBox.Show("Đã có người dùng tên tài khoản này!");
                    Application.Exit();
                    break;
                //Nhận thông báo có client khác đã đăng nhập vào phòng chat
                case "NOTI":
                    DisplayText(cmd[1]);
                    break;
                //Nhận được chuỗi list người online,tách chuỗi để show lên listview Userlist
                case "USERS":
                    //Server sent Online Users List.
                    LoadUser(cmd);
                    break;
                //Chat public
                case "REFRESHCHAT":
                    ChatPublic(cmd);
                    break;
                // nhan duoc yeu cau nhan tin rieng tu server
                case "Chat":
                    RequestPrivate(cmd);
                    break;
                case "pMessage": //Nhan duoc tin nhan private cua server
                    PrivateMessFromServer(cmd);                  
                    break;
                //Tin nhan public tu server
                case "BROAD":
                    DisplayText(cmd[1]);
                    break;
                case "FROMTO":
                    RequestPrivateClient(cmd);
                    break;
                case "PrivateMess": // nhan duoc tin nhan rieng tu giua 2 User do server gui ve va bat dau xu ly
                    PrivateMess(cmd);
                    break;
                //xem lich su chat
                case "DETAIL":
                    History(cmd);
                    break;
                //Nhan thong bao da xoa lich su chat
                case "Delete":
                    DelHistory(cmd);
                    break;
                //Thong bao join group chat thanh cong
                case "JOINSUCCESS":
                    GroupChat(cmd);            
                    break;
                case "GroupMess":
                    GroupMess(cmd);
                    break;
                //Server disconnect client
                case "BAN": // Server disconnect client
                    Application.Exit();
                    break;
            }
        }

        //Load người dùng online
        void LoadUser(string []cmd)
        {
            UserList.Items.Clear();
            for (int i = 1; i < cmd.Length; i++)
            {
                if (cmd[i] != "USERS" | cmd[i] != "online")
                {
                    UserList.Items.Add(cmd[i]);
                }
            }
        }

        //Chat public 
        void ChatPublic(string[] cmd)
        {
            string temp = cmd[1].Split(':')[0];
            string from = temp.Substring(0, temp.Length - 1);
            if (user != from)
                DisplayText(cmd[1]);
        }
        //Nhan duoc yeu cau chat rieng tu server
        void RequestPrivate(string[] cmd)
        {
            if (listpChat.Count == 0)
            {
                this.Invoke(() =>
                {
                    pChat = new PrivateChat(this);
                    pChat.SetFrom(cmd[1]);
                    pChat.Show();
                    listpChat.Add(pChat);
                });
            }

        }
        //Nhan tin nhan rieng tu server
        void PrivateMessFromServer(string[] cmd)
        {
            if (listpChat.Count == 0) // lúc này chưa có form pChat vì form pChat trước đã bị đóng lại
             {
                        this.Invoke(() =>
                        {
                            pChat = new PrivateChat(this);
                            //Check = false;
                            pChat.SetFrom(cmd[2]);
                            pChat.Show();
                            listpChat.Add(pChat);
                            pChat.richTextBox1.Text += "Server : " + cmd[1] + "\r\n";
                        });
             }
            else
                    {
                        var p = listpChat.SingleOrDefault(r => r.getFrom() == cmd[2]);
                        if (p != null)
                            p.richTextBox1.Text += "Server : " + cmd[1] + "\r\n";
                    }
        }

        //Nhan yeu cau chat rieng tu tu client khac
        void RequestPrivateClient(string[] cmd)
        {
            string fromUser = cmd[1];
            string toUser = cmd[2];
            if (toUser == user)
            {

                var value = listuChat.SingleOrDefault(r => r.From() == cmd[2]); // kiem tra xem co ton tai form nao chua
                if (value == null)
                {
                    this.Invoke(() =>
                    {
                        uChat = new PrivateUser(this);
                        uChat.SetFrom(toUser);
                        uChat.SetTo(fromUser);
                        uChat.Text = "From :" + toUser + ":To:" + fromUser;
                        uChat.Show();
                        listuChat.Add(uChat);
                    });
                }
                else value.Show();

            }
        }

        //Tin nhan rieng giua 2 client
        void PrivateMess(string[] cmd)
        {
            string pfrom = cmd[1]; //gui tu
            string pto = cmd[2]; // gui den

            if (pto == user) //kiểm tra xem có đúng là tin nhắn gửi tới mình hay không
            {
                var value = listuChat.SingleOrDefault(r => r.To() == cmd[1]);
                if (value != null)
                    value.richTextBox1.Text += cmd[1] + " : " + cmd[3] + "\r\n";
                else
                {
                    this.Invoke(() =>
                    {
                        uChat = new PrivateUser(this);
                        uChat.SetFrom(pto);
                        uChat.SetTo(pfrom);
                        uChat.Text = "From :" + pto + ":To:" + pfrom;
                        uChat.Show();
                        checkuChat = false;
                        listuChat.Add(uChat);
                        uChat.richTextBox1.Text += cmd[1] + " : " + cmd[3] + "\r\n";

                    });
                }
                //}

            }
        }


        //Xem lich su chat
        void History(string[] cmd)
        {
            string receive = cmd[1];
            string mess = string.Empty;
            labelHistory.Text = receive;
            txtHistory.Text = string.Empty;
            for (int i = 2; i < cmd.Length; i++)
            {
                txtHistory.Text += cmd[i] + "\r\n";
            }
        }


        //Xoa lich su chat
        void DelHistory(string []cmd)
        {
            if (user == cmd[1] || user == cmd[2])
                MessageBox.Show("Đã xóa cuộc trò chuyện giữa " + cmd[1] + " và " + cmd[2], "Thông báo",MessageBoxButtons.OK,MessageBoxIcon.Information);
        }


        //Tham gia chat nhom thanh cong
        void GroupChat(string[] cmd)
        {
            var values = listgChat.SingleOrDefault(r => r.getName() == cmd[1]);
            if (values != null)
            {
                values.gchatBox.Text += cmd[2] + "\r\n";
            }
        }

        //Tin nhan chat nhom
        void GroupMess(string[] cmd)
        {
            var y = listgChat.SingleOrDefault(r => r.getName() == cmd[1]);
            if (y != null)
                y.gchatBox.Text += cmd[2] + "\r\n";
        }
        
        
        //Hien thi Mess
        private void DisplayText(string text)
        {
            richTextBox1.Text += text + "\r\n";
        }

        private void MainChat_FormClosing(object sender, FormClosingEventArgs e)
        {
            SendData("DISCONNECT|");
        }


        //Gui tin nhan public
        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != string.Empty)
            {
                string s = user + " : " + textBox1.Text;
                SendData("MESSAGE|" + user + "|" + textBox1.Text);
                DisplayText(s);
                textBox1.Text = string.Empty;
            }
        }
        public string getName()
        {
            return user;
        }



        //Chat rieeng giua 2 client
        private void privateChatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string to = UserList.SelectedItem.ToString();
            if(to!=null)
            {
                if (to != user) // dam bao khong chat voi chinh minh
                {
                    SendData("PRIVATECHAT|" + user + "|" + to);
                    var value = listuChat.SingleOrDefault(r => r.To() == to);
                    if (value != null)
                        value.Show();
                    else
                    {
                        uChat = new PrivateUser(this);
                        uChat.SetFrom(user); // set thuoc tinh nguoi gui
                        uChat.SetTo(to); // set thuoc tinh nguoi nhan
                        uChat.Text = "From :" + user + ":To:" + to;
                        uChat.Show();
                        checkuChat = false;
                        listuChat.Add(uChat);
                    }
                }
            
            }
        }

        //Yeu cau chat nhom
        void JoinGroup(string name)
        {
            var value = listgChat.SingleOrDefault(r => r.getName() == name);
            if (value == null)
            {
                SendData("JOINGROUP|" + name + "|" + user);
                this.Invoke(() =>
                {
                    gChat = new GroupChat(this);

                    gChat.setFrom(user);
                    gChat.setName(name);
                    gChat.label1.Text = "Group " + name;
                    gChat.From.Text = user;
                    gChat.Show();
                    gChat.gchatBox.Text += user + " vừa vào phòng chat " + name + "\r\n";
                    listgChat.Add(gChat);
                });
            }
        }

        //Remove khoi list form Chat private User
        public void Remove(PrivateUser p)
        {
            listuChat.Remove(p);
        }
        //Remove khoi list privateChat server khi dong form
        public void RemovePrivate(PrivateChat f)
        {
            listpChat.Remove(f);
        }

        //Remove khoi list form GroupChat
        public void RemoveGroup(GroupChat g)
        {
            listgChat.Remove(g);
        }


        #region Thao tac ngoai
        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if(textBox1.Text!=string.Empty)
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    button1.PerformClick();
                }
            }
            
        }

        private void đăngXuấtToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            MainChat main = new MainChat();
            main.Show();
            this.Dispose();
        }

        private void clearChatboxToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = string.Empty;
        }

        private void thoátToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void englishToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ml.SetLanguage((int)eLanguage.TiengAnh);
            this.ml.ChangeLanguage( this);
            flag = false;
            //lg.SetLanguage(this, "en-US");
        }

        private void vietnameseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ml.SetLanguage((int)eLanguage.TiengViet);
            this.ml.ChangeLanguage(this);
            flag = true;
            //lg.SetLanguage(this, "vi-VN");
        }


        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            string to = UserList.SelectedItem.ToString();

            if (to != user && to != null) // dam bao khong chat voi chinh minh
            {
                SendData("History|" + user + "|" + to);
            }
        }

        private void deleteChatHistoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string to = UserList.SelectedItem.ToString();
            if (to != user && to != null) // dam bao khong chat voi chinh minh
            {
                SendData("DeleteHistory|" + user + "|" + to);
            }
        }

        //Cac thao tac yeu cau chat nhom
        private void tPHCMToolStripMenuItem_Click(object sender, EventArgs e)
        {

            JoinGroup("TPHCM");
        }

        private void hANOIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            JoinGroup("HANOI");
        }

        private void uITToolStripMenuItem_Click(object sender, EventArgs e)
        {
            JoinGroup("UIT");
        }
        #endregion



        public bool getFlag()
        {
            return flag;
        }

        private void richTextBox1_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            Process p = Process.Start("Chrome.exe", e.LinkText);
        }


        
    }
}
