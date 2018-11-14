using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Server
{
    public enum eLanguage
    {
        TiengViet = 0,
        TiengAnh = 1
    }
    public class MultiLang
    {
        private DataSet ds = new DataSet();
        private int flag_language = (int)eLanguage.TiengViet;
        string from = "viet";
        string to = "anh";


        public MultiLang()
        {
            ds.ReadXml(Application.StartupPath + "\\Language\\Language.xml");
        }
        public void SetLanguage(int val)
        {
            this.flag_language = val;
            switch (this.flag_language)
            {
                case (int)eLanguage.TiengViet:
                    from = "viet";
                    to = "anh";
                    break;
                case (int)eLanguage.TiengAnh:
                    from = "anh";
                    to = "viet";
                    break;
            }
        }


        private void ChangeMenu(ToolStripMenuItem item)
        {
            item.Text = this.get_text(ds.Tables[0], to, item.Text, from);
            for (int i = 0; i < item.DropDown.Items.Count; i++)
            {
                ToolStripItem subItem = item.DropDown.Items[i];
                if (item is ToolStripMenuItem)
                {
                    ChangeMenu(subItem as ToolStripMenuItem);
                }
            }
        }

        private void ChangeButton(Control item)
        {
            if (item is Button)
                item.Text = this.get_text(ds.Tables[0], to, item.Text, from);

        }
        private void ChangeLabel(Control item)
        {
            if (item is Label)
                item.Text = this.get_text(ds.Tables[0], to, item.Text, from);

        }

        private void ChangeToolStrip(Control item)
        {
            if (item is ContextMenuStrip)
            {
                var temp = item as ContextMenuStrip;
                for (int i = 0; i < temp.Items.Count; i++)
                {
                    temp.Items[i].Text = get_text(ds.Tables[0], to, temp.Items[i].Text, from);
                }
            }
        }


        //Đổi ngôn ngữ
        public void ChangeLanguage(Form frm)
        {

            frm.Text = this.get_text(this.ds.Tables[0], to, frm.Text, from);
            foreach (Control control in frm.Controls)
            {

                switch (control.GetType().ToString())
                {
                    case "System.Windows.Forms.Label":
                    case "System.Windows.Forms.Button":
                        control.Text = this.get_text(this.ds.Tables[0], to, control.Text, from);
                        break;
                    case "System.Windows.Forms.MenuStrip":
                        MenuStrip menuControl = control as MenuStrip;
                        menuControl.Text = this.get_text(this.ds.Tables[0], to, menuControl.Text, from);
                        foreach (ToolStripMenuItem menu in menuControl.Items)
                        {
                            ChangeMenu(menu);
                        }

                        break;
                    case "System.Windows.Forms.ContextMenuStrip":
                        ChangeToolStrip(control);
                        break;
                    default:
                        break;

                }
            }
        }
        private string get_text(DataTable dt, string to, string text, string from)
        {
            string str = text;
            string dkt = to + "='" + text + "'";
            DataRow dataRow = this.getrowbyid(dt, dkt);
            if (dataRow != null)
            {
                str = dataRow[from].ToString();
            }
            return str.Trim();
        }

        public DataRow getrowbyid(DataTable dt, string exp)
        {
            DataRow dataRow;
            try
            {
                dataRow = dt.Select(exp)[0];
            }
            catch
            {
                dataRow = null;
            }
            return dataRow;
        }
    }
}
