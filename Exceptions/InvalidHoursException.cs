namespace HRMS.Exceptions
{
    public class InvalidHoursException : AppException
    {
        public InvalidHoursException() : base("Hours/duration values are invalid.") { }
        public InvalidHoursException(string message) : base(message) { }
        public InvalidHoursException(string message, Exception innerException) : base(message, innerException) { }
    }
}
