namespace HRMS.Exceptions
{
    public class LeaveTypeNotFoundException : AppException
    {
        public LeaveTypeNotFoundException() : base("Leave type not found.") { }
        public LeaveTypeNotFoundException(string message) : base(message) { }
        public LeaveTypeNotFoundException(string message, Exception innerException) : base(message, innerException) { }
    }
}
