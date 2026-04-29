namespace Services.General;

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

using HRMS.Models;
using HRMS.Exceptions;

public class AccountService : IAccountService
{
    private readonly HrmsContext context;

    public AccountService(HrmsContext context)
    {
        this.context = context;
    }

    public async Task<bool> LoginAsync(int employeeID)
    {
        var connection = context.Database.GetDbConnection();

        using var command = connection.CreateCommand();
        command.CommandText = "FindEmployee";
        command.CommandType = CommandType.StoredProcedure;

        // input parameter
        command.Parameters.Add(new SqlParameter("@employeeID", employeeID));

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
        int success = successParam.Value == DBNull.Value ? 0 : (int)successParam.Value;

        if (success == 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public async Task<int> CreateEmployeeAsync
    (
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
    )
    {
        var connection = context.Database.GetDbConnection();

        using var command = connection.CreateCommand();
        command.CommandText = "CreateEmployeeProfile";
        command.CommandType = CommandType.StoredProcedure;

        // input parameter
        command.Parameters.Add(new SqlParameter("@firstName", firstName));
        command.Parameters.Add(new SqlParameter("@lastName", lastName));
        command.Parameters.Add(new SqlParameter("@departmentID", departmentID));
        command.Parameters.Add(new SqlParameter("@roleID", roleID));
        command.Parameters.Add(new SqlParameter("@hireDate", hireDate));
        command.Parameters.Add(new SqlParameter("@email", email));
        command.Parameters.Add(new SqlParameter("@phone", phone));
        command.Parameters.Add(new SqlParameter("@nationalID", nationalID));
        command.Parameters.Add(new SqlParameter("@birthDate", birthDate));
        command.Parameters.Add(new SqlParameter("@birthCountry", birthCountry));
        command.Parameters.Add(new SqlParameter("@type", type));

        // validating type parameter
        if (type != "SystemAdministrator" && type != "HRAdministrator" && type != "PayrollSpecialist" && type != "LineManager" && type != "Employee")
        {
            throw new InvalidAccountTypeException();
        }

        // output parameters
        var successParam = new SqlParameter("@success", SqlDbType.Int)
        {
            Direction = ParameterDirection.Output
        };

        command.Parameters.Add(successParam);

        var employeeIDParam = new SqlParameter("@employeeID", SqlDbType.Int)
        {
            Direction = ParameterDirection.Output
        };

        command.Parameters.Add(employeeIDParam);

        // opening the connection (if not already done)
        if (connection.State != ConnectionState.Open)
        {
            await connection.OpenAsync();
        }

        // calling the procedure
        await command.ExecuteNonQueryAsync();

        // assessing the final state of success
        int employeeID = employeeIDParam.Value == DBNull.Value ? 0 : (int)employeeIDParam.Value;
        int success = successParam.Value == DBNull.Value ? -99 : (int)successParam.Value;

        if (success == -1)
        {
            throw new DuplicateEmailException();
        }
        else if (success == -2)
        {
            throw new DuplicateNationalIDException();
        }
        else if (success == -3)
        {
            throw new DuplicatePhoneNumberException();
        }
        else
        {
            return employeeID;
        }
    }

    public async Task<List<Department>> GetDepartmentsAsync()
    {
        var result = new List<Department>();

        var connection = context.Database.GetDbConnection();
        using var command = connection.CreateCommand();

        command.CommandText = "GetAllDepartments";
        command.CommandType = CommandType.StoredProcedure;

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync();

        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            result.Add(new Department
            {
                DepartmentId = reader.GetInt32(0),
                Name = reader.GetString(1)
            });
        }

        return result;
    }

    public async Task<List<Role>> GetRolesAsync()
    {
        var result = new List<Role>();

        var connection = context.Database.GetDbConnection();
        using var command = connection.CreateCommand();

        command.CommandText = "GetAllRoles";
        command.CommandType = CommandType.StoredProcedure;

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync();

        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            result.Add(new Role
            {
                RoleId = reader.GetInt32(0),
                Name = reader.GetString(1)
            });
        }

        return result;
    }

    public async Task<string?> GetEmployeeTypeAsync(int employeeID)
    {
        if (await context.SystemAdministrators.AnyAsync(sa => sa.EmployeeId == employeeID))
        {
            return "SystemAdministrator";
        }
        else if (await context.Hradministrators.AnyAsync(ha => ha.EmployeeId == employeeID))
        {
            return "HRAdministrator";
        }
        else if (await context.PayrollSpecialists.AnyAsync(ps => ps.EmployeeId == employeeID))
        {
            return "PayrollSpecialist";
        }
        else if (await context.LineManagers.AnyAsync(lm => lm.EmployeeId == employeeID))
        {
            return "LineManager";
        }

        return null;
    }
}
