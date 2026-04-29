using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Models;

public partial class HrmsContext : DbContext
{
    public HrmsContext()
    {
    }

    public HrmsContext(DbContextOptions<HrmsContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AllowanceAndDeduction> AllowanceAndDeductions { get; set; }

    public virtual DbSet<Attendance> Attendances { get; set; }

    public virtual DbSet<AttendanceCorrectionRequest> AttendanceCorrectionRequests { get; set; }

    public virtual DbSet<AttendanceLog> AttendanceLogs { get; set; }

    public virtual DbSet<AttendanceSource> AttendanceSources { get; set; }
    public virtual DbSet<AttendanceRule> AttendanceRules { get; set; }

    public virtual DbSet<BonusPolicy> BonusPolicies { get; set; }

    public virtual DbSet<ConsultantContract> ConsultantContracts { get; set; }

    public virtual DbSet<Contract> Contracts { get; set; }

    public virtual DbSet<ContractSalaryType> ContractSalaryTypes { get; set; }

    public virtual DbSet<Currency> Currencies { get; set; }

    public virtual DbSet<DeductionPolicy> DeductionPolicies { get; set; }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<Device> Devices { get; set; }

    public virtual DbSet<EligibilityRule> EligibilityRules { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<EstablishesHierarchy> EstablishesHierarchies { get; set; }

    public virtual DbSet<Exception> Exceptions { get; set; }

    public virtual DbSet<FulfillsRole> FulfillsRoles { get; set; }

    public virtual DbSet<FullTimeContract> FullTimeContracts { get; set; }

    public virtual DbSet<HolidayLeaveType> HolidayLeaveTypes { get; set; }

    public virtual DbSet<HourlySalaryType> HourlySalaryTypes { get; set; }

    public virtual DbSet<Hradministrator> Hradministrators { get; set; }

    public virtual DbSet<Insurance> Insurances { get; set; }

    public virtual DbSet<InternshipContract> InternshipContracts { get; set; }

    public virtual DbSet<LatenessPolicy> LatenessPolicies { get; set; }

    public virtual DbSet<LeaveDocument> LeaveDocuments { get; set; }

    public virtual DbSet<LeaveEntitlement> LeaveEntitlements { get; set; }

    public virtual DbSet<LeavePolicy> LeavePolicies { get; set; }

    public virtual DbSet<LeaveRequest> LeaveRequests { get; set; }

    public virtual DbSet<LeaveType> LeaveTypes { get; set; }

    public virtual DbSet<LineManager> LineManagers { get; set; }

    public virtual DbSet<ManagerNote> ManagerNotes { get; set; }

    public virtual DbSet<Mission> Missions { get; set; }

    public virtual DbSet<MonthlySalaryType> MonthlySalaryTypes { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<OvertimePolicy> OvertimePolicies { get; set; }

    public virtual DbSet<PartTimeContract> PartTimeContracts { get; set; }

    public virtual DbSet<PayGrade> PayGrades { get; set; }

    public virtual DbSet<Payroll> Payrolls { get; set; }

    public virtual DbSet<PayrollLog> PayrollLogs { get; set; }

    public virtual DbSet<PayrollPeriod> PayrollPeriods { get; set; }

    public virtual DbSet<PayrollPolicy> PayrollPolicies { get; set; }

    public virtual DbSet<PayrollSpecialist> PayrollSpecialists { get; set; }

    public virtual DbSet<Position> Positions { get; set; }

    public virtual DbSet<PossessesSkill> PossessesSkills { get; set; }

    public virtual DbSet<ProbationLeaveType> ProbationLeaveTypes { get; set; }

    public virtual DbSet<ReceivesNotification> ReceivesNotifications { get; set; }

    public virtual DbSet<Reimbursement> Reimbursements { get; set; }

    public virtual DbSet<ReplacementRequest> ReplacementRequests { get; set; }

    public virtual DbSet<Responsibility> Responsibilities { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<RolePermission> RolePermissions { get; set; }

    public virtual DbSet<SalaryType> SalaryTypes { get; set; }

    public virtual DbSet<ShiftConfiguration> ShiftConfigurations { get; set; }

    public virtual DbSet<ShiftSchedule> ShiftSchedules { get; set; }

    public virtual DbSet<SickLeaveType> SickLeaveTypes { get; set; }

    public virtual DbSet<SigningBonu> SigningBonus { get; set; }

    public virtual DbSet<SigningBonusConfiguration> SigningBonusConfigurations { get; set; }

    public virtual DbSet<Skill> Skills { get; set; }

    public virtual DbSet<StipendFeature> StipendFeatures { get; set; }

    public virtual DbSet<SystemAdministrator> SystemAdministrators { get; set; }

    public virtual DbSet<TaxForm> TaxForms { get; set; }

    public virtual DbSet<Termination> Terminations { get; set; }

    public virtual DbSet<TerminationBenefit> TerminationBenefits { get; set; }

    public virtual DbSet<VacationLeaveType> VacationLeaveTypes { get; set; }

    public virtual DbSet<Verification> Verifications { get; set; }

    public virtual DbSet<WorksShift> WorksShifts { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=JOEMOHD\\SQLEXPRESS;Database=HRMS;Trusted_Connection=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AllowanceAndDeduction>(entity =>
        {
            entity.HasKey(e => e.AllowanceAndDeductionId).HasName("PK__Allowanc__F9A299EAC3AB3371");

            entity.ToTable("AllowanceAndDeduction");

            entity.Property(e => e.AllowanceAndDeductionId).HasColumnName("allowanceAndDeductionID");
            entity.Property(e => e.Amount)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("amount");
            entity.Property(e => e.CurrencyId).HasColumnName("currencyID");
            entity.Property(e => e.Duration).HasColumnName("duration");
            entity.Property(e => e.EmployeeId).HasColumnName("employeeID");
            entity.Property(e => e.PayrollId).HasColumnName("payrollID");
            entity.Property(e => e.TimeZone).HasColumnName("timeZone");

            entity.HasOne(d => d.Currency).WithMany(p => p.AllowanceAndDeductions)
                .HasForeignKey(d => d.CurrencyId)
                .HasConstraintName("uses_currency");

            entity.HasOne(d => d.Employee).WithMany(p => p.AllowanceAndDeductions)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("affects_employee");

            entity.HasOne(d => d.Payroll).WithMany(p => p.AllowanceAndDeductions)
                .HasForeignKey(d => d.PayrollId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("modifies_payroll");
        });

        modelBuilder.Entity<Attendance>(entity =>
        {
            entity.HasKey(e => e.AttendanceId).HasName("PK__Attendan__0F09E0C6655C4262");

            entity.ToTable("Attendance");

            entity.Property(e => e.AttendanceId).HasColumnName("attendanceID");
            entity.Property(e => e.DurationMinutes)
                .HasComputedColumnSql("(datediff(minute,[entryTime],[exitTime]))", false)
                .HasColumnName("durationMinutes");
            entity.Property(e => e.EmployeeId).HasColumnName("employeeID");
            entity.Property(e => e.EntryTime)
                .HasPrecision(0)
                .HasColumnName("entryTime");
            entity.Property(e => e.ExitTime)
                .HasPrecision(0)
                .HasColumnName("exitTime");
            entity.Property(e => e.ShiftId).HasColumnName("shiftID");

            entity.HasOne(d => d.Employee).WithMany(p => p.Attendances)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("logs_presence_of");

            entity.HasOne(d => d.Shift).WithMany(p => p.Attendances)
                .HasForeignKey(d => d.ShiftId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("adheres_to_shift");
        });

        modelBuilder.Entity<AttendanceCorrectionRequest>(entity =>
        {
            entity.HasKey(e => e.RequestId).HasName("PK__Attendan__E3C5DE519B8F16A2");

            entity.ToTable("AttendanceCorrectionRequest");

            entity.Property(e => e.RequestId).HasColumnName("requestID");
            entity.Property(e => e.CorrectionType)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("correctionType");
            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.EmployeeId).HasColumnName("employeeID");
            entity.Property(e => e.Reason)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("reason");
            entity.Property(e => e.RecordedBy).HasColumnName("recordedBy");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("status");

            entity.HasOne(d => d.Employee).WithMany(p => p.AttendanceCorrectionRequestEmployees)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Attendanc__emplo__0C85DE4D");

            entity.HasOne(d => d.RecordedByNavigation).WithMany(p => p.AttendanceCorrectionRequestRecordedByNavigations)
                .HasForeignKey(d => d.RecordedBy)
                .HasConstraintName("recorded_by");
        });

        modelBuilder.Entity<AttendanceLog>(entity =>
        {
            entity.HasKey(e => e.AttendanceLogId).HasName("PK__Attendan__F147FF8E1789C4F5");

            entity.ToTable("AttendanceLog");

            entity.Property(e => e.AttendanceLogId).HasColumnName("attendanceLogID");
            entity.Property(e => e.Actor).HasColumnName("actor");
            entity.Property(e => e.AttendanceId).HasColumnName("attendanceID");
            entity.Property(e => e.Reason)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("reason");
            entity.Property(e => e.Timestamp)
                .HasPrecision(0)
                .HasColumnName("timestamp");

            entity.HasOne(d => d.ActorNavigation).WithMany(p => p.AttendanceLogs)
                .HasForeignKey(d => d.Actor)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("acted_by_employee");

            entity.HasOne(d => d.Attendance).WithMany(p => p.AttendanceLogs)
                .HasForeignKey(d => d.AttendanceId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("tracks_change_in");
        });

        modelBuilder.Entity<AttendanceSource>(entity =>
        {
            entity.HasKey(e => new { e.AttendanceId, e.DeviceId }).HasName("PK__Attendan__6742018D4E92C204");

            entity.ToTable("AttendanceSource");

            entity.Property(e => e.AttendanceId).HasColumnName("attendanceID");
            entity.Property(e => e.DeviceId).HasColumnName("deviceID");
            entity.Property(e => e.Latitude)
                .HasColumnType("decimal(9, 6)")
                .HasColumnName("latitude");
            entity.Property(e => e.Longitude)
                .HasColumnType("decimal(9, 6)")
                .HasColumnName("longitude");
            entity.Property(e => e.RecordedAt)
                .HasPrecision(0)
                .HasColumnName("recordedAt");
            entity.Property(e => e.SourceType)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("sourceType");

            entity.HasOne(d => d.Attendance).WithMany(p => p.AttendanceSources)
                .HasForeignKey(d => d.AttendanceId)
                .HasConstraintName("has_source");

            entity.HasOne(d => d.Device).WithMany(p => p.AttendanceSources)
                .HasForeignKey(d => d.DeviceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("recorded_from");
        });

        modelBuilder.Entity<BonusPolicy>(entity =>
        {
            entity.HasKey(e => e.PayrollPolicyId).HasName("PK__BonusPol__CD123DECD80322E6");

            entity.ToTable("BonusPolicy");

            entity.Property(e => e.PayrollPolicyId)
                .ValueGeneratedNever()
                .HasColumnName("payrollPolicyID");
            entity.Property(e => e.BonusType)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("bonusType");
            entity.Property(e => e.EligibiltyCriteria).HasColumnName("eligibiltyCriteria");

            entity.HasOne(d => d.PayrollPolicy).WithOne(p => p.BonusPolicy)
                .HasForeignKey<BonusPolicy>(d => d.PayrollPolicyId)
                .HasConstraintName("FK__BonusPoli__payro__43D61337");
        });

        modelBuilder.Entity<ConsultantContract>(entity =>
        {
            entity.HasKey(e => e.ContractId).HasName("PK__Consulta__1382096175A56CC7");

            entity.ToTable("ConsultantContract");

            entity.Property(e => e.ContractId)
                .ValueGeneratedNever()
                .HasColumnName("contractID");
            entity.Property(e => e.Fees)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("fees");
            entity.Property(e => e.PaymentSchedule)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("paymentSchedule");
            entity.Property(e => e.ProjectScope)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("projectScope");

            entity.HasOne(d => d.Contract).WithOne(p => p.ConsultantContract)
                .HasForeignKey<ConsultantContract>(d => d.ContractId)
                .HasConstraintName("FK__Consultan__contr__7F2BE32F");
        });

        modelBuilder.Entity<Contract>(entity =>
        {
            entity.HasKey(e => e.ContractId).HasName("PK__Contract__138209619F0479B6");

            entity.ToTable("Contract");

            entity.HasIndex(e => e.EmployeeId, "UQ__Contract__C134C9A09112F227").IsUnique();

            entity.Property(e => e.ContractId).HasColumnName("contractID");
            entity.Property(e => e.CurrentState)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("currentState");
            entity.Property(e => e.EmployeeId).HasColumnName("employeeID");
            entity.Property(e => e.EndDate).HasColumnName("endDate");
            entity.Property(e => e.InsuranceId).HasColumnName("insuranceID");
            entity.Property(e => e.StartDate).HasColumnName("startDate");
            entity.Property(e => e.Type)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("type");

            entity.HasOne(d => d.Insurance).WithMany(p => p.Contracts)
                .HasForeignKey(d => d.InsuranceId)
                .HasConstraintName("insured");
        });

        modelBuilder.Entity<ContractSalaryType>(entity =>
        {
            entity.HasKey(e => e.SalaryTypeId).HasName("PK__Contract__6578493270F6224B");

            entity.ToTable("ContractSalaryType");

            entity.Property(e => e.SalaryTypeId)
                .ValueGeneratedNever()
                .HasColumnName("salaryTypeID");
            entity.Property(e => e.ContractValue)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("contractValue");
            entity.Property(e => e.Installment)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("installment");

            entity.HasOne(d => d.SalaryType).WithOne(p => p.ContractSalaryType)
                .HasForeignKey<ContractSalaryType>(d => d.SalaryTypeId)
                .HasConstraintName("FK__ContractS__salar__531856C7");
        });

        modelBuilder.Entity<Currency>(entity =>
        {
            entity.HasKey(e => e.CurrencyId).HasName("PK__Currency__DAF0B2EA268EB7C4");

            entity.ToTable("Currency");

            entity.HasIndex(e => e.CurrencyName, "UQ__Currency__FE869A39620EC842").IsUnique();

            entity.Property(e => e.CurrencyId).HasColumnName("currencyID");
            entity.Property(e => e.CurrencyName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("currencyName");
        });

        modelBuilder.Entity<DeductionPolicy>(entity =>
        {
            entity.HasKey(e => e.PayrollPolicyId).HasName("PK__Deductio__CD123DEC0400A703");

            entity.ToTable("DeductionPolicy");

            entity.Property(e => e.PayrollPolicyId)
                .ValueGeneratedNever()
                .HasColumnName("payrollPolicyID");
            entity.Property(e => e.CalculationMode).HasColumnName("calculationMode");
            entity.Property(e => e.DeductionReasons)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("deductionReasons");

            entity.HasOne(d => d.PayrollPolicy).WithOne(p => p.DeductionPolicy)
                .HasForeignKey<DeductionPolicy>(d => d.PayrollPolicyId)
                .HasConstraintName("FK__Deduction__payro__46B27FE2");
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.DepartmentId).HasName("PK__Departme__F9B8344DA968945D");

            entity.ToTable("Department");

            entity.HasIndex(e => e.Name, "UQ__Departme__72E12F1BCE85B2E9").IsUnique();

            entity.Property(e => e.DepartmentId).HasColumnName("departmentID");
            entity.Property(e => e.HeadId).HasColumnName("headID");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.Purpose)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("purpose");

            entity.HasOne(d => d.Head).WithMany(p => p.Departments)
                .HasForeignKey(d => d.HeadId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("is_managed_by");
        });

        modelBuilder.Entity<Device>(entity =>
        {
            entity.HasKey(e => e.DeviceId).HasName("PK__Device__84BE14B78D76F3A1");

            entity.ToTable("Device");

            entity.Property(e => e.DeviceId).HasColumnName("deviceID");
            entity.Property(e => e.DeviceType)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("deviceType");
            entity.Property(e => e.EmployeeId).HasColumnName("employeeID");
            entity.Property(e => e.Latitude)
                .HasColumnType("decimal(9, 6)")
                .HasColumnName("latitude");
            entity.Property(e => e.Longitude)
                .HasColumnType("decimal(9, 6)")
                .HasColumnName("longitude");
            entity.Property(e => e.TerminalId)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("terminalID");

            entity.HasOne(d => d.Employee).WithMany(p => p.Devices)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("device_belongs_to");
        });

        modelBuilder.Entity<EligibilityRule>(entity =>
        {
            entity.HasKey(e => e.EligibilityRuleId).HasName("PK__Eligibil__A7B08E2DA61EB0EE");

            entity.ToTable("EligibilityRule");

            entity.Property(e => e.EligibilityRuleId).HasColumnName("eligibilityRuleID");
            entity.Property(e => e.EligibilityRule1)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("eligibilityRule");
            entity.Property(e => e.LeavePolicyId).HasColumnName("leavePolicyID");

            entity.HasOne(d => d.LeavePolicy).WithMany(p => p.EligibilityRules)
                .HasForeignKey(d => d.LeavePolicyId)
                .HasConstraintName("FK__Eligibili__leave__31B762FC");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployeeId).HasName("PK__Employee__C134C9A143CC75EC");

            entity.ToTable("Employee");

            entity.HasIndex(e => e.EmailAddress, "UQ__Employee__347C30271902D198").IsUnique();

            entity.HasIndex(e => e.PhoneNumber, "UQ__Employee__4849DA015BBA81C2").IsUnique();

            entity.HasIndex(e => e.NationalId, "UQ__Employee__B5881E88696C8259").IsUnique();

            entity.Property(e => e.EmployeeId).HasColumnName("employeeID");
            entity.Property(e => e.Accountstatus)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("accountstatus");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("address");
            entity.Property(e => e.Age)
                .HasComputedColumnSql("(datediff(year,[birthDate],getdate()))", false)
                .HasColumnName("age");
            entity.Property(e => e.Biography)
                .IsUnicode(false)
                .HasColumnName("biography");
            entity.Property(e => e.BirthCountry)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("birthCountry");
            entity.Property(e => e.BirthDate).HasColumnName("birthDate");
            entity.Property(e => e.ContractId).HasColumnName("contractID");
            entity.Property(e => e.DepartmentId).HasColumnName("departmentID");
            entity.Property(e => e.EmailAddress)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("emailAddress");
            entity.Property(e => e.EmergencyContactName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("emergencyContactName");
            entity.Property(e => e.EmergencyContactPhone)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("emergencyContactPhone");
            entity.Property(e => e.EmploymentProgress)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("employmentProgress");
            entity.Property(e => e.EmploymentStatus)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("employmentStatus");
            entity.Property(e => e.FirstName)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("firstName");
            entity.Property(e => e.FullName)
                .HasMaxLength(602)
                .IsUnicode(false)
                .HasComputedColumnSql("((([firstName]+' ')+isnull([middleName]+' ',''))+[lastName])", false)
                .HasColumnName("fullName");
            entity.Property(e => e.HireDate).HasColumnName("hireDate");
            entity.Property(e => e.IsFlagged).HasColumnName("isFlagged");
            entity.Property(e => e.IsActive).HasColumnName("isActive");
            entity.Property(e => e.LastName)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("lastName");
            entity.Property(e => e.ManagerId).HasColumnName("managerID");
            entity.Property(e => e.MiddleName)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("middleName");
            entity.Property(e => e.NationalId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nationalID");
            entity.Property(e => e.PayGrade)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("payGrade");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("phoneNumber");
            entity.Property(e => e.PositionId).HasColumnName("positionID");
            entity.Property(e => e.ProfileCompletion).HasColumnName("profileCompletion");
            entity.Property(e => e.ProfileImage)
                .IsUnicode(false)
                .HasColumnName("profileImage");
            entity.Property(e => e.Relationship)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("relationship");
            entity.Property(e => e.SalaryTypeId).HasColumnName("salaryTypeID");
            entity.Property(e => e.TaxFormId).HasColumnName("taxFormID");

            entity.HasOne(d => d.Contract).WithMany(p => p.Employees)
                .HasForeignKey(d => d.ContractId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("covers_employee");

            entity.HasOne(d => d.Department).WithMany(p => p.Employees)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("works_for");

            entity.HasOne(d => d.Manager).WithMany(p => p.InverseManager)
                .HasForeignKey(d => d.ManagerId)
                .HasConstraintName("reports_to");

            entity.HasOne(d => d.PayGradeNavigation).WithMany(p => p.Employees)
                .HasForeignKey(d => d.PayGrade)
                .HasConstraintName("fits_pay_grade");

            entity.HasOne(d => d.Position).WithMany(p => p.Employees)
                .HasForeignKey(d => d.PositionId)
                .HasConstraintName("holds_position");

            entity.HasOne(d => d.SalaryType).WithMany(p => p.Employees)
                .HasForeignKey(d => d.SalaryTypeId)
                .HasConstraintName("earns_salary_type");

            entity.HasOne(d => d.TaxForm).WithMany(p => p.Employees)
                .HasForeignKey(d => d.TaxFormId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("files_tax_form");

            entity.HasMany(d => d.Exceptions).WithMany(p => p.Employees)
                .UsingEntity<Dictionary<string, object>>(
                    "Excused",
                    r => r.HasOne<Exception>().WithMany()
                        .HasForeignKey("ExceptionId")
                        .HasConstraintName("FK__excused__excepti__1F63A897"),
                    l => l.HasOne<Employee>().WithMany()
                        .HasForeignKey("EmployeeId")
                        .HasConstraintName("FK__excused__employe__1E6F845E"),
                    j =>
                    {
                        j.HasKey("EmployeeId", "ExceptionId").HasName("PK__excused__3D52DFDF9EBF3509");
                        j.ToTable("excused");
                        j.IndexerProperty<int>("EmployeeId").HasColumnName("employeeID");
                        j.IndexerProperty<int>("ExceptionId").HasColumnName("exceptionID");
                    });

            entity.HasMany(d => d.Verifications).WithMany(p => p.Employees)
                .UsingEntity<Dictionary<string, object>>(
                    "UndergoesVerification",
                    r => r.HasOne<Verification>().WithMany()
                        .HasForeignKey("VerificationId")
                        .HasConstraintName("FK__undergoes__verif__7C1A6C5A"),
                    l => l.HasOne<Employee>().WithMany()
                        .HasForeignKey("EmployeeId")
                        .HasConstraintName("FK__undergoes__emplo__7B264821"),
                    j =>
                    {
                        j.HasKey("EmployeeId", "VerificationId").HasName("PK__undergoe__7727052B2908738A");
                        j.ToTable("undergoes_verification");
                        j.IndexerProperty<int>("EmployeeId").HasColumnName("employeeID");
                        j.IndexerProperty<int>("VerificationId").HasColumnName("verificationID");
                    });
        });

        modelBuilder.Entity<EstablishesHierarchy>(entity =>
        {
            entity.HasKey(e => new { e.EmployeeId, e.HierarchyLevel }).HasName("PK__establis__34B2CA0CE486156F");

            entity.ToTable("establishes_hierarchy");

            entity.Property(e => e.EmployeeId).HasColumnName("employeeID");
            entity.Property(e => e.HierarchyLevel).HasColumnName("hierarchyLevel");
            entity.Property(e => e.ManagerId).HasColumnName("managerID");

            entity.HasOne(d => d.Employee).WithMany(p => p.EstablishesHierarchyEmployees)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__establish__emplo__12FDD1B2");

            entity.HasOne(d => d.Manager).WithMany(p => p.EstablishesHierarchyManagers)
                .HasForeignKey(d => d.ManagerId)
                .HasConstraintName("FK__establish__manag__13F1F5EB");
        });

        modelBuilder.Entity<Exception>(entity =>
        {
            entity.HasKey(e => e.ExceptionId).HasName("PK__Exceptio__C66167EEAC9326D9");

            entity.ToTable("Exception");

            entity.Property(e => e.ExceptionId).HasColumnName("exceptionID");
            entity.Property(e => e.Category)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("category");
            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.Status)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("status");
        });

        modelBuilder.Entity<FulfillsRole>(entity =>
        {
            entity.HasKey(e => new { e.EmployeeId, e.RoleId }).HasName("PK__fulfills__7DED4DC15704006F");

            entity.ToTable("fulfills_role");

            entity.Property(e => e.EmployeeId).HasColumnName("employeeID");
            entity.Property(e => e.RoleId).HasColumnName("roleID");
            entity.Property(e => e.AssignedDate)
                .HasPrecision(0)
                .HasColumnName("assignedDate");

            entity.HasOne(d => d.Employee).WithMany(p => p.FulfillsRoles)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("FK__fulfills___emplo__72910220");

            entity.HasOne(d => d.Role).WithMany(p => p.FulfillsRoles)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK__fulfills___roleI__73852659");
        });

        modelBuilder.Entity<FullTimeContract>(entity =>
        {
            entity.HasKey(e => e.ContractId).HasName("PK__FullTime__13820961F0A68A12");

            entity.ToTable("FullTimeContract");

            entity.Property(e => e.ContractId)
                .ValueGeneratedNever()
                .HasColumnName("contractID");
            entity.Property(e => e.InsuranceEligibility).HasColumnName("insuranceEligibility");
            entity.Property(e => e.LeaveEntitlement).HasColumnName("leaveEntitlement");
            entity.Property(e => e.WeeklyWorkingHours).HasColumnName("weeklyWorkingHours");

            entity.HasOne(d => d.Contract).WithOne(p => p.FullTimeContract)
                .HasForeignKey<FullTimeContract>(d => d.ContractId)
                .HasConstraintName("FK__FullTimeC__contr__797309D9");
        });

        modelBuilder.Entity<HolidayLeaveType>(entity =>
        {
            entity.HasKey(e => e.LeaveTypeId).HasName("PK__HolidayL__CEFA329811F07751");

            entity.ToTable("HolidayLeaveType");

            entity.Property(e => e.LeaveTypeId)
                .ValueGeneratedNever()
                .HasColumnName("leaveTypeID");
            entity.Property(e => e.HolidayName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("holidayName");
            entity.Property(e => e.OfficialRecognition)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("officialRecognition");
            entity.Property(e => e.RegionalScope)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("regionalScope");

            entity.HasOne(d => d.LeaveType).WithOne(p => p.HolidayLeaveType)
                .HasForeignKey<HolidayLeaveType>(d => d.LeaveTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__HolidayLe__leave__1EA48E88");
        });

        modelBuilder.Entity<HourlySalaryType>(entity =>
        {
            entity.HasKey(e => e.SalaryTypeId).HasName("PK__HourlySa__65784932EB76514C");

            entity.ToTable("HourlySalaryType");

            entity.Property(e => e.SalaryTypeId)
                .ValueGeneratedNever()
                .HasColumnName("salaryTypeID");
            entity.Property(e => e.HourlyRate)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("hourlyRate");
            entity.Property(e => e.MaxMonthlyHours).HasColumnName("maxMonthlyHours");

            entity.HasOne(d => d.SalaryType).WithOne(p => p.HourlySalaryType)
                .HasForeignKey<HourlySalaryType>(d => d.SalaryTypeId)
                .HasConstraintName("FK__HourlySal__salar__4D5F7D71");
        });

        modelBuilder.Entity<Hradministrator>(entity =>
        {
            entity.HasKey(e => e.EmployeeId).HasName("PK__HRAdmini__C134C9A1D098C5B9");

            entity.ToTable("HRAdministrator");

            entity.Property(e => e.EmployeeId)
                .ValueGeneratedNever()
                .HasColumnName("employeeID");
            entity.Property(e => e.ApprovalLevel).HasColumnName("approvalLevel");
            entity.Property(e => e.DocumentValidationRights)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("documentValidationRights");
            entity.Property(e => e.RecordAccessScope)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("recordAccessScope");

            entity.HasOne(d => d.Employee).WithOne(p => p.Hradministrator)
                .HasForeignKey<Hradministrator>(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__HRAdminis__emplo__59063A47");
        });

        modelBuilder.Entity<Insurance>(entity =>
        {
            entity.HasKey(e => e.InsuranceId).HasName("PK__Insuranc__79D82EF0CFA9D2F1");

            entity.ToTable("Insurance");

            entity.Property(e => e.InsuranceId).HasColumnName("insuranceID");
            entity.Property(e => e.ContributionRate)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("contributionRate");
            entity.Property(e => e.Coverage)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("coverage");
        });

        modelBuilder.Entity<InternshipContract>(entity =>
        {
            entity.HasKey(e => e.ContractId).HasName("PK__Internsh__13820961ADF6A95B");

            entity.ToTable("InternshipContract");

            entity.Property(e => e.ContractId)
                .ValueGeneratedNever()
                .HasColumnName("contractID");
            entity.Property(e => e.Evaluation)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("evaluation");
            entity.Property(e => e.Mentoring)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("mentoring");

            entity.HasOne(d => d.Contract).WithOne(p => p.InternshipContract)
                .HasForeignKey<InternshipContract>(d => d.ContractId)
                .HasConstraintName("FK__Internshi__contr__02084FDA");
        });

        modelBuilder.Entity<LatenessPolicy>(entity =>
        {
            entity.HasKey(e => e.PayrollPolicyId).HasName("PK__Lateness__CD123DEC1B408A2E");

            entity.ToTable("LatenessPolicy");

            entity.Property(e => e.PayrollPolicyId)
                .ValueGeneratedNever()
                .HasColumnName("payrollPolicyID");
            entity.Property(e => e.DeductionRate)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("deductionRate");
            entity.Property(e => e.GracePeriodMins).HasColumnName("gracePeriodMins");

            entity.HasOne(d => d.PayrollPolicy).WithOne(p => p.LatenessPolicy)
                .HasForeignKey<LatenessPolicy>(d => d.PayrollPolicyId)
                .HasConstraintName("FK__LatenessP__payro__40F9A68C");
        });

        modelBuilder.Entity<LeaveDocument>(entity =>
        {
            entity.HasKey(e => e.DocumentId).HasName("PK__LeaveDoc__EFAAADE5ADE6F258");

            entity.ToTable("LeaveDocument");

            entity.Property(e => e.DocumentId).HasColumnName("documentID");
            entity.Property(e => e.FilePath)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("filePath");
            entity.Property(e => e.LeaveRequestId).HasColumnName("leaveRequestID");
            entity.Property(e => e.UploadedAt)
                .HasPrecision(0)
                .HasColumnName("uploadedAt");

            entity.HasOne(d => d.LeaveRequest).WithMany(p => p.LeaveDocuments)
                .HasForeignKey(d => d.LeaveRequestId)
                .HasConstraintName("documents_request");
        });

        modelBuilder.Entity<LeaveEntitlement>(entity =>
        {
            entity.HasKey(e => new { e.EmployeeId, e.LeaveTypeId }).HasName("PK__LeaveEnt__5DDB6A884156C8E9");

            entity.ToTable("LeaveEntitlement");

            entity.Property(e => e.EmployeeId).HasColumnName("employeeID");
            entity.Property(e => e.LeaveTypeId).HasColumnName("leaveTypeID");
            entity.Property(e => e.Entitlement).HasColumnName("entitlement");

            entity.HasOne(d => d.Employee).WithMany(p => p.LeaveEntitlements)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("entitlement_for_employee");

            entity.HasOne(d => d.LeaveType).WithMany(p => p.LeaveEntitlements)
                .HasForeignKey(d => d.LeaveTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("entitlement_of_leave");
        });

        modelBuilder.Entity<LeavePolicy>(entity =>
        {
            entity.HasKey(e => e.LeavePolicyId).HasName("PK__LeavePol__B6B4A843A8117AA9");

            entity.ToTable("LeavePolicy");

            entity.Property(e => e.LeavePolicyId).HasColumnName("leavePolicyID");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.NoticePeriod).HasColumnName("noticePeriod");
            entity.Property(e => e.Purpose)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("purpose");
            entity.Property(e => e.ResetOnNewYear).HasColumnName("resetOnNewYear");
            entity.Property(e => e.SpecialLeaveType)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("specialLeaveType");
        });

        modelBuilder.Entity<LeaveRequest>(entity =>
        {
            entity.HasKey(e => e.LeaveRequestId).HasName("PK__LeaveReq__D3CE25020FADE640");

            entity.ToTable("LeaveRequest");

            entity.Property(e => e.LeaveRequestId).HasColumnName("leaveRequestID");
            entity.Property(e => e.ApprovalTiming).HasColumnName("approvalTiming");
            entity.Property(e => e.Duration).HasColumnName("duration");
            entity.Property(e => e.EmployeeId).HasColumnName("employeeID");
            entity.Property(e => e.Justification)
                .IsUnicode(false)
                .HasColumnName("justification");
            entity.Property(e => e.LeaveTypeId).HasColumnName("leaveTypeID");
            entity.Property(e => e.StartDate).HasColumnName("startDate");
            entity.Property(e => e.MedicalCertification)
                .IsUnicode(false)
                .HasColumnName("medicalCertification");
            entity.Property(e => e.PhysicianName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("physicianName");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("status");

            entity.HasOne(d => d.Employee).WithMany(p => p.LeaveRequests)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("is_requested_by");

            entity.HasOne(d => d.LeaveType).WithMany(p => p.LeaveRequests)
                .HasForeignKey(d => d.LeaveTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("specifies_leave_type");
        });

        modelBuilder.Entity<LeaveType>(entity =>
        {
            entity.HasKey(e => e.LeaveTypeId).HasName("PK__LeaveTyp__CEFA32982D3EA28A");

            entity.ToTable("LeaveType");

            entity.Property(e => e.LeaveTypeId).HasColumnName("leaveTypeID");
            entity.Property(e => e.Description)
                .IsUnicode(false)
                .HasColumnName("description");
            entity.Property(e => e.TypeName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("typeName");

            entity.HasMany(d => d.LeavePolicies).WithMany(p => p.LeaveTypes)
                .UsingEntity<Dictionary<string, object>>(
                    "AdheresToPolicy",
                    r => r.HasOne<LeavePolicy>().WithMany()
                        .HasForeignKey("LeavePolicyId")
                        .HasConstraintName("FK__adheres_t__leave__078C1F06"),
                    l => l.HasOne<LeaveType>().WithMany()
                        .HasForeignKey("LeaveTypeId")
                        .HasConstraintName("FK__adheres_t__leave__0697FACD"),
                    j =>
                    {
                        j.HasKey("LeaveTypeId", "LeavePolicyId").HasName("PK__adheres___E591781C76D74388");
                        j.ToTable("adheres_to_policy");
                        j.IndexerProperty<int>("LeaveTypeId").HasColumnName("leaveTypeID");
                        j.IndexerProperty<int>("LeavePolicyId").HasColumnName("leavePolicyID");
                    });
        });

        modelBuilder.Entity<LineManager>(entity =>
        {
            entity.HasKey(e => e.EmployeeId).HasName("PK__LineMana__C134C9A14D5BF2E3");

            entity.ToTable("LineManager");

            entity.Property(e => e.EmployeeId)
                .ValueGeneratedNever()
                .HasColumnName("employeeID");
            entity.Property(e => e.ApprovalLimit)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("approvalLimit");
            entity.Property(e => e.SupervisedDepartments)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("supervisedDepartments");
            entity.Property(e => e.TeamSize).HasColumnName("teamSize");

            entity.HasOne(d => d.Employee).WithOne(p => p.LineManager)
                .HasForeignKey<LineManager>(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__LineManag__emplo__619B8048");
        });

        modelBuilder.Entity<ManagerNote>(entity =>
        {
            entity.HasKey(e => e.NoteId).HasName("PK__ManagerN__03C97EDD53B14BA0");

            entity.Property(e => e.NoteId).HasColumnName("noteID");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasColumnName("createdAt");
            entity.Property(e => e.EmployeeId).HasColumnName("employeeID");
            entity.Property(e => e.ManagerId).HasColumnName("managerID");
            entity.Property(e => e.NoteContent)
                .IsUnicode(false)
                .HasColumnName("noteContent");

            entity.HasOne(d => d.Employee).WithMany(p => p.ManagerNoteEmployees)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ManagerNo__emplo__6477ECF3");

            entity.HasOne(d => d.Manager).WithMany(p => p.ManagerNoteManagers)
                .HasForeignKey(d => d.ManagerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ManagerNo__manag__656C112C");
        });

        modelBuilder.Entity<Mission>(entity =>
        {
            entity.HasKey(e => e.MissionId).HasName("PK__Mission__B3CFE5BBE960C28D");

            entity.ToTable("Mission");

            entity.Property(e => e.MissionId).HasColumnName("missionID");
            entity.Property(e => e.Destination)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("destination");
            entity.Property(e => e.EmployeeId).HasColumnName("employeeID");
            entity.Property(e => e.EndDate).HasColumnName("endDate");
            entity.Property(e => e.ManagerId).HasColumnName("managerID");
            entity.Property(e => e.StartDate).HasColumnName("startDate");
            entity.Property(e => e.Status)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("status");

            entity.HasOne(d => d.Employee).WithMany(p => p.MissionEmployees)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("is_performed_by");

            entity.HasOne(d => d.Manager).WithMany(p => p.MissionManagers)
                .HasForeignKey(d => d.ManagerId)
                .HasConstraintName("is_supervised_by");
        });

        modelBuilder.Entity<MonthlySalaryType>(entity =>
        {
            entity.HasKey(e => e.SalaryTypeId).HasName("PK__MonthlyS__65784932BC0E1D8B");

            entity.ToTable("MonthlySalaryType");

            entity.Property(e => e.SalaryTypeId)
                .ValueGeneratedNever()
                .HasColumnName("salaryTypeID");
            entity.Property(e => e.ContributionScheme)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("contributionScheme");
            entity.Property(e => e.TaxRule).HasColumnName("taxRule");

            entity.HasOne(d => d.SalaryType).WithOne(p => p.MonthlySalaryType)
                .HasForeignKey<MonthlySalaryType>(d => d.SalaryTypeId)
                .HasConstraintName("FK__MonthlySa__salar__503BEA1C");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__Notifica__4BA5CE893229C264");

            entity.ToTable("Notification");

            entity.Property(e => e.NotificationId).HasColumnName("notificationID");
            entity.Property(e => e.MessageContent)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("messageContent");
            entity.Property(e => e.ReadStatus)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("readStatus");
            entity.Property(e => e.Timestamp)
                .HasPrecision(0)
                .HasColumnName("timestamp");
            entity.Property(e => e.Type)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("type");
            entity.Property(e => e.Urgency)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("urgency");
        });

        modelBuilder.Entity<OvertimePolicy>(entity =>
        {
            entity.HasKey(e => e.PayrollPolicyId).HasName("PK__Overtime__CD123DEC0CBC3234");

            entity.ToTable("OvertimePolicy");

            entity.Property(e => e.PayrollPolicyId)
                .ValueGeneratedNever()
                .HasColumnName("payrollPolicyID");
            entity.Property(e => e.MaxHoursPerMonth).HasColumnName("maxHoursPerMonth");
            entity.Property(e => e.WeekdayRateMultiplier)
                .HasColumnType("decimal(4, 2)")
                .HasColumnName("weekdayRateMultiplier");
            entity.Property(e => e.WeekendRateMultiplier)
                .HasColumnType("decimal(4, 2)")
                .HasColumnName("weekendRateMultiplier");

            entity.HasOne(d => d.PayrollPolicy).WithOne(p => p.OvertimePolicy)
                .HasForeignKey<OvertimePolicy>(d => d.PayrollPolicyId)
                .HasConstraintName("FK__OvertimeP__payro__3E1D39E1");
        });

        modelBuilder.Entity<PartTimeContract>(entity =>
        {
            entity.HasKey(e => e.ContractId).HasName("PK__PartTime__138209612748B027");

            entity.ToTable("PartTimeContract");

            entity.Property(e => e.ContractId)
                .ValueGeneratedNever()
                .HasColumnName("contractID");
            entity.Property(e => e.HourlyRate)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("hourlyRate");
            entity.Property(e => e.WorkingHours).HasColumnName("workingHours");

            entity.HasOne(d => d.Contract).WithOne(p => p.PartTimeContract)
                .HasForeignKey<PartTimeContract>(d => d.ContractId)
                .HasConstraintName("FK__PartTimeC__contr__7C4F7684");
        });

        modelBuilder.Entity<PayGrade>(entity =>
        {
            entity.HasKey(e => e.PayGrade1).HasName("PK__PayGrade__C5F108C9FC99EFA0");

            entity.ToTable("PayGrade");

            entity.Property(e => e.PayGrade1)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("payGrade");
            entity.Property(e => e.GradeName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("gradeName");
            entity.Property(e => e.MaxSalary)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("maxSalary");
            entity.Property(e => e.MinSalary)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("minSalary");
        });

        modelBuilder.Entity<Payroll>(entity =>
        {
            entity.HasKey(e => e.PayrollId).HasName("PK__Payroll__EBDFA71A8F6FFC02");

            entity.ToTable("Payroll");

            entity.Property(e => e.PayrollId).HasColumnName("payrollID");
            entity.Property(e => e.ActualPay)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("actualPay");
            entity.Property(e => e.Adjustments)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("adjustments");
            entity.Property(e => e.BaseAmount)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("baseAmount");
            entity.Property(e => e.Contributions)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("contributions");
            entity.Property(e => e.EmployeeId).HasColumnName("employeeID");
            entity.Property(e => e.NetSalary)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("netSalary");
            entity.Property(e => e.PaymentDate).HasColumnName("paymentDate");
            entity.Property(e => e.PeriodEnd).HasColumnName("periodEnd");
            entity.Property(e => e.PeriodStart).HasColumnName("periodStart");
            entity.Property(e => e.Taxes)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("taxes");

            entity.HasOne(d => d.Employee).WithMany(p => p.Payrolls)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("compensates_employee");

            entity.HasMany(d => d.PayrollPolicies).WithMany(p => p.Payrolls)
                .UsingEntity<Dictionary<string, object>>(
                    "ChecksPayrollPolicy",
                    r => r.HasOne<PayrollPolicy>().WithMany()
                        .HasForeignKey("PayrollPolicyId")
                        .HasConstraintName("FK__checks_pa__payro__2334397B"),
                    l => l.HasOne<Payroll>().WithMany()
                        .HasForeignKey("PayrollId")
                        .HasConstraintName("FK__checks_pa__payro__22401542"),
                    j =>
                    {
                        j.HasKey("PayrollId", "PayrollPolicyId").HasName("PK__checks_p__370E84C4F1042CF4");
                        j.ToTable("checks_payroll_policy");
                        j.IndexerProperty<int>("PayrollId").HasColumnName("payrollID");
                        j.IndexerProperty<int>("PayrollPolicyId").HasColumnName("payrollPolicyID");
                    });
        });

        modelBuilder.Entity<PayrollLog>(entity =>
        {
            entity.HasKey(e => e.PayrollLogId).HasName("PK__PayrollL__1CC69F1FDFC62F3D");

            entity.ToTable("PayrollLog");

            entity.Property(e => e.PayrollLogId).HasColumnName("payrollLogID");
            entity.Property(e => e.Actor).HasColumnName("actor");
            entity.Property(e => e.ChangeDate)
                .HasPrecision(0)
                .HasColumnName("changeDate");
            entity.Property(e => e.ModificationType)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("modificationType");
            entity.Property(e => e.PayrollId).HasColumnName("payrollID");

            entity.HasOne(d => d.ActorNavigation).WithMany(p => p.PayrollLogs)
                .HasForeignKey(d => d.Actor)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("audited_by_employee");

            entity.HasOne(d => d.Payroll).WithMany(p => p.PayrollLogs)
                .HasForeignKey(d => d.PayrollId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("audits_payroll");
        });

        modelBuilder.Entity<PayrollPeriod>(entity =>
        {
            entity.HasKey(e => e.PayrollPeriodId).HasName("PK__PayrollP__E1F03571A25909DD");

            entity.ToTable("PayrollPeriod");

            entity.Property(e => e.PayrollPeriodId).HasColumnName("payrollPeriodID");
            entity.Property(e => e.EndDate).HasColumnName("endDate");
            entity.Property(e => e.PayrollId).HasColumnName("payrollID");
            entity.Property(e => e.StartDate).HasColumnName("startDate");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("status");

            entity.HasOne(d => d.Payroll).WithMany(p => p.PayrollPeriods)
                .HasForeignKey(d => d.PayrollId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__PayrollPe__payro__367C1819");
        });

        modelBuilder.Entity<PayrollPolicy>(entity =>
        {
            entity.HasKey(e => e.PayrollPolicyId).HasName("PK__PayrollP__CD123DEC437C0D8C");

            entity.ToTable("PayrollPolicy");

            entity.Property(e => e.PayrollPolicyId).HasColumnName("payrollPolicyID");
            entity.Property(e => e.ApprovedBy).HasColumnName("approvedBy");
            entity.Property(e => e.Description)
                .IsUnicode(false)
                .HasColumnName("description");
            entity.Property(e => e.EffectiveDate).HasColumnName("effectiveDate");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("status");

            entity.HasOne(d => d.ApprovedByNavigation).WithMany(p => p.PayrollPolicies)
                .HasForeignKey(d => d.ApprovedBy)
                .HasConstraintName("FK__PayrollPo__appro__3B40CD36");
        });

        modelBuilder.Entity<PayrollSpecialist>(entity =>
        {
            entity.HasKey(e => e.EmployeeId).HasName("PK__PayrollS__C134C9A1ED6D0C66");

            entity.ToTable("PayrollSpecialist");

            entity.Property(e => e.EmployeeId)
                .ValueGeneratedNever()
                .HasColumnName("employeeID");
            entity.Property(e => e.AssignedRegion)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("assignedRegion");
            entity.Property(e => e.LastProcessedPeriod)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("lastProcessedPeriod");
            entity.Property(e => e.ProcessingFrequency)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("processingFrequency");

            entity.HasOne(d => d.Employee).WithOne(p => p.PayrollSpecialist)
                .HasForeignKey<PayrollSpecialist>(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PayrollSp__emplo__5EBF139D");
        });

        modelBuilder.Entity<Position>(entity =>
        {
            entity.HasKey(e => e.PositionId).HasName("PK__Position__B07CF58E0ABE4272");

            entity.ToTable("Position");

            entity.HasIndex(e => e.Title, "UQ__Position__E52A1BB3059EBE87").IsUnique();

            entity.Property(e => e.PositionId).HasColumnName("positionID");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("status");
            entity.Property(e => e.Title)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("title");
        });

        modelBuilder.Entity<PossessesSkill>(entity =>
        {
            entity.HasKey(e => new { e.EmployeeId, e.SkillId }).HasName("PK__possesse__2BD26F1CF036B5E8");

            entity.ToTable("possesses_skill");

            entity.Property(e => e.EmployeeId).HasColumnName("employeeID");
            entity.Property(e => e.SkillId).HasColumnName("skillID");
            entity.Property(e => e.ProficiencyLevel).HasColumnName("proficiencyLevel");

            entity.HasOne(d => d.Employee).WithMany(p => p.PossessesSkills)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("FK__possesses__emplo__7755B73D");

            entity.HasOne(d => d.Skill).WithMany(p => p.PossessesSkills)
                .HasForeignKey(d => d.SkillId)
                .HasConstraintName("FK__possesses__skill__7849DB76");
        });

        modelBuilder.Entity<ProbationLeaveType>(entity =>
        {
            entity.HasKey(e => e.LeaveTypeId).HasName("PK__Probatio__CEFA329850B272B2");

            entity.ToTable("ProbationLeaveType");

            entity.Property(e => e.LeaveTypeId)
                .ValueGeneratedNever()
                .HasColumnName("leaveTypeID");
            entity.Property(e => e.StandardProbationDays).HasColumnName("standardProbationDays");

            entity.HasOne(d => d.LeaveType).WithOne(p => p.ProbationLeaveType)
                .HasForeignKey<ProbationLeaveType>(d => d.LeaveTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Probation__leave__245D67DE");
        });

        modelBuilder.Entity<ReceivesNotification>(entity =>
        {
            entity.HasKey(e => new { e.EmployeeId, e.NotificationId }).HasName("PK__receives__458E954977C9565D");

            entity.ToTable("receives_notification");

            entity.Property(e => e.EmployeeId).HasColumnName("employeeID");
            entity.Property(e => e.NotificationId).HasColumnName("notificationID");
            entity.Property(e => e.DeliveredAt)
                .HasPrecision(0)
                .HasColumnName("deliveredAt");
            entity.Property(e => e.DeliveryStatus)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("deliveryStatus");

            entity.HasOne(d => d.Employee).WithMany(p => p.ReceivesNotifications)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__receives___emplo__0F2D40CE");

            entity.HasOne(d => d.Notification).WithMany(p => p.ReceivesNotifications)
                .HasForeignKey(d => d.NotificationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__receives___notif__10216507");
        });

        modelBuilder.Entity<Reimbursement>(entity =>
        {
            entity.HasKey(e => e.ReimbursementId).HasName("PK__Reimburs__16A69D7144EAD7C4");

            entity.ToTable("Reimbursement");

            entity.Property(e => e.ReimbursementId).HasColumnName("reimbursementID");
            entity.Property(e => e.ApprovalDate).HasColumnName("approvalDate");
            entity.Property(e => e.ClaimType)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("claimType");
            entity.Property(e => e.CurrentStatus)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("currentStatus");
            entity.Property(e => e.EmployeeId).HasColumnName("employeeID");
            entity.Property(e => e.Type)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("type");

            entity.HasOne(d => d.Employee).WithMany(p => p.Reimbursements)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("reimburses_employee");
        });

        modelBuilder.Entity<ReplacementRequest>(entity =>
        {
            entity.HasKey(e => e.RequestId).HasName("PK__Replacem__E3C5DE5173E2EB13");

            entity.ToTable("ReplacementRequest");

            entity.Property(e => e.RequestId).HasColumnName("requestID");
            entity.Property(e => e.EmployeeId).HasColumnName("employeeID");
            entity.Property(e => e.Reason)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("reason");
            entity.Property(e => e.RequestDate)
                .HasPrecision(0)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("requestDate");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("Pending")
                .HasColumnName("status");

            entity.HasOne(d => d.Employee).WithMany(p => p.ReplacementRequests)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("FK__Replaceme__emplo__160F4887");
        });

        modelBuilder.Entity<Responsibility>(entity =>
        {
            entity.HasKey(e => e.ResponsibilityId).HasName("PK__Responsi__3B37D3BD6EA4B773");

            entity.ToTable("Responsibility");

            entity.Property(e => e.ResponsibilityId).HasColumnName("responsibilityID");
            entity.Property(e => e.PositionId).HasColumnName("positionID");
            entity.Property(e => e.Responsibility1)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("responsibility");

            entity.HasOne(d => d.Position).WithMany(p => p.Responsibilities)
                .HasForeignKey(d => d.PositionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Responsib__posit__73BA3083");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Role__CD98460A9263F858");

            entity.ToTable("Role");

            entity.Property(e => e.RoleId).HasColumnName("roleID");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.Purpose)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("purpose");
        });

        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.HasKey(e => e.RolePermissionId).HasName("PK__RolePerm__30F7E09C364A2C19");

            entity.ToTable("RolePermission");

            entity.HasIndex(e => new { e.RoleId, e.PermissionName }, "UQ__RolePerm__0A9E27E4B9438A5E").IsUnique();

            entity.Property(e => e.RolePermissionId).HasColumnName("rolePermissionID");
            entity.Property(e => e.AllowedAction)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("allowedAction");
            entity.Property(e => e.PermissionName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("permissionName");
            entity.Property(e => e.RoleId).HasColumnName("roleID");

            entity.HasOne(d => d.Role).WithMany(p => p.RolePermissions)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK__RolePermi__roleI__6B24EA82");
        });

        modelBuilder.Entity<SalaryType>(entity =>
        {
            entity.HasKey(e => e.SalaryTypeId).HasName("PK__SalaryTy__65784932B28658F9");

            entity.ToTable("SalaryType");

            entity.Property(e => e.SalaryTypeId).HasColumnName("salaryTypeID");
            entity.Property(e => e.CurrencyId).HasColumnName("currencyID");
            entity.Property(e => e.PaymentFrequency).HasColumnName("paymentFrequency");

            entity.HasOne(d => d.Currency).WithMany(p => p.SalaryTypes)
                .HasForeignKey(d => d.CurrencyId)
                .HasConstraintName("is_paid_in");
        });

        modelBuilder.Entity<ShiftConfiguration>(entity =>
        {
            entity.HasKey(e => e.ShiftConfigurationId).HasName("PK__ShiftCon__37D4DD90BE67C447");

            entity.ToTable("ShiftConfiguration");

            entity.HasIndex(e => e.ShiftType, "UQ__ShiftCon__8447359248AAA17C").IsUnique();

            entity.Property(e => e.ShiftConfigurationId).HasColumnName("shiftConfigurationID");
            entity.Property(e => e.AllowanceAmount)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("allowanceAmount");
            entity.Property(e => e.LastUpdatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("lastUpdatedAt");
            entity.Property(e => e.LastUpdatedBy).HasColumnName("lastUpdatedBy");
            entity.Property(e => e.ShiftType)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("shiftType");

            entity.HasOne(d => d.LastUpdatedByNavigation).WithMany(p => p.ShiftConfigurations)
                .HasForeignKey(d => d.LastUpdatedBy)
                .HasConstraintName("FK__ShiftConf__lastU__123EB7A3");
        });

        modelBuilder.Entity<ShiftSchedule>(entity =>
        {
            entity.HasKey(e => e.ShiftId).HasName("PK__ShiftSch__F2F06B22FB0178EF");

            entity.ToTable("ShiftSchedule");

            entity.Property(e => e.ShiftId).HasColumnName("shiftID");
            entity.Property(e => e.BreakDuration).HasColumnName("breakDuration");
            entity.Property(e => e.EndTime)
                .HasPrecision(0)
                .HasColumnName("endTime");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.ShiftDate).HasColumnName("shiftDate");
            entity.Property(e => e.StartTime)
                .HasPrecision(0)
                .HasColumnName("startTime");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("status");
            entity.Property(e => e.Type)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("type");
        });

        modelBuilder.Entity<SickLeaveType>(entity =>
        {
            entity.HasKey(e => e.LeaveTypeId).HasName("PK__SickLeav__CEFA3298764268FE");

            entity.ToTable("SickLeaveType");

            entity.Property(e => e.LeaveTypeId)
                .ValueGeneratedNever()
                .HasColumnName("leaveTypeID");
            entity.Property(e => e.RequiresPhysicianCert).HasColumnName("requiresPhysicianCert");

            entity.HasOne(d => d.LeaveType).WithOne(p => p.SickLeaveType)
                .HasForeignKey<SickLeaveType>(d => d.LeaveTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SickLeave__leave__2739D489");
        });

        modelBuilder.Entity<SigningBonu>(entity =>
        {
            entity.HasKey(e => e.SigningBonusId).HasName("PK__SigningB__08216CE7EBFB6FC8");

            entity.Property(e => e.SigningBonusId).HasColumnName("signingBonusID");
            entity.Property(e => e.BonusAmount)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("bonusAmount");
            entity.Property(e => e.EffectiveDate).HasColumnName("effectiveDate");
            entity.Property(e => e.EmployeeId).HasColumnName("employeeID");

            entity.HasOne(d => d.Employee).WithMany(p => p.SigningBonus)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("FK__SigningBo__emplo__534D60F1");
        });

        modelBuilder.Entity<SigningBonusConfiguration>(entity =>
        {
            entity.HasKey(e => e.ConfigId).HasName("PK__SigningB__3FEDA8C6D7E1566F");

            entity.ToTable("SigningBonusConfiguration");

            entity.Property(e => e.ConfigId).HasColumnName("configID");
            entity.Property(e => e.BonusType)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("bonusType");
            entity.Property(e => e.DefaultAmount)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("defaultAmount");
            entity.Property(e => e.EligibilityCriteria).HasColumnName("eligibilityCriteria");
            entity.Property(e => e.LastUpdated)
                .HasPrecision(0)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("lastUpdated");
        });

        modelBuilder.Entity<Skill>(entity =>
        {
            entity.HasKey(e => e.SkillId).HasName("PK__Skill__AE6A6BDF04BF03C5");

            entity.ToTable("Skill");

            entity.HasIndex(e => e.Name, "UQ__Skill__72E12F1B360F8979").IsUnique();

            entity.Property(e => e.SkillId).HasColumnName("skillID");
            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("name");
        });

        modelBuilder.Entity<StipendFeature>(entity =>
        {
            entity.HasKey(e => e.StipendFeatureId).HasName("PK__StipendF__3A3325E30E6D389E");

            entity.ToTable("StipendFeature");

            entity.Property(e => e.StipendFeatureId).HasColumnName("stipendFeatureID");
            entity.Property(e => e.Feature)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("feature");
            entity.Property(e => e.InternshipContractId).HasColumnName("internshipContractID");

            entity.HasOne(d => d.InternshipContract).WithMany(p => p.StipendFeatures)
                .HasForeignKey(d => d.InternshipContractId)
                .HasConstraintName("FK__StipendFe__inter__04E4BC85");
        });

        modelBuilder.Entity<SystemAdministrator>(entity =>
        {
            entity.HasKey(e => e.EmployeeId).HasName("PK__SystemAd__C134C9A13D75A3D0");

            entity.ToTable("SystemAdministrator");

            entity.Property(e => e.EmployeeId)
                .ValueGeneratedNever()
                .HasColumnName("employeeID");
            entity.Property(e => e.AuditVisibilityScope)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("auditVisibilityScope");
            entity.Property(e => e.ConfigurableFields)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("configurableFields");
            entity.Property(e => e.SystemPrivilegeLevel)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("systemPrivilegeLevel");

            entity.HasOne(d => d.Employee).WithOne(p => p.SystemAdministrator)
                .HasForeignKey<SystemAdministrator>(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SystemAdm__emplo__5BE2A6F2");
        });

        modelBuilder.Entity<TaxForm>(entity =>
        {
            entity.HasKey(e => e.TaxFormId).HasName("PK__TaxForm__5535AA836F0C0F43");

            entity.ToTable("TaxForm");

            entity.Property(e => e.TaxFormId).HasColumnName("taxFormID");
            entity.Property(e => e.FormContent)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("formContent");
            entity.Property(e => e.Jurisdiction)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("jurisdiction");
            entity.Property(e => e.ValidityEnd).HasColumnName("validityEnd");
            entity.Property(e => e.ValidityStart).HasColumnName("validityStart");
        });

        modelBuilder.Entity<Termination>(entity =>
        {
            entity.HasKey(e => e.TerminationId).HasName("PK__Terminat__5FF8C70255FAD02F");

            entity.ToTable("Termination");

            entity.Property(e => e.TerminationId).HasColumnName("terminationID");
            entity.Property(e => e.ContractId).HasColumnName("contractID");
            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.Reason)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("reason");

            entity.HasOne(d => d.Contract).WithMany(p => p.Terminations)
                .HasForeignKey(d => d.ContractId)
                .HasConstraintName("terminates_contract");
        });

        modelBuilder.Entity<TerminationBenefit>(entity =>
        {
            entity.HasKey(e => e.BenefitId).HasName("PK__Terminat__50D7FCB4FB05A19E");

            entity.ToTable("TerminationBenefit");

            entity.Property(e => e.BenefitId).HasColumnName("benefitID");
            entity.Property(e => e.CompensationAmount)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("compensationAmount");
            entity.Property(e => e.EffectiveDate).HasColumnName("effectiveDate");
            entity.Property(e => e.Reason)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("reason");
            entity.Property(e => e.TerminationId).HasColumnName("terminationID");

            entity.HasOne(d => d.Termination).WithMany(p => p.TerminationBenefits)
                .HasForeignKey(d => d.TerminationId)
                .HasConstraintName("FK__Terminati__termi__6166761E");
        });

        modelBuilder.Entity<VacationLeaveType>(entity =>
        {
            entity.HasKey(e => e.LeaveTypeId).HasName("PK__Vacation__CEFA3298E2F4BC93");

            entity.ToTable("VacationLeaveType");

            entity.Property(e => e.LeaveTypeId)
                .ValueGeneratedNever()
                .HasColumnName("leaveTypeID");
            entity.Property(e => e.MaxCarryOverDays).HasColumnName("maxCarryOverDays");
            entity.Property(e => e.RequiresManagerApproval).HasColumnName("requiresManagerApproval");

            entity.HasOne(d => d.LeaveType).WithOne(p => p.VacationLeaveType)
                .HasForeignKey<VacationLeaveType>(d => d.LeaveTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__VacationL__leave__2180FB33");
        });

        modelBuilder.Entity<Verification>(entity =>
        {
            entity.HasKey(e => e.VerificationId).HasName("PK__Verifica__613CC8AA697E9CBD");

            entity.ToTable("Verification");

            entity.Property(e => e.VerificationId).HasColumnName("verificationID");
            entity.Property(e => e.ExpiryPeriod).HasColumnName("expiryPeriod");
            entity.Property(e => e.IssueDate).HasColumnName("issueDate");
            entity.Property(e => e.Issuer)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("issuer");
            entity.Property(e => e.Type)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("type");
        });

        modelBuilder.Entity<WorksShift>(entity =>
        {
            entity.HasKey(e => e.AssignmentId).HasName("PK__works_sh__52C218C09CDBED35");

            entity.ToTable("works_shift");

            entity.Property(e => e.AssignmentId).HasColumnName("assignmentID");
            entity.Property(e => e.EmployeeId).HasColumnName("employeeID");
            entity.Property(e => e.EndDate).HasColumnName("endDate");
            entity.Property(e => e.ShiftId).HasColumnName("shiftID");
            entity.Property(e => e.StartDate).HasColumnName("startDate");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("status");

            entity.HasOne(d => d.Employee).WithMany(p => p.WorksShifts)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("FK__works_shi__emplo__7EF6D905");

            entity.HasOne(d => d.Shift).WithMany(p => p.WorksShifts)
                .HasForeignKey(d => d.ShiftId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__works_shi__shift__7FEAFD3E");
        });

        modelBuilder.Entity<AttendanceRule>(entity =>
        {
            entity.HasKey(e => e.RuleName).HasName("PK_AttendanceRule");

            entity.ToTable("AttendanceRule");

            entity.Property(e => e.RuleName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("ruleName");

            entity.Property(e => e.Value)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("value");

            entity.Property(e => e.NumericValue).HasColumnName("numericValue");

            entity.Property(e => e.LastUpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("lastUpdatedAt");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
