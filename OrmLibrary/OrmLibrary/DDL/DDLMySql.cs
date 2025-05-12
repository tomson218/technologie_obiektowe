using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace OrmLibrary.DDL
{
    class DDLMySql : DDLBase
    {
        public DDLMySql() : base() { }
        protected override DbConnection OpenConnection()
        {
            var conn = new MySqlConnection("server=127.0.0.1;uid=root;pwd=12345678;database=sys");
            conn.Open();
            return conn;
        }

        protected override DbCommand CreateCommand(DbConnection connection, string sql)
        {
            return new MySqlCommand(sql, (MySqlConnection)connection);
        }
    }
}
