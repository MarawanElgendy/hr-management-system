namespace DTOs;

public class OrgHierarchyDTO
{
    public int DepartmentID { get; set; }
    public string DepartmentName { get; set; } = string.Empty;
    public int EmployeeID { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public int? ManagerID { get; set; }
    public string? ManagerName { get; set; }
    public string? PositionTitle { get; set; }
    public int HierarchyLevel { get; set; }
}
