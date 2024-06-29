using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PaymentBackend.BL.Http;

namespace PaymentBackend.Functions.HttpTrigger
{
    public class PaymentsForDebitorHttpTrigger
    {
        private readonly IPaymentForUserResolver _resolver;

        public PaymentsForDebitorHttpTrigger(IPaymentForUserResolver resolver)
        {
            _resolver = resolver;
        }


        [FunctionName(nameof(GetPaymentsForDebitor))]
        public Task<IActionResult> GetPaymentsForDebitor(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "payments-for-debitor/{username}")] HttpRequest req,
            string username,
            ILogger log)
        {
            return _resolver.GetPaymentsForDebitor(username);
        }

        [FunctionName(nameof(GetPaymentOverviewForDebitor))]
        public Task<IActionResult> GetPaymentOverviewForDebitor(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "payment-overview-for-debitor/{username}")] HttpRequest req,
            string username,
            ILogger log)
        {
            return _resolver.GetPaymentOverviewForDebitor(username);
        }
    }
}
