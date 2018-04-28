using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Client
{
    class User
    {
        #region Thuoc tinh
        private string name;
        private string pass;
        private DBConnection db;

        public string Name { get => name; set => name = value; }
        public string Pass { get => pass; set => pass = value; }
        #endregion

        #region Phuong thuc
        public User(TextBox t)
        {
            db = new DBConnection(t);
            db.ConnectDatabase();
        }

        //Login
        public int Login(TextBox t1, TextBox t2)
        {

            string name = t1.Text;
            string pass = t2.Text;
            db.ConnectDatabase();
            int result = Check(name, pass);
            if (result > 0)
            {
                //Form f2 = new Form2();
                //f2.Show();
                return 1;
            }
            else
            {
                MessageBox.Show("Tên tài khoản hoặc mật khẩu đã sai! Vui lòng nhập lại");
                return 0;
            }
        }


        //Check user and password is correct
        int Check(string name, string pass)
        {
            string s = @"SELECT COUNT(*) FROM user WHERE username =@name and password=@pass";
            MySqlCommand cmd = new MySqlCommand(s, db.Conn);
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@pass", pass);
            var result = Convert.ToInt32(cmd.ExecuteScalar());
            return result;
        }


        //Sign up account
        public void SignUp(TextBox t1, TextBox t2)
        {
            string name = Convert.ToString(t1.Text);
            string pass = Convert.ToString(t2.Text);
            string saveStaff = "INSERT INTO `doanlttq`.`user` (`id`, `username`, `password`) VALUES(null,@username,@password)";
            db.ConnectDatabase();
            string s = @"SELECT COUNT(*) FROM user WHERE username =@name";
            MySqlCommand cmd = new MySqlCommand(s, db.Conn);
            cmd.Parameters.AddWithValue("@name", name);
            var result = Convert.ToInt32(cmd.ExecuteScalar());
            if (ValidatePassword(pass))
            {
                if (result > 0)
                {
                    MessageBox.Show("Tai khoan da ton tai!");
                }
                else
                {
                    MySqlCommand querySaveStaff = new MySqlCommand(saveStaff);
                    querySaveStaff.Connection = db.Conn;
                    querySaveStaff.Parameters.AddWithValue("@username", name);
                    querySaveStaff.Parameters.AddWithValue("@password", pass);
                    querySaveStaff.ExecuteNonQuery();
                    MessageBox.Show("Bạn đã đăng ký thành công!");
                }
                //close connection
                db.Conn.Close();
            }
            else
            {
                MessageBox.Show("Mật khẩu phải có ít nhất 4 ký tự, không quá 8 ký tự, bao gồm ít nhất 1 chữ in hoa, chữ thường, và một số");
            }
        }


        //Password Policy
        public bool ValidatePassword(string password)
        {
            string patternPassword = @"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{4,8}$";
            if (!string.IsNullOrEmpty(password))
            {
                if (!Regex.IsMatch(password, patternPassword))
                {
                    return false;
                }

            }
            return true;
        }
        #endregion
    }
}
