using Microsoft.Azure.Functions.Worker.Http;
using PaymentBackend.BL.Core;
using PaymentBackend.Database.DatabaseServices;

namespace PaymentBackend.BL.Http
{
    public interface IPaymentForUserResolver
    {
        Task<HttpResponseData> GetPaymentsForDebitorAsync(HttpRequestData req, long paymentContext, string username);
        Task<HttpResponseData> GetPaymentsForCreditorAsync(HttpRequestData req, long paymentContext, string username);
        Task<HttpResponseData> GetPaymentsForAuthorAsync(HttpRequestData req, long paymentContext, string username);

        Task<HttpResponseData> GetPaymentOverviewForDebitorAsync(HttpRequestData req, long paymentContext, string username);
        Task<HttpResponseData> GetPaymentOverviewForCreditorAsync(HttpRequestData req, long paymentContext, string username);
    }

    public class PaymentForUserResolver : AbstractHttpResolver, IPaymentForUserResolver
    {
        private readonly IPaymentDatabaseService _paymentsDatabaseService;

        private readonly IPaymentOverviewCalculator _paymentOverviewCalculator;

        public PaymentForUserResolver(IPaymentDatabaseService paymentsDatabaseService, 
            IPaymentOverviewCalculator paymentOverviewCalculator)
        {
            _paymentsDatabaseService = paymentsDatabaseService;
            _paymentOverviewCalculator = paymentOverviewCalculator;
        }

        public async Task<HttpResponseData> GetPaymentsForDebitorAsync(HttpRequestData req, long paymentContext, string username)
        {
            username = username.ToLower();
            
            List<Common.Model.Dto.FullPaymentDto> result = await _paymentsDatabaseService.SelectPaymentsByDebitorAsync(paymentContext, username);
            List<Common.Generated.Payment> mappedPayments =
            [
                .. result.Select(payment => new Common.Generated.Payment
                {
                    PaymentId = payment.Id,
                    Price = decimal.ToDouble(payment.Price),
                    Creditor = payment.Creditor,
                    Debitors = payment.Debitors,
                    Author = payment.Author,
                    PaymentDate = payment.PaymentDate,
                    UpdateTime = payment.UpdateTime,
                    PaymentDescription = payment.PaymentDescription
                })
            ];

            Common.Generated.GetPaymentsForDebitorResponse response = new()
            {
                Payments = mappedPayments
            };

            return await BuildOkResponseAsync(req, response);
        }

        public async Task<HttpResponseData> GetPaymentsForCreditorAsync(HttpRequestData req, long paymentContext, string username)
        {
            username = username.ToLower();
            
            List<Common.Model.Dto.FullPaymentDto> result = await _paymentsDatabaseService.SelectPaymentsByCreditorAsync(paymentContext, username);
            List<Common.Generated.Payment> mappedPayments =
            [
                .. result.Select(payment => new Common.Generated.Payment
                {
                    PaymentId = payment.Id,
                    Price = decimal.ToDouble(payment.Price),
                    Creditor = payment.Creditor,
                    Debitors = payment.Debitors,
                    Author = payment.Author,
                    PaymentDate = payment.PaymentDate,
                    UpdateTime = payment.UpdateTime,
                    PaymentDescription = payment.PaymentDescription
                })
            ];

            Common.Generated.GetPaymentsForDebitorResponse response = new()
            {
                Payments = mappedPayments
            };

            return await BuildOkResponseAsync(req, response);
        }

        public async Task<HttpResponseData> GetPaymentsForAuthorAsync(HttpRequestData req, long paymentContext, string username)
        {
            username = username.ToLower();
            
            List<Common.Model.Dto.FullPaymentDto> result = await _paymentsDatabaseService.SelectPaymentsByAuthorAsync(paymentContext, username);
            List<Common.Generated.Payment> mappedPayments =
            [
                .. result.Select(payment => new Common.Generated.Payment
                {
                    PaymentId = payment.Id,
                    Price = decimal.ToDouble(payment.Price),
                    Creditor = payment.Creditor,
                    Debitors = payment.Debitors,
                    Author = payment.Author,
                    PaymentDate = payment.PaymentDate,
                    UpdateTime = payment.UpdateTime,
                    PaymentDescription = payment.PaymentDescription
                })
            ];

            Common.Generated.GetPaymentsForAuthorResponse response = new()
            {
                Payments = mappedPayments
            };

            return await BuildOkResponseAsync(req, response);
        }

        public async Task<HttpResponseData> GetPaymentOverviewForDebitorAsync(HttpRequestData req, long paymentContext, string username)
        {
            username = username.ToLower();
            
            DateTime calculationTime = DateTime.Now;
            List<Common.Model.Dto.FullPaymentDto> allPayments = await _paymentsDatabaseService.SelectPaymentsByDebitorAsync(paymentContext, username);
            Common.Model.PaymentOverviewForDebitor paymentOverviewForDebitor = _paymentOverviewCalculator.GetPaymentOverviewForDebitor(allPayments, username);
            List<Common.Generated.Payment> mappedPayments =
            [
                .. paymentOverviewForDebitor.Payments
                    .Select(payment => new Common.Generated.Payment
                    {
                        PaymentId = payment.Id,
                        Price = decimal.ToDouble(payment.Price),
                        Creditor = payment.Creditor,
                        Debitors = payment.Debitors,
                        Author = payment.Author,
                        PaymentDate = payment.PaymentDate,
                        UpdateTime = payment.UpdateTime,
                        PaymentDescription = payment.PaymentDescription
                    })
            ];

            Common.Generated.GetPaymentOverviewForDebitorResponse response = new()
            {
                Payments = mappedPayments,
                TotalDebitorOnly = paymentOverviewForDebitor.TotalDebitorOnly,
                CalculationTime = calculationTime
            };

            return await BuildOkResponseAsync(req, response);
        }

        public async Task<HttpResponseData> GetPaymentOverviewForCreditorAsync(HttpRequestData req, long paymentContext, string username)
        {
            username = username.ToLower();
            
            DateTime calculationTime = DateTime.Now;
            List<Common.Model.Dto.FullPaymentDto> allPayments = await _paymentsDatabaseService.SelectPaymentsByCreditorAsync(paymentContext, username);
            Common.Model.PaymentOverviewForCreditor paymentOverviewForCreditor = _paymentOverviewCalculator.GetPaymentOverviewForCreditor(allPayments, username);
            List<Common.Generated.Payment> mappedPayments =
            [
                .. paymentOverviewForCreditor.Payments
                    .Select(payment => new Common.Generated.Payment
                    {
                        PaymentId = payment.Id,
                        Price = decimal.ToDouble(payment.Price),
                        Creditor = payment.Creditor,
                        Debitors = payment.Debitors,
                        Author = payment.Author,
                        PaymentDate = payment.PaymentDate,
                        UpdateTime = payment.UpdateTime,
                        PaymentDescription = payment.PaymentDescription
                    })
            ];

            Common.Generated.GetPaymentOverviewForCreditorResponse response = new()
            {
                Payments = mappedPayments,
                TotalWithCreditor = paymentOverviewForCreditor.TotalWithCreditor,
                TotalWithoutCreditor = paymentOverviewForCreditor.TotalWithoutCreditor,
                CalculationTime = calculationTime
            };

            return await BuildOkResponseAsync(req, response);
        }
    }
}