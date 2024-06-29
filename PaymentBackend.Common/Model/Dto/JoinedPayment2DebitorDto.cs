namespace PaymentBackend.Common.Model.Dto
{
    public class JoinedPayment2DebitorDto
    {
        public long PaymentId { get; set; }

        public long Payment2DebitorIdFk { get; set; }

        public long DebitorId { get; set; }
        public string DebitorUsername { get; set; } = string.Empty;

        public long CreditorId { get; set; }
        public string CreditorUsername { get; set; } = string.Empty;

        public long AuthorId { get; set; }
        public string AuthorUsername { get; set; } = string.Empty;


        public decimal Price { get; set; }

        public DateTime PaymentDate { get; set; }

        public DateTime PaymentUpdateTime { get; set; }

        public string? PaymentDescription { get; set; }
    }
}
