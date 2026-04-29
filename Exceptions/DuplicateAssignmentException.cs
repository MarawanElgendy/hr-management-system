namespace HRMS.Exceptions
{
    public class DuplicateAssignmentException : AppException
    {
        public DuplicateAssignmentException() : base("Employee already has this assignment.") { }
        public DuplicateAssignmentException(string message) : base(message) { }
        public DuplicateAssignmentException(string message, Exception innerException) : base(message, innerException) { }
    }
}
