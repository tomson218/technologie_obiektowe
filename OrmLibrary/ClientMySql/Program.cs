using OrmLibrary;
using ClientModel;
using Microsoft.Extensions.Configuration;

namespace ClientMySql
{
    class ClientMySql
    {
        static void Main(string[] args)
        {
            IConfigurationBuilder builder = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            IConfigurationRoot root = builder.Build();
            string connectionString = root.GetSection("ConnectionStrings")["DefaultConnection"];

            Orm orm = new("MySql", connectionString);

            Console.WriteLine("----------------------------");
            Console.WriteLine("DDL [START]");
            //orm.Create();
            orm.CreateAllTables("ClientModel");

            Console.WriteLine("DDL [KONIEC]");
            Console.WriteLine("----------------------------");

            Console.WriteLine("----------------------------");
            Console.WriteLine("DML [START]");
            //orm.InsertData();
            //orm.InsertData<Book>(new Dictionary<string, object> { { "id", 340 }, { "Date", "12/05/1904 00:00:00" }, { "Publisher", "2 Kroki i 4 pszczoly" } });
            //orm.UpdateData<Book>(340, new Dictionary<string, object> { { "Date", "14/06/1974 00:00:00" }, { "Publisher", "20 Krokow i 4 pszczoly" } });
            orm.DeleteById<Book>(340);

            /*var books = new List<Dictionary<string, object>>
        {
            new Dictionary<string, object>
            {
                { "Id", "400" },
                { "Date", "12/05/1904 00:00:00" },
                { "Publisher", "20 krokow i mewa" }
            },
            new Dictionary<string, object>
            {
                { "Id", "401" },
                { "Date", "13/05/1904 00:00:00" },
                { "Publisher", "24 krokow i mewa" }
            },
            new Dictionary<string, object>
            {
                { "Id", "402" },
                { "Date", "15/05/1904 00:00:00" },
                { "Publisher", "42 krokow i mewa" }
            }
        };*/

            var books = new List<Dictionary<string, object>>
       {
           new Dictionary<string, object>
           {
               { "Id", 500 },
               { "Date", DateTime.Parse("12/05/1904 00:00:00") },
               { "Publisher", "20 krokow i mewa" }
           },
           new Dictionary<string, object>
           {
               { "Id", 501 },
               { "Date", DateTime.Parse("15/05/1904 00:00:00") },
               { "Publisher", "24 krokow i mewa" }
           },
           new Dictionary<string, object>
           {
               { "Id", 502 },
               { "Date", DateTime.Parse("18/05/1904 00:00:00") },
               { "Publisher", "42 krokow i mewa" }
           }
       };


            orm.InsertMany<Book>(books);





            Console.WriteLine("DML [KONIEC]");
            Console.WriteLine("----------------------------");

            Console.WriteLine("----------------------------");
            Console.WriteLine("DQL [START]");
            Console.WriteLine("Wszystkie ksiazki [START]");
            orm.GetAll<Book>();
            Console.WriteLine("Wszystkie ksiazki [KONIEC]");
            Console.WriteLine("----------------------------");
            Console.WriteLine("Wszystkie ksiazki dla wybranych pol [START]");
            orm.GetAll<Book>("Date", "Publisher");
            Console.WriteLine("Wszystkie ksiazki dla wybranych pol [KONIEC]");
            Console.WriteLine("----------------------------");
            Console.WriteLine("ksiazka o id = 1 [START]");
            orm.GetById<Book>(2);
            Console.WriteLine("ksiazka o id = 1 [KONIEC]");
            Console.WriteLine("----------------------------");
            Console.WriteLine("ksiazka o id = 1 dla wybranych pol [START]");
            orm.GetById<Book>(2, "Date", "Publisher");
            Console.WriteLine("ksiazka o id = 1 dla wybranych pol [KONIEC]");
            Console.WriteLine("----------------------------");

            Console.WriteLine("----------------------------");
            Console.WriteLine("Ksiazki z uzyciem limitu i pominiecia danych [START]");
            orm.GetAllPagination<Book>(2, 0);
            Console.WriteLine("pierwsze ksiazki wg atrybutu publishera [KONIEC]");
            Console.WriteLine("----------------------------");
            Console.WriteLine("DQL [KONIEC]");
            Console.WriteLine("----------------------------");


           // Console.WriteLine("----------------------------");
           // Console.WriteLine("pierwsze ksiazki wg atrybutu publishera [START]");
           // orm.GetFirstBy<Book>("publisher");
            //Console.WriteLine("pierwsze ksiazki wg atrybutu publishera [KONIEC]");
            //Console.WriteLine("----------------------------");



            //Console.WriteLine("DQL [KONIEC]");
           // Console.WriteLine("----------------------------");


        }
    }

}

