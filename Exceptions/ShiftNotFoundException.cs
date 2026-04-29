namespace HRMS.Exceptions
{
    public class ShiftNotFoundException : AppException
    {
        public ShiftNotFoundException() : base("Shift not found.") { }
        public ShiftNotFoundException(string message) : base(message) { }
        public ShiftNotFoundException(string message, Exception innerException) : base(message, innerException) { }
    }
}
