using Microsoft.Azure.Functions.Worker.Http;
using PaymentBackend.Database.DatabaseServices;

namespace PaymentBackend.BL.Http
{
    public interface IPaymentContextResolver
    {
        Task<HttpResponseData> GetPaymentContexts(HttpRequestData req);
    }

    public class PaymentContextResolver : AbstractHttpResolver, IPaymentContextResolver
    {
        private readonly IPaymentContextDatabaseService _paymentContextDatabaseService;

        public PaymentContextResolver(
            IPaymentContextDatabaseService paymentContextDatabaseService
        )
        {
            _paymentContextDatabaseService = paymentContextDatabaseService;
        }

        public async Task<HttpResponseData> GetPaymentContexts(HttpRequestData req)
        {
            List<Common.Model.PaymentContext> result = _paymentContextDatabaseService.SelectAllPaymentContexts();

            List<Common.Generated.PaymentContext> mappedResults = result
                .Select(paymentContext => new Common.Generated.PaymentContext
                {
                    Id = (int)paymentContext.Id,
                    Name = paymentContext.ContextName,
                    IsClosed = paymentContext.IsClosed
                })
                .OrderByDescending(paymentContext => paymentContext.Id)
                .ToList();

            Common.Generated.GetPaymentContextsResponse response = new()
            {
                PaymentContexts = mappedResults
            };

            return await BuildOkResponse(req, response);
        }
    }
}
