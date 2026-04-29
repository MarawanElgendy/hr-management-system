namespace HRMS.Exceptions
{
    public class HolidayExceptionAlreadyAssignedException : AppException
    {
        public HolidayExceptionAlreadyAssignedException() : base("Holiday exception is already assigned to this employee.") { }
        public HolidayExceptionAlreadyAssignedException(string message) : base(message) { }
        public HolidayExceptionAlreadyAssignedException(string message, Exception innerException) : base(message, innerException) { }
    }
}
