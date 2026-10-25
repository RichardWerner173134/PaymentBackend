using Microsoft.Azure.Functions.Worker.Http;
using PaymentBackend.BL.Core;
using PaymentBackend.BL.Mapper;
using PaymentBackend.Database.DatabaseServices;
using System.Net;

namespace PaymentBackend.BL.Http
{
    public interface IBillResolver
    {
        Task<HttpResponseData> GetAllBillsAsync(HttpRequestData req, long paymentContext, CancellationToken cancellationToken);
        Task<HttpResponseData> GetBillsForUserAsync(HttpRequestData req, long paymentContext, string username, CancellationToken cancellationToken);
        Task<HttpResponseData> GetBillOverviewsForUserAsync(HttpRequestData req, long paymentContext, string username, CancellationToken cancellationToken);
        Task<HttpResponseData> GetAllBillOverviewsAsync(HttpRequestData req, long paymentContext, CancellationToken cancellationToken);
        Task<HttpResponseData> GetMinimizedBillsAsync(HttpRequestData req, long paymentContext, CancellationToken cancellationToken);
    }

    public class BillResolver : AbstractHttpResolver, IBillResolver
    {
        private readonly IBillCalculationService _billCalculationService;
        private readonly IBillMinimizationService _billMinimizationService;
        private readonly IPaymentDatabaseService _paymentDatabaseService;
        private readonly IBillHttpMapper _httpMapper;

        public BillResolver(IPaymentDatabaseService paymentDatabaseService,
            IBillCalculationService billCalculationService,
            IBillMinimizationService billMinimizationService,
            IBillHttpMapper httpMapper)
        {
            _paymentDatabaseService = paymentDatabaseService;
            _billCalculationService = billCalculationService;
            _billMinimizationService = billMinimizationService;
            _httpMapper = httpMapper;
        }

        public async Task<HttpResponseData> GetAllBillsAsync(HttpRequestData req, long paymentContext, CancellationToken cancellationToken)
        {
            DateTime calculationTime = DateTime.Now;

            List<Common.Model.Dto.FullPaymentDto> allPayments = await _paymentDatabaseService.SelectAllPaymentsAsync(paymentContext, cancellationToken);
            List<Common.Model.Bill> bills = _billCalculationService.GetBills(allPayments);
            List<Common.Generated.Bill> mappedBills = _httpMapper.MapBills(bills);

            Common.Generated.GetAllBillsResponse response = new()
            {
                Bills = mappedBills,
                CalculationTime = calculationTime
            };

            return await BuildOkResponseAsync(req, response, cancellationToken);
        }

        public async Task<HttpResponseData> GetBillsForUserAsync(HttpRequestData req, long paymentContext, string username, CancellationToken cancellationToken)
        {
            username = username.ToLower();
            
            DateTime calculationTime = DateTime.UtcNow;

            List<Common.Model.Dto.FullPaymentDto> paymentsForCreditor = await _paymentDatabaseService.SelectPaymentsByCreditorAsync(paymentContext, username, cancellationToken);
            List<Common.Model.Dto.FullPaymentDto> paymentsForDebitor = await _paymentDatabaseService.SelectPaymentsByDebitorAsync(paymentContext, username, cancellationToken);

            List<Common.Model.Dto.FullPaymentDto> allPayments = [.. paymentsForCreditor, .. paymentsForDebitor];

            List<Common.Model.Bill> bills = _billCalculationService.GetBills(allPayments);
            bills = FilterByUsername(bills, username);

            List<Common.Generated.Bill> mappedBills = _httpMapper.MapBills(bills);

            Common.Generated.GetBillsForUserResponse response = new()
            {
                Bills = mappedBills,
                CalculationTime = calculationTime
            };

            return await BuildOkResponseAsync(req, response, cancellationToken);
        }

        public async Task<HttpResponseData> GetAllBillOverviewsAsync(HttpRequestData req, long paymentContext, CancellationToken cancellationToken)
        {
            DateTime calculationTime = DateTime.UtcNow;

            List<Common.Model.Dto.FullPaymentDto> allPayments = await _paymentDatabaseService.SelectAllPaymentsAsync(paymentContext, cancellationToken);
            List<Common.Model.Bill> bills = _billCalculationService.GetBills(allPayments);
            List<Common.Generated.ShortBill> mappedBills = _httpMapper.MapShortBills(bills);

            Common.Generated.GetAllBillOverviewsResponse response = new()
            {
                Bills = mappedBills,
                CalculationTime = calculationTime
            };

            return await BuildOkResponseAsync(req, response, cancellationToken);
        }

        public async Task<HttpResponseData> GetMinimizedBillsAsync(HttpRequestData req, long paymentContext, CancellationToken cancellationToken)
        {
            DateTime calculationTime = DateTime.UtcNow;

            List<Common.Model.Dto.FullPaymentDto> allPayments = await _paymentDatabaseService.SelectAllPaymentsAsync(paymentContext, cancellationToken);
            List<Common.Model.Bill> bills = _billCalculationService.GetBills(allPayments);
            List<Common.Model.MinimizedBill> minimizedBills = _billMinimizationService.MinimizeBills(bills);
            List<Common.Generated.ShortBill> mappedBills = _httpMapper.MapMinimizedBills(minimizedBills);

            Common.Generated.GetMinimizedBillsResponse response = new()
            {
                Bills = mappedBills,
                CalculationTime = calculationTime
            };

            return await BuildOkResponseAsync(req, response, cancellationToken);
        }

        public async Task<HttpResponseData> GetBillOverviewsForUserAsync(HttpRequestData req, long paymentContext, string username, CancellationToken cancellationToken)
        {
            username = username.ToLower();
            
            DateTime calculationTime = DateTime.UtcNow;

            List<Common.Model.Dto.FullPaymentDto> paymentsForCreditor = await _paymentDatabaseService.SelectPaymentsByCreditorAsync(paymentContext, username, cancellationToken);
            List<Common.Model.Dto.FullPaymentDto> paymentsForDebitor = await _paymentDatabaseService.SelectPaymentsByDebitorAsync(paymentContext, username, cancellationToken);

            List<Common.Model.Dto.FullPaymentDto> allPayments = [.. paymentsForCreditor, .. paymentsForDebitor];

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

            return await BuildOkResponseAsync(req, response, cancellationToken);
        }

        private static List<Common.Model.Bill> FilterByUsername(List<Common.Model.Bill> bills, string username) =>
        [
            .. bills
                .Where(bill =>
                    bill.IssuedBy.ToLower().Equals(username.ToLower()) ||
                    bill.IssuedFor.ToLower().Equals(username.ToLower()))
        ];
    }
}
