using MySql.Data.MySqlClient;
using OrmLibrary.DDL;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmLibrary.DML
{
    class DMLMySql : DMLBase
    {

        private string _connectionString;

        public DMLMySql() : base()
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
