using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs;
using System.Threading.Tasks;
using PaymentBackend.BL.Http;

namespace PaymentBackend.Functions.HttpTrigger
{
    public class BillsHttpTrigger
    {
        private readonly IBillResolver _resolver;

        public BillsHttpTrigger(IBillResolver resolver)
        {
            _resolver = resolver;
        }

        [FunctionName(nameof(GetBills))]
        public async Task<IActionResult> GetBills(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "paymentContexts/{paymentContext}/bills")] HttpRequest req,
            long paymentContext
        )
        {
            return await _resolver.GetAllBills(paymentContext, req);
        }

        [FunctionName(nameof(GetBillsForUser))]
        public async Task<IActionResult> GetBillsForUser(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "paymentContexts/{paymentContext}/bills/all/users/{username}")] HttpRequest req,
            long paymentContext,
            string username
        )
        {
            return await _resolver.GetBillsForUser(paymentContext, req, username);
        }

        [FunctionName(nameof(GetBillOverviews))]
        public async Task<IActionResult> GetBillOverviews(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "paymentContexts/{paymentContext}/bill-overviews")] HttpRequest req,
            long paymentContext
        )
        {
            return await _resolver.GetAllBillOverviews(paymentContext);
        }

        [FunctionName(nameof(GetBillOverviewForUser))]
        public async Task<IActionResult> GetBillOverviewForUser(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "paymentContexts/{paymentContext}/bill-overviews/all/users/{username}")] HttpRequest req,
            long paymentContext,
            string username)
        {
            return await _resolver.GetBillOverviewsForUser(paymentContext, req, username);
        }
    }
}
