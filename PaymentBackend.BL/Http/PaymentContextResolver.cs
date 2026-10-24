using Microsoft.Azure.Functions.Worker.Http;
using PaymentBackend.Database.DatabaseServices;

namespace PaymentBackend.BL.Http
{
    public interface IPaymentContextResolver
    {
        Task<HttpResponseData> GetPaymentContextsAsync(HttpRequestData req, CancellationToken cancellationToken);
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

        public async Task<HttpResponseData> GetPaymentContextsAsync(HttpRequestData req, CancellationToken cancellationToken)
        {
            List<Common.Model.PaymentContext> result = await _paymentContextDatabaseService.SelectAllPaymentContextsAsync(cancellationToken);

            List<Common.Generated.PaymentContext> mappedResults =
            [
                .. result
                    .Select(paymentContext => new Common.Generated.PaymentContext
                    {
                        Id = (int)paymentContext.Id,
                        Name = paymentContext.ContextName,
                        IsClosed = paymentContext.IsClosed
                    })
                    .OrderByDescending(paymentContext => paymentContext.Id)
            ];

            Common.Generated.GetPaymentContextsResponse response = new()
            {
                PaymentContexts = mappedResults
            };

            return await BuildOkResponseAsync(req, response, cancellationToken);
        }
    }
}
