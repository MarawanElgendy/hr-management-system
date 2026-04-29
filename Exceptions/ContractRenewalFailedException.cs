namespace HRMS.Exceptions
{
    public class ContractRenewalFailedException : AppException
    {
        public ContractRenewalFailedException() : base("Failed to renew contract.") { }
        public ContractRenewalFailedException(string message) : base(message) { }
        public ContractRenewalFailedException(string message, Exception innerException) : base(message, innerException) { }
    }
}
