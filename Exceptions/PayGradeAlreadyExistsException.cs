namespace HRMS.Exceptions
{
    public class PayGradeAlreadyExistsException : AppException
    {
        public PayGradeAlreadyExistsException() : base("Pay grade name already exists.") { }
        public PayGradeAlreadyExistsException(string message) : base(message) { }
        public PayGradeAlreadyExistsException(string message, Exception innerException) : base(message, innerException) { }
    }
}
