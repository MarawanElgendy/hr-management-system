namespace HRMS.Exceptions
{
    public class DuplicateEmailException : AppException
    {
        public DuplicateEmailException() : base("The email address is already registered to another employee") { }
        public DuplicateEmailException(string message) : base(message) { }
        public DuplicateEmailException(string message, Exception innerException) : base(message, innerException) { }
    }
}
