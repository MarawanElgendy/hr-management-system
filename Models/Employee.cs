using System;
using System.Collections.Generic;

namespace HRMS.Models;

public partial class Employee
{
    public int EmployeeId { get; set; }

    public string FirstName { get; set; } = null!;

    public string? MiddleName { get; set; }

    public string LastName { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string NationalId { get; set; } = null!;

    public DateOnly BirthDate { get; set; }

    public int? Age { get; set; }

    public string? BirthCountry { get; set; }

    public string PhoneNumber { get; set; } = null!;

    public string EmailAddress { get; set; } = null!;

    public string? Address { get; set; }

    public string? EmergencyContactName { get; set; }

    public string? EmergencyContactPhone { get; set; }

    public string? Relationship { get; set; }

    public string? Biography { get; set; }

    public string? ProfileImage { get; set; }

    public string? EmploymentProgress { get; set; }

    public string? Accountstatus { get; set; }

    public string? EmploymentStatus { get; set; }

    public DateOnly? HireDate { get; set; }
    
    public bool IsFlagged { get; set; }

    public bool IsActive { get; set; }

    public int? ProfileCompletion { get; set; }

    public int? DepartmentId { get; set; }

    public int? PositionId { get; set; }

    public int? ManagerId { get; set; }

    public int? ContractId { get; set; }

    public int? TaxFormId { get; set; }

    public int? SalaryTypeId { get; set; }

    public string? PayGrade { get; set; }

    public virtual ICollection<AllowanceAndDeduction> AllowanceAndDeductions { get; set; } = new List<AllowanceAndDeduction>();

    public virtual ICollection<AttendanceCorrectionRequest> AttendanceCorrectionRequestEmployees { get; set; } = new List<AttendanceCorrectionRequest>();

    public virtual ICollection<AttendanceCorrectionRequest> AttendanceCorrectionRequestRecordedByNavigations { get; set; } = new List<AttendanceCorrectionRequest>();

    public virtual ICollection<AttendanceLog> AttendanceLogs { get; set; } = new List<AttendanceLog>();

    public virtual ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();

    public virtual Contract? Contract { get; set; }

    public virtual Department? Department { get; set; }

    public virtual ICollection<Department> Departments { get; set; } = new List<Department>();

    public virtual ICollection<Device> Devices { get; set; } = new List<Device>();

    public virtual ICollection<EstablishesHierarchy> EstablishesHierarchyEmployees { get; set; } = new List<EstablishesHierarchy>();

    public virtual ICollection<EstablishesHierarchy> EstablishesHierarchyManagers { get; set; } = new List<EstablishesHierarchy>();

    public virtual ICollection<FulfillsRole> FulfillsRoles { get; set; } = new List<FulfillsRole>();

    public virtual Hradministrator? Hradministrator { get; set; }

    public virtual ICollection<Employee> InverseManager { get; set; } = new List<Employee>();

    public virtual ICollection<LeaveEntitlement> LeaveEntitlements { get; set; } = new List<LeaveEntitlement>();

    public virtual ICollection<LeaveRequest> LeaveRequests { get; set; } = new List<LeaveRequest>();

    public virtual LineManager? LineManager { get; set; }

    public virtual Employee? Manager { get; set; }

    public virtual ICollection<ManagerNote> ManagerNoteEmployees { get; set; } = new List<ManagerNote>();

    public virtual ICollection<ManagerNote> ManagerNoteManagers { get; set; } = new List<ManagerNote>();

    public virtual ICollection<Mission> MissionEmployees { get; set; } = new List<Mission>();

    public virtual ICollection<Mission> MissionManagers { get; set; } = new List<Mission>();

    public virtual PayGrade? PayGradeNavigation { get; set; }

    public virtual ICollection<PayrollLog> PayrollLogs { get; set; } = new List<PayrollLog>();

    public virtual ICollection<PayrollPolicy> PayrollPolicies { get; set; } = new List<PayrollPolicy>();

    public virtual PayrollSpecialist? PayrollSpecialist { get; set; }

    public virtual ICollection<Payroll> Payrolls { get; set; } = new List<Payroll>();

    public virtual Position? Position { get; set; }

    public virtual ICollection<PossessesSkill> PossessesSkills { get; set; } = new List<PossessesSkill>();

    public virtual ICollection<ReceivesNotification> ReceivesNotifications { get; set; } = new List<ReceivesNotification>();

    public virtual ICollection<Reimbursement> Reimbursements { get; set; } = new List<Reimbursement>();

    public virtual ICollection<ReplacementRequest> ReplacementRequests { get; set; } = new List<ReplacementRequest>();

    public virtual SalaryType? SalaryType { get; set; }

    public virtual ICollection<ShiftConfiguration> ShiftConfigurations { get; set; } = new List<ShiftConfiguration>();

    public virtual ICollection<SigningBonu> SigningBonus { get; set; } = new List<SigningBonu>();

    public virtual SystemAdministrator? SystemAdministrator { get; set; }

    public virtual TaxForm? TaxForm { get; set; }

    public virtual ICollection<WorksShift> WorksShifts { get; set; } = new List<WorksShift>();

    public virtual ICollection<Exception> Exceptions { get; set; } = new List<Exception>();

    public virtual ICollection<Verification> Verifications { get; set; } = new List<Verification>();
}
