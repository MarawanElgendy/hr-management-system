namespace Services.General;

using HRMS.Models;

public interface IAccountService
{
    Task<bool> LoginAsync(int employeeID);
    
    Task<int> CreateEmployeeAsync(
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

    Task<string?> GetEmployeeTypeAsync(int employeeID);
}
