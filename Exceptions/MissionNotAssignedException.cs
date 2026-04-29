namespace HRMS.Exceptions
{
    public class MissionNotAssignedException : AppException
    {
        public MissionNotAssignedException() : base("Mission was not assigned to the specified manager.") { }
        public MissionNotAssignedException(string message) : base(message) { }
        public MissionNotAssignedException(string message, Exception innerException) : base(message, innerException) { }
    }
}
