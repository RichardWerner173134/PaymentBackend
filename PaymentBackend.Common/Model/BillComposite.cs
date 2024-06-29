using PaymentBackend.Common.Model.Dto;

namespace PaymentBackend.Common.Model
{
    public class BillComposite
    {
        public FullPaymentDto FullPayment { get; set; }

        public double AmountPerDebitor { get; set; }

        public BillComposite(FullPaymentDto fullPayment, bool isPositiveAmountPerDebitor)
        {
            FullPayment = fullPayment;
            AmountPerDebitor = Decimal.ToDouble(fullPayment.Price / fullPayment.Debitors.Count);

            if (isPositiveAmountPerDebitor == false)
            {
                InvertAmount();
            }
        }

        public void InvertAmount()
        {
            AmountPerDebitor *= -1;
        }
    }
}
