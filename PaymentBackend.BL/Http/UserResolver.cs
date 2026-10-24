using Microsoft.Azure.Functions.Worker.Http;
using PaymentBackend.Database.DatabaseServices;

namespace PaymentBackend.BL.Http
{
    public interface IUserResolver
    {
        Task<HttpResponseData> GetPaymentUsersAsync(HttpRequestData req, CancellationToken cancellationToken);
    }

    public class UserResolver : AbstractHttpResolver, IUserResolver
    {
        private readonly IUserDatabaseService _userDbService;

        public UserResolver(IUserDatabaseService userDbService)
        {
            _userDbService = userDbService;
        }

        public async Task<HttpResponseData> GetPaymentUsersAsync(HttpRequestData req, CancellationToken cancellationToken)
        {
            var allUsers = await _userDbService.SelectAllUsersAsync(cancellationToken);

            List<Common.Generated.User> httpMappedUsers =
            [
                .. allUsers.Select(user => new Common.Generated.User
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Username = user.Username
                })
            ];

            Common.Generated.GetUsersResponse response = new()
            {
                UserList = httpMappedUsers
            };

            return await BuildOkResponseAsync(req, response, cancellationToken);
        }
    }
}
