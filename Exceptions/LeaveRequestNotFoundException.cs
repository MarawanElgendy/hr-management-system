namespace HRMS.Exceptions
{
    public class LeaveRequestNotFoundException : AppException
    {
        public LeaveRequestNotFoundException() : base("Leave request not found.") { }
        public LeaveRequestNotFoundException(string message) : base(message) { }
        public LeaveRequestNotFoundException(string message, Exception innerException) : base(message, innerException) { }
    }
}
