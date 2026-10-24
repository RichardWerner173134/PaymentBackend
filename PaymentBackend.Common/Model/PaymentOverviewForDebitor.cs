using PaymentBackend.Common.Model.Dto;

namespace PaymentBackend.Common.Model
{
    public class PaymentOverviewForDebitor
    {
        public List<FullPaymentDto> Payments { get; set; } = [];

        public double TotalDebitorOnly { get; set; } = 0.0;
    }
}
