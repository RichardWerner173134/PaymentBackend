using System.Threading.Tasks;
using PaymentBackend.BL.Http;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker;

namespace PaymentBackend.Functions.HttpTrigger
{
    public class PaymentsForAuthorHttpTrigger
    {
        private readonly IPaymentForUserResolver _resolver;

        public PaymentsForAuthorHttpTrigger(IPaymentForUserResolver resolver)
        {
            _resolver = resolver;
        }


        [Function(nameof(GetPaymentsForAuthor))]
        public async Task<HttpResponseData> GetPaymentsForAuthor(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "paymentContexts/{paymentContext}/payments-for-author/{username}")] HttpRequestData req,
            long paymentContext,
            string username
        )
        {
            return await _resolver.GetPaymentsForAuthor(req, paymentContext, username);
        }
    }
}
