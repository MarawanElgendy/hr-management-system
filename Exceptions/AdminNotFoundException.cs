namespace HRMS.Exceptions
{
    public class AdminNotFoundException : AppException
    {
        public AdminNotFoundException() : base("HR Administrator not found.") { }
        public AdminNotFoundException(string message) : base(message) { }
        public AdminNotFoundException(string message, Exception innerException) : base(message, innerException) { }
    }
}
