using Microsoft.Azure.Functions.Worker.Http;
using PaymentBackend.BL.Core;
using PaymentBackend.Database.DatabaseServices;

namespace PaymentBackend.BL.Http
{
    public interface IPaymentForUserResolver
    {
        Task<HttpResponseData> GetPaymentsForDebitor(HttpRequestData req, long paymentContext, string username);
        Task<HttpResponseData> GetPaymentsForCreditor(HttpRequestData req, long paymentContext, string username);
        Task<HttpResponseData> GetPaymentsForAuthor(HttpRequestData req, long paymentContext, string username);

        Task<HttpResponseData> GetPaymentOverviewForDebitor(HttpRequestData req, long paymentContext, string username);
        Task<HttpResponseData> GetPaymentOverviewForCreditor(HttpRequestData req, long paymentContext, string username);
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

        public async Task<HttpResponseData> GetPaymentsForDebitor(HttpRequestData req, long paymentContext, string username)
        {
            username = username.ToLower();
            
            List<Common.Model.Dto.FullPaymentDto> result = _paymentsDatabaseService.SelectPaymentsByDebitor(paymentContext, username);
            List<Common.Generated.Payment> mappedPayments = result.Select(payment => new Common.Generated.Payment()
            {
                PaymentId = payment.Id,
                Price = decimal.ToDouble(payment.Price),
                Creditor = payment.Creditor,
                Debitors = payment.Debitors,
                Author = payment.Author,
                PaymentDate = payment.PaymentDate,
                UpdateTime = payment.UpdateTime,
                PaymentDescription = payment.PaymentDescription
            }).ToList();

            Common.Generated.GetPaymentsForDebitorResponse response = new()
            {
                Payments = mappedPayments
            };

            return await BuildOkResponse(req, response);
        }

        public async Task<HttpResponseData> GetPaymentsForCreditor(HttpRequestData req, long paymentContext, string username)
        {
            username = username.ToLower();
            
            List<Common.Model.Dto.FullPaymentDto> result = _paymentsDatabaseService.SelectPaymentsByCreditor(paymentContext, username);
            List<Common.Generated.Payment> mappedPayments = result.Select(payment => new Common.Generated.Payment()
            {
                PaymentId = payment.Id,
                Price = decimal.ToDouble(payment.Price),
                Creditor = payment.Creditor,
                Debitors = payment.Debitors,
                Author = payment.Author,
                PaymentDate = payment.PaymentDate,
                UpdateTime = payment.UpdateTime,
                PaymentDescription = payment.PaymentDescription
            }).ToList();

            Common.Generated.GetPaymentsForDebitorResponse response = new()
            {
                Payments = mappedPayments
            };

            return await BuildOkResponse(req, response);
        }

        public async Task<HttpResponseData> GetPaymentsForAuthor(HttpRequestData req, long paymentContext, string username)
        {
            username = username.ToLower();
            
            List<Common.Model.Dto.FullPaymentDto> result = _paymentsDatabaseService.SelectPaymentsByAuthor(paymentContext, username);
            List<Common.Generated.Payment> mappedPayments = result.Select(payment => new Common.Generated.Payment()
            {
                PaymentId = payment.Id,
                Price = decimal.ToDouble(payment.Price),
                Creditor = payment.Creditor,
                Debitors = payment.Debitors,
                Author = payment.Author,
                PaymentDate = payment.PaymentDate,
                UpdateTime = payment.UpdateTime,
                PaymentDescription = payment.PaymentDescription
            }).ToList();

            Common.Generated.GetPaymentsForAuthorResponse response = new()
            {
                Payments = mappedPayments
            };

            return await BuildOkResponse(req, response);
        }

        public async Task<HttpResponseData> GetPaymentOverviewForDebitor(HttpRequestData req, long paymentContext, string username)
        {
            username = username.ToLower();
            
            DateTime calculationTime = DateTime.Now;
            List<Common.Model.Dto.FullPaymentDto> allPayments = _paymentsDatabaseService.SelectPaymentsByDebitor(paymentContext, username);
            Common.Model.PaymentOverviewForDebitor paymentOverviewForDebitor = _paymentOverviewCalculator.GetPaymentOverviewForDebitor(allPayments, username);
            List<Common.Generated.Payment> mappedPayments = paymentOverviewForDebitor.Payments
                .Select(payment => new Common.Generated.Payment()
                {
                    PaymentId = payment.Id,
                    Price = decimal.ToDouble(payment.Price),
                    Creditor = payment.Creditor,
                    Debitors = payment.Debitors,
                    Author = payment.Author,
                    PaymentDate = payment.PaymentDate,
                    UpdateTime = payment.UpdateTime,
                    PaymentDescription = payment.PaymentDescription
                }).ToList();

            Common.Generated.GetPaymentOverviewForDebitorResponse response = new()
            {
                Payments = mappedPayments,
                TotalDebitorOnly = paymentOverviewForDebitor.TotalDebitorOnly,
                CalculationTime = calculationTime
            };

            return await BuildOkResponse(req, response);
        }

        public async Task<HttpResponseData> GetPaymentOverviewForCreditor(HttpRequestData req, long paymentContext, string username)
        {
            username = username.ToLower();
            
            DateTime calculationTime = DateTime.Now;
            List<Common.Model.Dto.FullPaymentDto> allPayments = _paymentsDatabaseService.SelectPaymentsByCreditor(paymentContext, username);
            Common.Model.PaymentOverviewForCreditor paymentOverviewForCreditor = _paymentOverviewCalculator.GetPaymentOverviewForCreditor(allPayments, username);
            List<Common.Generated.Payment> mappedPayments = paymentOverviewForCreditor.Payments
                .Select(payment => new Common.Generated.Payment()
                {
                    PaymentId = payment.Id,
                    Price = decimal.ToDouble(payment.Price),
                    Creditor = payment.Creditor,
                    Debitors = payment.Debitors,
                    Author = payment.Author,
                    PaymentDate = payment.PaymentDate,
                    UpdateTime = payment.UpdateTime,
                    PaymentDescription = payment.PaymentDescription
                }).ToList();

            Common.Generated.GetPaymentOverviewForCreditorResponse response = new()
            {
                Payments = mappedPayments,
                TotalWithCreditor = paymentOverviewForCreditor.TotalWithCreditor,
                TotalWithoutCreditor = paymentOverviewForCreditor.TotalWithoutCreditor,
                CalculationTime = calculationTime
            };

            return await BuildOkResponse(req, response);
        }
    }
}