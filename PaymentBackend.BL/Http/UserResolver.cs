using Microsoft.Azure.Functions.Worker.Http;
using PaymentBackend.Database.DatabaseServices;

namespace PaymentBackend.BL.Http
{
    public interface IUserResolver
    {
        Task<HttpResponseData> GetPaymentUsers(HttpRequestData req);
    }

    public class UserResolver : AbstractHttpResolver, IUserResolver
    {
        private readonly IUserDatabaseService _userDbService;

        public UserResolver(IUserDatabaseService userDbService)
        {
            _userDbService = userDbService;
        }

        public async Task<HttpResponseData> GetPaymentUsers(HttpRequestData req)
        {
            var allUsers = _userDbService.SelectAllUsers();

            List<Common.Generated.User> httpMappedUsers = allUsers.Select(user => new Common.Generated.User()
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Username= user.Username
            }).ToList();

            Common.Generated.GetUsersResponse response = new()
            {
                UserList = httpMappedUsers
            };

            return await BuildOkResponse(req, response);
        }
    }
}
