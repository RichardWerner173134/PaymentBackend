using PaymentBackend.Common.Model.Dto;

namespace PaymentBackend.Common.Model
{
    public class PaymentOverviewForDebitor
    {
        public List<FullPaymentDto> Payments { get; set; } = new List<FullPaymentDto>();

        public double TotalDebitorOnly { get; set; } = 0.0;
    }
}
