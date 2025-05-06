using Microsoft.Azure.Functions.Worker.Http;
using PaymentBackend.BL.Core;
using PaymentBackend.BL.Mapper;
using PaymentBackend.Database.DatabaseServices;
using System.Net;

namespace PaymentBackend.BL.Http
{
    public interface IBillResolver
    {
        Task<HttpResponseData> GetAllBills(HttpRequestData req, long paymentContext);
        Task<HttpResponseData> GetBillsForUser(HttpRequestData req, long paymentContext, string username);
        Task<HttpResponseData> GetBillOverviewsForUser(HttpRequestData req, long paymentContext, string username);
        Task<HttpResponseData> GetAllBillOverviews(HttpRequestData req, long paymentContext);
    }

    public class BillResolver : AbstractHttpResolver, IBillResolver
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

        public async Task<HttpResponseData> GetAllBills(HttpRequestData req, long paymentContext)
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

            return await BuildOkResponse(req, response);
        }

        public async Task<HttpResponseData> GetBillsForUser(HttpRequestData req, long paymentContext, string username)
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

            return await BuildOkResponse(req, response);
        }

        public async Task<HttpResponseData> GetAllBillOverviews(HttpRequestData req, long paymentContext)
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

            return await BuildOkResponse(req, response);
        }

        public async Task<HttpResponseData> GetBillOverviewsForUser(HttpRequestData req, long paymentContext, string username)
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

            return await BuildOkResponse(req, response);
        }

        private static List<Common.Model.Bill> FilterByUsername(List<Common.Model.Bill> bills, string username) => 
            bills
                .Where(bill => bill.IssuedBy.ToLower().Equals(username.ToLower()) || bill.IssuedFor.ToLower().Equals(username.ToLower()))
                .ToList();
    }
}
