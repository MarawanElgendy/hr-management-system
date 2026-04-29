namespace DTOs;

public class DepartmentStatisticsDTO
{
    public int DepartmentID { get; set; }
    public string DepartmentName { get; set; } = string.Empty;
    public string HeadName { get; set; } = "N/A";
    public int TeamSize { get; set; }
    public decimal AverageSalary { get; set; }
    public double SpanOfControl { get; set; } // Avg employees per manager
}
