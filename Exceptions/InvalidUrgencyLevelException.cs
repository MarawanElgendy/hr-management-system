namespace HRMS.Exceptions
{
    public class InvalidUrgencyLevelException : AppException
    {
        public InvalidUrgencyLevelException() : base("Urgency level is invalid.") { }
        public InvalidUrgencyLevelException(string message) : base(message) { }
        public InvalidUrgencyLevelException(string message, Exception innerException) : base(message, innerException) { }
    }
}
