using System;
using System.Collections.Generic;
using Moq;
using OrmLibrary;
using Xunit;
using OrmLibrary.DQL;
using OrmLibrary.DML;
using Xunit.Abstractions;
using Org.BouncyCastle.Asn1.Pkcs;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace OrmLibraryTests
{
    public class Book
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string? Publisher { get; set; }
    }

    public class Order { }

    public class OrmLibraryTests 
    {
        private readonly ITestOutputHelper _output;


        [Fact]
        public void GetAll_ShouldReturnMockedData()
        {
            List<Dictionary<string, object>> mockData = new List<Dictionary<string, object>>
            {
                new Dictionary<string, object> { { "Id", 1 }, { "Title", "C# dla początkujących" }, { "Author", "John Doe" } },
                new Dictionary<string, object> { { "Id", 2 }, { "Title", "Zaawansowane SQL" }, { "Author", "Jane Smith" } }
            };

            var ormMock = new Mock<Orm>();
            ormMock.Setup(o => o.GetAll<Book>()).Returns(mockData);
            var result = ormMock.Object.GetAll<Book>();           

            Xunit.Assert.NotNull(result);
            Xunit.Assert.Equal(2, result.Count);
            Xunit.Assert.Equal("C# dla początkujących", result[0]["Title"]);
            Xunit.Assert.Equal("Jane Smith", result[1]["Author"]);

           

        }

        [Fact]
        public void GetAll_EmptyList()
        {
            List<Dictionary<string, object>> mockData = new List<Dictionary<string, object>>();

            var ormMock = new Mock<Orm>();
            ormMock.Setup(o => o.GetAll<Book>()).Returns(mockData);
            var result = ormMock.Object.GetAll<Book>();

            Xunit.Assert.NotNull(result);
            Xunit.Assert.Empty(result); 

        }

        [Fact]
        public void GetAll_NullList()
        {
            List<Dictionary<string, object>> mockData = null;

            var orm = new Orm("PostgreSql", "Host=localhost;Port=5432;Database=postgres;User Id=postgres;Password=12345678;");
            var result = orm.GetAll<Order>();

            Xunit.Assert.Null(result);
        }

        [Fact]
        public void GetById_ShouldReturnMockedData()
        {
            List<Dictionary<string, object>> mockData = new List<Dictionary<string, object>>
            {
                new Dictionary<string, object> { { "Id", 1 }, { "Title", "C# dla początkujących" }, { "Author", "John Doe" } },
                new Dictionary<string, object> { { "Id", 2 }, { "Title", "Zaawansowane SQL" }, { "Author", "Jane Smith" } }
            };

            var ormMock = new Mock<Orm>();

            ormMock.Setup(o => o.GetById<Book>(1))
    .Returns(mockData.Where(row => (int)row["Id"] == 1).ToList());

            var result = ormMock.Object.GetById<Book>(1);

            Xunit.Assert.NotNull(result);
            Xunit.Assert.Single(result);
            Xunit.Assert.Equal("C# dla początkujących", result[0]["Title"]);
            Xunit.Assert.Equal("John Doe", result[0]["Author"]);
        }


        [Fact]
        public void GetById_EmptyList()
        {
            List<Dictionary<string, object>> mockData = new List<Dictionary<string, object>>();

            var ormMock = new Mock<Orm>();

            ormMock.Setup(o => o.GetById<Book>(1))
       .Returns(mockData.Where(row => (int)row["Id"] == 1).ToList() ?? new List<Dictionary<string, object>>());

            var result = ormMock.Object.GetById<Book>(1);

            Xunit.Assert.NotNull(result);
            Xunit.Assert.Empty(result);

        }

        [Fact]
        public void GetTableName_WithCallBase_ShouldReturnClassName()
        {
            var mockDql = new Mock<DQLBase>() { CallBase = true };

            var tableName = mockDql.Object.GetTableName<Order>();

            Xunit.Assert.Equal("Order", tableName);
        }

        [Fact]
        public void ShowAllDataToList_ShouldPrintMockedData()
        {
            var mockData = new List<Dictionary<string, object>>
    {
        new Dictionary<string, object> { { "Id", 1 }, { "Title", "C# dla początkujących" }, { "Author", "John Doe" } },
        new Dictionary<string, object> { { "Id", 2 }, { "Title", "Zaawansowane SQL" }, { "Author", "Jane Smith" } }
    };

            var stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            var mockDql = new Mock<DQLBase>() { CallBase = true };

            mockDql.Object.ShowAllDataToList(mockData);

            var output = stringWriter.ToString();
            Xunit.Assert.Contains("Id: 1", output);
            Xunit.Assert.Contains("Title: C# dla początkujących", output);
            Xunit.Assert.Contains("Author: John Doe", output);
            Xunit.Assert.Contains("Id: 2", output);
            Xunit.Assert.Contains("Title: Zaawansowane SQL", output);
            Xunit.Assert.Contains("Author: Jane Smith", output);

            Console.SetOut(new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true });
        }


        [Fact]
        public void CheckAndConvert_ValidProperty_ShouldConvertValue()
        {
            var item = new KeyValuePair<string, object>("Date", "18/05/1904 00:00:00");
            var mockDml = new Mock<DMLBase>() { CallBase = true };
            var result = mockDml.Object.CheckAndConvert<Book>(item);
            Xunit.Assert.Equal(DateTime.Parse("1904-05-18T00:00:00.0000000").Ticks, ((DateTime)result).Ticks);
        }


        [Fact]
        public void GetAllPagination_ShouldReturnMockedData()
        {
            List<Dictionary<string, object>> mockData = new List<Dictionary<string, object>>
            {
                new Dictionary<string, object> { { "Id", 1 }, { "Title", "C# dla początkujących" }, { "Author", "John Doe" } },
                new Dictionary<string, object> { { "Id", 2 }, { "Title", "Zaawansowane SQL" }, { "Author", "Jane Smith" } },
                new Dictionary<string, object> { { "Id", 3 }, { "Title", "Zaawansowany kurs JavaScriptu" }, { "Author", "Jane Ahonen" } }
            };

            var ormMock = new Mock<Orm>();
            var filteredMockData = mockData.Take(2).ToList();
            ormMock.Setup(o => o.GetAllPagination<Book>(2, 0)).Returns(filteredMockData);

            var result = ormMock.Object.GetAllPagination<Book>(2,0);

           
            Xunit.Assert.NotNull(result);
            Xunit.Assert.Equal(2, result.Count);
            Xunit.Assert.Equal("C# dla początkujących", result[0]["Title"]);
            Xunit.Assert.Equal("Jane Smith", result[1]["Author"]);
        }
    }
}
