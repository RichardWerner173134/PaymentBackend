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
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "payments")] HttpRequest req)
        {
            return _resolver.GetPayments();
        }

        [FunctionName(nameof(GetPaymentById))]
        public Task<IActionResult> GetPaymentById(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "payments/{paymentId}")] HttpRequest req,
            long paymentId)
        {
            return _resolver.GetPaymentById(paymentId);
        }

        [FunctionName(nameof(PostPayment))]
        public async Task<IActionResult> PostPayment([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "payments")] HttpRequest req)
        {
            return await _resolver.ProcessNewPaymentAsync(req);
        }
    }
}
