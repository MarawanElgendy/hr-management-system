namespace HRMS.Exceptions
{
    public class DuplicateNationalIDException : AppException
    {
        public DuplicateNationalIDException() : base("The national ID is already registered to another employee") { }
        public DuplicateNationalIDException(string message) : base(message) { }
        public DuplicateNationalIDException(string message, Exception innerException) : base(message, innerException) { }
    }
}
