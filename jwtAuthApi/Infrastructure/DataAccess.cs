using Dapper;
using Microsoft.Data.SqlClient;

namespace jwtAuthApi.Infrastructure
{
    public class DataAccess : IDisposable
    {
        private SqlConnection sqlConnection;
        public DataAccess(IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("DbConnection")!;
            sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();
        }

        public void Dispose()
        {
            if(sqlConnection != null)
            {
                sqlConnection.Dispose();
                sqlConnection = null;
            }
        }

        public bool RegisterUser(string email, string password, string role)
        {
            var userCount = sqlConnection.ExecuteScalar<int>(
                    "SELECT Count(1) FROM [AppUser] WHERE [Email] = @email", new { email = email }
                );

            if(userCount > 0)
            {
                return false;
            }

            var query = "INSERT INTO [AppUser] (Email, Password, Role) VALUES (@email, @password, @role)";

            var result = sqlConnection.Execute(query, new { email = email, password = password, role = role });

            return result > 0;
        }


    }
}
