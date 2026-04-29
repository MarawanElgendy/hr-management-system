namespace HRMS.Exceptions
{
    public class InvalidTaxRateException : AppException
    {
        public InvalidTaxRateException() : base("Tax rate is outside valid range (0-100).") { }
        public InvalidTaxRateException(string message) : base(message) { }
        public InvalidTaxRateException(string message, Exception innerException) : base(message, innerException) { }
    }
}
