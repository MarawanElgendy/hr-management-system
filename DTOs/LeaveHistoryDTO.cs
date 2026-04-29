namespace DTOs;

public class LeaveHistoryDTO
{
    public int LeaveRequestId { get; set; }

    public string LeaveType { get; set; }
    public string Description { get; set; }
    public string Justification { get; set; }
    public int Duration { get; set; }
    public string Status { get; set; }
    public int ApprovalTiming { get; set; }
}
