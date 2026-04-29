namespace Services.General;

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

using DTOs;
using HRMS.Models;
using HRMS.Exceptions;

public class HierarchyService : IHierarchyService
{
    private readonly HrmsContext context;

    public HierarchyService(HrmsContext context)
    {
        this.context = context;
    }

    public async Task<TeamProfilesDTO> ViewTeamDetailsAsync(int managerID)
    {
        // setting up the connection
        var connection = context.Database.GetDbConnection();

        using var command = connection.CreateCommand();
        command.CommandText = "ViewTeamProfiles";
        command.CommandType = CommandType.StoredProcedure;

        // input parameter
        command.Parameters.Add(new SqlParameter("@ManagerID", managerID));

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

        using var reader = await command.ExecuteReaderAsync();

        // getting the results set
        var result = new TeamProfilesDTO();

        while (await reader.ReadAsync())
        {
            var member = new TeamMemberDTO
            {
                EmployeeID = reader.GetInt32(reader.GetOrdinal("employeeID")),
                FullName = reader.GetString(reader.GetOrdinal("fullName")),
                EmailAddress = reader.GetString(reader.GetOrdinal("emailAddress")),
                PhoneNumber = reader.GetString(reader.GetOrdinal("phoneNumber")),
                PositionTitle = reader.GetString(reader.GetOrdinal("title")),
                DepartmentName = reader.GetString(reader.GetOrdinal("name")),
                EmploymentStatus = reader.GetString(reader.GetOrdinal("employmentStatus"))
            };

            result.TeamMembers.Add(member);
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
}
