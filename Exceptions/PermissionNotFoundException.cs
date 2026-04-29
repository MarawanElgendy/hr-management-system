namespace HRMS.Exceptions
{
    public class PermissionNotFoundException : AppException
    {
        public PermissionNotFoundException() : base("Permission not found or is not allowed.") { }
        public PermissionNotFoundException(string message) : base(message) { }
        public PermissionNotFoundException(string message, Exception innerException) : base(message, innerException) { }
    }
}
