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

namespace Server
{
    public partial class Form1 : Form
    {

        #region Thuoc tinh
       
        private readonly Listener listener;
        private Private pChat;
        public List<Socket> clients = new List<Socket>(); //Luu tru tat ca cac Client vao list
        #endregion

        public void Broad(string s) //Server gui All client
        {
            foreach(var socket in clients)
            {
                try
                {
                    socket.Send(Encoding.ASCII.GetBytes(s));
                }
                catch(Exception)
                {

                }
            }
        }
        public Form1()
        {
            this.Icon = Properties.Resources.Home;
            pChat = new Private(this);
            InitializeComponent();
            listener = new Listener(2018);
            listener.SocketAccepted += listenerSocketAccepted;
  
        }

        private bool checkExist(string s)
        {
            for (int i=0;i<listView1.Items.Count;i++)
            {
                if (s == listView1.Items[i].SubItems[1].Text)
                    return true;
            }
            return false;
        }
        private void listenerSocketAccepted(Socket e)
        {
            var client = new Client(e);
            client.Received += clientReceived;
            this.Invoke(() =>
            {
                string ip = client.ip.ToString().Split(':')[0];
                var item = new ListViewItem(ip); // ip
                item.SubItems.Add(""); // nickname
                item.SubItems.Add(""); // status
                item.Tag = client;
                listView1.Items.Add(item);
                clients.Add(e);
            });
            client.Disconnected += clientDisconnected;
           
        }

        private void clientReceived(Client sender,byte []data)
        {
            this.Invoke(() =>
            {
                for(int i=0;i<listView1.Items.Count;i++)
                {
                    var client = listView1.Items[i].Tag as Client;
                    if (client == null || client.ip != sender.ip) continue;
                    var command = Encoding.ASCII.GetString(data).Split('|');
                    switch (command[0])
                    {
                        case "Connect":
                            //richTextBox1.Text += command[1] + " vua dang nhap \r\n";
                            //string s = command[1] + " vua dang nhap \r\n";
                            //GhiFile(DateTime.Now.ToString() + " " + s);
                            //listView1.Items[listView1.Items.Count - 1].SubItems[1].Text = command[1]; // Username
                            //listView1.Items[listView1.Items.Count - 1].SubItems[2].Text = command[2]; //Status
                            //Broad("RefreshChat|" + s);
                            break;
                        case "Public":
                            if (checkExist(command[1]) == true)
                            {
                                client = listView1.Items[listView1.Items.Count - 1].SubItems[1].Tag as Client;
                                client.SendData("Exist|");
                            }
                            else
                            {
                                listView1.Items[listView1.Items.Count - 1].SubItems[1].Text = command[1]; // Username
                                listView1.Items[listView1.Items.Count - 1].SubItems[2].Text = command[2];

                                string u = string.Empty;
                                for (int j = 0; j < listView1.Items.Count; j++)
                                {
                                    u += listView1.Items[j].SubItems[1].Text + "|";
                                }
                                Broad("Users|" + u.TrimEnd('|'));
                                string text = command[1] + "| vua vao phong chat";
                                richTextBox1.Text += command[1] + " vua vao phong chat \r\n";
                                Broad("Noti|" + text);

                            }
                           
                            break;

                        case "Message":
                            string v = command[1] + " : " + command[2] + "\r\n";
                            richTextBox1.Text += v;
                            Broad("RefreshChat|" + v);
                            break;
                        case "pMessage":
                            this.Invoke(() =>
                            {
                                if(pChat.getCheck()==true)
                                {
                                    pChat = new Private(this);
                                    pChat.SetCheck();
                                    pChat.Show(); 
                                }
                                pChat.richTextBox1.Text += command[1] + " : " + command[2]+"\r\n";
                            }
                            );
                            break;
                        case "Private":
                            string temp = command[1];
                            string temp2 = command[2];
                            //GhiFile2("da nhan duoc yeu cau tu "+temp+" den"+temp2);
                            Broad("FromTo|"+temp+"|"+temp2); // yeu cau tu temp den temp2
                            break;
                        case "PrivateFromTo":
                            Broad("PrivateMess|" + command[1] + "|" + command[2] + "|" + command[3]);
                            break;
                        case "pChat":
                            break;
                        case "Disconnect":   
                            break;


                    }
                }
            }
            );
        }

        private void clientDisconnected(Client sender)
        {
            this.Invoke(() =>
            {
                for(int i=0;i<listView1.Items.Count;i++)
                {
                    var client = listView1.Items[i].Tag as Client;
                    if(client.ip==sender.ip)
                    {
                        string s = listView1.Items[i].SubItems[1].Text + " da thoat ra \r\n";
                        richTextBox1.Text += s;
                        Broad("RefreshChat|" + s);
                        string user2 = string.Empty;
                       
                        listView1.Items.RemoveAt(i);
                        for (int j = 0; j < listView1.Items.Count; j++)
                        {
                            user2 += listView1.Items[j].SubItems[1].Text + "|";
                        }
                        Broad("Users|" + user2.TrimEnd('|'));
                    }
                }
            });
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            listener.Start();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            listener.Stop();
            Application.Exit();

        }
        //Chat public gui tat ca cac client
        private void button1_Click(object sender, EventArgs e)
        {
            if(textBox1.Text!=string.Empty)
            {
                string s = "Server : " + textBox1.Text;
                Broad("Message|" + s);
                richTextBox1.Text +=s + "\r\n";
                textBox1.Text = string.Empty;

            }
        }


        private void disconnectUserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach(var client in from ListViewItem item in listView1.SelectedItems select (Client) item.Tag)
            {
                client.SendData("Disconnect|");
            }
        }

        private void privateChatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (var client in from ListViewItem item in listView1.SelectedItems select (Client)item.Tag)
            {
                        
                client.SendData("Chat|");
                pChat = new Private(this);
                pChat.Show();
                
            }
        }

        private void clearChatBoxToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
        }

        // Save list nguoi dang nhap vao file txt
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using(SaveFileDialog file=new SaveFileDialog() { Filter = "Text Documents|*.txt", ValidateNames = true })
            {
                if(file.ShowDialog()==DialogResult.OK)
                {                  
                    using (StreamWriter sw = new StreamWriter(file.FileName,true))
                    {
                       sw.WriteLine(DateTime.Now.ToString() + "\n");
                       foreach (ListViewItem item in listView1.Items)
                       {
                            sw.WriteLine(item.SubItems[0].Text + ":" + item.SubItems[1].Text);
                       }
                       
                    }
                }
            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode==Keys.Enter)
            {
                e.SuppressKeyPress = true;
                button1.PerformClick();
            }
        }
    }
}
