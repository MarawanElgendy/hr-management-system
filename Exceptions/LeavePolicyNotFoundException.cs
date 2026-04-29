namespace HRMS.Exceptions
{
    public class LeavePolicyNotFoundException : AppException
    {
        public LeavePolicyNotFoundException() : base("Leave policy not found.") { }
        public LeavePolicyNotFoundException(string message) : base(message) { }
        public LeavePolicyNotFoundException(string message, Exception innerException) : base(message, innerException) { }
    }
}
