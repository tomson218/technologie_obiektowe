using Npgsql;
using System;
using System.Collections.Generic;
using System.Data.Common;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mysqlx.Crud;
using System.Diagnostics;
using System.Security.Policy;
using static Mysqlx.Expect.Open.Types.Condition.Types;
using Npgsql.Internal;
using MySqlX.XDevAPI.Common;
using Mysqlx.Session;
using static Mysqlx.Expect.Open.Types;

namespace OrmLibrary.DQL
{
    public abstract class DQLBase
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

        protected DQLBase() { }

        private List<Dictionary<string, object>> AddAllDataToList(DbDataReader dataReader)
        {            
            var allRowsData = new List<Dictionary<string, object>>();

            while (dataReader.Read())
            {
                var rowData = new Dictionary<string, object>();

                for (int i = 0; i < dataReader.FieldCount; i++)
                {
                    rowData[dataReader.GetName(i)] = dataReader.GetValue(i);
                }
              
                allRowsData.Add(rowData);
            }

            return allRowsData;
        }

        public void ShowAllDataToList(List<Dictionary<string, object>> allRowsData)
        {
            foreach (var row in allRowsData)
            {
                foreach (var item in row)
                {
                    Console.WriteLine($"{item.Key}: {item.Value}");
                }
            }
        }

        public string GetAggregatePair(Dictionary<string, string> aggregation)
        {
            List<string> aggregationList = new List<string>();

            foreach (KeyValuePair<string, string> items in aggregation)
            {
                aggregationList.Add($"{items.Value}({items.Key})");
            }

            string resultNames = String.Join(", ", aggregationList);
            return resultNames;

        }

        public string GetTableName<T>() where T : class
        {
            Type tableType = typeof(T);
            string tableName = tableType.Name;
            return tableName;
        }

        public (string Key, object Value) GetKeyValue(Dictionary<string, object> dictionary)
        {
            foreach (var (key, value) in dictionary)
            {
                return (key, value);
            }
            return (null, null);
        }

        protected abstract DbConnection OpenConnection();

        protected virtual string PaginationSql(string tableName, int limit, int skip)
        {
            return $@"SELECT * 
                    FROM {tableName} 
                    LIMIT {limit} 
                    OFFSET {skip};";
        }

        protected abstract DbCommand CreateCommand(DbConnection connection, string sql);

