using Helpers;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Text;

namespace Database
{
    public sealed class Database : DbContext
    {
        private static readonly string connectionString = "Server=localhost,1433;User Id=sa;Password=S3cr3tP4sSw0rd#123;Encrypt=false;TrustServerCertificate=True;";

        static Database()
        {
            SetupDatabase();
        }

        private static void SetupDatabase()
        {
            try
            {
                SqlConnection sqlConnection = new SqlConnection(connectionString);
                sqlConnection.Open();

                string cs = "IF OBJECT_ID(N'dbo.Tweet', N'U') IS NULL BEGIN   CREATE TABLE dbo.Tweet (Id INT IDENTITY(1,1), Content varchar(280) not null, UserId int not null, Hashtags varchar(255), Timestamp DateTime not null); END;";
                SqlCommand command = new SqlCommand(cs, sqlConnection);
                command.ExecuteNonQuery();

                cs = "IF OBJECT_ID(N'dbo.User', N'U') IS NULL BEGIN   CREATE TABLE dbo.User (Id INT IDENTITY(1,1), Name varchar(100) not null, UserTag varchar(100) not null, Password varchar(100) not null, Email varchar(100); END;";
                command = new SqlCommand(cs, sqlConnection);
                command.ExecuteNonQuery();

                cs = "IF NOT EXISTS(SELECT * FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME = 'FK_TweetUserId')" +
                    "BEGIN" +
                    "ALTER TABLE dbo.Tweet ADD CONSTRAINT [FK_TweetUserId] REFERENCES dbo.User(Id);" +
                    "END;";
                command = new SqlCommand(cs, sqlConnection);
                command.ExecuteNonQuery();

                sqlConnection.Close();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error SetupDatabase: " + ex.Message);
            }
        }

        public static async Task CreateUser(UserDto dto)
        {
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();

            StringBuilder sb = new StringBuilder($"INSERT INTO dbo.User (Name, Usertag, Password, Email) VALUES ('{dto.Name})', '{dto.UserTag}', '{dto.Email}', '{dto.Password}';");

            SqlCommand command = new SqlCommand(sb.ToString(), sqlConnection);
            await command.ExecuteNonQueryAsync();

            sqlConnection.Close();
        }
    }
}
