using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using SiHan.Libs.Ado;
using SiHan.Libs.AdoTests1.Libs;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiHan.Libs.Ado.Tests
{
    [TestClass()]
    public class DbConnectionExtensionsTests
    {
        [AssemblyInitialize()]
        public static void CreateTable(TestContext context)
        {
            string sql = @"CREATE TABLE IF NOT EXISTS account (
                            id UUID PRIMARY KEY,
                            user_name VARCHAR ( 50 ) NOT NULL,
                            password text NOT NULL,
                            email text NOT NULL,
                            created_time TIMESTAMP NOT NULL,
                            login_time TIMESTAMP,
                            money numeric(18,2) not null default 0
                           );";
            using (var conn = DbFactory.Create())
            {
                conn.Open();
                DbCommand command = conn.CreateCommand();
                command.CommandText = sql;
                command.ExecuteNonQuery();
            }
        }

        [TestMethod()]

        public async Task SelectAsyncTest()
        {
            using (var connection = DbFactory.Create())
            {
                connection.Open();
                await connection.DeleteAllAsync<Account>();
                Account account = new Account
                {
                    Id = Guid.NewGuid().ToString(),
                    CreatedTime = DateTime.Now,
                    Email = "abc@google.com",
                    LoginTime = DateTime.Now,
                    Money = 15,
                    Password = "123",
                    UserName = "abc"
                };
                await connection.InsertAsync(account);
                List<Account> accounts = await connection.SelectAsync<Account>("select * from account where user_name=@UserName;", new { UserName = account.UserName });
                Assert.IsTrue(accounts.Count == 1 && accounts[0].Id == account.Id);
            }
        }

        [TestMethod()]
        public async Task DeleteAllAsyncTest()
        {
            using (var connection = DbFactory.Create())
            {
                connection.Open();
                Account account = new Account
                {
                    Id = Guid.NewGuid().ToString(),
                    CreatedTime = DateTime.Now,
                    Email = "abc@google.com",
                    LoginTime = DateTime.Now,
                    Money = 15,
                    Password = "123",
                    UserName = "abc"
                };
                await connection.InsertAsync(account);
                int rows = await connection.DeleteAllAsync<Account>();
                int count = await connection.CountAsync<Account>();
                Assert.IsTrue(count == 0);
            }
        }

        [TestMethod()]
        public async Task GetAllAsyncTest()
        {
            using (var conn = DbFactory.Create())
            {
                conn.Open();
                await conn.DeleteAllAsync<Account>();
                for (int i = 0; i < 5; i++)
                {
                    Account account = new Account
                    {
                        Id = Guid.NewGuid().ToString(),
                        CreatedTime = DateTime.Now,
                        Email = "abc@google.com",
                        LoginTime = DateTime.Now,
                        Money = 15,
                        Password = "123",
                        UserName = "abc"
                    };
                    await conn.InsertAsync(account);
                }
                List<Account> accounts = await conn.GetAllAsync<Account>();
                Assert.IsTrue(accounts.Count == 5);
            }
        }

        [TestMethod()]
        public async Task SingleByIdAsyncTest()
        {
            using (var conn = DbFactory.Create())
            {
                conn.Open();
                await conn.DeleteAllAsync<Account>();
                List<Account> accounts = new List<Account>();
                for (int i = 0; i < 5; i++)
                {
                    Account account = new Account
                    {
                        Id = Guid.NewGuid().ToString(),
                        CreatedTime = DateTime.Now,
                        Email = "abc@google.com",
                        LoginTime = DateTime.Now,
                        Money = 15,
                        Password = "123",
                        UserName = "abc" + i
                    };
                    accounts.Add(account);
                }
                await conn.InsertAsync(accounts);
                Account one = await conn.SingleByIdAsync<Account>(accounts[0].Id);
                Assert.IsTrue(one.UserName == accounts[0].UserName);
            }
        }

        [TestMethod()]
        public async Task FirstOrDefaultAsyncTest()
        {
            using (var conn = DbFactory.Create())
            {
                conn.Open();
                await conn.DeleteAllAsync<Account>();
                List<Account> accounts = new List<Account>();
                for (int i = 0; i < 5; i++)
                {
                    Account account = new Account
                    {
                        Id = Guid.NewGuid().ToString(),
                        CreatedTime = DateTime.Now,
                        Email = "abc@google.com",
                        LoginTime = DateTime.Now,
                        Money = 15,
                        Password = "123",
                        UserName = "abc" + i
                    };
                    accounts.Add(account);
                }
                await conn.InsertAsync(accounts);
                string userName = "abc2";
                Account one = await conn.FirstOrDefaultAsync<Account>("SELECT * FROM account WHERE user_name = @p;", new { p = userName });
                Assert.IsTrue(one.Id == accounts.FirstOrDefault(p => p.UserName == userName).Id);
            }
        }
        [TestMethod()]
        public async Task FirstOrDefaultNullValueAsyncTest()
        {
            using (var conn = DbFactory.Create())
            {
                conn.Open();
                await conn.DeleteAllAsync<Account>();
                List<Account> accounts = new List<Account>();
                for (int i = 0; i < 5; i++)
                {
                    Account account = new Account
                    {
                        Id = Guid.NewGuid().ToString(),
                        CreatedTime = DateTime.Now,
                        Email = "abc@google.com",
                        LoginTime = DateTime.Now,
                        Money = 15,
                        Password = "123",
                        UserName = "abc" + i
                    };
                    accounts.Add(account);
                }
                await conn.InsertAsync(accounts);
                string userName = "ppp";
                Account one = await conn.FirstOrDefaultAsync<Account>("SELECT * FROM account WHERE user_name = @p;", new { p = userName });
                Assert.IsTrue(one == null);
            }
        }

        [TestMethod()]
        public async Task ScalarAsyncTest()
        {
            using (var conn = DbFactory.Create())
            {
                conn.Open();
                await conn.DeleteAllAsync<Account>();
                List<Account> accounts = new List<Account>();
                for (int i = 0; i < 5; i++)
                {
                    Account account = new Account
                    {
                        Id = Guid.NewGuid().ToString(),
                        CreatedTime = DateTime.Now,
                        Email = "abc@google.com",
                        LoginTime = DateTime.Now,
                        Money = 15,
                        Password = "123",
                        UserName = "abc" + i
                    };
                    accounts.Add(account);
                }
                await conn.InsertAsync(accounts);
                decimal sum = await conn.ScalarAsync<decimal>("SELECT sum(money) FROM account;");
                Assert.IsTrue(sum == 75);
            }
        }

        [TestMethod()]
        public async Task ExecuteNonQueryAsyncTest()
        {
            using (var conn = DbFactory.Create())
            {
                conn.Open();
                await conn.DeleteAllAsync<Account>();
                Account account = new Account
                {
                    Id = Guid.NewGuid().ToString(),
                    CreatedTime = DateTime.Now,
                    LoginTime = DateTime.Now,
                    Email = "aaa@gmail.com",
                    Money = 40,
                    Password = "123",
                    UserName = "abc"
                };
                await conn.InsertAsync(account);
                string newEmail = "bbb@gmail.com";
                await conn.ExecuteNonQueryAsync($"UPDATE account SET email = '{newEmail}' WHERE user_name='{account.UserName}';");
                Account newObj = await conn.SingleByIdAsync<Account>(account.Id);
                Assert.IsTrue(newObj.Email == newEmail);
            }
        }

        [TestMethod()]
        public async Task DeleteAsyncTest()
        {
            using (var conn = DbFactory.Create())
            {
                conn.Open();
                await conn.DeleteAllAsync<Account>();
                Account account = new Account
                {
                    Id = Guid.NewGuid().ToString(),
                    CreatedTime = DateTime.Now,
                    LoginTime = DateTime.Now,
                    Email = "aaa@gmail.com",
                    Money = 10,
                    Password = "123",
                    UserName = "abc"
                };
                await conn.InsertAsync(account);
                int rowNum = await conn.DeleteAsync(account);
                Account newObj = await conn.SingleByIdAsync<Account>(account.Id);
                Assert.IsTrue(newObj == null && rowNum == 1);
            }
        }

        [TestMethod()]
        public async Task DeleteListAsyncTest()
        {
            using (var conn = DbFactory.Create())
            {
                conn.Open();
                await conn.DeleteAllAsync<Account>();
                List<Account> accounts = new List<Account>();
                for (int i = 0; i < 10; i++)
                {
                    Account account = new Account
                    {
                        Id = Guid.NewGuid().ToString(),
                        CreatedTime = DateTime.Now,
                        LoginTime = DateTime.Now,
                        Email = $"a{i}@gmail.com",
                        Money = 10,
                        Password = "123",
                        UserName = "user" + i
                    };
                    accounts.Add(account);
                }
                int count = 0;
                int rowNum = 0;
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        count = await conn.InsertAsync(accounts, trans);
                        trans.Commit();
                    }
                    catch (Exception)
                    {
                        trans.Rollback();
                        throw;
                    }
                }
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        rowNum = await conn.DeleteAsync(accounts, trans);
                        trans.Commit();
                    }
                    catch (Exception)
                    {
                        trans.Rollback();
                        throw;
                    }
                }
                int newCount = await conn.CountAsync<Account>();
                Assert.IsTrue(count == 10 && rowNum == 10 && newCount == 0);
            }
        }

        [TestMethod()]
        public async Task DeleteByIdAsyncTest()
        {
            using (var conn = DbFactory.Create())
            {
                conn.Open();
                await conn.DeleteAllAsync<Account>();
                Account account = new Account
                {
                    Id = Guid.NewGuid().ToString(),
                    CreatedTime = DateTime.Now,
                    Email = "abc@hotmail.com",
                    LoginTime = DateTime.Now,
                    Money = 20,
                    Password = "abc",
                    UserName = "abc"
                };
                int addNum = await conn.InsertAsync(account);
                int delNum = await conn.DeleteByIdAsync<Account>(account.Id);
                Account obj = await conn.SingleByIdAsync<Account>(account.Id);
                Assert.IsTrue(addNum == 1 && delNum == 1 && obj == null);
            }
        }

        [TestMethod()]
        public async Task UpdateAsyncTest()
        {
            using (var conn = DbFactory.Create())
            {
                conn.Open();
                await conn.DeleteAllAsync<Account>();
                Account account = new Account
                {
                    Id = Guid.NewGuid().ToString(),
                    CreatedTime = DateTime.Now,
                    Email = "abc@hotmail.com",
                    LoginTime = DateTime.Now,
                    Money = 20,
                    Password = "abc",
                    UserName = "abc"
                };
                await conn.InsertAsync(account);
                string newEmail = "abc@gmail.com";
                account.Email = newEmail;
                await conn.UpdateAsync(account);
                Account newObj = await conn.SingleByIdAsync<Account>(account.Id);
                Assert.IsTrue(newObj.Email == newEmail);
            }
        }

        [TestMethod()]
        public async Task UpdateListAsyncTest()
        {
            using (var conn = DbFactory.Create())
            {
                conn.Open();
                await conn.DeleteAllAsync<Account>();
                List<Account> accounts = new List<Account>();
                for (int i = 0; i < 5; i++)
                {
                    Account account = new Account
                    {
                        Id = Guid.NewGuid().ToString(),
                        CreatedTime = DateTime.Now,
                        Email = "abc@hotmail.com",
                        LoginTime = DateTime.Now,
                        Money = i * 10,
                        Password = "abc",
                        UserName = "user" + i
                    };
                    accounts.Add(account);
                    await conn.InsertAsync(account);
                }
                for (int i = 0; i < accounts.Count; i++)
                {
                    accounts[i].Money = accounts[i].Money + 5;
                }
                int rowNum = 0;
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        rowNum = await conn.UpdateAsync(accounts, trans);
                        trans.Commit();
                    }
                    catch (Exception)
                    {
                        trans.Rollback();
                        throw;
                    }
                }
                Account newObj = await conn.SingleByIdAsync<Account>(accounts[1].Id);
                Assert.IsTrue(rowNum == accounts.Count && newObj.Money == accounts[1].Money);
            }
        }
    }
}


