namespace DTOs;

public class EmployeeSummaryDTO
{
    public int EmployeeID { get; set; }
    public string FullName { get; set; } = string.Empty;
    public int? DepartmentID { get; set; }
    public string? DepartmentName { get; set; }
    public string? PositionTitle { get; set; }
    public string? EmailAddress { get; set; }
    public string? PhoneNumber { get; set; }
}