        public virtual List<Dictionary<string, object>> GetData<T>(string sql) where T : class
        {
            var allRowsData = new List<Dictionary<string, object>>();
            try
            {
                string tableName = GetTableName<T>();
                using var connection = OpenConnection();
                using var cmd = CreateCommand(connection, sql);
                using var reader = cmd.ExecuteReader();
                allRowsData = AddAllDataToList(reader);
                ShowAllDataToList(allRowsData);
            }
            catch (DbException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            return allRowsData;
        }


        public List<Dictionary<string, object>> GetAllWithWhere<T>() where T : class
            {
            return GetData<T>("SELECT * FROM Book WHERE Publisher = 'Publisher C'");
            }


        public List<Dictionary<string, object>> GetAll<T>() where T : class
        {
            string tableName = GetTableName<T>();
            return GetData<T>($@"SELECT * 
                          FROM {tableName}");
        }

        public List<Dictionary<string, object>> GetAll<T>(params string[] fields) where T : class
        {

            string tableName = GetTableName<T>();
            string chosenFields = string.Join(", ", fields);
            return GetData<T>($@"SELECT {chosenFields} 
                            FROM {tableName}");
        }


        public virtual List<Dictionary<string, object>> GetById<T>(int id) where T : class
        {
            string tableName = GetTableName<T>();

            return GetData<T>($@"SELECT * 
                            FROM {tableName} 
                            WHERE id={id}");
           
        }

        public List<Dictionary<string, object>> GetById<T>(int id, params string[] fields) where T : class
        {
            string tableName = GetTableName<T>();

            return GetData<T>($@"SELECT {string.Join(", ", fields)} 
                            FROM {tableName} 
                            WHERE id={id}");
        }

        public List<Dictionary<string, object>> GetAllPagination<T>(int limit, int skip = 0) where T : class
        {
            string tableName = GetTableName<T>();
            return GetData<T>(PaginationSql(tableName, limit, skip));
        }


        public List<Dictionary<string, object>> GetAllGroupBy<T>(string[] groupAttr, Dictionary<string, string> aggregation) where T : class
        {
            string tableName = GetTableName<T>();
            string resultGroupAttr = String.Join(", ", groupAttr);
            string resultNames = GetAggregatePair(aggregation);
            return GetData<T>($@"SELECT {resultGroupAttr}, {resultNames} 
                            FROM {tableName}
                            GROUP BY {resultGroupAttr}");
        }

        public List<Dictionary<string, object>> GetFirstBy<T>(string groupName) where T : class
        {
            string tableName = GetTableName<T>();
            return GetData<T>($"WITH ranked_books AS ( SELECT publisher, id, date, ROW_NUMBER() OVER (PARTITION BY publisher " +
                    $"ORDER BY date ASC) AS rn FROM Book) SELECT id, date, publisher FROM ranked_books WHERE rn = 1;");
        }

        public List<Dictionary<string, object>> GetAllGroupByWithGreater<T>(string[] groupAttr, Dictionary<string, string> aggregation, Dictionary<string, object> conditionals) where T : class
        {
            string tableName = GetTableName<T>();
            string stringGroupAttr = String.Join(", ", groupAttr);
            string stringAggregation = GetAggregatePair(aggregation);
            var result = GetKeyValue(conditionals);
            return GetData<T>($@"SELECT {stringGroupAttr}, {stringAggregation} 
                            FROM {tableName}
                            WHERE {result.Key} > {result.Value}                            
                            GROUP BY {stringGroupAttr}");
        }

        public List<Dictionary<string, object>> GetAllGroupByWithGreaterOrEqual<T>(string[] groupAttr, Dictionary<string, string> aggregation, Dictionary<string, object> conditionals) where T : class
        {
            string tableName = GetTableName<T>();
            string stringGroupAttr = String.Join(", ", groupAttr);
            string stringAggregation = GetAggregatePair(aggregation);
            var result = GetKeyValue(conditionals);

            return GetData<T>($@"SELECT {stringGroupAttr}, {stringAggregation} 
                            FROM {tableName}
                            W WHERE {result.Key} >= {result.Value}                             
                            GROUP BY {stringGroupAttr}");
        }

        public List<Dictionary<string, object>> GetAllGroupByWithEqual<T>(string[] groupAttr, Dictionary<string, string> aggregation, Dictionary<string, object> conditionals) where T : class
        {
            string tableName = GetTableName<T>();
            string stringGroupAttr = String.Join(", ", groupAttr);
            string stringAggregation = GetAggregatePair(aggregation);
            var result = GetKeyValue(conditionals);

            return GetData<T>($@"SELECT {stringGroupAttr}, {stringAggregation} 
                            FROM {tableName}
                            WHERE {result.Key} =  {result.Value}                           
                            GROUP BY {stringGroupAttr}");
        }

        public List<Dictionary<string, object>> GetAllGroupByWithLess<T>(string[] groupAttr, Dictionary<string, string> aggregation, Dictionary<string, object> conditionals) where T : class
        {
            string tableName = GetTableName<T>();
            string stringGroupAttr = String.Join(", ", groupAttr);
            string stringAggregation = GetAggregatePair(aggregation);
            var result = GetKeyValue(conditionals);

            return GetData<T>($@"SELECT {stringGroupAttr}, {stringAggregation} 
                            FROM {tableName}
                            WHERE {result.Key} < {result.Value}                        
                            GROUP BY {stringGroupAttr}");
        }

        public List<Dictionary<string, object>> GetAllGroupByWithLessOrEqual<T>(string[] groupAttr, Dictionary<string, string> aggregation, Dictionary<string, object> conditionals) where T : class
        {
            string tableName = GetTableName<T>();
            string stringGroupAttr = String.Join(", ", groupAttr);
            string stringAggregation = GetAggregatePair(aggregation);
            var result = GetKeyValue(conditionals);

            return GetData<T>($@"SELECT {stringGroupAttr}, {stringAggregation} 
                            FROM {tableName}
                            WHERE {result.Key} <= {result.Value}                        
                            GROUP BY {stringGroupAttr}"); 
        }

        public List<Dictionary<string, object>> GetAllGroupByWithBetween<T>(string[] groupAttr, Dictionary<string, string> aggregation, (string Column, int Min, int Max) conditional) where T : class
        {
            List<Dictionary<string, object>> allRowsData = [];
            try
            {
                string tableName = GetTableName<T>();
                using var connection = OpenConnection();
                string resultGroupAttr = String.Join(", ", groupAttr);

                string atr = conditional.Column;
                object min = conditional.Min;
                object max = conditional.Max;

                string resultNames = GetAggregatePair(aggregation);
                string stringAggregation = GetAggregatePair(aggregation);

                var sql = $@"SELECT {resultGroupAttr}, {resultNames} 
                            FROM {tableName}
                            WHERE {atr} BETWEEN {min} AND {max}                          
                            GROUP BY {resultGroupAttr};";

                Console.WriteLine(sql);

                using var cmd = CreateCommand(connection, sql);
                using var reader = cmd.ExecuteReader();
                allRowsData = AddAllDataToList(reader);
                ShowAllDataToList(allRowsData);
            }
            catch (DbException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            return allRowsData;
        }

        public List<Dictionary<string, object>> GetAllGroupByWithIn<T>(string[] groupAttr, Dictionary<string, string> aggregation, Dictionary<string, object[]> conditionals) where T : class
        {
            string tableName = GetTableName<T>();

            string stringGroupAttr = String.Join(", ", groupAttr);
            string stringAggregation = GetAggregatePair(aggregation);
            string nameKey = "";
            string valuesAsString = "";

            foreach (var key in conditionals.Keys)
            {
                nameKey = key;
                var values = conditionals[key];

                valuesAsString = string.Join(", ", values);
            }

            return GetData<T>($@"SELECT {stringGroupAttr}, {stringAggregation} 
                            FROM {tableName}
                            WHERE {nameKey} IN ({valuesAsString})                         
                            GROUP BY {stringGroupAttr}");
        }
    }
}