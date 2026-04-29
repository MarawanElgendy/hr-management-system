-- ANY EXTRA TABLES THAT WERE IN THE SCHEMA BUT NOT PROJECT DESCRIPTION
CREATE TABLE PayGrade (
    payGrade VARCHAR(50) PRIMARY KEY,
    gradeName VARCHAR(50),
    minSalary DECIMAL(12, 2),
    maxSalary DECIMAL(12, 2)
);

CREATE TABLE Currency (
    currencyID INT PRIMARY KEY IDENTITY,
    currencyName VARCHAR(50) UNIQUE
);

SELECT employeeID
    FROM Employee;

-- EMPLOYEE AND ITS SUBTYPES

CREATE TABLE Employee (
    employeeID INT IDENTITY PRIMARY KEY,
    firstName VARCHAR(200) NOT NULL,
    middleName VARCHAR(200),
    lastName VARCHAR(200) NOT NULL,
    fullName AS firstName + ' ' + ISNULL(middleName + ' ', '') + lastName,
    nationalID VARCHAR(50) UNIQUE NOT NULL,
    birthDate DATE NOT NULL,
    age AS DATEDIFF(YEAR, birthDate, GETDATE()),
    birthCountry VARCHAR(100),
    phoneNumber VARCHAR(50) UNIQUE NOT NULL,
    emailAddress VARCHAR(100) UNIQUE NOT NULL,
    address VARCHAR(255),
    emergencyContactName VARCHAR(100),
    emergencyContactPhone VARCHAR(50),
    relationship VARCHAR(50),
    biography VARCHAR(MAX),
    profileImage VARCHAR(MAX),
    employmentProgress VARCHAR(100),
    accountstatus VARCHAR(50),
    employmentStatus VARCHAR(50),
    hireDate DATE,
    isActive BIT NOT NULL,
    profileCompletion INT,
    departmentID INT,
    positionID INT,
    managerID INT,
    contractID INT,
    taxFormID INT,
    salaryTypeID INT,
    payGrade VARCHAR(50),
    type VARCHAR(20)
);

CREATE TABLE SigningBonus (
    -- this is a multi-value attrbite of Employee
    signingBonusID INT PRIMARY KEY IDENTITY,
    employeeID INT FOREIGN KEY REFERENCES Employee(employeeID),
    bonusAmount DECIMAL(12, 2) NOT NULL,
    effectiveDate DATE NOT NULL
);

CREATE TABLE SigningBonusConfiguration (
    configID INT PRIMARY KEY IDENTITY,
    bonusType VARCHAR(50) NOT NULL, -- 'Fixed' or 'Percentage'
    defaultAmount DECIMAL(10, 2) NOT NULL,
    eligibilityCriteria NVARCHAR(MAX),
    lastUpdated DATETIME2(0) DEFAULT GETDATE()
);

CREATE TABLE HRAdministrator (
    employeeID INT PRIMARY KEY FOREIGN KEY REFERENCES Employee(employeeID),
    approvalLevel INTEGER,
    recordAccessScope VARCHAR(100),
    documentValidationRights VARCHAR(100)
);

CREATE TABLE SystemAdministrator (
    employeeID INT PRIMARY KEY FOREIGN KEY REFERENCES Employee(employeeID),
    systemPrivilegeLevel VARCHAR(100),
    configurableFields VARCHAR(255),
    auditVisibilityScope VARCHAR(255)
);

CREATE TABLE PayrollSpecialist (
    employeeID INT PRIMARY KEY FOREIGN KEY REFERENCES Employee(employeeID),
    assignedRegion VARCHAR(100),
    processingFrequency VARCHAR(50),
    lastProcessedPeriod VARCHAR(50)
);

CREATE TABLE LineManager (
    employeeID INT PRIMARY KEY FOREIGN KEY REFERENCES Employee(employeeID),
    teamSize INT,
    supervisedDepartments VARCHAR(255),
    approvalLimit DECIMAL(10,2)
);

CREATE TABLE ManagerNotes (
    -- this is a multi-value attribute for LineManager
    noteID INT PRIMARY KEY IDENTITY,
    employeeID INT NOT NULL FOREIGN KEY REFERENCES Employee(employeeID),
    managerID INT NOT NULL FOREIGN KEY REFERENCES Employee(employeeID),
    noteContent VARCHAR(MAX),
    createdAt DATETIME2(0)
);

