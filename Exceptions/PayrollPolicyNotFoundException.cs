namespace HRMS.Exceptions
{
    public class PayrollPolicyNotFoundException : AppException
    {
        public PayrollPolicyNotFoundException() : base("Payroll policy not found.") { }
        public PayrollPolicyNotFoundException(string message) : base(message) { }
        public PayrollPolicyNotFoundException(string message, Exception innerException) : base(message, innerException) { }
    }
}
