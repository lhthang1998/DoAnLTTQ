using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Server
{
    public partial class PrivateChat : Form
    {
        MultiLang ml = new MultiLang();
        private readonly Main form;
        private string to;
        public PrivateChat(Main f1)
        {
            InitializeComponent();
            form = f1;
            if(form.Flag()==true)
            {
                this.ml.SetLanguage((int)eLanguage.TiengViet);
                this.ml.ChangeLanguage(this);
            }
            else
            {
                this.ml.SetLanguage((int)eLanguage.TiengAnh);
                this.ml.ChangeLanguage(this);
            }
        }

        private void PrivateChat_Load(object sender, EventArgs e)
        {
            this.Icon = Properties.Resources.ChatClient;
            Text = "Private with " + to;
        }

        public string getTo()
        {
            return to;
        }
        public void SetTo(string s)
        {
            to = s;
        }

        private void PrivateChat_FormClosing(object sender, FormClosingEventArgs e)
        {
            form.Remove(this);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (txtBox.Text != null)
            {

                var item = form.userlist.FindItemWithText(to);
                if (item != null)
                {
                    item.Selected = true;
                    foreach (var client in from ListViewItem temp in form.userlist.SelectedItems select (User)temp.Tag)
                    {

                        client.SendData("pMessage|" + txtBox.Text + "|" + to);
                        chatBox.Text += "Server : " + txtBox.Text + "\r\n";

                    }
                    txtBox.Text = string.Empty;
                }
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
    }
}
