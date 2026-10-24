using System.Threading.Tasks;
using PaymentBackend.BL.Http;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker;

namespace PaymentBackend.Functions.HttpTrigger
{
    public class PaymentsForDebitorHttpTrigger
    {
        private readonly IPaymentForUserResolver _resolver;

        public PaymentsForDebitorHttpTrigger(IPaymentForUserResolver resolver)
        {
            _resolver = resolver;
        }


        [Function("GetPaymentsForDebitor")]
        public Task<HttpResponseData> GetPaymentsForDebitorAsync(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "paymentContexts/{paymentContext}/payments-for-debitor/{username}")] HttpRequestData req,
            long paymentContext, 
            string username
        )
        {
            return _resolver.GetPaymentsForDebitorAsync(req, paymentContext, username);
        }

        [Function("GetPaymentOverviewForDebitor")]
        public Task<HttpResponseData> GetPaymentOverviewForDebitorAsync(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "paymentContexts/{paymentContext}/payment-overview-for-debitor/{username}")] HttpRequestData req,
            long paymentContext,
            string username
        )
        {
            return _resolver.GetPaymentOverviewForDebitorAsync(req, paymentContext, username);
        }
    }
}
