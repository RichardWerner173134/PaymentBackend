using System.Threading;
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

        [Function("GetAllPayments")]
        public Task<HttpResponseData> GetAllPaymentsAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "paymentContexts/{paymentContext}/payments")] HttpRequestData req, 
            long paymentContext,
            CancellationToken cancellationToken
        )
        {
            return _resolver.GetPaymentsAsync(req, paymentContext, cancellationToken);
        }

        [Function("GetPaymentById")]
        public Task<HttpResponseData> GetPaymentByIdAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "paymentContexts/{paymentContext}/payments/{paymentId}")] HttpRequestData req,
            long paymentContext,
            long paymentId,
            CancellationToken cancellationToken
        )
        {
            return _resolver.GetPaymentByIdAsync(req, paymentContext, paymentId, cancellationToken);
        }

        [Function("PostPayment")]
        public async Task<HttpResponseData> PostPaymentAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "paymentContexts/{paymentContext}/payments")] HttpRequestData req, 
            long paymentContext,
            CancellationToken cancellationToken
        )
        {
            return await _resolver.ProcessNewPaymentAsync(req, paymentContext, cancellationToken);
        }

        [Function("DeletePaymentById")]
        public Task<HttpResponseData> DeletePaymentByIdAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "paymentContexts/{paymentContext}/payments/{paymentId}")] HttpRequestData req, 
            long paymentContext, 
            long paymentId,
            CancellationToken cancellationToken
        )
        {
            return _resolver.DeletePaymentByIdAsync(req, paymentContext, paymentId, cancellationToken);
        }
    }
}
