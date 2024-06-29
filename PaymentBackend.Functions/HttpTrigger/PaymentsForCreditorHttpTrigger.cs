using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PaymentBackend.BL.Http;

namespace PaymentBackend.Functions.HttpTrigger
{
    public class PaymentsForCreditorHttpTrigger
    {
        private readonly IPaymentForUserResolver _resolver;

        public PaymentsForCreditorHttpTrigger(IPaymentForUserResolver resolver)
        {
            _resolver = resolver;
        }


        [FunctionName(nameof(GetPaymentsForCreditor))]
        public async Task<IActionResult> GetPaymentsForCreditor(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "payments-for-creditor/{username}")] HttpRequest req,
            string username,
            ILogger log)
        {
            return await _resolver.GetPaymentsForCreditor(username);
        }

        [FunctionName(nameof(GetPaymentOverviewForCreditor))]
        public async Task<IActionResult> GetPaymentOverviewForCreditor(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "payment-overview-for-creditor/{username}")] HttpRequest req,
            string username,
            ILogger log)
        {
            return await _resolver.GetPaymentOverviewForCreditor(username);
        }
    }
}
