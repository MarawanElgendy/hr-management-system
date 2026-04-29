namespace Services.Mission;

using HRMS.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System.Data;
using System;
using System.Threading.Tasks;
using System.Linq;

public class ShiftSchedulingService : IShiftSchedulingService
{
    private readonly HrmsContext context;

    public ShiftSchedulingService(HrmsContext context)
    {
        this.context = context;
    }

    public async Task CreateSplitScheduleAsync(DateTime startDate, DateTime endDate, TimeSpan start1, TimeSpan end1, TimeSpan start2, TimeSpan end2)
    {
        // Loop through each day
        for (var date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
        {
            // Create Shift 1
            await CreateShiftEntryAsync("Split Morning", "Split", date, start1, end1);

            // Create Shift 2
            await CreateShiftEntryAsync("Split Afternoon", "Split", date, start2, end2);
        }
    }

    public async Task CreateRotationalScheduleAsync(DateTime startDate, DateTime endDate, int cycleLengthDays, string[] cycleTypes)
    {
        int dayCounter = 0;
        int typeIndex = 0;

        for (var date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
        {
            string currentType = cycleTypes[typeIndex];
            
            // Define standard times based on type (Logic hardcoded for simplicity as per "Tool" approach)
            TimeSpan start, end;
            if (currentType == "Morning") { start = new TimeSpan(8, 0, 0); end = new TimeSpan(16, 0, 0); }
            else if (currentType == "Night") { start = new TimeSpan(20, 0, 0); end = new TimeSpan(4, 0, 0); }
            else { start = new TimeSpan(9, 0, 0); end = new TimeSpan(17, 0, 0); } // Default is Day

            await CreateShiftEntryAsync(currentType, "Rotational", date, start, end);

            dayCounter++;
            if (dayCounter >= cycleLengthDays)
            {
                dayCounter = 0;
                typeIndex = (typeIndex + 1) % cycleTypes.Length;
            }
        }
    }

    private async Task<int> CreateShiftEntryAsync(string name, string type, DateTime date, TimeSpan start, TimeSpan end)
    {
        var connection = context.Database.GetDbConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "CreateShiftScheduleEntry";
        command.CommandType = CommandType.StoredProcedure;
        
        // Convert to DateTime2 compatible (Date + Time)
        var startDt = date.Add(start);
        var endDt = date.Add(end);
        if (end < start) endDt = endDt.AddDays(1); // Handle overnight

        command.Parameters.Add(new SqlParameter("@name", name));
        command.Parameters.Add(new SqlParameter("@type", type));
        command.Parameters.Add(new SqlParameter("@startTime", startDt));
        command.Parameters.Add(new SqlParameter("@endTime", endDt));
        command.Parameters.Add(new SqlParameter("@breakDuration", new TimeSpan(0, 0, 0))); // Default 0
        command.Parameters.Add(new SqlParameter("@shiftDate", date));
        command.Parameters.Add(new SqlParameter("@status", "Scheduled"));

        var shiftIdParam = new SqlParameter("@shiftID", SqlDbType.Int) { Direction = ParameterDirection.Output };
        command.Parameters.Add(shiftIdParam);

        if (connection.State != ConnectionState.Open) await connection.OpenAsync();
        await command.ExecuteNonQueryAsync();

        return (int)shiftIdParam.Value;
    }

    public async Task<List<string>> GetScheduledShiftNamesAsync()
    {
        return await context.Set<ShiftSchedule>()
            .Select(s => s.Name)
            .Distinct()
            .ToListAsync();
    }

    public async Task AssignShiftsToEmployeeAsync(int employeeID, DateTime startDate, DateTime endDate, string shiftType)
    {
        // Convert input DateTime to DateOnly for comparison if DB uses DateOnly
        var start = DateOnly.FromDateTime(startDate);
        var end = DateOnly.FromDateTime(endDate);

        var shifts = await context.Set<ShiftSchedule>()
            .Where(s => s.ShiftDate >= start && s.ShiftDate <= end && s.Name == shiftType) // Exact match now
            .Select(s => new { s.ShiftId, s.ShiftDate })
            .ToListAsync();

        foreach (var shift in shifts)
        {
            await AssignToEmployeeAsync(employeeID, shift.ShiftId, shift.ShiftDate.ToDateTime(TimeOnly.MinValue));
        }
    }

    public async Task AssignShiftsToDepartmentAsync(int departmentId, DateTime startDate, DateTime endDate, string shiftType)
    {
        var employeeIds = await context.Employees
            .Where(e => e.DepartmentId == departmentId)
            .Select(e => e.EmployeeId)
            .ToListAsync();

        if (!employeeIds.Any()) return;

        var start = DateOnly.FromDateTime(startDate);
        var end = DateOnly.FromDateTime(endDate);

        var shifts = await context.Set<ShiftSchedule>()
            .Where(s => s.ShiftDate >= start && s.ShiftDate <= end && s.Name == shiftType) // Exact match now
            .Select(s => new { s.ShiftId, s.ShiftDate })
            .ToListAsync();

        foreach (var empId in employeeIds)
        {
            foreach (var shift in shifts)
            {
                await AssignToEmployeeAsync(empId, shift.ShiftId, shift.ShiftDate.ToDateTime(TimeOnly.MinValue));
            }
        }
    }

    public async Task<List<object>> GetEmployeeScheduleAsync(int employeeId, DateTime start, DateTime end)
    {
        var sDate = DateOnly.FromDateTime(start);
        var eDate = DateOnly.FromDateTime(end);

        // We need to join works_shift (Assigned) with ShiftSchedule (Details)
        // works_shift is likely not mapped as a DbSet directly if it's M:N, checking context...
        // Context has WorksShifts DbSet.

        var assignments = await context.WorksShifts
            .Include(ws => ws.Shift)
            .Where(ws => ws.EmployeeId == employeeId && ws.StartDate <= eDate && ws.EndDate >= sDate)
            .Select(ws => new 
            {
                ws.Shift.Name,
                ws.Shift.Type,
                ws.Shift.StartTime,
                ws.Shift.EndTime,
                ws.AssignmentId, // Needed for deletion/update
                ws.StartDate,
                ws.EndDate
            })
            .ToListAsync();

        // Must convert to list of objects or DTOs
        return assignments.Cast<object>().ToList();
    }

    public async Task AssignCustomShiftAsync(int employeeId, DateTime date, TimeSpan start, TimeSpan end, string name)
    {
        // 1. Create the Custom Shift Entry (using existing private helper if possible, or new logic)
        // We reuse CreateShiftEntryAsync but need to make sure it handles single date creation
        // The helper takes 'date', 'start', 'end'. perfect.
        
        int shiftId = await CreateShiftEntryAsync(name, "Custom", date, start, end);

        // 2. Assign it
        await AssignToEmployeeAsync(employeeId, shiftId, date);
    }

    public async Task DeleteAssignmentAsync(int assignmentId)
    {
        var assignment = await context.WorksShifts.FindAsync(assignmentId);
        if (assignment != null)
        {
            context.WorksShifts.Remove(assignment);
            await context.SaveChangesAsync();
        }
    }

    private async Task AssignToEmployeeAsync(int employeeID, int shiftID, DateTime date)
    {
        var connection = context.Database.GetDbConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "AssignShiftToEmployee";
        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.Add(new SqlParameter("@employeeID", employeeID));
        command.Parameters.Add(new SqlParameter("@shiftID", shiftID));
        command.Parameters.Add(new SqlParameter("@startDate", date));
        command.Parameters.Add(new SqlParameter("@endDate", date));
        command.Parameters.Add(new SqlParameter("@status", "Assigned"));

        var successParam = new SqlParameter("@success", SqlDbType.Int) { Direction = ParameterDirection.Output };
        command.Parameters.Add(successParam);

        if (connection.State != ConnectionState.Open) await connection.OpenAsync();
        await command.ExecuteNonQueryAsync();
    }


}
