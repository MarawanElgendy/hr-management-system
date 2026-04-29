namespace Services.Leave;

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

using HRMS.Models;
using DTOs;
using HRMS.Exceptions;

public class LeaveRequestService : ILeaveRequestService
{
    private readonly HrmsContext context;

    public LeaveRequestService(HrmsContext context)
    {
        this.context = context;
    }

    public async Task<List<RequestStatusDTO>> ViewRequestStatusAsync(int employeeId)
    {
        var employeeParam = new SqlParameter("@EmployeeID", employeeId);

        return await context.Set<RequestStatusDTO>()
            .FromSqlRaw("EXEC ViewRequestStatus @EmployeeID", employeeParam)
            .ToListAsync();
    }

    public async Task ApproveRejectLeaveRequest(int managerId, int leaveRequestId, string newStatus)
    {
        // validating input
        if (managerId <= 0)
            throw new ManagerNotFoundException();

        if (leaveRequestId <= 0)
            throw new LeaveRequestNotFoundException();

        if (string.IsNullOrWhiteSpace(newStatus))
            throw new LeaveRequestInvalidStateException();

        using var connection = context.Database.GetDbConnection();

        // input parameters
        var pManager = new SqlParameter("@ManagerID", managerId);
        var pRequest = new SqlParameter("@LeaveRequestID", leaveRequestId);
        var pStatus = new SqlParameter("@NewStatus", newStatus);

        // output parameter
        var pSuccess = new SqlParameter("@success", System.Data.SqlDbType.Int)
        {
            Direction = System.Data.ParameterDirection.Output
        };

        // open connection
        if (connection.State != System.Data.ConnectionState.Open)
            await connection.OpenAsync();

        // call procedure
        using var command = connection.CreateCommand();
        command.CommandText = "ApproveRejectLeaveRequest";
        command.CommandType = System.Data.CommandType.StoredProcedure;

        command.Parameters.AddRange(new[] {
            pManager, pRequest, pStatus, pSuccess
        });

        await command.ExecuteNonQueryAsync();

        // handle result
        int success = (int)pSuccess.Value;

        switch (success)
        {
            case 1: return;
            case -1: throw new ManagerNotFoundException();
            case -2: throw new LeaveRequestNotFoundException();
            case -3: throw new LeaveRequestInvalidStateException("Leave request does not belong to this manager");
            case -4: throw new InvalidLeaveStatusException();
            default: throw new UnexpectedErrorException();
        }
    }

    public async Task<List<PendingLeaveRequestDTO>> GetPendingLeaveRequestsAsync(int managerId)
    {
        if (managerId <= 0)
            throw new ManagerNotFoundException();

        using var connection = context.Database.GetDbConnection();

        var pManager = new SqlParameter("@ManagerID", managerId);

        var pSuccess = new SqlParameter("@success", SqlDbType.Int)
        {
            Direction = ParameterDirection.Output
        };

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = "GetPendingLeaveRequests";
        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.Add(pManager);
        command.Parameters.Add(pSuccess);

        var result = new List<PendingLeaveRequestDTO>();

        using (var reader = await command.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                result.Add(new PendingLeaveRequestDTO
                {
                    LeaveRequestID = reader.GetInt32(reader.GetOrdinal("leaveRequestID")),
                    EmployeeID = reader.GetInt32(reader.GetOrdinal("employeeID")),
                    FullName = reader.GetString(reader.GetOrdinal("fullName")),
                    LeaveTypeID = reader.GetInt32(reader.GetOrdinal("leaveTypeID")),
                    Duration = reader.GetInt32(reader.GetOrdinal("duration")),
                    Status = reader.GetString(reader.GetOrdinal("status"))
                });
            }
        }

        int success = (int)pSuccess.Value;

