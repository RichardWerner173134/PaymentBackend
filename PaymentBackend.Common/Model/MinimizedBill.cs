namespace PaymentBackend.Common.Model
{
    public class MinimizedBill
    {
        public string IssuedBy { get; }

        public string IssuedFor { get; }

        public decimal Amount { get; }

        public MinimizedBill(string issuedBy, string issuedFor, decimal amount)
        {
            IssuedBy = issuedBy;
            IssuedFor = issuedFor;
            Amount = amount;
        }
    }
}
