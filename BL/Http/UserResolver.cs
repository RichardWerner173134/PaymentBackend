using Common.Generated;
using Common.Model;
using Database;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace BL.Http
{
    public interface IUserResolver
    {
        IActionResult GetPaymentUsersAsync();
    }

    public class UserResolver : IUserResolver
    {
        private readonly IUserDatabaseService _userDbService;

        public UserResolver(IUserDatabaseService userDbService)
        {
            _userDbService = userDbService;
        }

        public IActionResult GetPaymentUsersAsync()
        {
            var allUsers = _userDbService.SelectAllUsers();

            UserResponse response = new UserResponse();
            var httpMappedUsers = allUsers.Select(user => new User()
            {
                Id = user.Id,
                FirstName = user.Name,
                LastName = user.LastName,
                Email = user.Email
            }).ToList();

            response.UserList = httpMappedUsers;

            return new JsonResult(response);
        }
    }
}
