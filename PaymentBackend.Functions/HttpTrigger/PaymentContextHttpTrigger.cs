using Microsoft.Extensions.Logging;
using PaymentBackend.BL.Http;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker;

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

        [Function(nameof(GetPaymentContexts))]
        public async Task<HttpResponseData> GetPaymentContexts(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "paymentContexts")] HttpRequestData req,
            ILogger log)
        {
            return await _paymentContextResolver.GetPaymentContexts(req);
        }
    }
}
