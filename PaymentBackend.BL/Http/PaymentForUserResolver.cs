using Microsoft.AspNetCore.Mvc;
using PaymentBackend.BL.Core;
using PaymentBackend.Database.DatabaseServices;

namespace PaymentBackend.BL.Http
{
    public interface IPaymentForUserResolver
    {
        Task<IActionResult> GetPaymentsForDebitor(string username);
        Task<IActionResult> GetPaymentsForCreditor(string username);
        Task<IActionResult> GetPaymentsForAuthor(string username);

        Task<IActionResult> GetPaymentOverviewForDebitor(string username);
        Task<IActionResult> GetPaymentOverviewForCreditor(string username);
    }

    public class PaymentForUserResolver : IPaymentForUserResolver
    {
        private readonly IPaymentDatabaseService _paymentsDatabaseService;

        private readonly IPaymentOverviewCalculator _paymentOverviewCalculator;

        public PaymentForUserResolver(IPaymentDatabaseService paymentsDatabaseService, 
            IPaymentOverviewCalculator paymentOverviewCalculator)
        {
            _paymentsDatabaseService = paymentsDatabaseService;
            _paymentOverviewCalculator = paymentOverviewCalculator;
        }

        public Task<IActionResult> GetPaymentsForDebitor(string username)
        {
            username = username.ToLower();
            
            List<Common.Model.Dto.FullPaymentDto> result = _paymentsDatabaseService.SelectPaymentsByDebitor(username);
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

            return Task.FromResult<IActionResult>(new JsonResult(response));
        }

        public Task<IActionResult> GetPaymentsForCreditor(string username)
        {
            username = username.ToLower();
            
            List<Common.Model.Dto.FullPaymentDto> result = _paymentsDatabaseService.SelectPaymentsByCreditor(username);
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

            return Task.FromResult<IActionResult>(new JsonResult(response));
        }

        public Task<IActionResult> GetPaymentsForAuthor(string username)
        {
            username = username.ToLower();
            
            List<Common.Model.Dto.FullPaymentDto> result = _paymentsDatabaseService.SelectPaymentsByAuthor(username);
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

            return Task.FromResult<IActionResult>(new JsonResult(response));
        }

        public Task<IActionResult> GetPaymentOverviewForDebitor(string username)
        {
            username = username.ToLower();
            
            DateTime calculationTime = DateTime.Now;
            List<Common.Model.Dto.FullPaymentDto> allPayments = _paymentsDatabaseService.SelectPaymentsByDebitor(username);
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

            return Task.FromResult<IActionResult>(new JsonResult(response));
        }

        public Task<IActionResult> GetPaymentOverviewForCreditor(string username)
        {
            username = username.ToLower();
            
            DateTime calculationTime = DateTime.Now;
            List<Common.Model.Dto.FullPaymentDto> allPayments = _paymentsDatabaseService.SelectPaymentsByCreditor(username);
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

            return Task.FromResult<IActionResult>(new JsonResult(response));
        }
    }
}