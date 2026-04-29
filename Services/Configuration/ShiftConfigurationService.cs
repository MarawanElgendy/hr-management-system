namespace Services.Configuration;

using DTOs;
using HRMS.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System.Data;

public class ShiftConfigurationService : IShiftConfigurationService
{
    private readonly HrmsContext context;

    public ShiftConfigurationService(HrmsContext context)
    {
        this.context = context;
    }

    public async Task ConfigureShiftAllowanceAsync(string shiftType, decimal allowanceAmount, int createdBy)
    {
        var connection = context.Database.GetDbConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "ConfigureShiftAllowance";
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.Add(new SqlParameter("@shiftType", shiftType));
        command.Parameters.Add(new SqlParameter("@allowanceAmount", allowanceAmount));
        command.Parameters.Add(new SqlParameter("@createdBy", createdBy));
        
        var successParam = new SqlParameter("@success", SqlDbType.Int) { Direction = ParameterDirection.Output };
        command.Parameters.Add(successParam);

        if (connection.State != ConnectionState.Open) await connection.OpenAsync();
        await command.ExecuteNonQueryAsync();

        int success = (int)(successParam.Value ?? 0);
        if (success == 0) throw new System.Exception("Manager/Creator not found.");
        // success == -1 check was removed in DB, so no need to check here unless we want to keep it safe.
    }

    public async Task<List<ShiftConfigurationDTO>> GetAllShiftTypesAsync()
    {
        var result = new List<ShiftConfigurationDTO>();
        var connection = context.Database.GetDbConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "GetAllShiftTypes";
        command.CommandType = CommandType.StoredProcedure;

        if (connection.State != ConnectionState.Open) await connection.OpenAsync();
        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            result.Add(new ShiftConfigurationDTO
            {
                ShiftConfigurationID = reader.GetInt32(reader.GetOrdinal("shiftConfigurationID")),
                ShiftType = reader.GetString(reader.GetOrdinal("shiftType")),
                AllowanceAmount = reader.GetDecimal(reader.GetOrdinal("allowanceAmount"))
            });
        }
        return result;
    }
}
