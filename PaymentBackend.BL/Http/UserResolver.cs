using Microsoft.AspNetCore.Mvc;
using PaymentBackend.Common.Generated;
using PaymentBackend.Database.DatabaseServices;

namespace PaymentBackend.BL.Http
{
    public interface IUserResolver
    {
        IActionResult GetPaymentUsers();
    }

    public class UserResolver : IUserResolver
    {
        private readonly IUserDatabaseService _userDbService;

        public UserResolver(IUserDatabaseService userDbService)
        {
            _userDbService = userDbService;
        }

        public IActionResult GetPaymentUsers()
        {
            var allUsers = _userDbService.SelectAllUsers();

            var httpMappedUsers = allUsers.Select(user => new User()
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Username= user.Username
            }).ToList();

            GetUsersResponse response = new()
            {
                UserList = httpMappedUsers
            };

            return new JsonResult(response);
        }
    }
}
