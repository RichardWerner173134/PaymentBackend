using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PaymentBackend.BL.Http;

namespace PaymentBackend.Functions.HttpTrigger
{
    public class PaymentsForAuthorHttpTrigger
    {
        private readonly IPaymentForUserResolver _resolver;

        public PaymentsForAuthorHttpTrigger(IPaymentForUserResolver resolver)
        {
            _resolver = resolver;
        }


        [FunctionName(nameof(GetPaymentsForAuthor))]
        public async Task<IActionResult> GetPaymentsForAuthor(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "payments-for-author/{username}")] HttpRequest req,
            string username,
            ILogger log)
        {
            return await _resolver.GetPaymentsForAuthor(username);
        }
    }
}
