using Microsoft.Data.Sqlite;
using System.Data;

namespace DAL
{
    public static class DatabaseOperations
    {
        private static string connectionString = "Data Source=D:\\Projects\\Databases\\chatApp.db";
        public static bool Login(string username, string password)
        {
            try
            {
                using (SqliteConnection connection = new SqliteConnection(connectionString))
                {
                    connection.Open();
                    string sql = $"SELECT COUNT(*) FROM user WHERE username = @username AND password = @password";

                    using (SqliteCommand command = new SqliteCommand(sql, connection))
                    {
                        command.Parameters.Add(new SqliteParameter("@username", username));
                        command.Parameters.Add(new SqliteParameter("@password", password));
                        Int64 userCount = (Int64)command.ExecuteScalar();

                        if (userCount == 1)
                        {
                            return true;
                        }
                    }
                }
            }
            catch
            {
                return false;
            }

            return false;
        }
    }
}
