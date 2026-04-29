namespace HRMS.Exceptions
{
    public class PayrollNotFoundException : AppException
    {
        public PayrollNotFoundException() : base("Payroll record not found.") { }
        public PayrollNotFoundException(string message) : base(message) { }
        public PayrollNotFoundException(string message, Exception innerException) : base(message, innerException) { }
    }
}
