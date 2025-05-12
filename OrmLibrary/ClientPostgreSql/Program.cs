using OrmLibrary;
using ClientModel;
using Microsoft.Extensions.Configuration;

namespace ClientPostgreSql
{
  

    class ClientPostgreSql
    {
        static void Main(string[] args)
        {
            IConfigurationBuilder builder = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            IConfigurationRoot root = builder.Build();
            string connectionString = root.GetSection("ConnectionStrings")["DefaultConnection"];


            Orm orm = new("PostgreSql", connectionString);

            Console.WriteLine("----------------------------");
            Console.WriteLine("DDL [START]");
            //orm.Create();

            orm.CreateAllTables("ClientModel");

            Console.WriteLine("DDL [KONIEC]");
            Console.WriteLine("----------------------------");

            Console.WriteLine("----------------------------");
            Console.WriteLine("DML [START]");
            // orm.InsertData();


            Dictionary<string, object> addBook = new Dictionary<string, object> { { "id", 470 }, 
                { "Date", "12/05/1904 00:00:00" }, { "Publisher", "2 Kroki i 4 pszczoly" } };

             orm.InsertData<Book>(addBook);
            //orm.UpdateData<Book>(340, new Dictionary<string, object> { { "Date", "14/06/1974 00:00:00" }, { "Publisher", "20 Krokow i 4 pszczoly" } });

            int idToDelete = 340;

            orm.DeleteById<Book>(idToDelete);

        

            var newBooks = new List<Dictionary<string, object>>
       {
           new Dictionary<string, object>
           {
               { "Id", 800 },
               { "Date","12/05/1904 00:00:00" },
               { "Publisher", "20 krokow i mewa" }
           },
           new Dictionary<string, object>
           {
               { "Id", 801 },
               { "Date", "15/05/1904 00:00:00" },
               { "Publisher", "24 krokow i mewa" }
           },
           new Dictionary<string, object>
           {
               { "Id", 802 },
               { "Date", "18/05/1904 00:00:00" },
               { "Publisher", "42 krokow i mewa" }
           }
       };


            Console.WriteLine("PROBA INSERT MANY [start]");

            orm.InsertMany<Book>(newBooks);

            Console.WriteLine("PROBA INSERT MANY [koniec]");

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
            Console.WriteLine("pierwsze ksiazki wg atrybutu publishera [START]");
            orm.GetFirstBy<Book>("publisher");
            Console.WriteLine("pierwsze ksiazki wg atrybutu publishera [KONIEC]");
            Console.WriteLine("----------------------------");


            Console.WriteLine("----------------------------");
            Console.WriteLine("Ksiazki z uzyciem limitu i pominiecia danych [START]");

          

            Console.WriteLine("pierwsze ksiazki wg atrybutu publishera [KONIEC]");
            Console.WriteLine("----------------------------");
            Console.WriteLine("DQL [KONIEC]");
            Console.WriteLine("----------------------------");


            string[] groupAttr = ["publisher"];
            Dictionary<string, string> aggregation = new Dictionary<string, string> { { "Id", "MAX" }, { "Date", "MIN" } };

            Dictionary<string, object> greater = new Dictionary<string, object> { { "Id", 400 } };




            //var conditional = (name: "Example", items: new object[] { 1, "test", 3.14 });
           // Console.WriteLine(conditional.name); // Outputs: Example
           // Console.WriteLine(conditional.items[1]); // Outputs: test



            var conditional = new Dictionary<string, object[]>
{
    { "Id", new object[] { 1, 2, 3, 4, 5 } }
};


            //orm.GetAllGroupBy<Book>(groupAttr, aggregation);

            /* [1] */
            /*Console.WriteLine("Metoda: GetAllGroupByWithGreater [START]");
            orm.GetAllGroupByWithGreater<Book>(groupAttr, aggregation, greater);
            Console.WriteLine("Metoda: GetAllGroupByWithGreater [KONIEC]");*/

            /* [2] */
            /* Console.WriteLine("Metoda: GetAllGroupByWithGreaterOrEqual [START]");
             orm.GetAllGroupByWithGreaterOrEqual<Book>(groupAttr, aggregation, greater);
             Console.WriteLine("Metoda: GetAllGroupByWithGreaterOrEqual [KONIEC]");*/

            /* [3] */

            /*Dictionary<string, object> greater2 = new Dictionary<string, object> { { "Id", 2 } };
            Console.WriteLine("Metoda: GetAllGroupByWithEqual [START]");
            orm.GetAllGroupByWithEqual<Book>(groupAttr, aggregation, greater2);
            Console.WriteLine("Metoda: GetAllGroupByWithEqual [KONIEC]");*/

            /* [4] */

            /*Console.WriteLine("Metoda: GetAllGroupByWithLess [START]");
            orm.GetAllGroupByWithLess<Book>(groupAttr, aggregation, greater);
            Console.WriteLine("Metoda: GetAllGroupByWithLess [KONIEC]");*/


            /* [5] */
            /*Console.WriteLine("Metoda: GetAllGroupByWithLessOrEqual [START]");
             orm.GetAllGroupByWithLessOrEqual<Book>(groupAttr, aggregation, greater);
             Console.WriteLine("Metoda: GetAllGroupByWithLessOrEqual [KONIEC]");*/


            var conditional2 = (Column: "Id", Min: 2, Max: 4);

            /* 6 */
            Console.WriteLine("Metoda: GetAllGroupByWithBetween [START]");
            orm.GetAllGroupByWithBetween<Book>(groupAttr, aggregation, conditional2);
            Console.WriteLine("Metoda: GetAllGroupByWithBetween [KONIEC]");

            /* 7 */
            /*Console.WriteLine("Metoda: GetAllGroupByWithIn [START]");
            orm.GetAllGroupByWithIn<Book>(groupAttr, aggregation, conditional);
            Console.WriteLine("Metoda: GetAllGroupByWithBetween [KONIEC]");

            var getBooks = orm.GetAll<Book>();


            foreach (var row in getBooks)
            {
                foreach (var item in row)
                {
                    Console.WriteLine($"{item.Key}: {item.Value}");
                }
            }*/


            /*int limit = 6;
            int skip = 0;

            var firstSixBooks = orm.GetAllPagination<Book>(limit, skip);

            foreach (var row in firstSixBooks)
            {
                foreach (var item in row)
                {
                    Console.WriteLine($"{item.Key}: {item.Value}");
                }
            }*/


            /*int bookId = 2;

            var secondBook = orm.GetById<Book>(bookId);

            foreach (var row in secondBook)
            {
                foreach (var item in row)
                {
                    Console.WriteLine($"{item.Key}: {item.Value}");
                }
            }*/



            /*var thirdBookWithChosenFields = orm.GetById<Book>(3, "Date", "Publisher");


            foreach (var row in thirdBookWithChosenFields)
            {
                foreach (var item in row)
                {
                    Console.WriteLine($"{item.Key}: {item.Value}");
                }
            }*/

            // orm.GetAllWithWhere<Book>();

            //Console.WriteLine("-------------------------------");
            //Console.WriteLine("grupowane ksiazki wg pulisher");
            //orm.GetAllGroupBy<Book>("publisher");
            //Console.WriteLine("-------------------------------");

            //GetCountBy z wyborem pol do obliczenia

            //GetMinBy string columnToMin, string[] groupFileds (group BY: id, publisher, date)

            //GetMaxBy

            //GetSumBy

            //GetAvgBy
        }
    }
}

