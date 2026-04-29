namespace DTOs;

public class CreateContractDTO
{
    public int EmployeeID { get; set; }
    public string Type { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}
