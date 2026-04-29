namespace DTOs;

public class ExpiringContractDTO
{
    public int ContractID { get; set; }
    public int EmployeeID { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string ContractType { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string CurrentState { get; set; } = string.Empty;
}
