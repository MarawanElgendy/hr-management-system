namespace DTOs;

public class EmployeeFullProfileDTO
{
    public int EmployeeID { get; set; }
    public string FirstName { get; set; }
    public string MiddleName { get; set; }
    public string LastName { get; set; }
    public string FullName { get; set; }
    public string NationalID { get; set; }
    public DateTime? BirthDate { get; set; }
    public string BirthCountry { get; set; }
    public string EmailAddress { get; set; }
    public string PhoneNumber { get; set; }
    public string Address { get; set; }
    public string EmergencyContactName { get; set; }
    public string EmergencyContactPhone { get; set; }
    public string Relationship { get; set; }
    public string Biography { get; set; }
    public int ProfileCompletion { get; set; }
    public bool IsActive { get; set; }
    public string EmploymentStatus { get; set; }
    public DateTime? HireDate { get; set; }

    public string DepartmentName { get; set; }
    public int? DepartmentID { get; set; }
    public string PositionTitle { get; set; }
    public int? PositionID { get; set; }
    public string ManagerName { get; set; }
    public int? ManagerID { get; set; }

    public string ContractType { get; set; }
    public int? ContractID { get; set; }
    public DateTime? ContractStart { get; set; }
    public DateTime? ContractEnd { get; set; }

    public int? TaxFormID { get; set; }
    public int? SalaryTypeID { get; set; }
    public string PayGrade { get; set; }

    // Skills list (second SELECT)
    public List<EmployeeSkillDTO> Skills { get; set; } = new();
}
