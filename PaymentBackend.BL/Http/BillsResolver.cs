using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PaymentBackend.BL.Core;
using PaymentBackend.BL.Mapper;
using PaymentBackend.Database.DatabaseServices;

namespace PaymentBackend.BL.Http
{
    public interface IBillResolver
    {
        Task<IActionResult> GetAllBills(long paymentContext, HttpRequest req);
        Task<IActionResult> GetBillsForUser(long paymentContext, HttpRequest req, string username);
        Task<IActionResult> GetBillOverviewsForUser(long paymentContext, HttpRequest req, string username);
        Task<IActionResult> GetAllBillOverviews(long paymentContext);
    }

    public class BillResolver : IBillResolver
    {
        private readonly IBillCalculationService _billCalculationService;
        private readonly IPaymentDatabaseService _paymentDatabaseService;
        private readonly IBillHttpMapper _httpMapper;

        public BillResolver(IPaymentDatabaseService paymentDatabaseService,
            IBillCalculationService billCalculationService,
            IBillHttpMapper httpMapper)
        {
            _paymentDatabaseService = paymentDatabaseService;
            _billCalculationService = billCalculationService;
            _httpMapper = httpMapper;
        }

        public Task<IActionResult> GetAllBills(long paymentContext, HttpRequest req)
        {
            DateTime calculationTime = DateTime.Now;

            List<Common.Model.Dto.FullPaymentDto> allPayments = _paymentDatabaseService.SelectAllPayments(paymentContext);
            List<Common.Model.Bill> bills = _billCalculationService.GetBills(allPayments);
            List<Common.Generated.Bill> mappedBills = _httpMapper.MapBills(bills);

            Common.Generated.GetAllBillsResponse response = new()
            {
                Bills = mappedBills,
                CalculationTime = calculationTime
            };

            return Task.FromResult<IActionResult>(new JsonResult(response));
        }

        public Task<IActionResult> GetBillsForUser(long paymentContext, HttpRequest req, string username)
        {
            username = username.ToLower();
            
            DateTime calculationTime = DateTime.UtcNow;

            List<Common.Model.Dto.FullPaymentDto> paymentsForCreditor = _paymentDatabaseService.SelectPaymentsByCreditor(paymentContext, username);
            List<Common.Model.Dto.FullPaymentDto> paymentsForDebitor = _paymentDatabaseService.SelectPaymentsByDebitor(paymentContext, username);

            List<Common.Model.Dto.FullPaymentDto> allPayments = paymentsForCreditor.Concat(paymentsForDebitor).ToList();

            List<Common.Model.Bill> bills = _billCalculationService.GetBills(allPayments);
            bills = FilterByUsername(bills, username);

            List<Common.Generated.Bill> mappedBills = _httpMapper.MapBills(bills);

            Common.Generated.GetBillsForUserResponse response = new()
            {
                Bills = mappedBills,
                CalculationTime = calculationTime
            };

            return Task.FromResult<IActionResult>(new JsonResult(response));
        }

        public Task<IActionResult> GetAllBillOverviews(long paymentContext)
        {
            DateTime calculationTime = DateTime.UtcNow;

            List<Common.Model.Dto.FullPaymentDto> allPayments = _paymentDatabaseService.SelectAllPayments(paymentContext);
            List<Common.Model.Bill> bills = _billCalculationService.GetBills(allPayments);
            List<Common.Generated.ShortBill> mappedBills = _httpMapper.MapShortBills(bills);

            Common.Generated.GetAllBillOverviewsResponse response = new()
            {
                Bills = mappedBills,
                CalculationTime = calculationTime
            };

            return Task.FromResult<IActionResult>(new JsonResult(response));
        }

        public Task<IActionResult> GetBillOverviewsForUser(long paymentContext, HttpRequest req, string username)
        {
            username = username.ToLower();
            
            DateTime calculationTime = DateTime.UtcNow;

            List<Common.Model.Dto.FullPaymentDto> paymentsForCreditor = _paymentDatabaseService.SelectPaymentsByCreditor(paymentContext, username);
            List<Common.Model.Dto.FullPaymentDto> paymentsForDebitor = _paymentDatabaseService.SelectPaymentsByDebitor(paymentContext, username);

            List<Common.Model.Dto.FullPaymentDto> allPayments = paymentsForCreditor.Concat(paymentsForDebitor).ToList();

            List<Common.Model.Bill> bills = _billCalculationService.GetBills(allPayments);
            bills = FilterByUsername(bills, username);
            decimal balance = _billCalculationService.GetBalanceForUser(bills, username);

            List<Common.Generated.ShortBill> mappedBills = _httpMapper.MapShortBills(bills);

            Common.Generated.GetBillOverviewsForUserResponse response = new()
            {
                Bills = mappedBills,
                Balance = Decimal.ToDouble(balance),
                CalculationTime = calculationTime
            };
            

            return Task.FromResult<IActionResult>(new JsonResult(response));
        }

        private static List<Common.Model.Bill> FilterByUsername(List<Common.Model.Bill> bills, string username) => 
            bills
                .Where(bill => bill.IssuedBy.ToLower().Equals(username.ToLower()) || bill.IssuedFor.ToLower().Equals(username.ToLower()))
                .ToList();
    }
}
