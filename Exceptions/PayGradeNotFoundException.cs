namespace HRMS.Exceptions
{
    public class PayGradeNotFoundException : AppException
    {
        public PayGradeNotFoundException() : base("Pay grade not found.") { }
        public PayGradeNotFoundException(string message) : base(message) { }
        public PayGradeNotFoundException(string message, Exception innerException) : base(message, innerException) { }
    }
}
