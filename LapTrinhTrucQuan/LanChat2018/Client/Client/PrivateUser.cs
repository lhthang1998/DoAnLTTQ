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
    public partial class PrivateUser : Form
    {
        MultiLang ml = new MultiLang();
        private MainChat form;
        WindowsMediaPlayer wp = new WindowsMediaPlayer();
        private string from;
        private string to;
        
        public PrivateUser(MainChat f1)
        {
            InitializeComponent();
            form = f1;
            if (form.getFlag() == true)
            {
                this.ml.SetLanguage((int)eLanguage.TiengViet);
                this.ml.ChangeLanguage(this);
            }
            else
            {
                this.ml.SetLanguage((int)eLanguage.TiengAnh);
                this.ml.ChangeLanguage(this);
            }
            this.Icon = Properties.Resources.Private_Chat;
        }
        public void SetFrom(string s)
        {
            from = s;
        }
        public void SetTo(string s)
        {
            to = s;
        }
        public string From()
        {
            return from;
        }
        public string To()
        {
            return to;
        }
        private void PrivateUser_Load(object sender, EventArgs e)
        {
           
            wp.URL = Application.StartupPath + @"\Sounds\ding.mp3";
            wp.controls.play();
        }


        private void PrivateUser_FormClosing(object sender, FormClosingEventArgs e)
        {
            form.Remove(this);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != string.Empty)
            {
                form.SendData("PrivateFromTo|" + from + "|" + to + "|" + textBox1.Text);
                richTextBox1.Text += from + " : " + textBox1.Text + "\r\n";
                textBox1.Text = string.Empty;
            }
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

        private void richTextBox1_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            Process p = Process.Start("Chrome.exe", e.LinkText);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            
        }

        public void LoadHistory()
        {
            form.SendData("History|" + from + "|" + to);
        }
    }
}
