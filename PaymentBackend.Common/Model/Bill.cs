using PaymentBackend.Common.Model.Dto;

namespace PaymentBackend.Common.Model
{
    public class Bill
    {
        private const decimal Zero = (decimal)0.0;

        public string IssuedBy { get; private set; }

        public string IssuedFor { get; private set; }

        public decimal Amount { get; private set; }

        private readonly List<BillComposite> _billComposites;

        public Bill(string issuedBy, string issuedFor)
        {
            IssuedBy = issuedBy;
            IssuedFor = issuedFor;
            Amount = Zero;
            _billComposites = new List<BillComposite>();
        }

        public void AddBillComposite(FullPaymentDto payment, string creditor, string debitor)
        {
            bool paymentAlreadyProcessed = _billComposites.Exists(composite => composite.FullPayment.Id == payment.Id);
            if (paymentAlreadyProcessed)
            {
                return;
            }

            BillComposite composite;

            if (IssuedBy.ToLower().Equals(creditor.ToLower()) && IssuedFor.ToLower().Equals(debitor.ToLower()))
            {
                composite = new BillComposite(payment, true);
            }
            else if(IssuedBy.ToLower().Equals(debitor.ToLower()) && IssuedFor.ToLower().Equals(creditor.ToLower())) 
            {
                composite = new BillComposite(payment, false);
            }
            else
            {
                throw new ArgumentException($"Invalid combination of creditor=[{creditor}] and debitor=[{debitor}]. Cant add payment to bill with IssuedBy=[{IssuedBy}] and IssuedFor=[{IssuedFor}]");
            }

            _billComposites.Add(composite);

            Amount += composite.AmountPerDebitor;

            if (Amount < Zero)
            {
                SwapBillDirection();
            }
        }

        public List<BillComposite> GetBillComposites()
        {
            return _billComposites;
        }

        private void SwapBillDirection()
        {
            // swap direction
            (IssuedBy, IssuedFor) = (IssuedFor, IssuedBy);

            Amount = Zero;
            foreach (var billComposite in _billComposites)
            {
                billComposite.InvertAmount();
                Amount += billComposite.AmountPerDebitor;
            }
        }
    }
}
