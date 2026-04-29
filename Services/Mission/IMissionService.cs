namespace Services.Mission;

using DTOs;

public interface IMissionService
{
    Task<MissionsDTO> ViewAssignedMissionsAsync(int employeeID);
    Task ApproveMissionRequestAsync(int managerID, int missionID);
    Task RejectMissionRequestAsync(int managerID, int missionID);
    Task<int> AssignMissionAsync(int employeeID, int managerID, string destination, DateTime startDate, DateTime endDate);
    Task<List<MissionDTO>> GetPendingMissionsAsync(int managerID);
}