-- ROLE

CREATE TABLE Role (
    roleID INT PRIMARY KEY IDENTITY,
    name VARCHAR(100) NOT NULL,
    purpose VARCHAR(500)
);

CREATE TABLE RolePermission (
    -- this is a multi-value attribute of Role
    rolePermissionID INT PRIMARY KEY IDENTITY,
    roleID INT NOT NULL FOREIGN KEY REFERENCES Role(roleID) ON DELETE CASCADE,
    permissionName VARCHAR(100) NOT NULL,
    allowedAction VARCHAR(50),
    UNIQUE (roleID, permissionName)
);

-- DEPARTMENT

CREATE TABLE Department (
    departmentID INT PRIMARY KEY IDENTITY,
    name VARCHAR(100) NOT NULL UNIQUE,
    purpose VARCHAR(500),
    headID INT -- should be NULL during data entry, then modify after adding employees
);

-- POSITION

CREATE TABLE Position (
    positionID INT PRIMARY KEY IDENTITY,
    title VARCHAR(100) NOT NULL UNIQUE,
    status VARCHAR(50)
);

CREATE TABLE Responsibility (
    -- this is a multi-value attribute of Position
    responsibilityID INT PRIMARY KEY IDENTITY,
    positionID INT NOT NULL FOREIGN KEY REFERENCES Position(positionID),
    responsibility VARCHAR(100)
);

-- CONTRACT AND ITS SUBTYPES

CREATE TABLE Contract (
    contractID INT PRIMARY KEY IDENTITY,
    type VARCHAR(50) NOT NULL,
    startDate DATE NOT NULL,
    endDate DATE,
    currentState VARCHAR(50),
    employeeID INT UNIQUE,
    insuranceID INT
);

CREATE TABLE FullTimeContract (
    contractID INT PRIMARY KEY FOREIGN KEY REFERENCES Contract(contractID) ON DELETE CASCADE,
    leaveEntitlement INT,
    insuranceEligibility BIT,
    weeklyWorkingHours INT
);

CREATE TABLE PartTimeContract (
    contractID INT PRIMARY KEY FOREIGN KEY REFERENCES Contract(contractID) ON DELETE CASCADE,
    workingHours INT,
    hourlyRate DECIMAL(12,2)
);

CREATE TABLE ConsultantContract (
    contractID INT PRIMARY KEY FOREIGN KEY REFERENCES Contract(contractID) ON DELETE CASCADE,
    projectScope VARCHAR(50),
    fees DECIMAL(12,2),
    paymentSchedule VARCHAR(100)
);

CREATE TABLE InternshipContract (
    contractID INT PRIMARY KEY FOREIGN KEY REFERENCES Contract(contractID) ON DELETE CASCADE,
    mentoring VARCHAR(500),
    evaluation VARCHAR(500)
);

CREATE TABLE StipendFeature (
    -- this is a multi-value attribute for InternshipContract
    internshipContractID INT NOT NULL FOREIGN KEY REFERENCES InternshipContract(contractID) ON DELETE CASCADE,
    stipendFeatureID INT PRIMARY KEY IDENTITY,
    feature VARCHAR(100) NOT NULL
);

-- SKILL

CREATE TABLE Skill (
    skillID INT PRIMARY KEY IDENTITY,
    name VARCHAR(100) NOT NULL UNIQUE,
    description VARCHAR(500)
);

-- VERIFICATION

CREATE TABLE Verification (
    verificationID INT PRIMARY KEY IDENTITY,
    type VARCHAR(50),
    issuer VARCHAR(50),
    issueDate DATE,
    expiryPeriod INT
);

-- Attendance Correction Request
CREATE TABLE AttendanceCorrectionRequest (
    requestID INT PRIMARY KEY IDENTITY,
    employeeID INT NOT NULL FOREIGN KEY REFERENCES Employee(employeeID),
    date DATE NOT NULL,
    correctionType VARCHAR(100),
    reason VARCHAR(255),
    status VARCHAR(50),
    recordedBy INT
);


-- SHIFT SCHEDULE

