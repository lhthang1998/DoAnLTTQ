using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Client
{
    public partial class pChatUser : Form
    {
        public PublicChat pChat;
        private string From;
        private string To;
        private bool check = false; // mac dinh khi chua co form
        public void setFrom(string s)
        {
            From = s;
        }

        public void setTo(string s)
        {
            To = s;
        }
        public pChatUser(PublicChat f1)
        {
            this.Icon = Properties.Resources.Buddy_Chat;
            InitializeComponent();
            pChat = f1;
        }

        private void pChatUser_Load(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != string.Empty)
            {
                pChat.formLogin.client.SendData("PrivateFromTo|" + From + "|" +To+"|"+ textBox1.Text);
                richTextBox1.Text += From + " :" + textBox1.Text + "\r\n";
                textBox1.Text = string.Empty;
            }
        }

        private void pChatUser_FormClosing(object sender, FormClosingEventArgs e)
        {
            check = true; // khi dong cua so chat thi khoi phuc check=false ->chua co cua so
            this.Dispose();
        }

        public bool getCheck()
        {
            return check;
        }

        public void setCheck()
        {
            check = false;
        }
    }
}
