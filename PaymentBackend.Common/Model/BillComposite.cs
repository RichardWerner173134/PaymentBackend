using PaymentBackend.Common.Model.Dto;

namespace PaymentBackend.Common.Model
{
    public class BillComposite
    {
        public FullPaymentDto FullPayment { get; private set; }

        public decimal AmountPerDebitor { get; private set; }

        // always use this constructor
        public BillComposite(FullPaymentDto fullPayment, bool isPositiveAmountPerDebitor)
        {
            FullPayment = fullPayment;
            AmountPerDebitor = fullPayment.Price / fullPayment.Debitors.Count;

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
