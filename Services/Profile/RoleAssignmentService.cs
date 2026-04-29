namespace Services.Profile;

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

using HRMS.Exceptions;
using HRMS.Models;

public class RoleAssignmentService : IRoleAssignmentService
{
    private readonly HrmsContext context;

    public RoleAssignmentService(HrmsContext context)
    {
        this.context = context;
    }

    public async Task AssignRoleAsync(int employeeID, int roleID)
    {   
        // setting up the connection
        var connection = context.Database.GetDbConnection();

        using var command = connection.CreateCommand();
        command.CommandText = "AssignRole";
        command.CommandType = CommandType.StoredProcedure;

        // input parameters
        command.Parameters.Add(new SqlParameter("@EmployeeID", employeeID));
        command.Parameters.Add(new SqlParameter("@RoleID", roleID));

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
            throw new RoleAlreadyAssignedException();
        }
        else if (success == -1)
        {
            throw new EmployeeNotFoundException();
        }
        else if (success == -5)
        {
            throw new RoleNotFoundException();
        }
    }
}
