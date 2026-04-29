namespace HRMS.Exceptions
{
    public class CurrencyNotFoundException : AppException
    {
        public CurrencyNotFoundException() : base("Currency not found.") { }
        public CurrencyNotFoundException(string message) : base(message) { }
        public CurrencyNotFoundException(string message, Exception innerException) : base(message, innerException) { }
    }
}
