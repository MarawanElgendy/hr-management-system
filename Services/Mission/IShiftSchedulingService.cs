namespace Services.Mission;

using System;
using System.Threading.Tasks;

public interface IShiftSchedulingService
{
    Task CreateSplitScheduleAsync(DateTime startDate, DateTime endDate, TimeSpan start1, TimeSpan end1, TimeSpan start2, TimeSpan end2);
    Task CreateRotationalScheduleAsync(DateTime startDate, DateTime endDate, int cycleLengthDays, string[] cycleTypes);
    Task AssignShiftsToEmployeeAsync(int employeeID, DateTime startDate, DateTime endDate, string shiftType);
    Task AssignShiftsToDepartmentAsync(int departmentId, DateTime startDate, DateTime endDate, string shiftType);
    Task<List<string>> GetScheduledShiftNamesAsync();
    
    // New methods for Individual Management
    Task<List<object>> GetEmployeeScheduleAsync(int employeeId, DateTime start, DateTime end);
    Task AssignCustomShiftAsync(int employeeId, DateTime date, TimeSpan start, TimeSpan end, string name);
    Task DeleteAssignmentAsync(int assignmentId);
}
