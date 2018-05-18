using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using WMPLib;
using System.Windows.Forms;

namespace Client
{
    public partial class PrivateChat : Form
    {
        #region thuoc tinh
        WindowsMediaPlayer wp = new WindowsMediaPlayer();
        public PublicChat pChat;
        private bool check = false;
        private string from="";
        #endregion
        public PrivateChat(PublicChat f1)
        {
            this.Icon = Properties.Resources.Noti;
            InitializeComponent();
            pChat = f1;
        }


        private void PrivateChat_Load(object sender, EventArgs e)
        {
            wp.URL = @"C:\Users\Asus\Desktop\ding.mp3";
            wp.controls.play();
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(textBox1.Text!=string.Empty)
            {
                pChat.formLogin.client.SendData("pMessage|" + from + "|" + textBox1.Text);
                richTextBox1.Text += from + " : " + textBox1.Text + "\r\n";
                textBox1.Text = string.Empty;
            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                button1.PerformClick();
            }
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            richTextBox1.SelectionStart = richTextBox1.TextLength;
        }

        private void PrivateChat_FormClosing(object sender, FormClosingEventArgs e)
        {
            check = true;
            this.Dispose();
        }
        public bool getCheck()
        {
            return check;
        }
        public void setCheck()
        {
            check=false;
        }

        public void setFrom(string s)
        {
            from = s;
        }

        
    }
}
