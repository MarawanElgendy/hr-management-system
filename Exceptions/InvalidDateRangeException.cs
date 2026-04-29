namespace HRMS.Exceptions
{
    public class InvalidDateRangeException : AppException
    {
        public InvalidDateRangeException() : base("Start date is after end date.") { }
        public InvalidDateRangeException(string message) : base(message) { }
        public InvalidDateRangeException(string message, Exception innerException) : base(message, innerException) { }
    }
}
