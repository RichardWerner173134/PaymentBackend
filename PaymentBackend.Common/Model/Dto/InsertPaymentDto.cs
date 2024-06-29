namespace PaymentBackend.Common.Model.Dto
{
    public class InsertPaymentDto
    {
        public long PaymentId { get; set; }
        public User Creditor { get; set; }
        public List<User> Debitors { get; set; } = new();
        public User Author { get; set; }
        public decimal Price { get; set; }
        public DateTime PaymentDate { get; set; }
        public DateTime UpdateTime { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
