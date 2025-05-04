namespace PaymentBackend.Common.Model.Dto
{
    public class InsertPaymentDto
    {
        public long PaymentId { get; set; } = -1;

        public required long PaymentContext { get; set; }
        public required User Creditor { get; set; }
        public required List<User> Debitors { get; set; } = new();
        public required User Author { get; set; }
        public required decimal Price { get; set; }
        public required DateTime PaymentDate { get; set; }
        public required DateTime UpdateTime { get; set; }
        public required string Description { get; set; }
        public required short IsDeleted { get; set; } = 0;
    }
}
