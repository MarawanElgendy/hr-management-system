namespace HRMS.Exceptions
{
    public class ReimbursementRequestNotFoundException : AppException
    {
        public ReimbursementRequestNotFoundException() : base("Reimbursement request not found.") { }
        public ReimbursementRequestNotFoundException(string message) : base(message) { }
        public ReimbursementRequestNotFoundException(string message, Exception innerException) : base(message, innerException) { }
    }
}
