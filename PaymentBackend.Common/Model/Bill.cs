using PaymentBackend.Common.Model.Dto;

namespace PaymentBackend.Common.Model
{
    public class Bill
    {
        public string IssuedBy { get; set; }

        public string IssuedFor { get; set; }

        public double Amount { get; private set; }

        private readonly List<BillComposite> _billComposites;

        public Bill(string issuedBy, string issuedFor)
        {
            IssuedBy = issuedBy;
            IssuedFor = issuedFor;
            Amount = 0.0;
            _billComposites = new();
        }

        public void AddIncludedPayment(FullPaymentDto payment, string creditor, string debitor)
        {
            bool paymentAlreadyProcessed = _billComposites.Exists(composite => composite.FullPayment.Id == payment.Id);
            if (paymentAlreadyProcessed)
            {
                return;
            }

            BillComposite composite;

            if (IssuedBy.Equals(creditor))
            {
                composite = new(payment, true);
            }
            else
            {
                composite = new(payment, false);
            }

            _billComposites.Add(composite);

            Amount += composite.AmountPerDebitor;

            if (Amount < 0.0)
            {
                SwapBillDirection();
            }
        }

        public List<BillComposite> GetIncludedPayments()
        {
            return _billComposites;
        }

        private void SwapBillDirection()
        {
            // swap direction
            (IssuedBy, IssuedFor) = (IssuedFor, IssuedBy);

            Amount = 0.0;
            foreach (var billComposite in _billComposites)
            {
                billComposite.InvertAmount();
                Amount += billComposite.AmountPerDebitor;
            }
        }
    }
}
