using OrmLibrary.DQL;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using MySql.Data.MySqlClient;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using MySqlX.XDevAPI.Relational;

namespace OrmLibrary.DDL
{
    class DDLMicrosoftSql : DDLBase
    {
        public DDLMicrosoftSql() : base() { }

        public override string ConvertType(Type type)
        {
            if (type == typeof(string)) return "NVARCHAR(MAX)";
            else if (type == typeof(int)) return "int";
            else if (type == typeof(DateTime)) return "DateTime";
            else if (type == typeof(bool)) return "boolean";
            else if (type == typeof(short)) return "smallint";
            else if (type == typeof(long)) return "bigint";
            else if (type == typeof(float)) return "real";
            else if (type == typeof(double)) return "double";
            else if (type == typeof(byte[])) return "bytea";
   
            else if (type.IsClass) return "smallint";
            else return "";
        }

        protected override DbConnection OpenConnection()
        {
            SqlConnection connection = new SqlConnection("Server=localhost,1433;Database=testowa;User Id=sa;Password=Str0ng!Passw0rd;TrustServerCertificate=True;");
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

        protected override string CheckIsTableExists(string tableName, string columnsDefinition)
        {
            return $@"
            IF OBJECT_ID('dbo.{tableName}', 'U') IS NULL
            BEGIN
                CREATE TABLE {tableName} ({columnsDefinition})
            END";
        }
    }
    
}
