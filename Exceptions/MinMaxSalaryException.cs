namespace HRMS.Exceptions
{
    public class MinMaxSalaryException : AppException
    {
        public MinMaxSalaryException() : base("Minimum salary cannot be greater than or equal to maximum salary.") { }
        public MinMaxSalaryException(string message) : base(message) { }
        public MinMaxSalaryException(string message, Exception innerException) : base(message, innerException) { }
    }
}
