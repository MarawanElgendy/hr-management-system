namespace HRMS.Exceptions
{
    public class LeaveEntitlementNotFoundException : AppException
    {
        public LeaveEntitlementNotFoundException() : base("Employee has no entitlement record for this leave type.") { }
        public LeaveEntitlementNotFoundException(string message) : base(message) { }
        public LeaveEntitlementNotFoundException(string message, Exception innerException) : base(message, innerException) { }
    }
}
