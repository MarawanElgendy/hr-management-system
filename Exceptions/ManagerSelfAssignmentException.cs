namespace HRMS.Exceptions
{
    public class ManagerSelfAssignmentException : AppException
    {
        public ManagerSelfAssignmentException() : base("Employee cannot be assigned as their own manager.") { }
        public ManagerSelfAssignmentException(string message) : base(message) { }
        public ManagerSelfAssignmentException(string message, Exception innerException) : base(message, innerException) { }
    }
}
