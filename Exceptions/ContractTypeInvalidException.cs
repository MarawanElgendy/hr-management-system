namespace HRMS.Exceptions
{
    public class ContractTypeInvalidException : AppException
    {
        public ContractTypeInvalidException() : base("Contract type is invalid.") { }
        public ContractTypeInvalidException(string message) : base(message) { }
        public ContractTypeInvalidException(string message, Exception innerException) : base(message, innerException) { }
    }
}
