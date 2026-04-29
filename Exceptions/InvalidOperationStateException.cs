namespace HRMS.Exceptions
{
    public class InvalidOperationStateException : AppException
    {
        public InvalidOperationStateException() : base("Operation cannot be performed in current state.") { }
        public InvalidOperationStateException(string message) : base(message) { }
        public InvalidOperationStateException(string message, Exception innerException) : base(message, innerException) { }
    }
}