        switch (success)
        {
            case 1: return result;
            case 0: throw new ManagerNotFoundException();
            default: throw new UnexpectedErrorException();
        }
    }

    public async Task OverrideLeaveDecision(int hrAdminId, int leaveRequestId, string newStatus)
    {
        // input validation
        if (hrAdminId <= 0)
            throw new AdminNotFoundException();

        if (leaveRequestId <= 0)
            throw new LeaveRequestNotFoundException();

        if (string.IsNullOrWhiteSpace(newStatus))
            throw new InvalidLeaveStatusException();

        using var connection = context.Database.GetDbConnection();

        var pHr = new SqlParameter("@HRAdminID", hrAdminId);
        var pRequest = new SqlParameter("@LeaveRequestID", leaveRequestId);
        var pStatus = new SqlParameter("@NewStatus", newStatus);

        var pSuccess = new SqlParameter("@success", SqlDbType.Int)
        {
            Direction = ParameterDirection.Output
        };

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = "OverrideLeaveDecision";
        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.AddRange(new[]
        {
            pHr, pRequest, pStatus, pSuccess
        });

        await command.ExecuteNonQueryAsync();

        int success = (int) pSuccess.Value;

        switch (success)
        {
            case 1: return;
            case -1: throw new AdminNotFoundException();
            case -2: throw new LeaveRequestNotFoundException();
            case -3: throw new InvalidLeaveStatusException();
            default: throw new UnexpectedErrorException();
        }
    }

    public async Task SubmitLeaveAfterAbsenceAsync(int employeeID, string justification)
    {
        // validating the input
        if (employeeID <= 0)
            throw new EmployeeNotFoundException();

        // TODO CORRECT EXCEPTION TYPE
        if (string.IsNullOrWhiteSpace(justification))
            throw new ArgumentException("Justification is required.");

        // setting up the connection
        using var connection = context.Database.GetDbConnection();

        // input parameters
        var employeeParam = new SqlParameter("@EmployeeID", employeeID);
        var justificationParam = new SqlParameter("@Justification", justification);

        // output parameter
        var successParam = new SqlParameter("@success", SqlDbType.Int)
        {
            Direction = ParameterDirection.Output
        };

        // opening the connection
        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync();

        // calling the procedure
        using var command = connection.CreateCommand();
        command.CommandText = "SubmitLeaveAfterAbsence";
        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.Add(employeeParam);
        command.Parameters.Add(justificationParam);
        command.Parameters.Add(successParam);

        await command.ExecuteNonQueryAsync();

        // assessing success code and throwing exceptions
        int success = (int)successParam.Value;

        switch (success)
        {
            case 1:
                return;

            case -1:
                throw new EmployeeNotFoundException();

            default:
                throw new UnexpectedErrorException();
        }
    }

    public async Task SubmitLeaveRequestAsync(int employeeId, int leaveTypeId, DateTime startDate, DateTime endDate, string reason)
    {
        // input validation
        if (employeeId <= 0)
            throw new EmployeeNotFoundException();

        if (leaveTypeId <= 0)
            throw new LeaveTypeNotFoundException();

        if (startDate > endDate)
            throw new InvalidDateRangeException();

        using var connection = context.Database.GetDbConnection();

        var pEmployee = new SqlParameter("@EmployeeID", employeeId);
        var pLeaveType = new SqlParameter("@LeaveTypeID", leaveTypeId);
        var pStart = new SqlParameter("@StartDate", startDate);
        var pEnd = new SqlParameter("@EndDate", endDate);
        var pReason = new SqlParameter("@Reason", reason);

        var pSuccess = new SqlParameter("@success", System.Data.SqlDbType.Int)
        {
            Direction = System.Data.ParameterDirection.Output
        };

        if (connection.State != System.Data.ConnectionState.Open)
            await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = "SubmitLeaveRequest";
        command.CommandType = System.Data.CommandType.StoredProcedure;

        command.Parameters.AddRange(new[] {
            pEmployee, pLeaveType, pStart, pEnd, pReason, pSuccess
        });

        await command.ExecuteNonQueryAsync();

        int success = (int)pSuccess.Value;

        switch (success)
        {
            case 1: return;
            case -1: throw new EmployeeNotFoundException();
            case -2: throw new LeaveTypeNotFoundException();
            case -3: throw new InvalidDateRangeException();
            default: throw new UnexpectedErrorException();
        }
    }
}
