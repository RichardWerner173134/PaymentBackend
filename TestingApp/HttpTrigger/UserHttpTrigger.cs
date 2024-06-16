using BL.Http;
using Common.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace TestingApp.HttpTrigger
{
    public class UserHttpTrigger
    {
        private readonly ILogger<UserHttpTrigger> _logger;

        private readonly IUserResolver _resolver;

        public UserHttpTrigger(ILogger<UserHttpTrigger> log, IUserResolver resolver)
        {
            _logger = log;
            _resolver = resolver;
        }

        [FunctionName(nameof(GetAllUsers))]
        public IActionResult GetAllUsers(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "users")] HttpRequest req)
        {
            return _resolver.GetPaymentUsersAsync();
        }
    }
}

