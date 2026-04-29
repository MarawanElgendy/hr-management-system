using System.ComponentModel.DataAnnotations;

namespace DTOs;

public enum ReassignmentType
{
    Department,
    Manager
}

public class ReassignEmployeeDTO
{
    [Required]
    public int EmployeeID { get; set; }

    [Required]
    public ReassignmentType Type { get; set; }

    public int? NewDepartmentID { get; set; }

    public int? NewManagerID { get; set; }
}
