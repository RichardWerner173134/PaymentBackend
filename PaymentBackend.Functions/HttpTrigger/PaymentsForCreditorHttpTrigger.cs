using System.Threading.Tasks;
using PaymentBackend.BL.Http;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker;

namespace PaymentBackend.Functions.HttpTrigger
{
    public class PaymentsForCreditorHttpTrigger
    {
        private readonly IPaymentForUserResolver _resolver;

        public PaymentsForCreditorHttpTrigger(IPaymentForUserResolver resolver)
        {
            _resolver = resolver;
        }


        [Function(nameof(GetPaymentsForCreditor))]
        public async Task<HttpResponseData> GetPaymentsForCreditor(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "paymentContexts/{paymentContext}/payments-for-creditor/{username}")] HttpRequestData req,
            long paymentContext,
            string username
        )
        {
            return await _resolver.GetPaymentsForCreditor(req, paymentContext, username);
        }

        [Function(nameof(GetPaymentOverviewForCreditor))]
        public async Task<HttpResponseData> GetPaymentOverviewForCreditor(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "paymentContexts/{paymentContext}/payment-overview-for-creditor/{username}")] HttpRequestData req,
            long paymentContext,
            string username
        )
        {
            return await _resolver.GetPaymentOverviewForCreditor(req, paymentContext, username);
        }
    }
}
