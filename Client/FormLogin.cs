using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Client
{
    public partial class FormLogin : Form
    {
        #region Thuoctinh
        private string name;
        public ClientSetting client { get; set; }
        #endregion
        public FormLogin()
        {
            InitializeComponent();
            //user = new User(textBox2);
            client = new ClientSetting();
            this.Icon = Properties.Resources.chat;
            this.pictureBox1.BackgroundImage = Properties.Resources.LanChat;
        }

        private void FormLogin_Load(object sender, EventArgs e)
        {
           
        }
        private void ConnectServer()
        {

            client.Connected += ClientConnected;
            client.Connect(textBox3.Text, 2018);
            string s = textBox1.Text;
            //Gui thong bao Connect toi Server
            client.SendData("Connect|" + textBox1.Text + "|online"); 
            
        }
        private void ClientConnected(object sender, EventArgs e)
        {
            this.Invoke(Close);
        }
        private void button1_Click(object sender, EventArgs e)
        {
           
            //int i = user.Login(textBox1, textBox2);
            
            //if (i == 1)
            if(textBox1.Text!=string.Empty)
            {
                ConnectServer();
            }

        }

   

        private void FormLogin_FormClosed(object sender, FormClosedEventArgs e)
        {
       
        }


        


    }
}
