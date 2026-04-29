namespace HRMS.Exceptions
{
    public class MissingDateException : AppException
    {
        public MissingDateException() : base("Required date parameters are missing.") { }
        public MissingDateException(string message) : base(message) { }
        public MissingDateException(string message, Exception innerException) : base(message, innerException) { }
    }
}
