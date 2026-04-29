namespace Services.Profile;

using DTOs;

using HRMS.Models;

public interface IEmployeeProfileService
{
    Task<EmployeeFullProfileDTO> ViewEmployeeProfileAsync(int employeeID);
    Task FindEmployeeAsync(int employeeID);
    Task UpdateEmployeeProfileAsync(int employeeID, string field, string value);
    Task UpdateEmployeeEmergencyContactAsync(int employeeID, string name, string number, string relationship);
    Task SetProfileCompletenessAsync(int employeeID, int completenessPercentage);

    Task<int> CreateEmployeeProfileAsync(
        string firstName,
        string lastName,
        int departmentID,
        int roleID,
        DateTime hireDate,
        string email,
        string phone,
        string nationalID,
        DateTime birthDate,
        string birthCountry,
        string type
    );

    Task<List<Department>> GetDepartmentsAsync();
    Task<List<Role>> GetRolesAsync();
    Task<List<EmployeeSummaryDTO>> SearchEmployeesAsync(string? query, int? departmentId);

    Task<List<EmployeeSummaryDTO>> GetTeamMembersAsync(int managerID);

    Task<int> CreateContractAsync(CreateContractDTO contractDto);
    Task<List<DepartmentStatisticsDTO>> GetDepartmentStatisticsAsync();
    Task<List<OrgHierarchyDTO>> GetOrgHierarchyAsync();
    Task ReassignEmployeeAsync(int employeeID, int newDepartmentID, int? newManagerID);
    Task<DiversityReportDTO> GetDiversityReportAsync();
    Task<List<EmployeeSummaryDTO>> GetLineManagersAsync();
}
