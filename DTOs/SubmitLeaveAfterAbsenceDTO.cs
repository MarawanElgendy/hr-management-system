namespace DTOs;

public class SubmitLeaveAfterAbsenceDTO
{
    public int EmployeeId { get; set; }
    public string Justification { get; set; } = string.Empty;
}
