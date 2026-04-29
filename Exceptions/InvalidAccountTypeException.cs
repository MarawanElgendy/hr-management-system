public class InvalidAccountTypeException : AppException
{
        public InvalidAccountTypeException() : base("Invalid account type.") { }
        public InvalidAccountTypeException(string message) : base(message) { }
        public InvalidAccountTypeException(string message, Exception innerException) : base(message, innerException) { }
}
