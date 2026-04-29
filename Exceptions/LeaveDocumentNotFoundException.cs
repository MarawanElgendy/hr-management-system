namespace HRMS.Exceptions
{
    public class LeaveDocumentNotFoundException : AppException
    {
        public LeaveDocumentNotFoundException() : base("Leave document not found.") { }
        public LeaveDocumentNotFoundException(string message) : base(message) { }
        public LeaveDocumentNotFoundException(string message, Exception innerException) : base(message, innerException) { }
    }
}
