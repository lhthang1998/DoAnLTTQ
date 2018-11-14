using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WMPLib;

namespace Client
{
    public partial class PrivateChat : Form
    {
        MultiLang ml = new MultiLang();
        private MainChat pChat;
        private string from;
        WindowsMediaPlayer wp = new WindowsMediaPlayer();
        public PrivateChat(MainChat f1)
        {
            InitializeComponent();
            pChat = f1;
            if(pChat.getFlag()==true)
            {
                this.ml.SetLanguage((int)eLanguage.TiengViet);
                this.ml.ChangeLanguage(this);
            }
            else
            {
                this.ml.SetLanguage((int)eLanguage.TiengAnh);
                this.ml.ChangeLanguage(this);
            }
            this.Icon = Properties.Resources.Noti;
        }
        public void SetFrom(string s)
        {
            from = s;
        }

        public string getFrom()
        {
            return from;
        }

        private void PrivateChat_Load(object sender, EventArgs e)
        {
            Text = "Private with Server";
            wp.URL = Application.StartupPath + @"\Sounds\ding.mp3";
            wp.controls.play();
        }


        private void PrivateChat_FormClosing(object sender, FormClosingEventArgs e)
        {
            pChat.RemovePrivate(this);
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (textBox1.Text != string.Empty)
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    button1.PerformClick();
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != string.Empty)
            {
                pChat.SendData("pMessage|" + from + "|" + textBox1.Text);

                richTextBox1.Text += from + " : " + textBox1.Text + "\r\n";
                textBox1.Text = string.Empty;
            }
        }

        private void richTextBox1_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            Process p = Process.Start("Chrome.exe", e.LinkText);
        }
    }
}
