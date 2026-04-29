namespace HRMS.Exceptions
{
    public class ContractNotFoundException : AppException
    {
        public ContractNotFoundException() : base("Contract not found.") { }
        public ContractNotFoundException(string message) : base(message) { }
        public ContractNotFoundException(string message, Exception innerException) : base(message, innerException) { }
    }
}
