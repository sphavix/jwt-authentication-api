using Dapper;
using jwtAuthApi.Models;
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

        // register user
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

        // findUser by email
        public User? FindByEmail(string email)
        {
            var query = "SELECT * FROM [AppUser] WHERE [Email] = @email";

            return sqlConnection.QueryFirstOrDefault<User>(query, new { email = email });
        }

        // AddRefreshToken (RefreshToken token, email)
        public bool AddRefreshToken(RefreshToken token, string email)
        {
            var query = "INSERT INTO [RefreshToken] (Token, CreatedAt, ExpiresAt, Enabled, Email) VALUES (@token, @createdat, @expiresat, @enabled, @email)";

            var result = sqlConnection.Execute(query, new
            {
                token.Token,
                token.CreatedAt,
                token.ExpiresAt,
                token.Enabled,
                email
            });

            return result > 0;
        }

        // disable user tokens (string email)
        public bool DisableUserTokenByEmail(string email)
        {
            var query = "UPDATE [RefreshToken] SET [Enabled] = 0 WHERE [Email] = @email";

            var result = sqlConnection.Execute(query, new { email });

            return result > 0;
        }

        // disbale user token (string token)
        public bool DisableUserToken(string token)
        {
            var query = "UPDATE [RefreshToken] SET [Enabled] = 0 WHERE [Token] = @token";

            var result = sqlConnection.Execute(query, new { token });

            return result > 0;
        }

        // isRefreshToken valid (string token)
        public bool IsRefreshTokenValid(string token)
        {
            var query = "SELECT COUNT(1) FROM [RefreshToken] WHERE [Token] = @token AND [Enabled] = 1 AND [ExpiresAt] >= CAST(GETDATE() AS DATE)";

            var result = sqlConnection.ExecuteScalar<int>(query, new { token });

            return result > 0;
        }

        // findUserByTokon (string token)
        public User? FindUserByToken(string token)
        {
            var query = "SELECT [AppUser].* FROM [RefreshToken] JOIN [AppUser] ON [RefreshToken].Email = [AppUser].Email WHERE [Token] = @token";

            return sqlConnection.QueryFirstOrDefault<User>(query, new { token });
        }

    }
}
