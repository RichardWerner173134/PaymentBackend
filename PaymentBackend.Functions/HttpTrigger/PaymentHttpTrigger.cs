using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs;
using PaymentBackend.BL.Http;

namespace PaymentBackend.Functions.HttpTrigger
{
    public class PaymentHttpTrigger
    {
        private readonly IPaymentResolver _resolver;

        public PaymentHttpTrigger(IPaymentResolver resolver)
        {
            _resolver = resolver;
        }

        [FunctionName(nameof(GetAllPayments))]
        public Task<IActionResult> GetAllPayments(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "paymentContexts/{paymentContext}/payments")] HttpRequest req, 
            long paymentContext
        )
        {
            return _resolver.GetPayments(paymentContext);
        }

        [FunctionName(nameof(GetPaymentById))]
        public Task<IActionResult> GetPaymentById(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "paymentContexts/{paymentContext}/payments/{paymentId}")] HttpRequest req,
            long paymentContext,
            long paymentId
        )
        {
            return _resolver.GetPaymentById(paymentContext, paymentId);
        }

        [FunctionName(nameof(PostPayment))]
        public async Task<IActionResult> PostPayment(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "paymentContexts/{paymentContext}/payments")] HttpRequest req, 
            long paymentContext
        )
        {
            return await _resolver.ProcessNewPaymentAsync(paymentContext, req);
        }

        [FunctionName(nameof(DeletePaymentById))]
        public Task<IActionResult> DeletePaymentById(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "paymentContexts/{paymentContext}/payments/{paymentId}")] HttpRequest req, 
            long paymentContext, 
            long paymentId
        )
        {
            return _resolver.DeletePaymentById(paymentContext, paymentId);
        }
    }
}
