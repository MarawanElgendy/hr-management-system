namespace Services.Profile;

public interface IRoleAssignmentService
{
    Task AssignRoleAsync(int employeeID, int roleID);
}
