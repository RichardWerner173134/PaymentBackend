using PaymentBackend.Common.Model.Dto;

namespace PaymentBackend.Common.Model
{
    public class PaymentOverviewForCreditor
    {
        public List<FullPaymentDto> Payments { get; set; } = new List<FullPaymentDto>();

        public double TotalWithCreditor { get; set; } = 0.0;

        public double TotalWithoutCreditor { get; set; } = 0.0;
    }
}
