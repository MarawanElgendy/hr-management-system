namespace HRMS.Exceptions
{
    public class DepartmentNotFoundException : AppException
    {
        public DepartmentNotFoundException() : base("Department not found.") { }
        public DepartmentNotFoundException(string message) : base(message) { }
        public DepartmentNotFoundException(string message, Exception innerException) : base(message, innerException) { }
    }
}
