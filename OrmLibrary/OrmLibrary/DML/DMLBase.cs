using Google.Protobuf.WellKnownTypes;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Type = System.Type;

namespace OrmLibrary.DML
{
    public abstract class DMLBase
    {
        protected static string ConnectionString { get; private set; }

        public static void SetConnectionString(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public static string GetConnectionString()
        {
            return ConnectionString;
        }

        protected DMLBase() { }
        private string GetTableName<T>() where T : class
        {
            Type tableType = typeof(T);
            string tableName = tableType.Name;
            return tableName;
        }

        protected abstract DbConnection OpenConnection();

        protected abstract DbCommand CreateCommand(DbConnection connection, string sql);

       

        void PrintAllProperties(dynamic o)
        {
            var type = o.GetType();
            var columns = new List<string>();

            foreach (var prop in type.GetProperties())
            {
                string name = prop.Name;
                columns.Add(name);
                object value = prop.GetValue(o);
            }
        }

        public object CheckAndConvert<T>(KeyValuePair<string, object> item)
        {
            object value = item.Value;

            Type tableType = typeof(T);
            string tableName = tableType.Name;
            PropertyInfo propertyInfo = tableType?.GetProperty(item.Key);

            if (propertyInfo != null && item.Value.GetType() != propertyInfo.PropertyType)
            {
                object convertedValue = Convert.ChangeType(item.Value, propertyInfo.PropertyType);
                value = Convert.ChangeType(item.Value, propertyInfo.PropertyType);
                Console.WriteLine($"Konwersja udana. Wynik: {convertedValue} (Typ: {convertedValue.GetType()})");
            }

            return value;
        }

        public void InsertData<T>(Dictionary<string, object> newData) where T : class
        {
            try
            {
                string tableName = GetTableName<T>();
                string columns = String.Join(", ", newData.Keys);
                string values = String.Join(", ", newData.Keys.Select(k => "@" + k));
                var sql = $@"INSERT INTO {tableName} 
                           ({columns}) VALUES ({values})";

                using var connection = OpenConnection();
             
                using var cmd = CreateCommand(connection, sql);

                foreach (var item in newData)
                {
                    object value = CheckAndConvert<T>(item);
                    var parameter = cmd.CreateParameter();
                    parameter.ParameterName = $"@{item.Key}";
                    parameter.Value = value;
                    cmd.Parameters.Add(parameter);
                }

                cmd.ExecuteNonQuery();
            }
            catch (DbException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public void InsertMany<T>(List<Dictionary<string, object>> manyData) where T : class
        {
            try
            {
                var firstDictionary = manyData.First();
                string tableName = GetTableName<T>();
                string columns = String.Join(", ", firstDictionary.Keys);
                var valuesList = manyData.Select((book, index) =>
                    "(" + String.Join(", ", book.Keys.Select(k => $"@{k}{index}")) + ")"
                );
                string values = String.Join(", ", valuesList);

                using var connection = OpenConnection();
                var sql = $@"INSERT INTO {tableName} 
                           ({columns}) VALUES {values}";

                using var cmd = CreateCommand(connection, sql);

                int recordIndex = 0;
                foreach (var book in manyData)
                {
                    foreach (var kvp in book)
                    {
                        object value = CheckAndConvert<T>(kvp);
                        var parameter = cmd.CreateParameter();
                        parameter.ParameterName = $"@{kvp.Key}{recordIndex}";
                        parameter.Value = value;
                        cmd.Parameters.Add(parameter);
                    }
                    recordIndex++;
                }

                cmd.ExecuteNonQuery();
            }
            catch (DbException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public void UpdateData<T>(int id, Dictionary<string, object> updatedData) where T : class
        {
            try
            {
                string tableName = GetTableName<T>();
                using var connection = OpenConnection();
                string columns = String.Join(", ", updatedData.Keys.Select(key => $"{key} = @{key}"));

                var sql = $@"UPDATE {tableName} 
                            SET {columns} 
                            WHERE id = @id";

                using var cmd = CreateCommand(connection, sql);

                foreach (var item in updatedData)
                {
                    object value = CheckAndConvert<T>(item);

                    var parameter = cmd.CreateParameter();
                    parameter.ParameterName = $"@{item.Key}";
                    parameter.Value = value;

                    cmd.Parameters.Add(parameter);
                }

                var idParameter = cmd.CreateParameter();
                idParameter.ParameterName = "@id";
                idParameter.Value = id;
                cmd.Parameters.Add(idParameter);

                cmd.ExecuteNonQuery();
                Console.WriteLine("The row has been updated successfully.");
            }

            catch (DbException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public void DeleteAllRecords<T>() where T : class
        {
            try
            {
                string tableName = GetTableName<T>();
                var sql = $"DELETE FROM {tableName}";
                using var connection = OpenConnection();
                using var cmd = CreateCommand(connection, sql);

                cmd.ExecuteNonQuery();
                Console.WriteLine("The row has been deleted successfully.");
            }

            catch (DbException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public void DeleteById<T>(int id) where T : class
        {
            try
            {
                string tableName = GetTableName<T>();
                var sql = $@"DELETE FROM {tableName} 
                            WHERE id = @id";
                using var connection = OpenConnection();
                using var cmd = CreateCommand(connection, sql);
                
                var idParameter = cmd.CreateParameter();
                idParameter.ParameterName = "@id";
                idParameter.Value = id;
                cmd.Parameters.Add(idParameter);

                cmd.ExecuteNonQuery();
                Console.WriteLine("The row has been deleted successfully.");
            }

            catch (DbException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
