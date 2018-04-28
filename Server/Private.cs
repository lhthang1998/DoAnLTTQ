using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Server
{
    public partial class Private : Form
    {
        private readonly Form1 form;
        //private string to;
        private bool check = false;
        public Private(Form1 f1)
        {
            this.Icon = Properties.Resources.chat_client;
            InitializeComponent();
            form = f1;
        }

        private void Private_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(textBox1.Text!=null)
            {
                foreach(var client in from ListViewItem item in form.listView1.SelectedItems select(Client) item.Tag)
                {
                    client.SendData("pMessage|" + textBox1.Text);
                }
                richTextBox1.Text += "Server : " + textBox1.Text + "\r\n";
                textBox1.Text = string.Empty;
            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode==Keys.Enter)
            {
                button1.PerformClick();
            }
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            richTextBox1.SelectionStart = richTextBox1.TextLength;
        }

        public bool getCheck()
        {
            return check;
        }

        public void SetCheck()
        {
            check = false;
        }

        private void Private_FormClosing(object sender, FormClosingEventArgs e)
        {
            check = true;
            this.Dispose();
        }

        //public void setTo(string s)
        //{
        //    to = s;
        //}
    }
}
