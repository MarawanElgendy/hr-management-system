namespace Services.Profile;

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

using HRMS.Models;
using HRMS.Exceptions;
using DTOs;

public class EmployeeProfileService : IEmployeeProfileService
{
    private readonly HrmsContext context;

    public EmployeeProfileService(HrmsContext context)
    {
        this.context = context;
    }

    public async Task<EmployeeFullProfileDTO> ViewEmployeeProfileAsync(int employeeID)
    {
        var connection = context.Database.GetDbConnection();

        using var command = connection.CreateCommand();
        command.CommandText = "ViewEmployeeProfile";
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

        EmployeeFullProfileDTO? profile = null;

        using var reader = await command.ExecuteReaderAsync();

        // getting the first results set
        if (await reader.ReadAsync())
        {
            profile = new EmployeeFullProfileDTO
            {
                EmployeeID = reader.GetInt32(reader.GetOrdinal("employeeID")),
                FirstName = reader.IsDBNull(reader.GetOrdinal("firstName")) ? null : reader.GetString(reader.GetOrdinal("firstName")),
                MiddleName = reader.IsDBNull(reader.GetOrdinal("middleName")) ? null : reader.GetString(reader.GetOrdinal("middleName")),
                LastName = reader.IsDBNull(reader.GetOrdinal("lastName")) ? null : reader.GetString(reader.GetOrdinal("lastName")),
                FullName = reader.IsDBNull(reader.GetOrdinal("fullName")) ? null : reader.GetString(reader.GetOrdinal("fullName")),
                NationalID = reader.IsDBNull(reader.GetOrdinal("nationalID")) ? null : reader.GetString(reader.GetOrdinal("nationalID")),
                
                BirthDate = reader.IsDBNull(reader.GetOrdinal("birthDate"))
                    ? null
                    : reader.GetDateTime(reader.GetOrdinal("birthDate")),
                
                BirthCountry = reader.IsDBNull(reader.GetOrdinal("birthCountry")) ? null : reader.GetString(reader.GetOrdinal("birthCountry")),
                EmailAddress = reader.IsDBNull(reader.GetOrdinal("emailAddress")) ? null : reader.GetString(reader.GetOrdinal("emailAddress")),
                PhoneNumber = reader.IsDBNull(reader.GetOrdinal("phoneNumber")) ? null : reader.GetString(reader.GetOrdinal("phoneNumber")),
                Address = reader.IsDBNull(reader.GetOrdinal("address")) ? null : reader.GetString(reader.GetOrdinal("address")),
                EmergencyContactName = reader.IsDBNull(reader.GetOrdinal("emergencyContactName")) ? null : reader.GetString(reader.GetOrdinal("emergencyContactName")),
                EmergencyContactPhone = reader.IsDBNull(reader.GetOrdinal("emergencyContactPhone")) ? null : reader.GetString(reader.GetOrdinal("emergencyContactPhone")),
                Relationship = reader.IsDBNull(reader.GetOrdinal("relationship")) ? null : reader.GetString(reader.GetOrdinal("relationship")),
                Biography = reader.IsDBNull(reader.GetOrdinal("biography")) ? null : reader.GetString(reader.GetOrdinal("biography")),
                ProfileCompletion = reader.IsDBNull(reader.GetOrdinal("profileCompletion")) ? 0 : reader.GetInt32(reader.GetOrdinal("profileCompletion")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("isActive")),
                EmploymentStatus = reader.IsDBNull(reader.GetOrdinal("employmentStatus")) ? null : reader.GetString(reader.GetOrdinal("employmentStatus")),

                HireDate = reader.IsDBNull(reader.GetOrdinal("hireDate"))
                    ? null
                    : reader.GetDateTime(reader.GetOrdinal("hireDate")),

                DepartmentName = reader.IsDBNull(reader.GetOrdinal("departmentName")) ? null : reader.GetString(reader.GetOrdinal("departmentName")),
                DepartmentID = reader.IsDBNull(reader.GetOrdinal("departmentID")) ? null : reader.GetInt32(reader.GetOrdinal("departmentID")),
                PositionTitle = reader.IsDBNull(reader.GetOrdinal("positionTitle")) ? null : reader.GetString(reader.GetOrdinal("positionTitle")),
                PositionID = reader.IsDBNull(reader.GetOrdinal("positionID")) ? null : reader.GetInt32(reader.GetOrdinal("positionID")),
                ManagerName = reader.IsDBNull(reader.GetOrdinal("managerName")) ? null : reader.GetString(reader.GetOrdinal("managerName")),
                ManagerID = reader.IsDBNull(reader.GetOrdinal("managerID")) ? null : reader.GetInt32(reader.GetOrdinal("managerID")),
                ContractType = reader.IsDBNull(reader.GetOrdinal("contractType")) ? null : reader.GetString(reader.GetOrdinal("contractType")),
                ContractID = reader.IsDBNull(reader.GetOrdinal("contractID")) ? null : reader.GetInt32(reader.GetOrdinal("contractID")),

                ContractStart = reader.IsDBNull(reader.GetOrdinal("contractStart"))
                    ? null
                    : reader.GetDateTime(reader.GetOrdinal("contractStart")),

                ContractEnd = reader.IsDBNull(reader.GetOrdinal("contractEnd"))
                    ? null
                    : reader.GetDateTime(reader.GetOrdinal("contractEnd")),
                
                TaxFormID = reader.IsDBNull(reader.GetOrdinal("taxFormID")) ? null : reader.GetInt32(reader.GetOrdinal("taxFormID")),
                SalaryTypeID = reader.IsDBNull(reader.GetOrdinal("salaryTypeID")) ? null : reader.GetInt32(reader.GetOrdinal("salaryTypeID")),
                PayGrade = reader.IsDBNull(reader.GetOrdinal("payGrade")) ? null : reader.GetString(reader.GetOrdinal("payGrade"))
            };
        }

        // getting the second results set (the skills)
        if (profile != null && await reader.NextResultAsync())
        {
            while (await reader.ReadAsync())
            {
                profile.Skills.Add(new EmployeeSkillDTO
                {
                    SkillID = reader.GetInt32(reader.GetOrdinal("skillID")),
                    SkillName = reader.GetString(reader.GetOrdinal("skillName")),
                    ProficiencyLevel = reader.GetInt32(reader.GetOrdinal("proficiencyLevel"))
                });
            }
        }

        await reader.CloseAsync();

        int success = (int)(successParam.Value ?? -1);

        if (success == -1)
        {
            throw new EmployeeNotFoundException();
        }
        else
        {
                return profile;
        }
    }