CREATE TABLE ShiftSchedule (
    shiftID INT PRIMARY KEY IDENTITY,
    name VARCHAR(50),
    type VARCHAR(50),
    startTime DATETIME2(0) NOT NULL,
    endTime DATETIME2(0) NOT NULL,
    breakDuration TIME NOT NULL,
    shiftDate DATE NOT NULL,
    status VARCHAR(50)
);

CREATE TABLE ShiftConfiguration (
    shiftConfigurationID INT PRIMARY KEY IDENTITY,
    shiftType VARCHAR(50) NOT NULL UNIQUE,
    allowanceAmount DECIMAL(10, 2) NOT NULL,
    lastUpdatedBy INT FOREIGN KEY REFERENCES Employee(employeeID),
    lastUpdatedAt DATETIME2(0) DEFAULT GETDATE()
);

CREATE TABLE ReplacementRequest (
    requestID INT PRIMARY KEY IDENTITY,
    employeeID INT FOREIGN KEY REFERENCES Employee(employeeID),
    reason VARCHAR(150),
    requestDate DATETIME2(0) DEFAULT GETDATE(),
    status VARCHAR(50) DEFAULT 'Pending'
);

-- ATTENDANCE

CREATE TABLE Attendance (
    attendanceID INT PRIMARY KEY IDENTITY,
    entryTime DATETIME2(0),
    exitTime DATETIME2(0),
    durationMinutes AS DATEDIFF(minute, entryTime, exitTime),
    employeeID INT NOT NULL,
    shiftID INT
);

-- LEAVE TYPE AND ITS SUBTYPES

CREATE TABLE LeaveType (
    leaveTypeID INT PRIMARY KEY IDENTITY,
    typeName VARCHAR(50) NOT NULL,
    description VARCHAR(MAX)
);

CREATE TABLE HolidayLeaveType (
    leaveTypeID INT PRIMARY KEY FOREIGN KEY REFERENCES LeaveType(leaveTypeID),
    regionalScope VARCHAR(50),
    officialRecognition VARCHAR(50),
    holidayName VARCHAR(50)
);

CREATE TABLE VacationLeaveType (
    leaveTypeID INT PRIMARY KEY FOREIGN KEY REFERENCES LeaveType(leaveTypeID),
    maxCarryOverDays INT NOT NULL,
    requiresManagerApproval BIT NOT NULL
);

CREATE TABLE ProbationLeaveType (
    leaveTypeID INT PRIMARY KEY FOREIGN KEY REFERENCES LeaveType(leaveTypeID),
    standardProbationDays INT NOT NULL
);

CREATE TABLE SickLeaveType (
    leaveTypeID INT PRIMARY KEY FOREIGN KEY REFERENCES LeaveType(leaveTypeID),
    requiresPhysicianCert BIT NOT NULL
);

-- LEAVE REQUEST

CREATE TABLE LeaveRequest (
    leaveRequestID INT PRIMARY KEY IDENTITY,
    employeeID INT NOT NULL,
    leaveTypeID INT NOT NULL,
    justification VARCHAR(MAX) NOT NULL,
    duration INT NOT NULL,
    physicianName VARCHAR(100),
    medicalCertification VARCHAR(MAX),
    approvalTiming INT,
    status VARCHAR(50)
);

-- LEAVE ENTITLEMENT
CREATE TABLE LeaveEntitlement (
    employeeID INT NOT NULL,
    leaveTypeID INT NOT NULL,
    entitlement INT NOT NULL,

    PRIMARY KEY (employeeID, leaveTypeID)
);

-- uploaded documentation supporting a leave request
CREATE TABLE LeaveDocument (
    documentID INT PRIMARY KEY IDENTITY,
    leaveRequestID INT NOT NULL,
    filePath VARCHAR(255),
    uploadedAt DATETIME2(0)
);

-- LEAVE POLICY

CREATE TABLE LeavePolicy (
    leavePolicyID INT PRIMARY KEY IDENTITY,
    name VARCHAR(50),
    purpose VARCHAR(100),
    noticePeriod INT NOT NULL,
    specialLeaveType VARCHAR(50),
    resetOnNewYear BIT
);

