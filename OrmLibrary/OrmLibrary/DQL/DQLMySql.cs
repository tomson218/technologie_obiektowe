using MySql.Data.MySqlClient;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmLibrary.DQL
{
    public class DQLMySql : DQLBase
    {
        private string _connectionString;

        public DQLMySql() : base()
        {
        }

        protected override DbConnection OpenConnection()
        {
            var conn = new MySqlConnection(GetConnectionString());
            conn.Open();
            return conn;

        }

        protected override DbCommand CreateCommand(DbConnection connection, string sql)
        {
            return new MySqlCommand(sql, (MySqlConnection)connection);
        }
    }
}
