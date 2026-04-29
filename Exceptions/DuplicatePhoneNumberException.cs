namespace HRMS.Exceptions
{
    public class DuplicatePhoneNumberException : AppException
    {
        public DuplicatePhoneNumberException() : base("The phone number is already registered to another employee") { }
        public DuplicatePhoneNumberException(string message) : base(message) { }
        public DuplicatePhoneNumberException(string message, Exception innerException) : base(message, innerException) { }
    }
}
