namespace Services.Attendance;

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

using HRMS.Models;
using HRMS.Exceptions;

public class ShiftService : IShiftService
{
    private readonly HrmsContext context;
    private readonly Services.Notification.INotificationService _notificationService;

    public ShiftService(HrmsContext context, Services.Notification.INotificationService notificationService)
    {
        this.context = context;
        _notificationService = notificationService;
    }

    //SHIFT MANAGEMENT
    //a)
    public async Task CreateShiftTypeAsync(int shiftID, string name, string type, TimeSpan startTime, TimeSpan endTime, int breakDuration, DateTime shiftDate, string status)
    {
        // validating the input (optional)

        // setting up the connection
        var connection = context.Database.GetDbConnection();

        using var command = connection.CreateCommand();
        command.CommandText = "CreateShiftType";
        command.CommandType = CommandType.StoredProcedure;

        // input parameters
        command.Parameters.Add(new SqlParameter("@ShiftID", shiftID));
        command.Parameters.Add(new SqlParameter("@Name", name));
        command.Parameters.Add(new SqlParameter("@Type", type));
        command.Parameters.Add(new SqlParameter("@Start_Time", startTime));
        command.Parameters.Add(new SqlParameter("@End_Time", endTime));
        command.Parameters.Add(new SqlParameter("@Break_Duration", breakDuration));
        command.Parameters.Add(new SqlParameter("@Shift_Date", shiftDate));
        command.Parameters.Add(new SqlParameter("@Status", status));

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
            throw new DuplicateAssignmentException();
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
    public async Task AssignCustomShiftAsync(int employeeID, string shiftName, string shiftType, TimeSpan startTime, TimeSpan endTime, DateTime startDate, DateTime endDate)
    {
        // validating the input (optional)

        // setting up the connection
        var connection = context.Database.GetDbConnection();

        using var command = connection.CreateCommand();
        command.CommandText = "AssignCustomShift";
        command.CommandType = CommandType.StoredProcedure;

        // input parameters
        command.Parameters.Add(new SqlParameter("@EmployeeID", employeeID));
        command.Parameters.Add(new SqlParameter("@ShiftName", shiftName));
        command.Parameters.Add(new SqlParameter("@ShiftType", shiftType));
        command.Parameters.Add(new SqlParameter("@StartTime", startTime));
        command.Parameters.Add(new SqlParameter("@EndTime", endTime));
        command.Parameters.Add(new SqlParameter("@StartDate", startDate));
        command.Parameters.Add(new SqlParameter("@EndDate", endDate));

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

        if (success == -2)
        {
            throw new InvalidFieldException();
        }
        else if (success == -1)
        {
            throw new EmployeeNotFoundException();
        }
        else if (success == -3)
        {
            throw new DuplicateAssignmentException();
        }
        else if (success == 1)
        {
            // Notify
            try 
            {
                await _notificationService.SendNotificationAsync(employeeID, $"You have been assigned a Custom Shift: {shiftName} ({startDate.ToShortDateString()} - {endDate.ToShortDateString()})", "Info", "Normal");
            } catch {}
        }
        else
        {
            throw new UnexpectedErrorException();
        }
    }

    public async Task AssignRotationalShiftAsync(int employeeID, int shiftCycle, DateTime startDate, DateTime endDate, string status)
    {
        // validating the input (optional)

        // setting up the connection
        var connection = context.Database.GetDbConnection();

        using var command = connection.CreateCommand();
        command.CommandText = "AssignRotationalShift";
        command.CommandType = CommandType.StoredProcedure;

        // input parameters
        command.Parameters.Add(new SqlParameter("@EmployeeID", employeeID));
        command.Parameters.Add(new SqlParameter("@ShiftCycle", shiftCycle));
        command.Parameters.Add(new SqlParameter("@StartDate", startDate));
        command.Parameters.Add(new SqlParameter("@EndDate", endDate));
        command.Parameters.Add(new SqlParameter("@status", status));

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
            // Notify
            try 
            {
                await _notificationService.SendNotificationAsync(employeeID, $"You have been assigned a Rotational Shift (Cycle {shiftCycle}) starting {startDate.ToShortDateString()}", "Info", "Normal");
            } catch {}
        }
        else
        {
            throw new UnexpectedErrorException();
        }
    }

    //c)
    public async Task AssignShiftAsync(int employeeID, int shiftID)
    {
        // validating the input (optional)

        // setting up the connection
        var connection = context.Database.GetDbConnection();

        using var command = connection.CreateCommand();
        command.CommandText = "AssignShift";
        command.CommandType = CommandType.StoredProcedure;

        // input parameters
        command.Parameters.Add(new SqlParameter("@employeeID", employeeID));
        command.Parameters.Add(new SqlParameter("@shiftID", shiftID));

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
            // Notify
            try 
            {
                await _notificationService.SendNotificationAsync(employeeID, $"You have been assigned a new Shift (ID: {shiftID})", "Info", "Normal");
            } catch {}
        }
        else
        {
            throw new UnexpectedErrorException();
        }
    }

    public async Task AssignShiftToDepartmentAsync(int departmentID, int shiftID, DateTime startDate, DateTime endDate)
    {
        // validating the input (optional)

        // setting up the connection
        var connection = context.Database.GetDbConnection();

        using var command = connection.CreateCommand();
        command.CommandText = "AssignShiftToDepartment";
        command.CommandType = CommandType.StoredProcedure;

        // input parameters
        command.Parameters.Add(new SqlParameter("@DepartmentID", departmentID));
        command.Parameters.Add(new SqlParameter("@ShiftID", shiftID));
        command.Parameters.Add(new SqlParameter("@StartDate", startDate));
        command.Parameters.Add(new SqlParameter("@EndDate", endDate));

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
            throw new DepartmentNotFoundException();
        }
        else if (success == -2)
        {
            throw new ShiftNotFoundException();
        }
        else if (success == -3)
        {
            throw new InvalidFieldException();
        }
        else if (success == -4)
        {
            throw new NoEligibleEmployeesException();
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
    public async Task UpdateShiftStatusAsync(int shiftAssignmentID, string status)
    {
        // validating the input (optional)

        // setting up the connection
        var connection = context.Database.GetDbConnection();

        using var command = connection.CreateCommand();
        command.CommandText = "UpdateShiftStatus";
        command.CommandType = CommandType.StoredProcedure;

        // input parameters
        command.Parameters.Add(new SqlParameter("@ShiftAssignmentID", shiftAssignmentID));
        command.Parameters.Add(new SqlParameter("@Status", status));

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
            throw new ShiftNotAssignedException();
        }
        else if (success == -2)
        {
            throw new InvalidFieldException();
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
}
