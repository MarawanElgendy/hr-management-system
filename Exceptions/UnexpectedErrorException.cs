namespace HRMS.Exceptions
{
    public class UnexpectedErrorException : AppException
    {
        public UnexpectedErrorException() : base("An unexpected error occurred.") { }
        public UnexpectedErrorException(string message) : base(message) { }
        public UnexpectedErrorException(string message, Exception innerException) : base(message, innerException) { }
    }
}
