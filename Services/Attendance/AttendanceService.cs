namespace Services.Attendance;

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

using HRMS.Models;
using HRMS.Exceptions;
using DTOs;

public class AttendanceService : IAttendanceService
{
    private readonly HrmsContext context;

    public AttendanceService(HrmsContext context)
    {
        this.context = context;
    }

    //ATTENDANCE TRACKING
    //a)
    public async Task RecordAttendanceAsync(int employeeID, int shiftID, TimeSpan entryTime, TimeSpan exitTime)
    {
        // validating the input (optional)

        // setting up the connection
        var connection = context.Database.GetDbConnection();

        using var command = connection.CreateCommand();
        command.CommandText = "RecordAttendance";
        command.CommandType = CommandType.StoredProcedure;

        // input parameters
        command.Parameters.Add(new SqlParameter("@employeeID", employeeID));
        command.Parameters.Add(new SqlParameter("@shiftID", shiftID));
        command.Parameters.Add(new SqlParameter("@entryTime", entryTime));
        command.Parameters.Add(new SqlParameter("@exitTime", exitTime));

        // output parameter
        var successParam = new SqlParameter("@success", SqlDbType.Int)
        {
            Direction = ParameterDirection.Output
        };

        command.Parameters.Add(successParam);

        // opening the connection (if not already done)
        if (connection.State != ConnectionState.Open)
        {
            await connection.OpenAsync();
        }

        // calling the procedure
        await command.ExecuteNonQueryAsync();

        // assessing the final state of success
        int success = (int)(successParam.Value ?? -1);

        if (success == 0)
        {
            throw new EmployeeNotFoundException();
        }
        else if (success == -1)
        {
            throw new ShiftNotFoundException();
        }
        else if (success == 1)
        {
            // success
        }
        else
        {
            throw new UnexpectedErrorException();
        }
    }

    //b)
    public async Task SubmitCorrectionRequestAsync(int employeeID, DateTime date, string correctionType, string reason)
    {
        // validating the input (optional)

        // setting up the connection
        var connection = context.Database.GetDbConnection();

        using var command = connection.CreateCommand();
        command.CommandText = "SubmitCorrectionRequest";
        command.CommandType = CommandType.StoredProcedure;

        // input parameters
        command.Parameters.Add(new SqlParameter("@EmployeeID", employeeID));
        command.Parameters.Add(new SqlParameter("@Date", date));
        command.Parameters.Add(new SqlParameter("@CorrectionType", correctionType));
        command.Parameters.Add(new SqlParameter("@Reason", reason));

        // output parameter
        var successParam = new SqlParameter("@success", SqlDbType.Int)
        {
            Direction = ParameterDirection.Output
        };

        command.Parameters.Add(successParam);

        // opening the connection (if not already done)
        if (connection.State != ConnectionState.Open)
        {
            await connection.OpenAsync();
        }

        // calling the procedure
        await command.ExecuteNonQueryAsync();

        // assessing the final state of success
        int success = (int)(successParam.Value ?? -1);

        if (success == -1)
        {
            throw new InvalidFieldException();
        }
        else if (success == -2)
        {
            throw new EmployeeNotFoundException();
        }
        else if (success == 1)
        {
            // success
        }
        else
        {
            throw new UnexpectedErrorException();
        }
    }

    //c)
    public async Task SyncLeaveToAttendanceAsync(int leaveRequestID)
    {
        // validating the input (optional)

        // setting up the connection
        var connection = context.Database.GetDbConnection();

        using var command = connection.CreateCommand();
        command.CommandText = "SyncLeaveToAttendance";
        command.CommandType = CommandType.StoredProcedure;

        // input parameters
        command.Parameters.Add(new SqlParameter("@LeaveRequestID", leaveRequestID));

        // output parameter
        var successParam = new SqlParameter("@success", SqlDbType.Int)
        {
            Direction = ParameterDirection.Output
        };

        command.Parameters.Add(successParam);

        // opening the connection (if not already done)
        if (connection.State != ConnectionState.Open)
        {
            await connection.OpenAsync();
        }

        // calling the procedure
        await command.ExecuteNonQueryAsync();

        // assessing the final state of success
        int success = (int)(successParam.Value ?? -1);

        if (success == -1)
        {
            throw new InvalidFieldException();
        }
        else if (success == -2)
        {
            throw new LeaveRequestNotFoundException();
        }
        else if (success == 1)
        {
            // success
        }
        else
        {
            throw new UnexpectedErrorException();
        }
    }

