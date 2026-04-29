namespace HRMS.Exceptions
{
    public class InvalidPercentageException : AppException
    {
        public InvalidPercentageException() : base("Value is not between 0 and 100.") { }
        public InvalidPercentageException(string message) : base(message) { }
        public InvalidPercentageException(string message, Exception innerException) : base(message, innerException) { }
    }
}
