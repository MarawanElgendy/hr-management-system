namespace DTOs;

public class ShiftConfigurationDTO
{
    public int ShiftConfigurationID { get; set; }
    public string ShiftType { get; set; } = string.Empty;
    public decimal AllowanceAmount { get; set; }
}
