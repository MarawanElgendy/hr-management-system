namespace HRMS.Exceptions
{
    public class NoEligibleEmployeesException : AppException
    {
        public NoEligibleEmployeesException() : base("No eligible employees found for this operation.") { }
        public NoEligibleEmployeesException(string message) : base(message) { }
        public NoEligibleEmployeesException(string message, Exception innerException) : base(message, innerException) { }
    }
}
