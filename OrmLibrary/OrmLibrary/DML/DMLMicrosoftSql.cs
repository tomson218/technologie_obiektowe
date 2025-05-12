using OrmLibrary.DQL;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using MySql.Data.MySqlClient;

namespace OrmLibrary.DML
{
    public class DMLMicrosoftSql : DMLBase
    {
        private string _connectionString;

        public DMLMicrosoftSql() : base()
        {
           
        }

        protected override DbConnection OpenConnection()
        {
            SqlConnection connection = new(GetConnectionString());
            connection.Open();
            return connection;
        }

        protected override DbCommand CreateCommand(DbConnection connection, string sql)
        {
            SqlCommand command = new SqlCommand(sql, (SqlConnection)connection);
            command.CommandTimeout = 15;
            command.CommandType = CommandType.Text;
            return command;
        }
    }
}
