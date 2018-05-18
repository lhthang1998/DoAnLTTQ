using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Client
{
    public partial class PublicChat : Form
    {
        #region Thuoc tinh
        private  PrivateChat pChat;
        private pChatUser uChat;
        public readonly FormLogin formLogin = new FormLogin();
        #endregion
        public PublicChat()
        {
            this.Icon = Properties.Resources.chat;
            pChat = new PrivateChat(this);
            uChat = new pChatUser(this);
            InitializeComponent();
        }

        void GhiFile(string s)
        {
            using (StreamWriter sw = new StreamWriter("Test.txt"))
            {
                sw.WriteLine(s);
            }
        }
        private void PublicChat_Load(object sender, EventArgs e)
        {

        }

        void MarkAsDisconnect()
        {
            UserList.Enabled = false;
            textBox1.Enabled = false;
            richTextBox1.Enabled = false;
        }
        //void GhiFile(string s)
        //{
        //    using (StreamWriter sw = new StreamWriter("PublicChatClient.txt"))
        //    {
        //        sw.WriteLine(s);
        //    }
        //}
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            
            formLogin.client.Received += clientReceived;
            formLogin.client.Disconnected += clientDisconnected;
            formLogin.ShowDialog();
            string s = formLogin.textBox1.Text;
            formLogin.client.SendData("Public|" + formLogin.textBox1.Text + "|online");
            richTextBox1.Text += formLogin.textBox1.Text + " vua vao phong chat \r\n";
            Text = "Public Chat ";
            label1.Text = "USER : " + s;
        }
        private static void clientDisconnected(ClientSetting cs)
        {
        }

        public void clientReceived(ClientSetting cs,string data)
        {
            var cmd = data.Split('|');

            switch (cmd[0])
            {
                case "Users":
                    this.Invoke(() =>
                    {
                        UserList.Items.Clear();
                        
                        for(int i=1;i<cmd.Length;i++)
                        {
                            if(cmd[i]!="online" |cmd[i]!="Noti")
                            {
                                UserList.Items.Add(cmd[i]);
                            }
                        }
                    });
                    break;
                case "Noti":
                    string username = formLogin.textBox1.Text;
                    string pubfrom = cmd[1];
                    if (username != pubfrom)
                        richTextBox1.Text += cmd[1] + cmd[2]+"\r\n";
                    break;
                case "Message":
                    richTextBox1.Text += cmd[1] + "\r\n";
                    break;
                case "RefreshChat":
                    string user = formLogin.Text;
                    string temp = cmd[1].Split(':')[0];
                    string from = temp.Substring(0, temp.Length - 1);
                    if (user != from)
                        richTextBox1.Text += cmd[1];
                    break;
                case "Chat":
                    this.Invoke(() =>
                    {
                        pChat = new PrivateChat(this);
                        pChat.setFrom(formLogin.textBox1.Text);
                        pChat.Show(); 
                    });
                    
                    break;
                case "FromTo":
                    string fromUser = cmd[1];
                    string toUser = cmd[2];
                    if(toUser==formLogin.textBox1.Text)
                    {
                        this.Invoke(() =>
                        {   if(uChat.getCheck()==true)
                            {
                                uChat = new pChatUser(this);
                                
                            }
                            uChat.setFrom(toUser);
                            uChat.setTo(fromUser);
                            uChat.Text = "From :" + toUser + ":To:" + fromUser;
                            uChat.Show();
                            

                        });
                    }    
                    break;
                // Nhan tin nhan tu ca nhan tu server chuyen ve
                case "PrivateMess":
                    string pfrom = cmd[1];
                    string pto = cmd[2];
                    if(pto==formLogin.textBox1.Text)
                    {
                       uChat.richTextBox1.Text += cmd[1] + " : " + cmd[3] + "\r\n";         
                    }
                    break;
                case "pMessage":
                    this.Invoke(() =>
                    {
                        if (pChat.getCheck() == true)
                        {
                            pChat = new PrivateChat(this);
                            pChat.setFrom(formLogin.textBox1.Text);
                            pChat.setCheck();
                            pChat.Show();
                        }
                        pChat.richTextBox1.Text += "Server : " + cmd[1] + "\r\n";

                    });
                    break;
                case "Disconnect":
                    {
                        Application.Exit();
                    }
                    break;
                case "Exist":
                    GhiFile(cmd[0]);
                    MessageBox.Show("Da co nguoi dang nhap bang ten nay!");
                    
                    Application.Exit();
                    break;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(textBox1.Text!=string.Empty)
            {
                formLogin.client.SendData("Message|" + formLogin.textBox1.Text + "|" + textBox1.Text);
                textBox1.Text = string.Empty;
            }
        }

        private void button1_KeyDown(object sender, KeyEventArgs e)
        {
           if(e.KeyCode==Keys.Enter)
            {
                button1.PerformClick();
            }
        }
        private void privateChat_Click(object sender, EventArgs e)
        {
            formLogin.client.SendData("pChat|" + formLogin.textBox1.Text);
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            richTextBox1.SelectionStart = richTextBox1.TextLength;
        }

        private void privatechat_Click_1(object sender, EventArgs e)
        {
                string s = UserList.SelectedItem.ToString();
                if(s!=formLogin.textBox1.Text) // dam bao khong chat voi chinh minh
                {
                    formLogin.client.SendData("Private|" + formLogin.textBox1.Text + "|" + s); // from ->to
                    uChat = new pChatUser(this);
                    uChat.setFrom(formLogin.textBox1.Text); // set thuoc tinh nguoi gui
                    uChat.setTo(s); // set thuoc tinh nguoi nhan
                    uChat.Text = "From :" + formLogin.textBox1.Text + ":To:" + s;
                    uChat.Show();
                }
                
        }

        private void PublicChat_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void clearChatboxToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                button1.PerformClick();
            }
        }





        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
    }
}