CREATE TABLE EligibilityRule (
    -- this is a multi-value attribute of LeavePolicy
    leavePolicyID INT FOREIGN KEY REFERENCES LeavePolicy(leavePolicyID),
    eligibilityRuleID INT PRIMARY KEY IDENTITY,
    eligibilityRule VARCHAR(100)
);

-- PAYROLL

CREATE TABLE Payroll (
    payrollID INT PRIMARY KEY IDENTITY,
    employeeID INT,
    taxes DECIMAL(12, 2) NOT NULL,
    periodStart DATE,
    periodEnd DATE,
    baseAmount DECIMAL(12, 2) NOT NULL,
    adjustments VARCHAR(50),
    contributions DECIMAL(12, 2) NOT NULL,
    actualPay DECIMAL(12, 2) NOT NULL,
    netSalary DECIMAL(12, 2) NOT NULL,
    paymentDate DATE
);

-- PAYROLL PERIOD

CREATE TABLE PayrollPeriod (
    payrollPeriodID INT PRIMARY KEY IDENTITY,
    payrollID INT FOREIGN KEY REFERENCES Payroll(payrollID) ON DELETE CASCADE,
    startDate DATE,
    endDate DATE,
    status VARCHAR(50)
);

-- ALLOWANCE AND DEDUCTION

CREATE TABLE AllowanceAndDeduction (
    allowanceAndDeductionID INT PRIMARY KEY IDENTITY,
    payrollID INT,
    employeeID INT,
    amount DECIMAL(12, 2) NOT NULL,
    duration INT NOT NULL,
    timeZone INT,
    currencyID INT
);

-- PAYROLL POLICY AND ITS SUBTYPES

CREATE TABLE PayrollPolicy (
    payrollPolicyID INT PRIMARY KEY IDENTITY,
    effectiveDate DATE,
    description VARCHAR(MAX),
    status VARCHAR(50),
    approvedBy INT FOREIGN KEY REFERENCES Employee(employeeID)
);

CREATE TABLE OvertimePolicy (
    payrollPolicyID INT PRIMARY KEY FOREIGN KEY REFERENCES PayrollPolicy(payrollPolicyID) ON DELETE CASCADE,
    weekdayRateMultiplier DECIMAL(4,2) NOT NULL,
    weekendRateMultiplier DECIMAL(4,2) NOT NULL,
    maxHoursPerMonth INT NOT NULL
);

CREATE TABLE LatenessPolicy (
    payrollPolicyID INT PRIMARY KEY FOREIGN KEY REFERENCES PayrollPolicy(payrollPolicyID) ON DELETE CASCADE,
    gracePeriodMins INT NOT NULL,
    deductionRate DECIMAL(5,2) NOT NULL
);

CREATE TABLE BonusPolicy (
    payrollPolicyID INT PRIMARY KEY FOREIGN KEY REFERENCES PayrollPolicy(payrollPolicyID) ON DELETE CASCADE,
    bonusType VARCHAR(50),
    eligibiltyCriteria INT NOT NULL
);

CREATE TABLE DeductionPolicy (
    payrollPolicyID INT PRIMARY KEY FOREIGN KEY REFERENCES PayrollPolicy(payrollPolicyID) ON DELETE CASCADE,
    deductionReasons VARCHAR(50),
    calculationMode BIT NOT NULL
);

-- INSURANCE

CREATE TABLE Insurance (
    insuranceID INT PRIMARY KEY IDENTITY,
    contributionRate DECIMAL(5,2) NOT NULL,
    coverage VARCHAR(50)
);

-- SALARY TYPE AND ITS SUBTYPES

CREATE TABLE SalaryType (
    salaryTypeID INT PRIMARY KEY IDENTITY,
    paymentFrequency INT,
    currencyID INT
);

CREATE TABLE HourlySalaryType (
    salaryTypeID INT PRIMARY KEY FOREIGN KEY REFERENCES SalaryType(salaryTypeID) ON DELETE CASCADE,
    hourlyRate DECIMAL(12,2) NOT NULL,
    maxMonthlyHours INT
);

CREATE TABLE MonthlySalaryType (
    salaryTypeID INT PRIMARY KEY FOREIGN KEY REFERENCES SalaryType(salaryTypeID) ON DELETE CASCADE,
    taxRule INT NOT NULL,
    contributionScheme VARCHAR(50)
);

