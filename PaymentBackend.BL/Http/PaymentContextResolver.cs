using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PaymentBackend.Database.DatabaseServices;

namespace PaymentBackend.BL.Http
{
    public interface IPaymentContextResolver
    {
        Task<IActionResult> GetPaymentContexts(HttpRequest req);
    }

    public class PaymentContextResolver : IPaymentContextResolver
    {
        private readonly IPaymentContextDatabaseService _paymentContextDatabaseService;

        public PaymentContextResolver(
            IPaymentContextDatabaseService paymentContextDatabaseService
        )
        {
            _paymentContextDatabaseService = paymentContextDatabaseService;
        }

        public async Task<IActionResult> GetPaymentContexts(HttpRequest req)
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

            return await Task.FromResult<IActionResult>(new JsonResult(response));
        }
    }
}
