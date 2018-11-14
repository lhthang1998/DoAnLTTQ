using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Server
{
    public partial class Main : Form
    {
        DataTable table = new DataTable();
        Server sv = new Server();
        bool flag = true; //Cờ hiệu ngôn ngữ
        MultiLang ml = new MultiLang();
        const int PORT_NUM = 2018;
        private Hashtable clients = new Hashtable();
        User client;
        private TcpListener listener;
        private Thread listenerThread;
        private PrivateChat pChat;
        private List<PrivateChat> listpChat = new List<PrivateChat>();
        public Main()
        {
            InitializeComponent();
        }

        private void bunifuImageButton1_Click(object sender, EventArgs e)
        {
            listener.Stop();
            Application.Exit();
        }

        private void bunifuImageButton2_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void Main_Load(object sender, EventArgs e)
        {
            this.Icon = Properties.Resources.Home;
            label1.Text = "SERVER UIT LAN CHAT";
            label2.Text = "ChatBox";
            menuStrip1.BackColor = Color.Transparent;
            //Mac dinh ngon ngu Tieng Viet
            this.ml.SetLanguage((int)eLanguage.TiengViet);
            this.ml.ChangeLanguage(this);
            //Tạo luồng lắng nghe và bắt đầu luồng lắng nghe
            listenerThread = new Thread(new ThreadStart(DoListen));
            listenerThread.Start();
        }


        //Hàm lắng nghe kết nối
        void DoListen()
        {
            try
            {
                // Lắng nghe kết nối ở địa chỉ IP với port chỉ định
                listener = new TcpListener(System.Net.IPAddress.Any, PORT_NUM);
                listener.Start();
                do
                {
                    // Create a new user connection using TcpClient returned by
                    // TcpListener.AcceptTcpClient()
                    client = new User(listener.AcceptTcpClient());
                    // Create an event handler to allow the UserConnection to communicate
                    // with the window.
                    client.LineReceived += new LineReceive(OnLineReceived);
                    //AddHandler client.LineReceived, AddressOf OnLineReceived;
                } while (true);
            }
            catch
            {
            }
        }

        //Hien thi len richtextbox
        private void UpdateMess(string s)
        {
            chatBox.Text += s + "\r\n";
        }

        
        private void OnLineReceived(User sender, string data)
        {
            string[] cmd;
            // Message parts are divided by "|"  Break the string into an array accordingly.
            cmd = data.Split((char)124);

            // dataArray(0) is the command.
            switch (cmd[0])
            {
                //Đăng ký tài khoản
                case "SIGNUP":
                    if (sv.CheckUserExist(cmd[1]) == true)
                    {
                        ReplyToSender("EXISTNAME|", sender);
                    }
                    else
                    {
                        sv.AddUser(cmd[1], cmd[2]);
                        ReplyToSender("SUCCESS|", sender);
                    }

                    break;
                //Đăng nhập
                case "CONNECT":
                    ConnectUser(cmd[1],cmd[2], sender);
                    break;
                //Chat public
                case "MESSAGE":
                    string v = cmd[1] + " : " + cmd[2];
                    UpdateMess(v);
                    Send("REFRESHCHAT|" + v);
                    break;
                //Chat riêng giữa server và client
                case "pMessage":
                    ChatPrivateWithUser(cmd);
                    break;
                //Nhận yêu cầu chat riêng tư giữa client A và gửi yêu cầu này đến client B
                case "PRIVATECHAT":
                    Send("FROMTO|" + cmd[1] + "|" + cmd[2]);
                    break;
                //server da nhan duoc tin nhan rieng giua 2 nguoi va bat dau dieu huong
                case "PrivateFromTo": 
                    sv.AddHistory(cmd[1], cmd[2], cmd[3]);
                    Send("PrivateMess|" + cmd[1] + "|" + cmd[2] + "|" + cmd[3]);    
                    break;
                //nhận được yêu cầu join group chat
                case "JOINGROUP":
                    SendToClients("JOINSUCCESS|" + cmd[1] + "|" + cmd[2] + " vừa vào phòng chat "+cmd[1],sender);
                    break;
                //Điều hướng tin nhắn group chat đến các client tham gia trong group chat phù hợp
                case "GroupChat":
                    SendToClients("GroupMess|" + cmd[1] + "|" + cmd[2], sender);
                    break;
                //nhận được yêu cầu thoát group chat và thông báo cho các client còn lại
                case "QuitGroupChat":
                    SendToClients("GroupMess|" + cmd[1] + "|" + cmd[2]+cmd[3], sender);
                    break;
                //nhận được yêu cầu xem lịch sử chat giữa 2 client từ client A và trả lời lại theo yêu cầu của client A
                case "History":
                    ReplyHistoryChat(cmd, sender);
                    break;
                //nhận yêu cầu xóa lịch sử chat.server thực hiện và gửi thông báo đến 2 client liên quan
                case "DeleteHistory":
                    sv.DeleteHistory(cmd[1], cmd[2]);
                    Send("Delete|" + cmd[1] +"|"+ cmd[2]);
                    break;
                //ngắt kết nối với client
                case "DISCONNECT":
                    DisconnectUser(sender);
                    break;
            }
        }


        //Ham kiem tra xem da co ten dang nhap hay chua va tra lai ket qua cho client yeu cau dang nhap
        private void ConnectUser(string userName,string pass, User sender)
        {
            if (clients.Contains(userName))
            {
                ReplyToSender("REFUSE", sender);
            }
            else
            {
                bool x = sv.CheckLogin(userName, pass);
                if (x == true)
                {
                    sender.Name = userName;
                    chatBox.Text += userName + " vừa vào phòng chat \r\n";
                    clients.Add(userName, sender);
                    Add(userName);
                    // Send a JOIN to sender, and notify all other clients that sender joined
                    ReplyToSender("JOIN", sender);
                    GuiUserOnline();
                    SendToClients("NOTI|" + sender.Name + " vừa vào phòng chat ", sender);
                }
                if (x == false)
                {
                    ReplyToSender("WRONG", sender);
                }
            }
        }

        /// <summary>
        /// Disconnect toi Client
        /// </summary>
        /// <param name="sender"></param>
        void DisconnectUser(User sender)
        {
            clients.Remove(sender.Name); // Xoa khoi bang bam client
            for (int i = 0; i < userlist.Items.Count; i++)
            {
                var item = userlist.Items[i].Tag as User;
                if (item.Name == sender.Name) //Tim trong list view client vua moi thoat de xoa khoi list view
                {
                    if (userlist.Items[i].SubItems[0].Text != string.Empty)
                    {
                        string s = userlist.Items[i].SubItems[0].Text + " đã thoát ra";
                        UpdateMess(s);
                        Send("REFRESHCHAT|" + s); // thong bao cho cac client con lai
                    }
                    userlist.Items.RemoveAt(i); // Xoa khoi list view
                    //Gui lai list online user cho cac client con lai
                    GuiUserOnline();
                }
            }
        }


        //Them vao listview
        private void Add(string name)
        {
            ListViewItem item = new ListViewItem(name);
            item.SubItems.Add("online"); // status
            item.Tag = client;
            userlist.Items.Add(item);
        }
        /// <summary>
        ///Gui List User Online
        /// </summary>
        void GuiUserOnline()
        {
            string u = string.Empty;
            for (int j = 0; j < userlist.Items.Count; j++)
            {
                u += userlist.Items[j].SubItems[0].Text + "|";
            }
            Send("USERS|" + u.TrimEnd('|'));
        }

        //Tra loi client gui yeu cau
        private void ReplyToSender(string strMessage, User sender)
        {
            sender.SendData(strMessage);
        }


        //Gui cho toan bo client ngoai tru client gui yeu cau
        private void SendToClients(string strMessage, User sender)
        {
            User client;
            // All entries in the clients Hashtable are UserConnection so it is possible
            // to assign it safely.
            foreach (DictionaryEntry entry in clients)
            {
                client = (User)entry.Value;
                // Exclude the sender.
                if (client.Name != sender.Name)
                {
                    client.SendData(strMessage);
                }
            }
        }

        //Gui cho tat ca cac client
        private void Send(string strMessage)
        {
            User client;
            // All entries in the clients Hashtable are UserConnection so it is possible
            // to assign it safely.
            foreach (DictionaryEntry entry in clients)
            {
                client = (User)entry.Value;
                client.SendData(strMessage);
            }
        }

        //Chat riêng tư với client
        private void ChatPrivateWithUser(string []cmd)
        {
            var value = listpChat.SingleOrDefault(r => r.getTo() == cmd[1]);
            if (value != null) //Neu trong list ton tai form 
                value.chatBox.Text += cmd[1] + " : " + cmd[2] + "\r\n";
            else
            {
                this.Invoke(() =>
                {
                    pChat = new PrivateChat(this);
                    pChat.SetTo(cmd[1]);
                    pChat.Show();
                    listpChat.Add(pChat);
                    pChat.chatBox.Text += cmd[1] + " : " + cmd[2] + "\r\n";
                });

            }
        }

        //Nhan yeu cau xem lich su chat cua client
        void ReplyHistoryChat(string []cmd,User sender)
        {
            string Gui = cmd[1];
            string Nhan = cmd[2];
            table = sv.getHistory(Gui, Nhan);
            string temp = string.Empty;
            for (int i = 0; i < table.Rows.Count; i++)
            {
                string send = table.Rows[i]["NameGui"].ToString();
                string message = table.Rows[i]["NoiDung"].ToString();
                temp += send + " : " + message + "|";
            }
            ReplyToSender("DETAIL|" + Nhan + "|" + temp.TrimEnd('|'), sender);
        }

        //Gửi yêu cầu chat riêng với client
        private void privateChatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (var client in from ListViewItem item in userlist.SelectedItems select (User)item.Tag)
            {
                string to = userlist.SelectedItems[0].SubItems[0].Text;
                var value = listpChat.SingleOrDefault(r => r.getTo() == to);
                if (value == null) //Neu chua co form
                {
                    client.SendData("Chat|" + to);
                    pChat = new PrivateChat(this);
                    pChat.SetTo(to);
                    pChat.Show();
                    listpChat.Add(pChat);
                }
                
            }
        }

        private void disconnectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (var client in from ListViewItem item in userlist.SelectedItems select (User)item.Tag)
            {
                client.SendData("BAN");
            }
        }
       

        //Save list nguoi online 
        private void saveListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog file = new SaveFileDialog() { Filter = "Text Documents|*.txt", ValidateNames = true })
            {
                if (file.ShowDialog() == DialogResult.OK)
                {
                    using (StreamWriter sw = new StreamWriter(file.FileName, true))
                    {
                        sw.WriteLine(DateTime.Now.ToString() + "\n");
                        foreach (ListViewItem item in userlist.Items)
                        {
                            sw.WriteLine(item.SubItems[0].Text);
                        }

                    }
                }
            }
        }


        
        private void button1_Click(object sender, EventArgs e)
        {
            if (txtBox.Text != "")
            {
                chatBox.Text += "Server : " + txtBox.Text + "\r\n";
                Send("BROAD|" + "Server : " + txtBox.Text);
                txtBox.Text = string.Empty;
            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (txtBox.Text != string.Empty)
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    btnSend.PerformClick();
                }
            }
        }

        private void richTextBox1_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            Process p = Process.Start("Chrome.exe", e.LinkText);
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            About about = new About(this);
            about.Show();
        }

        private void englishToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ml.SetLanguage((int)eLanguage.TiengAnh);
            this.ml.ChangeLanguage(this);
            flag = false;
        }

        private void vietnameseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ml.SetLanguage((int)eLanguage.TiengViet);
            this.ml.ChangeLanguage(this);
            flag = true;
        }

        public bool Flag()
        {
            return flag;
        }

        private void clearChatboxToolStripMenuItem_Click(object sender, EventArgs e)
        {
            chatBox.Text = string.Empty;
        }
        public void Remove(PrivateChat p)
        {
            listpChat.Remove(p);
        }

    }
}
