namespace HRMS.Exceptions
{
    public class ManagerNotFoundException : AppException
    {
        public ManagerNotFoundException() : base("Manager not found.") { }
        public ManagerNotFoundException(string message) : base(message) { }
        public ManagerNotFoundException(string message, Exception innerException) : base(message, innerException) { }
    }
}
