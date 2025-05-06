namespace PaymentBackend.Common.Exceptions
{
    public class PaymentContextClosedException : Exception
    {
        public PaymentContextClosedException(string message) : base(message)
        {

        }
    }
}
