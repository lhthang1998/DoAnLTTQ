using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Server
{
    class Server
    {
        SqlConnection connect;
        string source = @"Data Source=REBORN;Initial Catalog=Chat;Integrated Security=True";


        //Kiem tra xem User da ton tai hay chua
        public bool CheckUserExist(string name)
        {
            DataTable data = new DataTable();
            bool x;
            connect = new SqlConnection();
            connect.ConnectionString = source;
            connect.Open();
            SqlCommand cmd = new SqlCommand("CheckThemUser", connect);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            SqlParameter p = new SqlParameter("@Name", name);
            cmd.Parameters.Add(p);
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            adapter.Fill(data);
            if (data.Rows.Count > 0)
            {
                x = true;
            }
            else x = false;
            connect.Close();
            return x;
        }

        //Them moi User
        public void AddUser(string name,string pass)
        {
            
            connect = new SqlConnection();
            connect.ConnectionString = source;
            connect.Open();
            SqlCommand cmd = new SqlCommand("ThemUser", connect);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            SqlParameter p = new SqlParameter("@Name", name);
            cmd.Parameters.Add(p);
            p = new SqlParameter("@Pass", pass);
            cmd.Parameters.Add(p);
            cmd.ExecuteNonQuery();
            connect.Close();

        }

        //Kiem tra dang nhap co dung
        public bool CheckLogin(string name,string pass)
        {
            DataTable data = new DataTable();
            bool x;
            connect = new SqlConnection();
            connect.ConnectionString = source;
            connect.Open();
            SqlCommand cmd = new SqlCommand("CheckLogin", connect);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            SqlParameter p = new SqlParameter("@Name", name);
            cmd.Parameters.Add(p);
            p = new SqlParameter("@Pass", pass);
            cmd.Parameters.Add(p);
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            adapter.Fill(data);
            if (data.Rows.Count > 0)
            {
                x= true;
            }
            else x= false;
            connect.Close();
            return x;
        }

        //Them vao lich su chat
        public void AddHistory(string from,string to,string mess)
        {
            connect = new SqlConnection();
            connect.ConnectionString = source;
            connect.Open();
            SqlCommand cmd = new SqlCommand("InsertPrivateChat", connect);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            SqlParameter p = new SqlParameter("@From", from);
            cmd.Parameters.Add(p);
            p = new SqlParameter("@To",to);
            cmd.Parameters.Add(p);
            p = new SqlParameter("@Mess", mess);
            cmd.Parameters.Add(p);
            cmd.ExecuteNonQuery();
            connect.Close();
        }
        //Tra ve lich su chat theo yeu cau cua client
        public DataTable getHistory(string from,string to)
        {
            DataTable datatable=new DataTable();
            connect = new SqlConnection();
            connect.ConnectionString = source;
            connect.Open();
            SqlCommand cmd = new SqlCommand("History", connect);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            SqlParameter p = new SqlParameter("@From", from);
            cmd.Parameters.Add(p);
            p = new SqlParameter("@To", to);
            cmd.Parameters.Add(p);
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            adapter.Fill(datatable);
            connect.Close();
            return datatable;
        }
        //Xoa lich su chat
        public void DeleteHistory(string from,string to)
        {
            connect = new SqlConnection();
            connect.ConnectionString = source;
            connect.Open();
            SqlCommand cmd = new SqlCommand("DeleteHistory", connect);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            SqlParameter p = new SqlParameter("@From", from);
            cmd.Parameters.Add(p);
            p = new SqlParameter("@To", to);
            cmd.Parameters.Add(p);
            cmd.ExecuteNonQuery();
            connect.Close();
        }
    }
}
