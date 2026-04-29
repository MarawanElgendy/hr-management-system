namespace HRMS.Exceptions
{
    public class TerminationNotFoundException : AppException
    {
        public TerminationNotFoundException() : base("Employee has not been terminated.") { }
        public TerminationNotFoundException(string message) : base(message) { }
        public TerminationNotFoundException(string message, Exception innerException) : base(message, innerException) { }
    }
}
