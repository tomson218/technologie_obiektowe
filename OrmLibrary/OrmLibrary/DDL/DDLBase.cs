using Npgsql;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OrmLibrary.DDL
{
    public abstract class DDLBase
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

        protected DDLBase() { }

        protected abstract DbConnection OpenConnection();
        protected abstract DbCommand CreateCommand(DbConnection connection, string sql);

        protected virtual string CheckIsTableExists(string tableName, string columnsDefinition)
        {
            return $"CREATE TABLE IF NOT EXISTS {tableName} ({columnsDefinition});";
        }

        public virtual string ConvertType(Type type)
        {
            if (type == typeof(string)) return "text";
            else if (type == typeof(bool)) return "boolean";
            else if (type == typeof(short)) return "smallint";
            else if (type == typeof(int)) return "integer";
            else if (type == typeof(long)) return "bigint";
            else if (type == typeof(float)) return "real";
            else if (type == typeof(double)) return "double";
            else if (type == typeof(byte[])) return "bytea";
            else if (type == typeof(DateTime)) return "Timestamp";
            else if (type.IsClass) return "smallint";
            else return "";
        }

        public IEnumerable<Type> GetAllClassesFromNamespace(string targetNamespace)
        {
            Assembly assembly = Assembly.Load(targetNamespace);

            var modelTypes = assembly.GetTypes()
                .Where(t => t.IsClass &&
                            t.Namespace != null &&
                            t.Namespace.StartsWith(targetNamespace) &&
                            !t.IsAbstract &&
                            !t.Name.Contains("DisplayClass") &&
                            t.Name != "Program");
            return modelTypes;
        }
 
        public void AddForeignKeys(List<(string, string, string, string)> foreignKeys)
        {
            try
            {
                using var connection = OpenConnection();

                foreach (var item in foreignKeys)
                {
                    var table1 = item.Item1;
                    var column1 = item.Item2;
                    var table2 = item.Item3;
                    var column2 = item.Item4;

                    string sql = $"ALTER TABLE {table1} ADD CONSTRAINT {column1}_fk FOREIGN KEY ({column1}) REFERENCES {table2} ({column2});";
                    Console.WriteLine(sql);
                    using var cmd = CreateCommand(connection, sql);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public void CreateAllTables(string targetNamespace)
        {
            List<(string, string, string, string)> allForeignKeys = new List<(string, string, string, string)>();

            var modelTypes = GetAllClassesFromNamespace(targetNamespace);

            foreach (var type in modelTypes)
            {
                var createTableMethod = this.GetType().GetMethod("CreateTable");
                var genericCreateTableMethod = createTableMethod.MakeGenericMethod(type);
                object result = genericCreateTableMethod.Invoke(this, null);
                var tableInfo = result as List<(string, string, string, string)>;

                if (tableInfo != null)
                {
                    Console.WriteLine($"Tabela dla {type.Name} została utworzona z następującymi danymi:");
                    foreach (var (item1, item2, item3, item4) in tableInfo)
                    {
                        Console.WriteLine($"Type: {item1}, Field: {item2} {item3} {item4}");
                        allForeignKeys.Add((item1, item2, item3, item4));
                    }
                }
            }
            AddForeignKeys(allForeignKeys);
        }

        public List<(string, string, string, string)> CreateTable<T>() where T : class
        {
            List<(string, string, string, string)> foreignKeys = new List<(string, string, string, string)>();
            try
            {
                using var connection = OpenConnection();
                Dictionary<string, string> columns = new Dictionary<string, string>();

                Type tableType = typeof(T);
                string tableName = tableType.Name;
                var propertiesInfo = tableType.GetProperties();

                foreach (PropertyInfo prop in propertiesInfo)
                {
                    string columnDefinition = $"{prop.Name} {ConvertType(prop.PropertyType)}";

                    if (prop.Name.ToLower() == "id")
                    {
                        columnDefinition += " PRIMARY KEY";
                    }

                    columns.Add(prop.Name, columnDefinition);

                    if (prop.PropertyType.IsClass && prop.PropertyType != typeof(string))
                    {
                        foreignKeys.Add((tableName, prop.Name, prop.PropertyType.Name, "id"));
                    }
                }

                string columnsDefinition = string.Join(", ", columns.Select(c => c.Value));

                string sql = CheckIsTableExists(tableName, columnsDefinition);

                using var cmd = CreateCommand(connection, sql);

                cmd.ExecuteNonQuery();

                Console.WriteLine("The tables have been created successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            return foreignKeys;
        }
    }
}