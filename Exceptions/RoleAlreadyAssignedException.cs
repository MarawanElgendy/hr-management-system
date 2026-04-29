namespace HRMS.Exceptions
{
    public class RoleAlreadyAssignedException : AppException
    {
        public RoleAlreadyAssignedException() : base("Employee already has this role.") { }
        public RoleAlreadyAssignedException(string message) : base(message) { }
        public RoleAlreadyAssignedException(string message, Exception innerException) : base(message, innerException) { }
    }
}
