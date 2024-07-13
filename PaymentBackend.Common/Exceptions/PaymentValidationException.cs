namespace PaymentBackend.Common.Exceptions
{
    public class PaymentValidationException : Exception
    {
        public PaymentValidationException(string message) : base(message)
        {

        }
    }
}
