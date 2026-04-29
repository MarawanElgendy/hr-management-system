namespace HRMS.Exceptions
{
    public class DeadlineConfigurationException : AppException
    {
        public DeadlineConfigurationException() : base("Deadline configuration is invalid. Start date is after end date.") { }
        public DeadlineConfigurationException(string message) : base(message) { }
        public DeadlineConfigurationException(string message, Exception innerException) : base(message, innerException) { }
    }
}
