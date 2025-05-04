using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using PaymentBackend.BL.Http;
using System.Threading.Tasks;

namespace PaymentBackend.Functions.HttpTrigger
{
    public class PaymentContextHttpTrigger
    {
        private readonly IPaymentContextResolver _paymentContextResolver;

        public PaymentContextHttpTrigger(
            IPaymentContextResolver paymentContextResolver
        )
        {
            _paymentContextResolver = paymentContextResolver;
        }

        [FunctionName(nameof(GetPaymentContexts))]
        public async Task<IActionResult> GetPaymentContexts(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "paymentContexts")] HttpRequest req,
            ILogger log)
        {
            return await _paymentContextResolver.GetPaymentContexts(req);
        }
    }
}
