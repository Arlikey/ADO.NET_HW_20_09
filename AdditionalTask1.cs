using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADO.NET_HW_20_09
{
    internal class AdditionalTask1
    {
        static string connectionString;
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
            var config = builder.Build();
            connectionString = config.GetConnectionString("AdditionalTask1Connection");
            /*CreateDatabase("Library");
            CreateTable("""
                CREATE TABLE [Books]
                (
                [Id] INT PRIMARY KEY IDENTITY,
                [Title] NVARCHAR(60) NOT NULL,
                [Price] INT NOT NULL,
                [Pages] INT NOT NULL
                );
                """);
*/
            /*InsertIntoTable("Books",
            [
                new Book { Title = "To Kill a Mockingbird", Price = 10, Pages = 281 },
                new Book { Title = "1984", Price = 15, Pages = 328 },
                new Book { Title = "The Great Gatsby", Price = 12, Pages = 180 },
                new Book { Title = "Moby Dick", Price = 18, Pages = 635 }
            ]);*/

            int price = GetSumPriceOfAllBooks();
            Console.WriteLine(price);

            int pages = GetSumPagesOfAllBooks();
            Console.WriteLine(pages);
        }

        private static void CreateDatabase(string dbName)
        {
            string connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=master;Trusted_Connection=True;";
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();
                SqlCommand sqlCommand = new SqlCommand($"IF NOT EXISTS (SELECT name FROM sys.databases WHERE name LIKE '{dbName}')" +
                    $"CREATE DATABASE [{dbName}]", sqlConnection);
                sqlCommand.ExecuteNonQuery();
            }
        }

        private static void CreateTable(string sqlQuery)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();
                SqlCommand sqlCommand = new SqlCommand(sqlQuery, sqlConnection);
                sqlCommand.ExecuteNonQuery();
            }
        }

        private static void InsertIntoTable(string table, params Book[] books)
        {
            StringBuilder stringBuilder = new StringBuilder($"INSERT INTO [{table}] ([Title], [Price], [Pages]) VALUES ");
            foreach (var book in books)
            {
                stringBuilder.Append(book.ToStringSql());
            }
            stringBuilder.Remove(stringBuilder.Length - 1, 1);
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();
                SqlCommand sqlCommand = new SqlCommand(stringBuilder.ToString(), sqlConnection);
                sqlCommand.ExecuteNonQuery();
            }
        }

        private static void RequestToDatabase(Action<SqlConnection> action)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    action(connection);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private static int GetSumPriceOfAllBooks()
        {
            int price = 0;
            RequestToDatabase(connection =>
            {
                SqlCommand sqlCommand = new SqlCommand("""
                    SELECT SUM(Price) FROM Books
                    """, connection);
                price = (int)sqlCommand.ExecuteScalar();
            });
            return price;
        }
        private static int GetSumPagesOfAllBooks()
        {
            int pages = 0;
            RequestToDatabase(connection =>
            {
                SqlCommand sqlCommand = new SqlCommand("""
                    SELECT SUM(Pages) FROM Books
                    """, connection);
                pages = (int)sqlCommand.ExecuteScalar();
            });
            return pages;
        }
    }
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Price { get; set; }
        public int Pages { get; set; }
        public string ToStringSql()
        {
            return $"('{Title}', {Price}, {Pages}),";
        }
    }
}
