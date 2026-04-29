namespace Services.Profile;

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

using DTOs;
using HRMS.Models;
using HRMS.Exceptions;

public class ContractService : IContractService
{
    private readonly HrmsContext context;
    private readonly Services.Notification.INotificationService _notificationService;

    public ContractService(HrmsContext context, Services.Notification.INotificationService notificationService)
    {
        this.context = context;
        _notificationService = notificationService;
    }

    public async Task<int> CreateEmploymentContractAsync(int employeeID, string type, DateTime startDate, DateTime endDate)
    {
        // setting up the connection
        var connection = context.Database.GetDbConnection();

        using var command = connection.CreateCommand();
        command.CommandText = "CreateContract";
        command.CommandType = CommandType.StoredProcedure;

        // input parameters
        command.Parameters.Add(new SqlParameter("@employeeID", employeeID));
        command.Parameters.Add(new SqlParameter("@type", type));
        command.Parameters.Add(new SqlParameter("@startDate", startDate));
        command.Parameters.Add(new SqlParameter("@endDate", endDate));

        // output parameters
        var successParam = new SqlParameter("@success", SqlDbType.Int)
        {
            Direction = ParameterDirection.Output
        };

        command.Parameters.Add(successParam);

        var contractParam = new SqlParameter("@contractID", SqlDbType.Int)
        {
            Direction = ParameterDirection.Output
        };

        command.Parameters.Add(contractParam);

        // opening the connection (if not already done)
        if (connection.State != ConnectionState.Open)
        {
            await connection.OpenAsync();
        }

        // calling the procedure
        await command.ExecuteNonQueryAsync();

        int success = (int)(successParam.Value ?? -99);
        int contractID = (int)(contractParam.Value ?? -1);

        // assessing the final state of success
        if (success == 0)
        {
            throw new ContractTypeInvalidException();
        }
        else if (success == 1)
        {
            // Notify Employee
            await _notificationService.SendNotificationAsync(employeeID, $"Your new {type} contract has been created. Start Date: {startDate.ToShortDateString()}", "Info", "High");
            return contractID;
        }
        else if (success == -1)
        {
            throw new EmployeeNotFoundException();
        }
        else
        {
            throw new UnexpectedErrorException();
        }
    }

    public async Task<List<ExpiringContractDTO>> GetActiveContractsAsync()
    {
        var result = new List<ExpiringContractDTO>();
        var connection = context.Database.GetDbConnection();
        using var command = connection.CreateCommand();

        command.CommandText = "GetActiveContracts";
        command.CommandType = CommandType.StoredProcedure;

        if (connection.State != ConnectionState.Open) await connection.OpenAsync();

        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            result.Add(new ExpiringContractDTO
            {
                ContractID = reader.GetInt32(reader.GetOrdinal("contractID")),
                EmployeeID = reader.GetInt32(reader.GetOrdinal("employeeID")),
                EmployeeName = reader.GetString(reader.GetOrdinal("EmployeeName")),
                ContractType = reader.GetString(reader.GetOrdinal("type")),
                StartDate = reader.GetDateTime(reader.GetOrdinal("startDate")),
                EndDate = reader.GetDateTime(reader.GetOrdinal("endDate")),
                CurrentState = reader.GetString(reader.GetOrdinal("currentState"))
            });
        }
        return result;
    }

    public async Task<List<ExpiringContractDTO>> GetExpiringContractsAsync(int daysBefore)
    {
        var result = new List<ExpiringContractDTO>();
        var connection = context.Database.GetDbConnection();
        using var command = connection.CreateCommand();

        command.CommandText = "GetExpiringContracts";
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.Add(new SqlParameter("@daysBefore", daysBefore));

        if (connection.State != ConnectionState.Open) await connection.OpenAsync();

        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            result.Add(new ExpiringContractDTO
            {
                ContractID = reader.GetInt32(reader.GetOrdinal("contractID")),
                EmployeeID = reader.GetInt32(reader.GetOrdinal("employeeID")),
                EmployeeName = reader.GetString(reader.GetOrdinal("EmployeeName")),
                ContractType = reader.GetString(reader.GetOrdinal("type")),
                StartDate = reader.GetDateTime(reader.GetOrdinal("startDate")),
                EndDate = reader.GetDateTime(reader.GetOrdinal("endDate")),
                CurrentState = reader.GetString(reader.GetOrdinal("currentState"))
            });
        }
        return result;
    }

    public async Task RenewContractAsync(int contractID, DateTime newEndDate)
    {
        var connection = context.Database.GetDbConnection();
        using var command = connection.CreateCommand();

        command.CommandText = "RenewContract";
        command.CommandType = CommandType.StoredProcedure;
        
        command.Parameters.Add(new SqlParameter("@contractID", contractID));
        command.Parameters.Add(new SqlParameter("@newEndDate", newEndDate));
        
        var successParam = new SqlParameter("@success", SqlDbType.Int) { Direction = ParameterDirection.Output };
        command.Parameters.Add(successParam);

        if (connection.State != ConnectionState.Open) await connection.OpenAsync();
        await command.ExecuteNonQueryAsync();

        int success = (successParam.Value != DBNull.Value) ? (int)successParam.Value : 0;
        
        if (success == 0) throw new ContractNotFoundException();

        // Notify Employee
        try 
        {
            var empId = await context.Contracts
                .Where(c => c.ContractId == contractID)
                .Select(c => c.EmployeeId)
                .FirstOrDefaultAsync();

            if (empId.HasValue)
            {
                await _notificationService.SendNotificationAsync(empId.Value, $"Your contract has been renewed until {newEndDate.ToShortDateString()}.", "Info", "Normal");
            }
        }
        catch { /* Ignore notification failure on renew */ }
    }

    public async Task NotifyExpiringContractsAsync(int daysBefore)
    {
        var expiring = await GetExpiringContractsAsync(daysBefore);
        foreach (var contract in expiring)
        {
            // Avoid spamming? Logic needed. For now, simple notification.
            // In real world, we'd check if we already notified today.
            string msg = $"Your contract is expiring on {contract.EndDate.ToShortDateString()}. Please contact HR.";
            string urgency = (contract.EndDate - DateTime.Now).TotalDays < 7 ? "Critical" : "High";
            await _notificationService.SendNotificationAsync(contract.EmployeeID, msg, "Alert", urgency);
        }
    }
}
