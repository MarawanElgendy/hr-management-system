namespace Services.Leave;

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

using HRMS.Models;
using DTOs;
using HRMS.Exceptions;

public class LeavePolicyService : ILeavePolicyService
{
    private readonly HrmsContext context;

    public LeavePolicyService(HrmsContext context)
    {
        this.context = context;
    }

    public async Task<List<LeaveHistoryDTO>> ViewLeaveHistoryAsync(int employeeId)
    {
        var employeeParam = new SqlParameter("@EmployeeID", employeeId);

        return await context.Set<LeaveHistoryDTO>()
            .FromSqlRaw("EXEC ViewLeaveHistory @EmployeeID", employeeParam)
            .ToListAsync();
    }

    public async Task<List<LeaveBalanceDTO>> GetLeaveBalanceAsync(int employeeId)
    {
        // input validation
        if (employeeId <= 0)
            throw new EmployeeNotFoundException();

        using var connection = context.Database.GetDbConnection();

        // input parameter
        var pEmployee = new SqlParameter("@EmployeeID", employeeId);

        // output parameter
        var pSuccess = new SqlParameter("@success", SqlDbType.Int)
        {
            Direction = ParameterDirection.Output
        };

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = "GetLeaveBalance";
        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.Add(pEmployee);
        command.Parameters.Add(pSuccess);

        var result = new List<LeaveBalanceDTO>();

        using (var reader = await command.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                result.Add(new LeaveBalanceDTO
                {
                    LeaveType = reader.GetString(0),
                    TotalEntitlement = reader.GetInt32(1),
                    UsedDays = reader.GetInt32(2),
                    RemainingDays = reader.GetInt32(3)
                });
            }
        }

        int success = (int)pSuccess.Value;

        switch (success)
        {
            case 1: return result;
            case -1: throw new EmployeeNotFoundException();
            default: throw new UnexpectedErrorException();
        }
    }
}
