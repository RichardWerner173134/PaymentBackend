using System.Threading.Tasks;
using PaymentBackend.BL.Http;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker;

namespace PaymentBackend.Functions.HttpTrigger
{
    public class PaymentHttpTrigger
    {
        private readonly IPaymentResolver _resolver;

        public PaymentHttpTrigger(IPaymentResolver resolver)
        {
            _resolver = resolver;
        }

        [Function(nameof(GetAllPayments))]
        public Task<HttpResponseData> GetAllPayments(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "paymentContexts/{paymentContext}/payments")] HttpRequestData req, 
            long paymentContext
        )
        {
            return _resolver.GetPayments(req, paymentContext);
        }

        [Function(nameof(GetPaymentById))]
        public Task<HttpResponseData> GetPaymentById(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "paymentContexts/{paymentContext}/payments/{paymentId}")] HttpRequestData req,
            long paymentContext,
            long paymentId
        )
        {
            return _resolver.GetPaymentById(req, paymentContext, paymentId);
        }

        [Function(nameof(PostPayment))]
        public async Task<HttpResponseData> PostPayment(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "paymentContexts/{paymentContext}/payments")] HttpRequestData req, 
            long paymentContext
        )
        {
            return await _resolver.ProcessNewPaymentAsync(req, paymentContext);
        }

        [Function(nameof(DeletePaymentById))]
        public Task<HttpResponseData> DeletePaymentById(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "paymentContexts/{paymentContext}/payments/{paymentId}")] HttpRequestData req, 
            long paymentContext, 
            long paymentId
        )
        {
            return _resolver.DeletePaymentById(req, paymentContext, paymentId);
        }
    }
}
