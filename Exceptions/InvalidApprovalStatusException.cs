namespace HRMS.Exceptions
{
    public class InvalidApprovalStatusException : AppException
    {
        public InvalidApprovalStatusException() : base("Status is invalid for approval.") { }
        public InvalidApprovalStatusException(string message) : base(message) { }
        public InvalidApprovalStatusException(string message, Exception innerException) : base(message, innerException) { }
    }
}
