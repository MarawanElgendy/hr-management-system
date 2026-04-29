namespace HRMS.Exceptions
{
    public class AuthenticationFailedException : AppException
    {
        public AuthenticationFailedException() : base("Password authentication failed.") { }
        public AuthenticationFailedException(string message) : base(message) { }
        public AuthenticationFailedException(string message, Exception innerException) : base(message, innerException) { }
    }
}
