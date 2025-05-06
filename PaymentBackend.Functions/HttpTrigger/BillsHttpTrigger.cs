using System.Threading.Tasks;
using PaymentBackend.BL.Http;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker;

namespace PaymentBackend.Functions.HttpTrigger
{
    public class BillsHttpTrigger
    {
        private readonly IBillResolver _resolver;

        public BillsHttpTrigger(IBillResolver resolver)
        {
            _resolver = resolver;
        }

        [Function(nameof(GetBills))]
        public async Task<HttpResponseData> GetBills(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "paymentContexts/{paymentContext}/bills")] HttpRequestData req,
            long paymentContext
        )
        {
            return await _resolver.GetAllBills(req, paymentContext);
        }

        [Function(nameof(GetBillsForUser))]
        public async Task<HttpResponseData> GetBillsForUser(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "paymentContexts/{paymentContext}/bills/all/users/{username}")] HttpRequestData req,
            long paymentContext,
            string username
        )
        {
            return await _resolver.GetBillsForUser(req, paymentContext, username);
        }

        [Function(nameof(GetBillOverviews))]
        public async Task<HttpResponseData> GetBillOverviews(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "paymentContexts/{paymentContext}/bill-overviews")] HttpRequestData req,
            long paymentContext
        )
        {
            return await _resolver.GetAllBillOverviews(req, paymentContext);
        }

        [Function(nameof(GetBillOverviewForUser))]
        public async Task<HttpResponseData> GetBillOverviewForUser(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "paymentContexts/{paymentContext}/bill-overviews/all/users/{username}")] HttpRequestData req,
            long paymentContext,
            string username)
        {
            return await _resolver.GetBillOverviewsForUser(req, paymentContext, username);
        }
    }
}
