namespace HRMS.Exceptions
{
    public class EmployeeNotFoundException : AppException
    {
        public EmployeeNotFoundException() : base("Employee not found.") { }
        public EmployeeNotFoundException(string message) : base(message) { }
        public EmployeeNotFoundException(string message, Exception innerException) : base(message, innerException) { }
    }
}
