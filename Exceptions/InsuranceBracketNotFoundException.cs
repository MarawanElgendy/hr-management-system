namespace HRMS.Exceptions
{
    public class InsuranceBracketNotFoundException : AppException
    {
        public InsuranceBracketNotFoundException() : base("Insurance bracket not found.") { }
        public InsuranceBracketNotFoundException(string message) : base(message) { }
        public InsuranceBracketNotFoundException(string message, Exception innerException) : base(message, innerException) { }
    }
}
