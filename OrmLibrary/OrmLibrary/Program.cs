using OrmLibrary.DML;
using OrmLibrary.DQL;
using OrmLibrary.DDL;
using System.Data;
using System;
using System.IO;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using System.Text.Json;
using System.Reflection;
namespace OrmLibrary;

public class Orm
{
    private readonly  DDLBase  _ddl;
    private readonly  DMLBase _dml;
    private readonly  DQLBase _dql;

        public Orm(string dbType, string connectionString)
        {
        DDLBase.SetConnectionString(connectionString);
        DMLBase.SetConnectionString(connectionString);
        DQLBase.SetConnectionString(connectionString);

        if (dbType == "MySql")
        {
            _ddl = new DDLMySql();
            _dml = new DMLMySql();
            _dql = new DQLMySql();
        }
        else if (dbType == "MicrosoftSql")
        {
            _ddl = new DDLMicrosoftSql();
            _dml = new DMLMicrosoftSql();
            _dql = new DQLMicrosoftSql();
        }
        else if (dbType == "PostgreSql")
        {
            _ddl = new DDLPostgreSql();
            _dml = new DMLPostgreSql();
            _dql = new DQLPostgreSql();
        }
        }

    public Orm() { }

    public void InsertData<T>(Dictionary<string, object> newData) where T : class => _dml.InsertData<T>(newData);
    public void UpdateData<T>(int id, Dictionary<string, object> updatedData) where T : class => _dml.UpdateData<T>(id, updatedData);
    public void DeleteById<T>(int id) where T : class => _dml.DeleteById<T>(id);
    public void InsertMany<T>(List<Dictionary<string, object>> manyData) where T : class => _dml.InsertMany<T>(manyData);

    public virtual List<Dictionary<string, object>> GetAll<T>() where T : class => _dql.GetAll<T>();
    public List<Dictionary<string, object>> GetAll<T>(params string[] fields) where T : class => _dql.GetAll<T>(fields);



    public virtual List<Dictionary<string, object>> GetById<T>(int id) where T : class => _dql.GetById<T>(id);
    public List<Dictionary<string, object>> GetById<T>(int id, params string[] fields) where T : class => _dql.GetById<T>(id, fields);

    public List<Dictionary<string, object>> GetFirstBy<T>(string groupName) where T : class => _dql.GetFirstBy<T>(groupName);

    public void CreateAllTables(string targetNamespace) => _ddl.CreateAllTables(targetNamespace);

    public void DeleteAllRecords<T>() where T : class => _dml.DeleteAllRecords<T>();

    public virtual List<Dictionary<string, object>> GetAllPagination<T>(int limit, int skip=0) where T : class => _dql.GetAllPagination<T>(limit, skip);

    public List<Dictionary<string, object>> GetAllGroupByWithGreater<T>(string[] groupAttr, Dictionary<string, string> aggregation, Dictionary<string, object> conditionals) where T : class => _dql.GetAllGroupByWithGreater<T>(groupAttr, aggregation, conditionals);


    public List<Dictionary<string, object>> GetAllGroupByWithIn<T>(string[] groupAttr, Dictionary<string, string> aggregation, Dictionary<string, object[]> conditionals) where T : class => _dql.GetAllGroupByWithIn<T>(groupAttr, aggregation, conditionals);

    public List<Dictionary<string, object>> GetAllGroupBy<T>(string[] groupAttr, Dictionary<string, string> aggregation) where T : class => _dql.GetAllGroupBy<T>(groupAttr, aggregation);

    public List<Dictionary<string, object>> GetAllGroupByWithGreaterOrEqual<T>(string[] groupAttr, Dictionary<string, string> aggregation, Dictionary<string, object> conditionals) where T : class => _dql.GetAllGroupByWithGreaterOrEqual<T>(groupAttr, aggregation, conditionals);

    public List<Dictionary<string, object>> GetAllGroupByWithEqual<T>(string[] groupAttr, Dictionary<string, string> aggregation, Dictionary<string, object> conditionals) where T : class => _dql.GetAllGroupByWithEqual<T>(groupAttr, aggregation, conditionals);

    public List<Dictionary<string, object>> GetAllGroupByWithLess<T>(string[] groupAttr, Dictionary<string, string> aggregation, Dictionary<string, object> conditionals) where T : class => _dql.GetAllGroupByWithLess<T>(groupAttr, aggregation, conditionals);

    public List<Dictionary<string, object>> GetAllGroupByWithLessOrEqual<T>(string[] groupAttr, Dictionary<string, string> aggregation, Dictionary<string, object> conditionals) where T : class => _dql.GetAllGroupByWithLessOrEqual<T>(groupAttr, aggregation, conditionals);

    public List<Dictionary<string, object>> GetAllGroupByWithBetween<T>(string[] groupAttr, Dictionary<string, string> aggregation, (string Column, int Min, int Max) conditional) where T : class => _dql.GetAllGroupByWithBetween<T>(groupAttr, aggregation, conditional);


    public List<Dictionary<string, object>> GetAllWithWhere<T>() where T : class => _dql.GetAllWithWhere<T>();


    public static void generateFile()
    {
        string exeDir = AppContext.BaseDirectory;
        string projectDir = Path.GetFullPath(Path.Combine(exeDir, "..", "..", ".."));

        string migrationDir = Path.Combine(projectDir, "models", "Migration");
        Directory.CreateDirectory(migrationDir);

        string outputPath = Path.Combine(migrationDir, "modelSnapshot.json");

        var baseDir = AppDomain.CurrentDomain.BaseDirectory;
        var dllPath = Path.Combine(baseDir, "ClientModel.dll");
        
        if (!File.Exists(dllPath))
            throw new FileNotFoundException($"Brak pliku: {dllPath}");

        var asm = Assembly.LoadFrom(dllPath);
        var entityTypes = asm.GetTypes()
                             .Where(t => t.IsClass
                                         && t.IsPublic
                                         && t.Namespace == "ClientModel")
                             .ToList();


        var model = new Dictionary<string, Dictionary<string, string>>();

        foreach (var type in entityTypes)
        {
            var props = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var propDict = new Dictionary<string, string>();
            foreach (var p in props)
            {
                propDict[p.Name] = p.PropertyType.Name;
            }
            model[type.Name] = propDict;
        }

        var options = new JsonSerializerOptions { WriteIndented = true };
        string json = JsonSerializer.Serialize(model, options);

        File.WriteAllText(outputPath, json);
        Console.WriteLine($"Zapisano model do: {outputPath}");
    }

    public static void Main(string[] args)
    {
        generateFile();
    }

 }