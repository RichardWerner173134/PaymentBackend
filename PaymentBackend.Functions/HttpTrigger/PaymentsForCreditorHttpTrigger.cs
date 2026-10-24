using System.Threading;
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


        [Function("GetPaymentsForCreditor")]
        public async Task<HttpResponseData> GetPaymentsForCreditorAsync(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "paymentContexts/{paymentContext}/payments-for-creditor/{username}")] HttpRequestData req,
            long paymentContext,
            string username,
            CancellationToken cancellationToken
        )
        {
            return await _resolver.GetPaymentsForCreditorAsync(req, paymentContext, username, cancellationToken);
        }

        [Function("GetPaymentOverviewForCreditor")]
        public async Task<HttpResponseData> GetPaymentOverviewForCreditorAsync(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "paymentContexts/{paymentContext}/payment-overview-for-creditor/{username}")] HttpRequestData req,
            long paymentContext,
            string username,
            CancellationToken cancellationToken
        )
        {
            return await _resolver.GetPaymentOverviewForCreditorAsync(req, paymentContext, username, cancellationToken);
        }
    }
}
