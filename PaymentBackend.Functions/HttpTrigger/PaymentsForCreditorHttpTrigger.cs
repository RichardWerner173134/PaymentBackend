using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
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
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "paymentContexts/{paymentContext}/payments-for-creditor/{username}")] HttpRequest req,
            long paymentContext,
            string username
        )
        {
            return await _resolver.GetPaymentsForCreditor(paymentContext, username);
        }

        [FunctionName(nameof(GetPaymentOverviewForCreditor))]
        public async Task<IActionResult> GetPaymentOverviewForCreditor(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "paymentContexts/{paymentContext}/payment-overview-for-creditor/{username}")] HttpRequest req,
            long paymentContext,
            string username
        )
        {
            return await _resolver.GetPaymentOverviewForCreditor(paymentContext, username);
        }
    }
}