    public async Task FindEmployeeAsync(int employeeID)
    {
        var connection = context.Database.GetDbConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "FindEmployee";
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.Add(new SqlParameter("@employeeID", employeeID));

        var successParam = new SqlParameter("@success", SqlDbType.Int)
        {
            Direction = ParameterDirection.Output
        };
        command.Parameters.Add(successParam);

        if (connection.State != ConnectionState.Open)
        {
            await connection.OpenAsync();
        }

        await command.ExecuteNonQueryAsync();

        int success = (int)(successParam.Value ?? 0);
        if (success == 0)
        {
            throw new EmployeeNotFoundException();
        }
    }

    public async Task UpdateEmployeeProfileAsync(int employeeID, string field, string value)
    {
        // setting up the connection
        var connection = context.Database.GetDbConnection();

        using var command = connection.CreateCommand();
        command.CommandText = "UpdateEmployeeProfile";
        command.CommandType = CommandType.StoredProcedure;

        // input parameters
        command.Parameters.Add(new SqlParameter("@employeeID", employeeID));
        command.Parameters.Add(new SqlParameter("@field", field));
        command.Parameters.Add(new SqlParameter("@value", value));

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
            throw new EmployeeNotFoundException();
        }
        else if (success == -1)
        {
            throw new InvalidFieldException();
        }
    }

    public async Task UpdateEmployeeEmergencyContactAsync(int employeeID, string name, string number, string relationship)
    {
        // validating the input
        if (name == null || number == null || relationship == null)
        {
            throw new InvalidFieldException();
        }

        name.Trim();
        number.Trim();
        relationship.Trim();

        // setting up the connection
        var connection = context.Database.GetDbConnection();

        using var command = connection.CreateCommand();
        command.CommandText = "UpdateEmergencyContact";
        command.CommandType = CommandType.StoredProcedure;

        // input parameters
        command.Parameters.Add(new SqlParameter("@EmployeeID", employeeID));
        command.Parameters.Add(new SqlParameter("@ContactName", name));
        command.Parameters.Add(new SqlParameter("@ContactPhone", number));
        command.Parameters.Add(new SqlParameter("@Relationship", relationship));

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
            throw new UnexpectedErrorException();
        }
        else if (success == -1)
        {
            throw new EmployeeNotFoundException();
        }
    }

    public async Task SetProfileCompletenessAsync(int employeeID, int completenessPercentage)
    {
        // setting up the connection
        var connection = context.Database.GetDbConnection();

        using var command = connection.CreateCommand();
        command.CommandText = "SetProfileCompleteness";
        command.CommandType = CommandType.StoredProcedure;

        // input parameters
        command.Parameters.Add(new SqlParameter("@employeeID", employeeID));
        command.Parameters.Add(new SqlParameter("@completenessPercentage", completenessPercentage));

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
            throw new EmployeeNotFoundException();
        }
        else if (success == -1)
        {
            throw new InvalidPercentageException();
        }
    }

    public async Task<int> CreateEmployeeProfileAsync
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

    public async Task<List<EmployeeSummaryDTO>> SearchEmployeesAsync(string? query, int? departmentId)
    {
        var result = new List<EmployeeSummaryDTO>();

        var connection = context.Database.GetDbConnection();
        using var command = connection.CreateCommand();

        command.CommandText = "SearchEmployees";
        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.Add(new SqlParameter("@Query", (object?)query ?? DBNull.Value));
        command.Parameters.Add(new SqlParameter("@DepartmentID", (object?)departmentId ?? DBNull.Value));

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync();

        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            result.Add(new EmployeeSummaryDTO
            {
                EmployeeID = reader.GetInt32(reader.GetOrdinal("employeeID")),
                FullName = reader.GetString(reader.GetOrdinal("fullName")),
                DepartmentName = reader.IsDBNull(reader.GetOrdinal("departmentName")) ? "No Department" : reader.GetString(reader.GetOrdinal("departmentName")),
                DepartmentID = reader.IsDBNull(reader.GetOrdinal("departmentID")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("departmentID")),
                PositionTitle = reader.IsDBNull(reader.GetOrdinal("positionTitle")) ? "No Position" : reader.GetString(reader.GetOrdinal("positionTitle")),
                EmailAddress = reader.IsDBNull(reader.GetOrdinal("emailAddress")) ? "N/A" : reader.GetString(reader.GetOrdinal("emailAddress")),
                PhoneNumber = reader.IsDBNull(reader.GetOrdinal("phoneNumber")) ? "N/A" : reader.GetString(reader.GetOrdinal("phoneNumber"))
            });
        }

        return result;
    }
    public async Task<List<EmployeeSummaryDTO>> GetTeamMembersAsync(int managerID)
    {
        var result = new List<EmployeeSummaryDTO>();

        var connection = context.Database.GetDbConnection();
        using var command = connection.CreateCommand();

        command.CommandText = "GetTeamByManager";
        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.Add(new SqlParameter("@managerID", managerID));

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync();

        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            result.Add(new EmployeeSummaryDTO
            {
                EmployeeID = reader.GetInt32(reader.GetOrdinal("employeeID")),
                FullName = reader.GetString(reader.GetOrdinal("fullName")),
                DepartmentName = reader.IsDBNull(reader.GetOrdinal("departmentName")) ? "No Department" : reader.GetString(reader.GetOrdinal("departmentName")),
                DepartmentID = reader.IsDBNull(reader.GetOrdinal("departmentID")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("departmentID")),
                PositionTitle = reader.IsDBNull(reader.GetOrdinal("positionTitle")) ? "No Position" : reader.GetString(reader.GetOrdinal("positionTitle")),
                EmailAddress = reader.IsDBNull(reader.GetOrdinal("emailAddress")) ? "N/A" : reader.GetString(reader.GetOrdinal("emailAddress")),
                PhoneNumber = reader.IsDBNull(reader.GetOrdinal("phoneNumber")) ? "N/A" : reader.GetString(reader.GetOrdinal("phoneNumber"))
            });
        }

        return result;
    }
    public async Task<int> CreateContractAsync(CreateContractDTO contractDto)
    {
        var connection = context.Database.GetDbConnection();
        using var command = connection.CreateCommand();

        command.CommandText = "CreateContract";
        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.Add(new SqlParameter("@employeeID", contractDto.EmployeeID));
        command.Parameters.Add(new SqlParameter("@type", contractDto.Type));
        command.Parameters.Add(new SqlParameter("@startDate", contractDto.StartDate));
        command.Parameters.Add(new SqlParameter("@endDate", contractDto.EndDate));

        var successParam = new SqlParameter("@success", SqlDbType.Int)
        {
            Direction = ParameterDirection.Output
        };
        command.Parameters.Add(successParam);

        var contractIDParam = new SqlParameter("@contractID", SqlDbType.Int)
        {
            Direction = ParameterDirection.Output
        };
        command.Parameters.Add(contractIDParam);

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync();

        await command.ExecuteNonQueryAsync();

        int success = successParam.Value == DBNull.Value ? 0 : (int)successParam.Value;

        if (success == 1)
        {
            return contractIDParam.Value == DBNull.Value ? 0 : (int)contractIDParam.Value;
        }
        else if (success == 0)
        {
             throw new ArgumentException("Invalid contract type.");
        }
        else // -1
        {
             throw new EmployeeNotFoundException();
        }
    }

    public async Task<List<DepartmentStatisticsDTO>> GetDepartmentStatisticsAsync()
    {
        var result = new List<DepartmentStatisticsDTO>();
        var connection = context.Database.GetDbConnection();
        using var command = connection.CreateCommand();

        command.CommandText = "GetDepartmentStatistics";
        command.CommandType = CommandType.StoredProcedure;

        if (connection.State != ConnectionState.Open) await connection.OpenAsync();

        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            result.Add(new DepartmentStatisticsDTO
            {
                DepartmentID = reader.GetInt32(reader.GetOrdinal("departmentID")),
                DepartmentName = reader.GetString(reader.GetOrdinal("DepartmentName")),
                HeadName = reader.GetString(reader.GetOrdinal("HeadName")),
                TeamSize = reader.GetInt32(reader.GetOrdinal("TeamSize")),
                AverageSalary = reader.GetDecimal(reader.GetOrdinal("AverageSalary")),
                SpanOfControl = reader.GetDouble(reader.GetOrdinal("SpanOfControl"))
            });
        }
        return result;
    }

    public async Task<List<OrgHierarchyDTO>> GetOrgHierarchyAsync()
    {
        var result = new List<OrgHierarchyDTO>();
        var connection = context.Database.GetDbConnection();
        using var command = connection.CreateCommand();

        command.CommandText = "ViewOrgHierarchy";
        command.CommandType = CommandType.StoredProcedure;

        if (connection.State != ConnectionState.Open) await connection.OpenAsync();

        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            var empIdObj = reader.GetValue(reader.GetOrdinal("employeeID"));
            if (empIdObj == DBNull.Value) continue; // Skip departments with no head/employees in this view

            result.Add(new OrgHierarchyDTO
            {
                DepartmentID = reader.GetInt32(reader.GetOrdinal("departmentID")),
                DepartmentName = reader.GetString(reader.GetOrdinal("departmentName")),
                EmployeeID = (int)empIdObj,
                EmployeeName = reader.IsDBNull(reader.GetOrdinal("employeeName")) ? "Unknown" : reader.GetString(reader.GetOrdinal("employeeName")),
                ManagerID = reader.IsDBNull(reader.GetOrdinal("managerID")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("managerID")),
                ManagerName = reader.IsDBNull(reader.GetOrdinal("managerName")) ? null : reader.GetString(reader.GetOrdinal("managerName")),
                PositionTitle = reader.IsDBNull(reader.GetOrdinal("positionTitle")) ? "N/A" : reader.GetString(reader.GetOrdinal("positionTitle")),
                HierarchyLevel = reader.GetInt32(reader.GetOrdinal("hierarchyLevel"))
            });
        }
        return result;
    }

    public async Task ReassignEmployeeAsync(int employeeID, int newDepartmentID, int? newManagerID)
    {
        var connection = context.Database.GetDbConnection();
        using var command = connection.CreateCommand();

        command.CommandText = "ReassignHierarchy";
        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.Add(new SqlParameter("@employeeID", employeeID));
        command.Parameters.Add(new SqlParameter("@newDepartmentID", newDepartmentID));
        command.Parameters.Add(new SqlParameter("@newManagerID", (object?)newManagerID ?? DBNull.Value));

        var successParam = new SqlParameter("@success", SqlDbType.Int)
        {
            Direction = ParameterDirection.Output
        };
        command.Parameters.Add(successParam);

        if (connection.State != ConnectionState.Open) await connection.OpenAsync();

        await command.ExecuteNonQueryAsync();

        int success = (int)(successParam.Value ?? 0);

        if (success == -1) throw new EmployeeNotFoundException();
        if (success == -2) throw new InvalidOperationException("An employee cannot be their own manager.");
        if (success == -3) throw new EmployeeNotFoundException("The selected manager does not exist.");
        if (success == -4) throw new ArgumentException("The selected department does not exist.");
    }

    public async Task<DiversityReportDTO> GetDiversityReportAsync()
    {
        var result = new DiversityReportDTO();
        var connection = context.Database.GetDbConnection();
        if (connection.State != ConnectionState.Open) await connection.OpenAsync();

        // 1. Nationality
        using (var command = connection.CreateCommand())
        {
            command.CommandText = "GetNationalityStats";
            command.CommandType = CommandType.StoredProcedure;
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                result.NationalityStats.Add(new NationalityStatDTO
                {
                    Country = reader.GetString(0),
                    Count = reader.GetInt32(1)
                });
            }
        }

        // 2. Age
        using (var command = connection.CreateCommand())
        {
            command.CommandText = "GetAgeStats";
            command.CommandType = CommandType.StoredProcedure;
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                result.AgeStats.Add(new AgeStatDTO
                {
                    AgeGroup = reader.GetString(0),
                    Count = reader.GetInt32(1)
                });
            }
        }

        // 3. Compliance
        using (var command = connection.CreateCommand())
        {
            command.CommandText = "GetComplianceStats";
            command.CommandType = CommandType.StoredProcedure;
            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                result.ComplianceStats = new ComplianceStatDTO
                {
                    AverageCompletion = reader.IsDBNull(0) ? 0 : reader.GetDouble(0),
                    FullyCompliant = reader.GetInt32(1),
                    NonCompliant = reader.GetInt32(2),
                    TotalActive = reader.GetInt32(3)
                };
            }
        }

        return result;
    }
    public async Task<List<EmployeeSummaryDTO>> GetLineManagersAsync()
    {
        // Using EF Core directly since SP might not exist for this specific filter
        return await context.LineManagers
            .Include(lm => lm.Employee)
            .Include(lm => lm.Employee.Department)
            .Include(lm => lm.Employee.Position)
            .Select(lm => new EmployeeSummaryDTO
            {
                EmployeeID = lm.EmployeeId,
                FullName = lm.Employee.FullName,
                DepartmentID = lm.Employee.DepartmentId,
                DepartmentName = lm.Employee.Department != null ? lm.Employee.Department.Name : "No Department",
                PositionTitle = lm.Employee.Position != null ? lm.Employee.Position.Title : "Line Manager",
                EmailAddress = lm.Employee.EmailAddress,
                PhoneNumber = lm.Employee.PhoneNumber
            })
            .ToListAsync();
    }
}