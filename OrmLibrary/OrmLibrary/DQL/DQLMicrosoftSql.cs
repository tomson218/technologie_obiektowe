using ClientModel;
using Microsoft.Data.SqlClient;
using MySql.Data.MySqlClient;
using Mysqlx.Crud;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace OrmLibrary.DQL
{
    public class DQLMicrosoftSql : DQLBase
    {

        private string _connectionString;
        public DQLMicrosoftSql() : base()
        {
           
        }

        protected override DbConnection OpenConnection()
        {
            SqlConnection connection = new (GetConnectionString()); 
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

        protected override string PaginationSql(string tableName, int limit, int skip)
        {
            return $"SELECT * FROM {tableName} ORDER BY id OFFSET {skip} ROWS FETCH NEXT {limit} ROWS ONLY";
        }
    }
}

