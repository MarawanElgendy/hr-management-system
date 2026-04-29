namespace HRMS.Exceptions
{
    public class InvalidConfigurationException : AppException
    {
        public InvalidConfigurationException() : base("Configuration parameters are invalid.") { }
        public InvalidConfigurationException(string message) : base(message) { }
        public InvalidConfigurationException(string message, Exception innerException) : base(message, innerException) { }
    }
}
