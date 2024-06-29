namespace PaymentBackend.Common.Model.Dto
{
    public class FullPaymentDto
    {
        public long Id { get; set; }

        public decimal Price { get; set; }
        
        public string Creditor { get; set; } = string.Empty;

        public List<string> Debitors { get; set; } = new();

        public string Author { get; set; } = string.Empty;

        public DateTime PaymentDate { get; set; }

        public DateTime UpdateTime { get; set; }

        public string? PaymentDescription { get; set; }
    }
}
