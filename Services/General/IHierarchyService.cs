namespace Services.General;

using DTOs;

public interface IHierarchyService
{
    Task<TeamProfilesDTO> ViewTeamDetailsAsync(int managerID);
}
