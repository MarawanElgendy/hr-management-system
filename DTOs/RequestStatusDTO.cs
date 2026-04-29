namespace DTOs;

public class RequestStatusDTO
{
    public int RequestId { get; set; }

    public string RequestType { get; set; }
    public string Status { get; set; }
    public string Justification { get; set; }
    public int Duration { get; set; }
    public int ApprovalTiming { get; set; }
}