CREATE TABLE ContractSalaryType (
    salaryTypeID INT PRIMARY KEY FOREIGN KEY REFERENCES SalaryType(salaryTypeID) ON DELETE CASCADE,
    contractValue DECIMAL(12,2) NOT NULL,
    installment VARCHAR(50)
);

-- TAX FORM

CREATE TABLE TaxForm (
    taxFormID INT PRIMARY KEY IDENTITY,
    jurisdiction VARCHAR(255),
    formContent VARCHAR(255),
    validityStart DATE,
    validityEnd DATE
);

-- NOTIFICATION

CREATE TABLE Notification (
    notificationID INT PRIMARY KEY IDENTITY,
    messageContent VARCHAR(255),
    timestamp DATETIME2(0),
    urgency VARCHAR(255),
    readStatus VARCHAR(255),
    type VARCHAR(50)
);

-- MISSION

CREATE TABLE Mission (
    missionID INT PRIMARY KEY IDENTITY,
    destination VARCHAR(255),
    employeeID INT,
    managerID INT,
    startDate date,
    endDate date,
    status VARCHAR(255)
);

-- EXCEPTION

CREATE TABLE Exception (
    exceptionID INT PRIMARY KEY IDENTITY,
    name VARCHAR(255),
    category VARCHAR(255),
    date DATE,
    status VARCHAR(255)
);

-- REIMBURSEMENT

CREATE TABLE Reimbursement (
    reimbursementID INT PRIMARY KEY IDENTITY,
    type VARCHAR(100),
    claimType VARCHAR(100),
    approvalDate DATE,
    currentStatus VARCHAR(100),
    employeeID INT
);

-- TERMINATION

CREATE TABLE Termination (
    terminationID INT PRIMARY KEY IDENTITY,
    date DATE,
    reason VARCHAR(100),
    contractID INT
);

CREATE TABLE TerminationBenefit (
    -- this is a multi-value attribute for Termination
    benefitID INT PRIMARY KEY IDENTITY,
    terminationID INT FOREIGN KEY REFERENCES Termination(terminationID),
    compensationAmount DECIMAL(10,2) NOT NULL,
    effectiveDate DATE NOT NULL,
    reason VARCHAR(50) NOT NULL
);

-- DEVICE

CREATE TABLE Device (
    deviceID INT PRIMARY KEY IDENTITY,
    deviceType VARCHAR(50),
    terminalID VARCHAR(100),
    latitude DECIMAL(9,6),
    longitude DECIMAL(9,6),
    employeeID INT
);

-- PAYROLL LOG

CREATE TABLE PayrollLog (
    payrollLogID INT PRIMARY KEY IDENTITY,
    payrollID INT,
    actor INT,
    changeDate DATETIME2(0),
    modificationType VARCHAR(100)
);

-- ATTENDANCE LOG

CREATE TABLE AttendanceLog (
    attendanceLogID INT PRIMARY KEY IDENTITY,
    attendanceID INT,
    actor INT,
    timestamp DATETIME2(0),
    reason VARCHAR(100)
);

-- additional tracking information for attendance captured by devices
CREATE TABLE AttendanceSource (
    attendanceID INT NOT NULL,
    deviceID INT NOT NULL,
    sourceType VARCHAR(50),
    latitude DECIMAL(9,6),
    longitude DECIMAL(9,6),
    recordedAt DATETIME2(0),

    PRIMARY KEY (attendanceID, deviceID)
);

-- ESTABLISHING RELATIONSHIPS BETWEEN TABLES

-- this is a one-to-many relationship between Employee and Department (one department has many employees)
ALTER TABLE Employee
    ADD CONSTRAINT works_for FOREIGN KEY(departmentID) REFERENCES Department(departmentID) ON DELETE SET NULL;

-- this is a one-to-many relationship between Employee and Position (multiple employees can have the same position)
ALTER TABLE Employee
    ADD CONSTRAINT holds_position FOREIGN KEY(positionID) REFERENCES Position(positionID) ON DELETE NO ACTION;

-- this is a one-to-many relationship between Employee and LineManager (one line manager manages many employees)
ALTER TABLE Employee
    ADD CONSTRAINT reports_to FOREIGN KEY(managerID) REFERENCES Employee(employeeID) ON DELETE NO ACTION;

