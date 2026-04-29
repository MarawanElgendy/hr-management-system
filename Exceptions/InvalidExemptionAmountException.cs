namespace HRMS.Exceptions
{
    public class InvalidExemptionAmountException : AppException
    {
        public InvalidExemptionAmountException() : base("Exemption amount is negative.") { }
        public InvalidExemptionAmountException(string message) : base(message) { }
        public InvalidExemptionAmountException(string message, Exception innerException) : base(message, innerException) { }
    }
}
