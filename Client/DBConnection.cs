using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Client
{
    class DBConnection
    {
        #region Thuoc tinh
        private static int userint = 0;
        private static int passwordint = 0;
        private MySqlCommand cmd;
        private MySqlConnection conn;

        public MySqlConnection Conn { get => conn; set => conn = value; }
        public MySqlCommand Cmd { get => cmd; set => cmd = value; }
        #endregion

        #region Constructor
        public DBConnection()
        {

        }
        public DBConnection(TextBox t1)
        {
            t1.PasswordChar = '*';
        }
        #endregion

        #region Phuong thuc

        //Method Connect to Database
        public void ConnectDatabase()
        {
            String server = "localhost";
            String database = "doanlttq";
            String uid = "root";
            String password = "1231301211";
            string connectionString;
            connectionString = "SERVER=" + server + ";" + "DATABASE=" + database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";";

            Conn = new MySqlConnection(connectionString);
            //open connection
            Conn.Open();

        }


        //Load user from Database
        public void LoadUser(ListBox lb1)
        {
            lb1.Items.Clear();
            ConnectDatabase();
            string s = "Select username from user";
            MySqlCommand command = new MySqlCommand(s, Conn);
            MySqlDataReader myReader;

            try
            {
                myReader = command.ExecuteReader();

                while (myReader.Read())
                {
                    string name = myReader.GetString("username");
                    lb1.Items.Add(name);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi");
            }
        }
        #endregion
    }
}
