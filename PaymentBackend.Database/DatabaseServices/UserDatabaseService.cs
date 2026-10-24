using System.Data;
using Microsoft.Extensions.Logging;
using Microsoft.Data.SqlClient;
using PaymentBackend.Common.Model;
using PaymentBackend.Settings;

namespace PaymentBackend.Database.DatabaseServices
{
    public interface IUserDatabaseService
    {
        Task<List<User>> SelectAllUsersAsync(CancellationToken cancellationToken);
        Task<User?> SelectUserByUsernameAsync(string username, CancellationToken cancellationToken);
    }

    public class UserDatabaseService : AbstractDatabaseService, IUserDatabaseService
    {
        public UserDatabaseService(IFunctionSettingsResolver functionSettingsResolver, 
            ILogger<UserDatabaseService> logger
        ) 
            : base(functionSettingsResolver, logger)
        {
        }

        public async Task<List<User>> SelectAllUsersAsync(CancellationToken cancellationToken)
        {
            var connectionString = GetConnectionString();
            await using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync(cancellationToken);

            var result = await SelectAllUsersAsync(connection, cancellationToken);
            
            await connection.CloseAsync();
            return result;
        }

        public async Task<User?> SelectUserByUsernameAsync(string username, CancellationToken cancellationToken)
        {
            var connectionString = GetConnectionString();
            await using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync(cancellationToken);

            var result = await SelectUserByUsernameAsync(connection, username, cancellationToken);

            await connection.CloseAsync();
            return result;
        }

        private async Task<User?> SelectUserByUsernameAsync(SqlConnection connection, string username, CancellationToken cancellationToken)
        {
            string sql = @"
select 
    u.Id,
    u.FirstName,
    u.LastName,
    u.Username
from
    PaymentUsers u
where
    u.Username = @Username
";

            await using SqlCommand cmd = new(sql, connection);
            cmd.CommandType = CommandType.Text;

            cmd.Parameters.AddWithValue("@Username", username);

            await using SqlDataReader reader = await cmd.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                return ConvertDbToObject(reader);
            }

            return null;
        }

        private async Task<List<User>> SelectAllUsersAsync(SqlConnection connection, CancellationToken cancellationToken)
        {
            string sql = @"
select 
    u.Id,
    u.FirstName,
    u.LastName,
    u.Username
from 
    PaymentUsers u
order by
    u.Username asc
";

            await using SqlCommand cmd1 = new(sql, connection);
            cmd1.CommandType = CommandType.Text;

            List<User> result = [];

            await using SqlDataReader reader = await cmd1.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                var user = ConvertDbToObject(reader);
                result.Add(user);
            }

            return result;
        }

        private static User ConvertDbToObject(SqlDataReader reader)
        {
            long? id = reader.SafeGetInt64("Id");
            string firstName = reader.SafeGetString("FirstName");
            string lastName = reader.SafeGetString("LastName");
            string username = reader.SafeGetString("Username");

            return new User
            {
                Id = id!.Value,
                FirstName = firstName,
                LastName = lastName,
                Username = username
            };
        }
    }
}
