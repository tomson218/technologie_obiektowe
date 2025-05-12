using MySql.Data.MySqlClient;
using Npgsql;
using OrmLibrary.DQL;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmLibrary.DML
{
    class DMLPostgreSql : DMLBase
    {
        private string _connectionString;

        public DMLPostgreSql( ) : base()
        {
          
        }

        protected override DbConnection OpenConnection()
        {
            var conn = new NpgsqlConnection(GetConnectionString());
            conn.Open();
            return conn;
        }

        protected override DbCommand CreateCommand(DbConnection connection, string sql)
        {
            return new NpgsqlCommand(sql, (NpgsqlConnection)connection);

        }
    }
}
