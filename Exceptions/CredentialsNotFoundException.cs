namespace HRMS.Exceptions
{
    public class CredentialsNotFoundException : AppException
    {
        public CredentialsNotFoundException() : base("Employee credentials not found.") { }
        public CredentialsNotFoundException(string message) : base(message) { }
        public CredentialsNotFoundException(string message, Exception innerException) : base(message, innerException) { }
    }
}
