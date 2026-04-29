namespace HRMS.Exceptions
{
    public class InvalidShiftTypeException : AppException
    {
        public InvalidShiftTypeException() : base("Shift type is invalid. Must be Morning, Evening, or Night.") { }
        public InvalidShiftTypeException(string message) : base(message) { }
        public InvalidShiftTypeException(string message, Exception innerException) : base(message, innerException) { }
    }
}
