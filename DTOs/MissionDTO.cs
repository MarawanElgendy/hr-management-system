namespace DTOs;

public class MissionDTO
{
    public int MissionID { get; set; }
    public string Destination { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; }
    public string ManagerName { get; set; }
    public string EmployeeName { get; set; }
}