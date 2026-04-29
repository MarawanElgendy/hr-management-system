namespace HRMS.Exceptions
{
    public class InvalidLeaveStatusException : AppException
    {
        public InvalidLeaveStatusException() : base("Leave status is invalid. Must be Approved or Rejected.") { }
        public InvalidLeaveStatusException(string message) : base(message) { }
        public InvalidLeaveStatusException(string message, Exception innerException) : base(message, innerException) { }
    }
}
