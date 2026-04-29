namespace HRMS.Exceptions
{
    public class RoleNotFoundException : AppException
    {
        public RoleNotFoundException() : base("Role not found.") { }
        public RoleNotFoundException(string message) : base(message) { }
        public RoleNotFoundException(string message, Exception innerException) : base(message, innerException) { }
    }
}
