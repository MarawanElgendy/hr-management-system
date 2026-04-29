namespace Services.Attendance;

public interface IShiftService
{
    Task CreateShiftTypeAsync(int shiftID, string name, string type, TimeSpan startTime, TimeSpan endTime, int breakDuration, DateTime shiftDate, string status);
    Task AssignCustomShiftAsync(int employeeID, string shiftName, string shiftType, TimeSpan startTime, TimeSpan endTime, DateTime startDate, DateTime endDate);
    Task AssignRotationalShiftAsync(int employeeID, int shiftCycle, DateTime startDate, DateTime endDate, string status);
    Task AssignShiftAsync(int employeeID, int shiftID);
    Task AssignShiftToDepartmentAsync(int departmentID, int shiftID, DateTime startDate, DateTime endDate);
    Task UpdateShiftStatusAsync(int shiftAssignmentID, string status);
}
