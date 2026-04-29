namespace HRMS.Exceptions
{
    public class InvalidContributionPercentageException : AppException
    {
        public InvalidContributionPercentageException() : base("Contribution percentage is outside valid range.") { }
        public InvalidContributionPercentageException(string message) : base(message) { }
        public InvalidContributionPercentageException(string message, Exception innerException) : base(message, innerException) { }
    }
}
