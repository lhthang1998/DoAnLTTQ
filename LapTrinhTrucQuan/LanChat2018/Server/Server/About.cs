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
    public partial class About : Form
    {
        private Main form;
        MultiLang ml = new MultiLang();
        public About(Main f1)
        {
            InitializeComponent();
            form = f1;
            if (form.Flag() == true)
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

        private void About_Load(object sender, EventArgs e)
        {
            label1.Text = "UIT LAN CHAT 2018";
            label2.Text = "Version 1.0";
        }
    }
}
