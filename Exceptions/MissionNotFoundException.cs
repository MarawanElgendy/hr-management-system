namespace HRMS.Exceptions
{
    public class MissionNotFoundException : AppException
    {
        public MissionNotFoundException() : base("Mission not found.") { }
        public MissionNotFoundException(string message) : base(message) { }
        public MissionNotFoundException(string message, Exception innerException) : base(message, innerException) { }
    }
}
