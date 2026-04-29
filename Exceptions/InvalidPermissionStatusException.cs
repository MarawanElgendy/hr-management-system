namespace HRMS.Exceptions
{
    public class InvalidPermissionStatusException : AppException
    {
        public InvalidPermissionStatusException() : base("Permission status is invalid.") { }
        public InvalidPermissionStatusException(string message) : base(message) { }
        public InvalidPermissionStatusException(string message, Exception innerException) : base(message, innerException) { }
    }
}
