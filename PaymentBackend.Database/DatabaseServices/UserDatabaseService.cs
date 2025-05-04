using System.Data;
using Microsoft.Extensions.Logging;
using System.Data.SqlClient;
using PaymentBackend.Common.Model;
using PaymentBackend.Settings;

namespace PaymentBackend.Database.DatabaseServices
{
    public interface IUserDatabaseService
    {
        List<User> SelectAllUsers();
        User? SelectUserByUsername(string username);
    }

    public class UserDatabaseService : AbstractDatabaseService, IUserDatabaseService
    {
        public UserDatabaseService(ISqlExceptionHandler exceptionHandler,
            IFunctionSettingsResolver functionSettingsResolver, 
            ILogger<UserDatabaseService> logger
        ) 
            : base(exceptionHandler, functionSettingsResolver, logger)
        {
        }

        public List<User> SelectAllUsers()
        {
            return _exceptionHandler.ExecuteOrThrow(() =>
            {
                var connectionString = GetConnectionString();
                using var connection = new SqlConnection(connectionString);
                connection.Open();

                var result = SelectAllUsers(connection);
                
                connection.Close();
                return result;
            });
        }

        public User? SelectUserByUsername(string username)
        {
            return _exceptionHandler.ExecuteOrThrow(() =>
            {
                var connectionString = GetConnectionString();
                using var connection = new SqlConnection(connectionString);
                connection.Open();

                var result = SelectUserByUsername(connection, username);

                connection.Close();
                return result;
            });
        }

        private User? SelectUserByUsername(SqlConnection connection, string username)
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

            using SqlCommand cmd = new(sql, connection);
            cmd.CommandType = CommandType.Text;

            cmd.Parameters.AddWithValue("@Username", username);

            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                return ConvertDbToObject(reader);
            }

            return null;
        }

        private List<User> SelectAllUsers(SqlConnection connection)
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

            using SqlCommand cmd1 = new(sql, connection);
            cmd1.CommandType = CommandType.Text;

            List<User> result = new();

            using SqlDataReader reader = cmd1.ExecuteReader();
            while (reader.Read())
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

            return new User()
            {
                Id = id!.Value,
                FirstName = firstName,
                LastName = lastName,
                Username = username
            };
        }
    }
}
