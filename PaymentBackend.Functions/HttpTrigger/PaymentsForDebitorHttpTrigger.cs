using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
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
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "paymentContexts/{paymentContext}/payments-for-debitor/{username}")] HttpRequest req,
            long paymentContext, 
            string username
        )
        {
            return _resolver.GetPaymentsForDebitor(paymentContext, username);
        }

        [FunctionName(nameof(GetPaymentOverviewForDebitor))]
        public Task<IActionResult> GetPaymentOverviewForDebitor(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "paymentContexts/{paymentContext}/payment-overview-for-debitor/{username}")] HttpRequest req,
            long paymentContext,
            string username
        )
        {
            return _resolver.GetPaymentOverviewForDebitor(paymentContext, username);
        }
    }
}
