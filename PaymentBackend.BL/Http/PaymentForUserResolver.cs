using Microsoft.AspNetCore.Mvc;
using PaymentBackend.BL.Core;
using PaymentBackend.Common.Generated;
using PaymentBackend.Common.Model;
using PaymentBackend.Common.Model.Dto;
using PaymentBackend.Database.DatabaseServices;
using User = PaymentBackend.Common.Model.User;

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
            var result = _paymentsDatabaseService.SelectPaymentsByDebitor(username);

            var mappedPayments = result.Select(payment => new Common.Generated.Payment()
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

            GetPaymentsForDebitorResponse response = new()
            {
                Payments = mappedPayments
            };

            return Task.FromResult<IActionResult>(new JsonResult(response));
        }

        public Task<IActionResult> GetPaymentsForCreditor(string username)
        {
            var result = _paymentsDatabaseService.SelectPaymentsByCreditor(username);

            var mappedPayments = result.Select(payment => new Common.Generated.Payment()
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

            GetPaymentsForDebitorResponse response = new()
            {
                Payments = mappedPayments
            };

            return Task.FromResult<IActionResult>(new JsonResult(response));
        }

        public Task<IActionResult> GetPaymentsForAuthor(string username)
        {
            var result = _paymentsDatabaseService.SelectPaymentsByAuthor(username);

            var mappedPayments = result.Select(payment => new Common.Generated.Payment()
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

            GetPaymentsForAuthorResponse response = new()
            {
                Payments = mappedPayments
            };

            return Task.FromResult<IActionResult>(new JsonResult(response));
        }

        public Task<IActionResult> GetPaymentOverviewForDebitor(string username)
        {
            DateTime calculationTime = DateTime.Now;

            var allPayments = _paymentsDatabaseService.SelectPaymentsByDebitor(username);

            PaymentOverviewForDebitor paymentOverviewForDebitor = _paymentOverviewCalculator.GetPaymentOverviewForDebitor(allPayments, username);

            List<Payment> mappedPayments = paymentOverviewForDebitor.Payments
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

            GetPaymentOverviewForDebitorResponse response = new()
            {
                Payments = mappedPayments,
                TotalDebitorOnly = paymentOverviewForDebitor.TotalDebitorOnly,
                CalculationTime = calculationTime
            };

            return Task.FromResult<IActionResult>(new JsonResult(response));
        }

        public Task<IActionResult> GetPaymentOverviewForCreditor(string username)
        {
            DateTime calculationTime = DateTime.Now;

            var allPayments = _paymentsDatabaseService.SelectPaymentsByCreditor(username);
            
            PaymentOverviewForCreditor paymentOverviewForCreditor = _paymentOverviewCalculator.GetPaymentOverviewForCreditor(allPayments, username);

            List<Payment> mappedPayments = paymentOverviewForCreditor.Payments
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

            GetPaymentOverviewForCreditorResponse response = new()
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