    //d)
    public async Task SyncOfflineAttendanceAsync(int deviceID, int employeeID, DateTime clockTime, string type)
    {
        // validating the input (optional)

        // setting up the connection
        var connection = context.Database.GetDbConnection();

        using var command = connection.CreateCommand();
        command.CommandText = "SyncOfflineAttendance";
        command.CommandType = CommandType.StoredProcedure;

        // input parameters
        command.Parameters.Add(new SqlParameter("@DeviceID", deviceID));
        command.Parameters.Add(new SqlParameter("@EmployeeID", employeeID));
        command.Parameters.Add(new SqlParameter("@ClockTime", clockTime));
        command.Parameters.Add(new SqlParameter("@Type", type));

        // output parameter
        var successParam = new SqlParameter("@success", SqlDbType.Int)
        {
            Direction = ParameterDirection.Output
        };

        command.Parameters.Add(successParam);

        // opening the connection (if not already done)
        if (connection.State != ConnectionState.Open)
        {
            await connection.OpenAsync();
        }

        // calling the procedure
        await command.ExecuteNonQueryAsync();

        // assessing the final state of success
        int success = (int)(successParam.Value ?? -1);

        if (success == -1)
        {
            throw new InvalidFieldException();
        }
        else if (success == -2)
        {
            throw new EmployeeNotFoundException();
        }
        else if (success == -3)
        {
            throw new InvalidOperationStateException();
        }
        else if (success == 1)
        {
            // success
        }
        else
        {
            throw new UnexpectedErrorException();
        }
    }

    //e)
    public async Task<List<HRMS.Models.AttendanceRule>> GetAttendanceRulesAsync()
    {
        return await context.AttendanceRules.ToListAsync();
    }

    public async Task SetGracePeriodAsync(int minutes)
    {
        // 1. Call Legacy SP (Optional, keeping for backward compatibility if needed)
        // await CallStoredProcedure("SetGracePeriod", ...); 
        // Note: Disabling SP call if we are fully moving to EF, but user might expect legacy behavior.
        // For now, I'll ONLY update the new table to ensure the View works. 
        // If the SP does "magic", we might need to restore it. 
        // Assuming the new Table is the Source of Truth for the UI.

        var rule = await context.AttendanceRules.FindAsync("GracePeriod");
        if (rule == null)
        {
            rule = new HRMS.Models.AttendanceRule { RuleName = "GracePeriod" };
            context.AttendanceRules.Add(rule);
        }
        rule.NumericValue = minutes;
        rule.LastUpdatedAt = DateTime.Now;
        await context.SaveChangesAsync();
    }

    public async Task DefineShortTimeRulesAsync(string ruleName, int lateMinutes, int earlyLeaveMinutes, string penaltyType)
    {
        // Update/Create Rule for Late Threshold
        var lateRule = await context.AttendanceRules.FindAsync("ShortTime_LateMinutes_" + ruleName);
        if (lateRule == null)
        {
            lateRule = new HRMS.Models.AttendanceRule { RuleName = "ShortTime_LateMinutes_" + ruleName };
            context.AttendanceRules.Add(lateRule);
        }
        lateRule.NumericValue = lateMinutes;
        lateRule.Value = penaltyType; // Storing penalty type in Value? Or just using this for config keys?
        lateRule.LastUpdatedAt = DateTime.Now;

        // Update/Create Rule for Early Leave Threshold
        var earlyRule = await context.AttendanceRules.FindAsync("ShortTime_EarlyLeaveMinutes_" + ruleName);
        if (earlyRule == null)
        {
            earlyRule = new HRMS.Models.AttendanceRule { RuleName = "ShortTime_EarlyLeaveMinutes_" + ruleName };
            context.AttendanceRules.Add(earlyRule);
        }
        earlyRule.NumericValue = earlyLeaveMinutes;
        earlyRule.LastUpdatedAt = DateTime.Now;

        await context.SaveChangesAsync();
    }

    public async Task DefinePenaltyThresholdAsync(int lateMinutes, string deductionType)
    {
        var rule = await context.AttendanceRules.FindAsync("PenaltyThreshold");
        if (rule == null)
        {
            rule = new HRMS.Models.AttendanceRule { RuleName = "PenaltyThreshold" };
            context.AttendanceRules.Add(rule);
        }
        rule.NumericValue = lateMinutes;
        rule.Value = deductionType;
        rule.LastUpdatedAt = DateTime.Now;
        await context.SaveChangesAsync();
    }

