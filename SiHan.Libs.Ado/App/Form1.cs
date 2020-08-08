using App.Libs;
using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SiHan.Libs.Ado;

namespace App
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                NpgsqlConnectionStringBuilder sb = new NpgsqlConnectionStringBuilder();
                sb.Username = "postgres";
                sb.Password = "123";
                sb.Host = "127.0.0.1";
                sb.Port = 5432;
                sb.Database = "testdb";
                string connString = sb.ToString();
                using (NpgsqlConnection conn = new NpgsqlConnection(connString))
                {
                    conn.Open();
                    //List<Account> accounts = await conn.SelectAsync<Account>("select * from account where username=@username;", new { username = "ygfweb" });
                    //foreach (var item in accounts)
                    //{
                    //    item.Email = "ygfweb@163.com";
                    //}
                    //await conn.UpdateAsync<Account>(accounts[0]);
                    //MessageBox.Show("成功");

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
