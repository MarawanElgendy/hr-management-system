namespace HRMS.Exceptions
{
    public class InvalidLeaveTypeException : AppException
    {
        public InvalidLeaveTypeException() : base("Leave type doesn't match requirements.") { }
        public InvalidLeaveTypeException(string message) : base(message) { }
        public InvalidLeaveTypeException(string message, Exception innerException) : base(message, innerException) { }
    }
}
