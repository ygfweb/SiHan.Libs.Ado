using Npgsql;
using SiHan.Libs.Ado;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace SiHan.Libs.AdoTests1.Libs
{
    public static class DbFactory
    {
        private static string ConnectionString { get; }

        static DbFactory()
        {
            DbConnectionExtensions.DefaultMapScheme = MapScheme.UnderScoreCase;
            NpgsqlConnectionStringBuilder sb = new NpgsqlConnectionStringBuilder();
            sb.Database = "testdb";
            sb.Host = "127.0.0.1";
            sb.Port = 5432;
            sb.Password = "123";
            sb.Username = "postgres";
            ConnectionString = sb.ToString();
        }

        public static DbConnection Create()
        {
            return new NpgsqlConnection(ConnectionString);
        }
    }
}
