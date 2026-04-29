namespace HRMS.Exceptions
{
    public class InvalidDayTypeException : AppException
    {
        public InvalidDayTypeException() : base("Day type is invalid. Must be Weekday, Weekend, or Holiday.") { }
        public InvalidDayTypeException(string message) : base(message) { }
        public InvalidDayTypeException(string message, Exception innerException) : base(message, innerException) { }
    }
}