-- this is a one-to-one relationship between Employee and Contract (each employee has one contract)
ALTER TABLE Employee
    ADD CONSTRAINT covers_employee FOREIGN KEY(contractID) REFERENCES Contract(contractID) ON DELETE CASCADE;

-- this is a one-to-many relationship between Employee and TaxForm (one tax form configuration can apply to many employees)
ALTER TABLE Employee
    ADD CONSTRAINT files_tax_form FOREIGN KEY(taxFormID) REFERENCES TaxForm(taxFormID) ON DELETE SET NULL;

-- this is a one-to-many relationship between Employee and SalaryType (one salary type can be assigned to many employees)
ALTER TABLE Employee
    ADD CONSTRAINT earns_salary_type FOREIGN KEY(salaryTypeID) REFERENCES SalaryType(salaryTypeID);

-- this is a one-to-many relationship between Employee and PayGrade (one pay grade can apply to many employees)
ALTER TABLE Employee
    ADD CONSTRAINT fits_pay_grade FOREIGN KEY(payGrade) REFERENCES PayGrade(payGrade);

-- this is a many-to-many relationship between Employee and Role
CREATE TABLE fulfills_role (
    employeeID INT NOT NULL FOREIGN KEY REFERENCES Employee(employeeID) ON DELETE CASCADE,
    roleID INT NOT NULL FOREIGN KEY REFERENCES Role(roleID) ON DELETE CASCADE,
    assignedDate DATETIME2(0) NOT NULL,

    PRIMARY KEY (employeeID, roleID)
);

-- this is a one-to-one relationship between Employee and Department (each department has one head)
ALTER TABLE Department
    ADD CONSTRAINT is_managed_by FOREIGN KEY(headID) REFERENCES Employee(employeeID) ON DELETE SET NULL;

-- this is a many-to-many relationship between Employee and Skill
CREATE TABLE possesses_skill (
    employeeID  INT NOT NULL FOREIGN KEY REFERENCES Employee(employeeID) ON DELETE CASCADE,
    skillID INT NOT NULL FOREIGN KEY REFERENCES Skill(skillID) ON DELETE CASCADE,
    proficiencyLevel INT,

    PRIMARY KEY (employeeID, skillID)
);

-- this is a many-to-many relationship between Employee and Verification
CREATE TABLE undergoes_verification (
    employeeID INT FOREIGN KEY REFERENCES Employee(employeeID) ON DELETE CASCADE,
    verificationID INT FOREIGN KEY REFERENCES Verification(verificationID) ON DELETE CASCADE,

    PRIMARY KEY (employeeID, verificationID)
);

-- this is a many-to-many relationship between Employee and ShiftSchedule
CREATE TABLE works_shift (
    assignmentID INT PRIMARY KEY IDENTITY,
    employeeID INT NOT NULL FOREIGN KEY REFERENCES Employee(employeeID) ON DELETE CASCADE,
    shiftID INT FOREIGN KEY REFERENCES ShiftSchedule(shiftID) ON DELETE SET NULL,
    startDate DATE NOT NULL,
    endDate DATE NOT NULL,
    status VARCHAR(50)
);

-- this is a one-to-many relationship between Employee and Attendance (each employee has multiple attendance records)
ALTER TABLE Attendance
    ADD CONSTRAINT logs_presence_of FOREIGN KEY(employeeID) REFERENCES Employee(employeeID) ON DELETE NO ACTION;

-- this is a one-to-many relationship between Attendance and ShiftSchedule (each shift schedule has multiple attendance records)
ALTER TABLE Attendance
ADD CONSTRAINT adheres_to_shift FOREIGN KEY(shiftID) REFERENCES ShiftSchedule(shiftID) ON DELETE SET NULL;

-- this is a one-to-many relationship between Employee and LeaveRequest (one employee can submit many requests)
ALTER TABLE LeaveRequest
    ADD CONSTRAINT is_requested_by FOREIGN KEY(employeeID) REFERENCES Employee(employeeID) ON DELETE NO ACTION;

