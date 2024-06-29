namespace PaymentBackend.Common.Exceptions
{
    public class PaymentNotFoundException : Exception
    {
        public PaymentNotFoundException(string message) : base(message)
        {

        }
    }
}
