namespace HRMS.Exceptions
{
    public class InvalidExchangeRateException : AppException
    {
        public InvalidExchangeRateException() : base("Exchange rate is zero or negative.") { }
        public InvalidExchangeRateException(string message) : base(message) { }
        public InvalidExchangeRateException(string message, Exception innerException) : base(message, innerException) { }
    }
}