-- this is a one-to-many relationship between LeaveType and LeaveRequest (one leave type can appear in many requests)
ALTER TABLE LeaveRequest
    ADD CONSTRAINT specifies_leave_type FOREIGN KEY(leaveTypeID) REFERENCES LeaveType(leaveTypeID);

-- this is a many-to-many relationship between LeaveType and LeavePolicy (multiple leave types can have the same policies)
CREATE TABLE adheres_to_policy (
    leaveTypeID INT FOREIGN KEY REFERENCES LeaveType(leaveTypeID) ON DELETE CASCADE,
    leavePolicyID INT FOREIGN KEY REFERENCES LeavePolicy(leavePolicyID) ON DELETE CASCADE,

    PRIMARY KEY(leaveTypeID, leavePolicyID)
);

-- this is a one-to-many relationship between Payroll and Employee (one employee has multiple payroll records)
ALTER TABLE Payroll
    ADD CONSTRAINT compensates_employee FOREIGN KEY(employeeID) REFERENCES Employee(employeeID) ON DELETE SET NULL;

-- this is a one-to-many relationship between AllowanceAndDeduction and Payroll (one payroll record contains multiple allowances or deductions)
ALTER TABLE AllowanceAndDeduction
    ADD CONSTRAINT modifies_payroll FOREIGN KEY(payrollID) REFERENCES Payroll(payrollID) ON DELETE CASCADE;

-- this is a one-to-many relationship between AllowanceAndDeduction and Employee (one employee has multiple allowance or deduction records)
ALTER TABLE AllowanceAndDeduction
    ADD CONSTRAINT affects_employee FOREIGN KEY(employeeID) REFERENCES Employee(employeeID) ON DELETE SET NULL;

-- this is a one-to-many relationship between AllowanceAndDeduction and Currency (one currency is used for multiple allowances or deductions)
ALTER TABLE AllowanceAndDeduction
    ADD CONSTRAINT uses_currency FOREIGN KEY(currencyID) REFERENCES Currency(currencyID) ON DELETE NO ACTION;

-- this is a one-to-many relationship between SalaryType and Currency (one currency applies to multiple salary types)
ALTER TABLE SalaryType
    ADD CONSTRAINT is_paid_in FOREIGN KEY(currencyID) REFERENCES Currency(currencyID) ON DELETE NO ACTION;

-- this is a many-to-many relationship between Employee and Notification (multiple employees can recieve the same notification)
CREATE TABLE receives_notification (
    employeeID INT FOREIGN KEY REFERENCES Employee(employeeID),
    notificationID INT FOREIGN KEY REFERENCES Notification(notificationID),
    deliveryStatus varchar(50),
    deliveredAt DATETIME2(0),

    PRIMARY KEY (employeeID, notificationID)
);

-- this is a one-to-many relationship between Employee and itself (organizational hierarchy)
CREATE TABLE establishes_hierarchy (
    employeeID INT NOT NULL FOREIGN KEY REFERENCES Employee(employeeID),
    managerID INT FOREIGN KEY REFERENCES Employee(employeeID),
    hierarchyLevel INT NOT NULL,

    PRIMARY KEY (employeeID, hierarchyLevel)
);

-- this is a one-to-many relationship between Mission and Employee (one employee performs multiple missions)
ALTER TABLE Mission
    ADD CONSTRAINT is_performed_by FOREIGN KEY(employeeID) REFERENCES Employee(employeeID) ON DELETE SET NULL;

-- this is a one-to-many relationship between Mission and Employee (one employee supervises multiple missions)
ALTER TABLE Mission
    ADD CONSTRAINT is_supervised_by FOREIGN KEY(managerID) REFERENCES Employee(employeeID) ON DELETE NO ACTION;

-- this is a one-to-many relationship between Reimbursement and Employee (one employee creates multiple reimbursement requests)
ALTER TABLE Reimbursement
    ADD CONSTRAINT reimburses_employee FOREIGN KEY (employeeID) REFERENCES Employee(employeeID) ON DELETE SET NULL;

-- this is a one-to-one relationship between Termination and Contract (each termination record corresponds to a specific contract)
ALTER TABLE Termination
    ADD CONSTRAINT terminates_contract FOREIGN KEY (contractID) REFERENCES Contract(contractID) ON DELETE NO ACTION;

