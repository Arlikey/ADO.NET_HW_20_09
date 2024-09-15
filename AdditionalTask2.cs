using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADO.NET_HW_20_09
{
    internal class AdditionalTask2
    {
        static string connectionString;
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
            var config = builder.Build();
            connectionString = config.GetConnectionString("AdditionalTask2Connection");
            /*CreateDatabase("StudentsMarks");
            CreateTable("""
                CREATE TABLE [Students]
                (
                [Id] INT PRIMARY KEY IDENTITY,
                [FullName] NVARCHAR(60) NOT NULL,
                [GroupName] NVARCHAR(15) NOT NULL,
                [AvgMark] DECIMAL(5, 1) NOT NULL,
                [MinAvgMarkSubject] NVARCHAR(20) NOT NULL,
                [MaxAvgMarkSubject] NVARCHAR(20) NOT NULL
                );
                """);*/

            /*InsertIntoTable("Students",
            [
                new Student { Fullname = "John Doe", GroupName = "P-22", AvgMark = 4, MinAvgMarkSubject = "History", MaxAvgMarkSubject = "Physics" },
                new Student { Fullname = "Jane Smith", GroupName = "QX-17", AvgMark = 3, MinAvgMarkSubject = "English", MaxAvgMarkSubject = "Mathematics" },
                new Student { Fullname = "Michael Johnson", GroupName = "RZ-09", AvgMark = 4, MinAvgMarkSubject = "Chemistry", MaxAvgMarkSubject = "Biology" },
                new Student { Fullname = "Emily Davis", GroupName = "LJ-34", AvgMark = 3, MinAvgMarkSubject = "Physics", MaxAvgMarkSubject = "Mathematics" }
            ]);*/

            //GetAllStudentsInfo();
            //GetAllStudentsFullName();
            //GetAllAvgMarks();
            //GetFullNameStudentsWithAvgMarkGreater(3);
            //GetMinAvgMarkWithSubject();
            //GetMaxAvgMark();
            //GetCountStudentWithMinAvgMarkMath();
            //GetCountStudentWithMaxAvgMarkMath();
            //GetCountStudentsInEveryGroup();
            //GetAvgMarkByGroup();
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

        private static void InsertIntoTable(string table, params Student[] students)
        {
            StringBuilder stringBuilder = new StringBuilder($"INSERT INTO [{table}] ([FullName], [GroupName], [AvgMark], " +
                $"[MinAvgMarkSubject], [MaxAvgMarkSubject]) VALUES ");
            foreach (var student in students)
            {
                stringBuilder.Append(student.ToStringSql());
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
        private static void GetAllStudentsInfo()
        {
            RequestToDatabase(connection =>
            {
                SqlCommand sqlCommand = new SqlCommand("""
                    SELECT [FullName], [GroupName], [AvgMark], [MinAvgMarkSubject], [MaxAvgMarkSubject] FROM Students
                    """, connection);
                SqlDataReader reader = sqlCommand.ExecuteReader();
                while (reader.Read())
                {
                    Console.WriteLine($"{reader["FullName"]}, {reader["GroupName"]}, {reader["AvgMark"]}, " +
                        $"{reader["MinAvgMarkSubject"]}, {reader["MaxAvgMarkSubject"]}");
                }
            });
        }
        private static void GetAllStudentsFullName()
        {
            RequestToDatabase(connection =>
            {
                SqlCommand sqlCommand = new SqlCommand("""
                    SELECT [FullName] FROM Students
                    """, connection);
                SqlDataReader reader = sqlCommand.ExecuteReader();
                while (reader.Read())
                {
                    Console.WriteLine($"{reader["FullName"]}");
                }
            });
        }
        private static void GetAllAvgMarks()
        {
            RequestToDatabase(connection =>
            {
                SqlCommand sqlCommand = new SqlCommand("""
                    SELECT [AvgMark] FROM Students
                    """, connection);
                SqlDataReader reader = sqlCommand.ExecuteReader();
                while (reader.Read())
                {
                    Console.WriteLine($"{reader["AvgMark"]}");
                }
            });
        }
        private static void GetFullNameStudentsWithAvgMarkGreater(double mark)
        {
            RequestToDatabase(connection =>
            {
                SqlCommand sqlCommand = new SqlCommand($"""
                    SELECT [FullName] FROM Students WHERE [AvgMark] > {mark}
                    """, connection);
                SqlDataReader reader = sqlCommand.ExecuteReader();
                while (reader.Read())
                {
                    Console.WriteLine($"{reader["FullName"]}");
                }
            });
        }
        private static void GetMinAvgMarkWithSubject()
        {
            RequestToDatabase(connection =>
            {
                SqlCommand sqlCommand = new SqlCommand("""
                    DECLARE @MinAvgMark DECIMAL(5, 1);
                    SELECT @MinAvgMark = MIN(AvgMark)
                    FROM Students;

                    SELECT DISTINCT MinAvgMarkSubject, @MinAvgMark AS MinAvgMark
                    FROM Students
                    WHERE AvgMark = @MinAvgMark;
                    """, connection);
                SqlDataReader reader = sqlCommand.ExecuteReader();
                while (reader.Read())
                {
                    Console.WriteLine($"{reader["MinAvgMarkSubject"]}, {reader["MinAvgMark"]}");
                }
            });
        }
        private static void GetMaxAvgMark()
        {
            RequestToDatabase(connection =>
            {
                SqlCommand sqlCommand = new SqlCommand("""
                    SELECT MAX(AvgMark) AS MaxAvgMark FROM Students
                    """, connection);
                SqlDataReader reader = sqlCommand.ExecuteReader();
                while (reader.Read())
                {
                    Console.WriteLine($"{reader["MaxAvgMark"]}");
                }
            });
        }
        private static void GetCountStudentWithMinAvgMarkMath()
        {
            RequestToDatabase(connection =>
            {
                SqlCommand sqlCommand = new SqlCommand("""
                    SELECT COUNT(*) AS Count FROM Students WHERE MinAvgMarkSubject LIKE 'Mathematics'
                    """, connection);
                SqlDataReader reader = sqlCommand.ExecuteReader();
                while (reader.Read())
                {
                    Console.WriteLine($"{reader["Count"]}");
                }
            });
        }
        private static void GetCountStudentWithMaxAvgMarkMath()
        {
            RequestToDatabase(connection =>
            {
                SqlCommand sqlCommand = new SqlCommand("""
                    SELECT COUNT(*) AS Count FROM Students WHERE MaxAvgMarkSubject LIKE 'Mathematics'
                    """, connection);
                SqlDataReader reader = sqlCommand.ExecuteReader();
                while (reader.Read())
                {
                    Console.WriteLine($"{reader["Count"]}");
                }
            });
        }
        private static void GetCountStudentsInEveryGroup()
        {
            RequestToDatabase(connection =>
            {
                SqlCommand sqlCommand = new SqlCommand("""
                    SELECT [GroupName], COUNT(*) AS Count FROM STUDENTS GROUP BY [GroupName]
                    """, connection);
                SqlDataReader reader = sqlCommand.ExecuteReader();
                while (reader.Read())
                {
                    Console.WriteLine($"{reader["GroupName"]} : {reader["Count"]}");
                }
            });
        }
        private static void GetAvgMarkByGroup()
        {
            RequestToDatabase(connection =>
            {
                SqlCommand sqlCommand = new SqlCommand("""
                    SELECT [GroupName], AVG(AvgMark) AS Avg FROM STUDENTS GROUP BY [GroupName]
                    """, connection);
                SqlDataReader reader = sqlCommand.ExecuteReader();
                while (reader.Read())
                {
                    Console.WriteLine($"{reader["GroupName"]} : {reader["Avg"]}");
                }
            });
        }
    }

    public class Student
    {
        public int Id { get; set; }
        public string Fullname { get; set; }
        public string GroupName { get; set; }
        public double AvgMark { get; set; }
        public string MinAvgMarkSubject { get; set; }
        public string MaxAvgMarkSubject { get; set; }
        public string ToStringSql()
        {
            return $"('{Fullname}', '{GroupName}', {AvgMark}, '{MinAvgMarkSubject}', '{MaxAvgMarkSubject}'),";
        }
    }
}
