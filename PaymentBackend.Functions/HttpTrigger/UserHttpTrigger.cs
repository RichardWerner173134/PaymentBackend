using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using PaymentBackend.BL.Http;
using System.Threading.Tasks;

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

        [Function(nameof(GetAllUsers))]
        public Task<HttpResponseData> GetAllUsers(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "users")] HttpRequestData req)
        {
            return _resolver.GetPaymentUsers(req);
        }
    }
}

