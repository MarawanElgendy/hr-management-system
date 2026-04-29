namespace DTOs;

public class TeamAttendanceDTO
{
    public int AttendanceID { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public DateTime ShiftDate { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public DateTime? EntryTime { get; set; }
    public DateTime? ExitTime { get; set; }
    public int? DurationMinutes { get; set; }
    public string ShiftName { get; set; } = string.Empty;
}