public class LeaveBalanceDTO
{
    public string LeaveType { get; set; } = null!;
    public decimal TotalEntitlement { get; set; }
    public decimal UsedDays { get; set; }
    public decimal RemainingDays { get; set; }
}
