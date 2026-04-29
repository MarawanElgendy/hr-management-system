namespace HRMS.Exceptions
{
    public class LeaveRequestInvalidStateException : AppException
    {
        public LeaveRequestInvalidStateException() : base("Leave request status doesn't allow this operation.") { }
        public LeaveRequestInvalidStateException(string message) : base(message) { }
        public LeaveRequestInvalidStateException(string message, Exception innerException) : base(message, innerException) { }
    }
}
