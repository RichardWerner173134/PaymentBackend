using PaymentBackend.Common.Model.Dto;

namespace PaymentBackend.BL.Mapper
{
    public interface IFullPaymentDto2HttpPaymentMapper
    {
        Common.Generated.Payment ConvertPayment(FullPaymentDto payment);
    }

    public class FullPaymentDto2HttpPaymentMapper : IFullPaymentDto2HttpPaymentMapper
    {
        public Common.Generated.Payment ConvertPayment(FullPaymentDto payment)
        {
            Common.Generated.Payment result = new()
            {
                PaymentId = payment.Id,
                Price = decimal.ToDouble(payment.Price),
                Creditor = payment.Creditor,
                Debitors = payment.Debitors,
                Author = payment.Author,
                PaymentDate = payment.PaymentDate,
                UpdateTime = payment.UpdateTime,
                PaymentDescription = payment.PaymentDescription
            };

            return result;
        }
    }
}
