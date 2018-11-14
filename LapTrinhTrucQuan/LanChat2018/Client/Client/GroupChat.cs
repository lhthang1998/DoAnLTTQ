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
    public partial class GroupChat : Form
    {
        MainChat form;
        string name;
        string from;
        public GroupChat(MainChat f1)
        {
            InitializeComponent();
            form = f1;
            this.Icon = Properties.Resources.group_chat;
        }

        private void GroupChat_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(txtBox.Text!=string.Empty)
            {
                form.SendData("GroupChat|" + name + "|" + from + " : " + txtBox.Text);
                gchatBox.Text += from + " : " + txtBox.Text+"\r\n";
                txtBox.Text = string.Empty;
            }
        }

        public void setFrom(string s)
        {
            from = s;
        }

        public void setName(string s)
        {
            name = s;
        }

        public string getName()
        {
            return name;
        }

        private void GroupChat_FormClosing(object sender, FormClosingEventArgs e)
        {
            form.SendData("QuitGroupChat|" + name + "|" + from + "|"+" đã thoát khỏi phòng chat "+name );
            form.RemoveGroup(this);
        }

        private void txtBox_KeyDown(object sender, KeyEventArgs e)
        {
            if(txtBox.Text!=string.Empty)
            {
                if(e.KeyCode==Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    btnSend.PerformClick();
                }
            }
        }
    }
}
