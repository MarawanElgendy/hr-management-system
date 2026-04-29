namespace HRMS.Exceptions
{
    public class PayrollPeriodNotFoundException : AppException
    {
        public PayrollPeriodNotFoundException() : base("Payroll period not found.") { }
        public PayrollPeriodNotFoundException(string message) : base(message) { }
        public PayrollPeriodNotFoundException(string message, Exception innerException) : base(message, innerException) { }
    }
}
