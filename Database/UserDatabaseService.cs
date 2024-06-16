using Common.Model;
using Masterarbeit.OrderService;
using Microsoft.Extensions.Logging;
using System.Data.SqlClient;

namespace Database
{
    public interface IUserDatabaseService
    {
        List<PaymentUser> SelectAllUsers();
    }

    public class UserDatabaseService : AbstractDatabaseService, IUserDatabaseService
    {
        public UserDatabaseService(ISqlExceptionHandler exceptionHandler,
            IFunctionSettingsResolver functionSettingsResolver, 
            ILogger<UserDatabaseService> logger
            ) : base(exceptionHandler, 
                functionSettingsResolver, 
                logger)
        {
        }

        public List<PaymentUser> SelectAllUsers()
        {
            List<PaymentUser> result = new();

            result.Add(new PaymentUser()
            {
                Id = 1,
                Email = "hans.wurst@hotmail.de",
                LastName = "Wurst",
                Name = "Hans"
            });
            result.Add(new PaymentUser()
            {
                Id = 2,
                Email = "peter.fox@concert.com",
                LastName = "Fox",
                Name = "Peter"
            });
            result.Add(new PaymentUser()
            {
                Id = 3,
                Email = "olaf.scholz@bt.de",
                LastName = "Scholz",
                Name = "Olaf"
            });
            result.Add(new PaymentUser()
            {
                Id = 4,
                Email = "hansi.flick@gmx.com",
                LastName = "FLick",
                Name = "Hansi"
            });
            result.Add(new PaymentUser()
            {
                Id = 5,
                Email = "fakeuser@admin.de",
                LastName = "Admin",
                Name = "Fake"
            });

            return result;


            return _exceptionHandler.ExecuteOrReturn(() =>
            {
                var connectionString = GetConnectionString();
                using var connection = new SqlConnection(connectionString);
                connection.Open();

                var result = SelectAllUsers(connection);
                
                connection.Close();
                return result;
            }, new List<PaymentUser>());
        }

        private List<PaymentUser> SelectAllUsers(SqlConnection connection)
        {
            throw new NotImplementedException();
        }
    }
}
