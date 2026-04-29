namespace HRMS.Exceptions
{
    public class InvalidReimbursementActionException : AppException
    {
        public InvalidReimbursementActionException() : base("Reimbursement action is invalid.") { }
        public InvalidReimbursementActionException(string message) : base(message) { }
        public InvalidReimbursementActionException(string message, Exception innerException) : base(message, innerException) { }
    }
}
