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


        [Function(nameof(GetPaymentsForDebitor))]
        public Task<HttpResponseData> GetPaymentsForDebitor(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "paymentContexts/{paymentContext}/payments-for-debitor/{username}")] HttpRequestData req,
            long paymentContext, 
            string username
        )
        {
            return _resolver.GetPaymentsForDebitor(req, paymentContext, username);
        }

        [Function(nameof(GetPaymentOverviewForDebitor))]
        public Task<HttpResponseData> GetPaymentOverviewForDebitor(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "paymentContexts/{paymentContext}/payment-overview-for-debitor/{username}")] HttpRequestData req,
            long paymentContext,
            string username
        )
        {
            return _resolver.GetPaymentOverviewForDebitor(req, paymentContext, username);
        }
    }
}
