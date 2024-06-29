using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using PaymentBackend.BL.Http;

namespace PaymentBackend.Functions.HttpTrigger
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
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "users")] HttpRequest req)
        {
            return _resolver.GetPaymentUsers();
        }
    }
}