    //f)
    public async Task<List<TeamAttendanceDTO>> ViewTeamAttendanceAsync(int managerID, DateTime dateRangeStart, DateTime dateRangeEnd)
    {
        // validating the input (optional)

        // setting up the connection
        var connection = context.Database.GetDbConnection();

        using var command = connection.CreateCommand();
        command.CommandText = "ViewTeamAttendance";
        command.CommandType = CommandType.StoredProcedure;

        // input parameters
        command.Parameters.Add(new SqlParameter("@ManagerID", managerID));
        command.Parameters.Add(new SqlParameter("@DateRangeStart", dateRangeStart));
        command.Parameters.Add(new SqlParameter("@DateRangeEnd", dateRangeEnd));

        // output parameter
        var successParam = new SqlParameter("@success", SqlDbType.Int)
        {
            Direction = ParameterDirection.Output
        };

        command.Parameters.Add(successParam);

        // opening the connection (if not already done)
        if (connection.State != ConnectionState.Open)
        {
            await connection.OpenAsync();
        }

        var result = new List<TeamAttendanceDTO>();

        using (var reader = await command.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                var attendance = new TeamAttendanceDTO
                {
                    AttendanceID = reader.GetInt32(reader.GetOrdinal("attendanceID")),
                    EmployeeName = reader.GetString(reader.GetOrdinal("employeeName")),
                    ShiftDate = reader.GetDateTime(reader.GetOrdinal("shiftDate")),
                    StartTime = reader.GetDateTime(reader.GetOrdinal("startTime")).TimeOfDay,
                    EndTime = reader.GetDateTime(reader.GetOrdinal("endTime")).TimeOfDay,
                    EntryTime = reader.IsDBNull(reader.GetOrdinal("entryTime")) ? null : reader.GetDateTime(reader.GetOrdinal("entryTime")),
                    ExitTime = reader.IsDBNull(reader.GetOrdinal("exitTime")) ? null : reader.GetDateTime(reader.GetOrdinal("exitTime")),
                    DurationMinutes = reader.IsDBNull(reader.GetOrdinal("durationMinutes")) ? null : reader.GetInt32(reader.GetOrdinal("durationMinutes")),
                    ShiftName = reader.GetString(reader.GetOrdinal("shiftName"))
                };
                result.Add(attendance);
            }
        }

        // assessing the final state of success
        int success = (int)(successParam.Value ?? -1);

        if (success == -3)
        {
            throw new ManagerNotFoundException();
        }
        else if (success == 1)
        {
            return result;
        }
        else
        {
            throw new UnexpectedErrorException();
        }
    }


    public async Task CheckInAsync(int employeeId)
    {
        // 1. Check if already checked in today (EntryTime today, ExitTime null)
        var today = DateTime.Today;
        var existing = await context.Attendances
            .Where(a => a.EmployeeId == employeeId && a.ExitTime == null)
            .FirstOrDefaultAsync();

        if (existing != null)
        {
            // Already checked in. Do nothing or throw? 
            // Ideally UI prevents this, but let's be safe.
            return; 
        }

        // 2. Create new record
        var attendance = new HRMS.Models.Attendance
        {
            EmployeeId = employeeId,
            EntryTime = DateTime.Now,
            // ShiftId left null or mapped if we have shift logic
        };

        context.Attendances.Add(attendance);
        await context.SaveChangesAsync();
    }

    public async Task CheckOutAsync(int employeeId)
    {
        // 1. Find open record
        var openRecord = await context.Attendances
            .Where(a => a.EmployeeId == employeeId && a.ExitTime == null)
            .OrderByDescending(a => a.EntryTime)
            .FirstOrDefaultAsync();

        if (openRecord == null) return; // Haven't checked in

        // 2. Close it
        openRecord.ExitTime = DateTime.Now;
        await context.SaveChangesAsync();
    }

    public async Task<List<HRMS.Models.Attendance>> GetMyAttendanceAsync(int employeeId)
    {
        return await context.Attendances
            .Where(a => a.EmployeeId == employeeId)
            .OrderByDescending(a => a.EntryTime)
            .Take(30)
            .ToListAsync();
    }
}
