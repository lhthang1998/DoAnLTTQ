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

namespace Client
{
    public partial class Login : Form
    {
        string ip;
        SignUp sign;
        public Login()
        {
            InitializeComponent();
            pictureBox1.Image = Properties.Resources.LanChat;
            Icon = Properties.Resources.chat;
            
        }
        void GhiFile(string s)
        {
            using (StreamWriter sw = new StreamWriter("Login.txt"))
            {


                sw.WriteLine(s);

            }
        }

        private void Login_Load(object sender, EventArgs e)
        {

        }

        private void Login_FormClosing(object sender, FormClosingEventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != string.Empty && textBox2.Text != string.Empty) ;
            {
                this.Hide();
                ip = textBox2.Text;
            }
                

        }
        public string IP()
        {
            return ip;
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if(textBox1.Text!=string.Empty)
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    button1.PerformClick();
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            sign = new SignUp(this);
            sign.Show();
        }
    }
}
