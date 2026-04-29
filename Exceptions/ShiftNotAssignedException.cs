namespace HRMS.Exceptions
{
    public class ShiftNotAssignedException : AppException
    {
        public ShiftNotAssignedException() : base("Shift is not assigned to the employee.") { }
        public ShiftNotAssignedException(string message) : base(message) { }
        public ShiftNotAssignedException(string message, Exception innerException) : base(message, innerException) { }
    }
}
