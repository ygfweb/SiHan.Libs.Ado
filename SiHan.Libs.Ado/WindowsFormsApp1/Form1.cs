using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Npgsql;
using SiHan.Libs.Ado;
using WindowsFormsApp1.Libs;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.textBox1.Text = "MyOtherTClass1Name";
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            DbConnectionExtensions.DefaultMapScheme = MapScheme.UnderScoreCase;
            NpgsqlConnectionStringBuilder sb = new NpgsqlConnectionStringBuilder();
            sb.Database = "mydb";
            sb.Host = "127.0.0.1";
            sb.Port = 5432;
            sb.Password = "123";
            sb.Username = "postgres";
            using (NpgsqlConnection connection = new NpgsqlConnection(sb.ToString()))
            {
                connection.Open();
                for (int i = 0; i < 100; i++)
                {
                    Account account = new Account()
                    {
                        Id = Guid.NewGuid().ToString(),
                        CreatedTime = DateTime.Now,
                        Email = $"ygfweb{i.ToString()}@126.com",
                        LoginTime = DateTime.Now,
                        Money =Convert.ToDecimal(i*7.33),
                        Password = "123",
                        UserName = $"ygfweb{i}"
                    };
                    await connection.InsertAsync(account);
                }


              
                List<Account> accounts = await connection.SelectAsync<Account>("select * from account where money < @money;",new {money=10 });
                foreach (var item in accounts)
                {
                    item.UserName = "ygf";
                }
                using (var trans = connection.BeginTransaction())
                {
                    try
                    {
                        await connection.UpdateAsync<Account>(accounts,trans);
                        trans.Commit();
                    }
                    catch (Exception)
                    {
                        trans.Rollback();
                        throw;
                    }
                }
                
            }
            MessageBox.Show("成功");
        }
    }
}