-- this is a one-to-many relationship between PayrollLog and Payroll (one payroll record can have multiple log entries)
ALTER TABLE PayrollLog
    ADD CONSTRAINT audits_payroll FOREIGN KEY (payrollID) REFERENCES Payroll(payrollID) ON DELETE CASCADE;

-- this is a one-to-many relationship between AttendanceLog and Attendance (one attendance record can have multiple log entries)
ALTER TABLE AttendanceLog
    ADD CONSTRAINT tracks_change_in FOREIGN KEY (attendanceID) REFERENCES Attendance(attendanceID) ON DELETE CASCADE;

-- add actor FKs for logs
ALTER TABLE PayrollLog
    ADD CONSTRAINT audited_by_employee FOREIGN KEY (actor) REFERENCES Employee(employeeID) ON DELETE SET NULL;

ALTER TABLE AttendanceLog
    ADD CONSTRAINT acted_by_employee FOREIGN KEY (actor) REFERENCES Employee(employeeID) ON DELETE SET NULL;

-- this is a many-to-many relationship between Employee and Exception
CREATE TABLE excused (
    employeeID INT NOT NULL FOREIGN KEY REFERENCES Employee(employeeID) ON DELETE CASCADE,
    exceptionID INT NOT NULL FOREIGN KEY REFERENCES Exception(exceptionID) ON DELETE CASCADE,

    PRIMARY KEY (employeeID, exceptionID)
);

-- this is a many-to-many relationship between Payroll and PayrollPolicy
CREATE TABLE checks_payroll_policy (
    payrollID INT NOT NULL FOREIGN KEY REFERENCES Payroll(payrollID) ON DELETE CASCADE,
    payrollPolicyID INT NOT NULL FOREIGN KEY REFERENCES PayrollPolicy(payrollPolicyID) ON DELETE CASCADE,

    PRIMARY KEY (payrollID, payrollPolicyID)
);

-- this is a one-to-many relationship between Employee and LeaveEntitlement
ALTER TABLE LeaveEntitlement
    ADD CONSTRAINT entitlement_for_employee FOREIGN KEY(employeeID) REFERENCES Employee(employeeID) ON DELETE CASCADE;

-- this is a one-to-many relationship between LeaveType and LeaveEntitlement
ALTER TABLE LeaveEntitlement
    ADD CONSTRAINT entitlement_of_leave FOREIGN KEY(leaveTypeID) REFERENCES LeaveType(leaveTypeID) ON DELETE NO ACTION;

-- this is a one-to-many relationship between LeaveRequest and LeaveDocument
ALTER TABLE LeaveDocument
    ADD CONSTRAINT documents_request FOREIGN KEY(leaveRequestID) REFERENCES LeaveRequest(leaveRequestID) ON DELETE CASCADE;

-- this is a one-to-many relationship between Employee and AttendanceCorrectionRequest (submits correction)
ALTER TABLE AttendanceCorrectionRequest
    ADD CONSTRAINT submitted_by FOREIGN KEY(employeeID) REFERENCES Employee(employeeID);

-- this is a one-to-many relationship between Employee and AttendanceCorrectionRequest (recorded by)
ALTER TABLE AttendanceCorrectionRequest
    ADD CONSTRAINT recorded_by FOREIGN KEY(recordedBy) REFERENCES Employee(employeeID);

-- this is a one-to-many relationship between Employee and Device
ALTER TABLE Device
    ADD CONSTRAINT device_belongs_to FOREIGN KEY(employeeID) REFERENCES Employee(employeeID) ON DELETE SET NULL;

-- this is a one-to-many relationship between Attendance and AttendanceSource
ALTER TABLE AttendanceSource
    ADD CONSTRAINT has_source FOREIGN KEY(attendanceID) REFERENCES Attendance(attendanceID) ON DELETE CASCADE;

-- this is a one-to-many relationship between Device and AttendanceSource
ALTER TABLE AttendanceSource
    ADD CONSTRAINT recorded_from FOREIGN KEY(deviceID) REFERENCES Device(deviceID) ON DELETE NO ACTION;

-- this is a one-to-many relationship between Contract and Insurance
ALTER TABLE Contract
    ADD CONSTRAINT insured FOREIGN KEY(insuranceID) REFERENCES Insurance(insuranceID);
