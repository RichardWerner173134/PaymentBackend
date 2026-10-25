using System.Threading;
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

        [Function("GetBills")]
        public async Task<HttpResponseData> GetBillsAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "paymentContexts/{paymentContext}/bills")] HttpRequestData req,
            long paymentContext,
            CancellationToken cancellationToken
        )
        {
            return await _resolver.GetAllBillsAsync(req, paymentContext, cancellationToken);
        }

        [Function("GetBillsForUser")]
        public async Task<HttpResponseData> GetBillsForUserAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "paymentContexts/{paymentContext}/bills/all/users/{username}")] HttpRequestData req,
            long paymentContext,
            string username,
            CancellationToken cancellationToken
        )
        {
            return await _resolver.GetBillsForUserAsync(req, paymentContext, username, cancellationToken);
        }

        [Function("GetBillOverviews")]
        public async Task<HttpResponseData> GetBillOverviewsAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "paymentContexts/{paymentContext}/bill-overviews")] HttpRequestData req,
            long paymentContext,
            CancellationToken cancellationToken
        )
        {
            return await _resolver.GetAllBillOverviewsAsync(req, paymentContext, cancellationToken);
        }

        [Function("GetBillOverviewForUser")]
        public async Task<HttpResponseData> GetBillOverviewForUserAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "paymentContexts/{paymentContext}/bill-overviews/all/users/{username}")] HttpRequestData req,
            long paymentContext,
            string username,
            CancellationToken cancellationToken)
        {
            return await _resolver.GetBillOverviewsForUserAsync(req, paymentContext, username, cancellationToken);
        }

        [Function("GetMinimizedBills")]
        public async Task<HttpResponseData> GetMinimizedBillsAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "paymentContexts/{paymentContext}/minimized-bills")] HttpRequestData req,
            long paymentContext,
            CancellationToken cancellationToken
        )
        {
            return await _resolver.GetMinimizedBillsAsync(req, paymentContext, cancellationToken);
        }
    }
}
