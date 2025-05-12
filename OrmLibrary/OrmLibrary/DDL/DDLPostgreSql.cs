using MySql.Data.MySqlClient;
using Npgsql;
using OrmLibrary.DQL;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmLibrary.DDL
{
    class DDLPostgreSql : DDLBase
    {
        public DDLPostgreSql() : base() { }

        protected override DbConnection OpenConnection()
        {
            var conn = new NpgsqlConnection("Host=localhost;Port=5432;Database=postgres;User Id=postgres;Password=12345678;");
            conn.Open();
            return conn;
        }

        protected override DbCommand CreateCommand(DbConnection connection, string sql)
        {
            return new NpgsqlCommand(sql, (NpgsqlConnection)connection);
        }
    }
}


