namespace PaymentBackend.Common.Model
{
    public class PaymentContext
    {
        public required long Id { get; set; }
        public required string ContextName { get; set; }
        public required bool IsClosed { get; set; }
    }
}
