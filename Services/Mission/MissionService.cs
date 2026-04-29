namespace Services.Mission;

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

using HRMS.Models;
using HRMS.Exceptions;
using DTOs;

public class MissionService : IMissionService
{
    private readonly HrmsContext context;
    private readonly Services.Notification.INotificationService _notificationService;

    public MissionService(HrmsContext context, Services.Notification.INotificationService notificationService)
    {
        this.context = context;
        _notificationService = notificationService;
    }

    public async Task<MissionsDTO> ViewAssignedMissionsAsync(int employeeID)
    {
        // setting up the connection
        var connection = context.Database.GetDbConnection();

        using var command = connection.CreateCommand();
        command.CommandText = "ViewMyMissions";
        command.CommandType = CommandType.StoredProcedure;

        // input parameter
        command.Parameters.Add(new SqlParameter("@EmployeeID", employeeID));

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

        // getting the results set
        var result = new MissionsDTO();

        using (var reader = await command.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                var mission = new MissionDTO
                {
                    MissionID = reader.GetInt32(reader.GetOrdinal("missionID")),
                    Destination = reader.GetString(reader.GetOrdinal("destination")),
                    StartDate = reader.GetDateTime(reader.GetOrdinal("startDate")),
                    EndDate = reader.GetDateTime(reader.GetOrdinal("endDate")),
                    Status = reader.GetString(reader.GetOrdinal("status")),
                    ManagerName = reader.GetString(reader.GetOrdinal("managerName"))
                };

                result.Missions.Add(mission);
            }
        }

        // assessing the final state of success
        int success = (int)(successParam.Value ?? -1);

        if (success == -1)
        {
            throw new EmployeeNotFoundException();
        }

        if (success == 1)
        {
            return result;
        } else
        {
            throw new UnexpectedErrorException();
        }
    }

    // TODO: stored procedure is missing (Now implemented)
    public async Task ApproveMissionRequestAsync(int managerID, int missionID)
    {
        var connection = context.Database.GetDbConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "ApproveMissionRequest";
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.Add(new SqlParameter("@missionID", missionID));
        command.Parameters.Add(new SqlParameter("@managerID", managerID));
        
        var successParam = new SqlParameter("@success", SqlDbType.Int) { Direction = ParameterDirection.Output };
        command.Parameters.Add(successParam);

        if (connection.State != ConnectionState.Open) await connection.OpenAsync();
        await command.ExecuteNonQueryAsync();

        int success = (int)(successParam.Value ?? 0);
        if (success <= 0) throw new InvalidOperationException("Failed to approve mission. Authorization or ID error.");
        
        // Notify
        try 
        {
            var mission = await context.Missions.FindAsync(missionID);
            if(mission != null && mission.EmployeeId.HasValue) 
                await _notificationService.SendNotificationAsync(mission.EmployeeId.Value, "Your Mission request has been Approved.", "Success", "Normal");
        } catch {}
    }

    // TODO: stored procedure is missing (Now implemented)
    public async Task RejectMissionRequestAsync(int managerID, int missionID)
    {
        var connection = context.Database.GetDbConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "RejectMissionRequest";
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.Add(new SqlParameter("@missionID", missionID));
        command.Parameters.Add(new SqlParameter("@managerID", managerID));
        
        var successParam = new SqlParameter("@success", SqlDbType.Int) { Direction = ParameterDirection.Output };
        command.Parameters.Add(successParam);

        if (connection.State != ConnectionState.Open) await connection.OpenAsync();
        await command.ExecuteNonQueryAsync();

        int success = (int)(successParam.Value ?? 0);
        if (success <= 0) throw new InvalidOperationException("Failed to reject mission. Authorization or ID error.");
        
        try 
        {
            var mission = await context.Missions.FindAsync(missionID);
            if(mission != null && mission.EmployeeId.HasValue) 
                await _notificationService.SendNotificationAsync(mission.EmployeeId.Value, "Your Mission request has been Rejected.", "Alert", "High");
        } catch {}
    }

    public async Task<List<MissionDTO>> GetPendingMissionsAsync(int managerID)
    {
        var result = new List<MissionDTO>();
        var connection = context.Database.GetDbConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "ViewPendingMissions";
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.Add(new SqlParameter("@managerID", managerID));

        if (connection.State != ConnectionState.Open) await connection.OpenAsync();
        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            result.Add(new MissionDTO
            {
                MissionID = reader.GetInt32(reader.GetOrdinal("missionID")),
                Destination = reader.GetString(reader.GetOrdinal("destination")),
                StartDate = reader.GetDateTime(reader.GetOrdinal("startDate")),
                EndDate = reader.GetDateTime(reader.GetOrdinal("endDate")),
                Status = reader.GetString(reader.GetOrdinal("status")),
                EmployeeName = reader.GetString(reader.GetOrdinal("EmployeeName")),
                // EmployeeID? DTO might not have it yet
            });
        }
        return result;
    }

    public async Task<int> AssignMissionAsync(int employeeID, int managerID, string destination, DateTime startDate, DateTime endDate)
    {
        // setting up the connection
        var connection = context.Database.GetDbConnection();

        using var command = connection.CreateCommand();
        command.CommandText = "AssignMission";
        command.CommandType = CommandType.StoredProcedure;

        // input parameters
        command.Parameters.Add(new SqlParameter("@employeeID", employeeID));
        command.Parameters.Add(new SqlParameter("@managerID", managerID));
        command.Parameters.Add(new SqlParameter("@destination", destination));
        command.Parameters.Add(new SqlParameter("@startDate", startDate));
        command.Parameters.Add(new SqlParameter("@endDate", endDate));

        // output parameters
        var successParam = new SqlParameter("@success", SqlDbType.Int)
        {
            Direction = ParameterDirection.Output
        };

        command.Parameters.Add(successParam);

        var missionIdParam = new SqlParameter("@missionID", SqlDbType.Int)
        {
            Direction = ParameterDirection.Output
        };
        command.Parameters.Add(missionIdParam);

        // opening the connection (if not already done)
        if (connection.State != ConnectionState.Open)
        {
            await connection.OpenAsync();
        }

        // calling the procedure
        await command.ExecuteNonQueryAsync();

        // assessing the final state of success
        int success = (int)(successParam.Value ?? -99);
        int missionID = (int)(missionIdParam.Value ?? -1);

        if (success == 0)
        {
            throw new EmployeeNotFoundException();
        }
        else if (success == -1)
        {
            throw new ManagerNotFoundException();
        }
        else if (success == 1)
        {
            // Notify
            try
            {
                await _notificationService.SendNotificationAsync(employeeID, $"You have been assigned a Mission to {destination} ({startDate.ToShortDateString()})", "Info", "High");
            } catch {}
            return missionID;
        }
        else
        {
            throw new UnexpectedErrorException();
        }
    }
}
