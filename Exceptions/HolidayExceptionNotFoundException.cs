namespace HRMS.Exceptions
{
    public class HolidayExceptionNotFoundException : AppException
    {
        public HolidayExceptionNotFoundException() : base("Holiday or exception not found.") { }
        public HolidayExceptionNotFoundException(string message) : base(message) { }
        public HolidayExceptionNotFoundException(string message, Exception innerException) : base(message, innerException) { }
    }
}
