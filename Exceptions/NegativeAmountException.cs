namespace HRMS.Exceptions
{
    public class NegativeAmountException : AppException
    {
        public NegativeAmountException() : base("Salary or amount value cannot be negative.") { }
        public NegativeAmountException(string message) : base(message) { }
        public NegativeAmountException(string message, Exception innerException) : base(message, innerException) { }
    }
}
