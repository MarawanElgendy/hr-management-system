namespace DTOs;

public class PendingLeaveRequestDTO
{
    public int LeaveRequestID { get; set; }

    public int EmployeeID { get; set; }

    public string FullName { get; set; }

    public int LeaveTypeID { get; set; }

    public int Duration { get; set; }

    public string Status { get; set; }
}
