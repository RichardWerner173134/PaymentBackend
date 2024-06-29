using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs;
using System.Threading.Tasks;
using PaymentBackend.BL.Core;
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
        public async Task<IActionResult> GetBills([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "bills")] HttpRequest req)
        {
            return await _resolver.GetAllBills(req);
        }

        [FunctionName(nameof(GetBillsForUser))]
        public async Task<IActionResult> GetBillsForUser([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "bills-for-user/{username}")] HttpRequest req, 
            string username)
        {
            return await _resolver.GetAllBillsForUser(req, username);
        }

        [FunctionName(nameof(GetBillOverviewForUser))]
        public async Task<IActionResult> GetBillOverviewForUser([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "bill-overview-for-user/{username}")] HttpRequest req,
            string username)
        {
            return await _resolver.GetBillOverviewForUser(req, username);
        }
    }
}
