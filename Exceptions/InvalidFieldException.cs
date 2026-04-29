namespace HRMS.Exceptions
{
    public class InvalidFieldException : AppException
    {
        public InvalidFieldException() : base("Field is invalid or unauthorized for modification.") { }
        public InvalidFieldException(string message) : base(message) { }
        public InvalidFieldException(string message, Exception innerException) : base(message, innerException) { }
    }
}
