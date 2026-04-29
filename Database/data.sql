-- ============================================
-- USER STORIES DATA SEED (FIXED UNIQUE CONSTRAINT)
-- ============================================

-- Disable ALL foreign key constraints
EXEC sp_MSforeachtable 'ALTER TABLE ? NOCHECK CONSTRAINT ALL';

-- Clear all tables
DELETE FROM works_shift;
DELETE FROM fulfills_role;
DELETE FROM receives_notification;
DELETE FROM possesses_skill;
DELETE FROM undergoes_verification;
DELETE FROM excused;
DELETE FROM establishes_hierarchy;
DELETE FROM checks_payroll_policy;
DELETE FROM adheres_to_policy;
DELETE FROM AttendanceSource;
DELETE FROM RolePermission;
DELETE FROM LeaveDocument;
DELETE FROM LeaveEntitlement;
DELETE FROM LeaveRequest;
DELETE FROM Attendance;
DELETE FROM AttendanceLog;
DELETE FROM AttendanceCorrectionRequest;
DELETE FROM Mission;
DELETE FROM ManagerNotes;
DELETE FROM Notification;
DELETE FROM Verification;
DELETE FROM Exception;
DELETE FROM Reimbursement;
DELETE FROM ReplacementRequest;
DELETE FROM PayrollLog;
DELETE FROM PayrollPeriod;
DELETE FROM Payroll;
DELETE FROM AllowanceAndDeduction;
DELETE FROM SigningBonus;
DELETE FROM TerminationBenefit;
DELETE FROM Termination;
DELETE FROM Device;
DELETE FROM Responsibility;
DELETE FROM EligibilityRule;
DELETE FROM ShiftConfiguration;
DELETE FROM ShiftSchedule;
DELETE FROM HRAdministrator;
DELETE FROM LineManager;
DELETE FROM SystemAdministrator;
DELETE FROM PayrollSpecialist;
DELETE FROM FullTimeContract;
DELETE FROM PartTimeContract;
DELETE FROM ConsultantContract;
DELETE FROM StipendFeature;
DELETE FROM InternshipContract;
DELETE FROM HolidayLeaveType;
DELETE FROM SickLeaveType;
DELETE FROM VacationLeaveType;
DELETE FROM ProbationLeaveType;
DELETE FROM MonthlySalaryType;
DELETE FROM HourlySalaryType;
DELETE FROM ContractSalaryType;
DELETE FROM OvertimePolicy;
DELETE FROM LatenessPolicy;
DELETE FROM BonusPolicy;
DELETE FROM DeductionPolicy;
DELETE FROM PayrollPolicy;
DELETE FROM PayrollPolicy;
DELETE FROM Department; -- Will fail if headID not null? No constraints disabled.
-- UPDATE Employee SET managerID = NULL, departmentID = NULL, positionID = NULL, contractID = NULL, taxFormID = NULL, salaryTypeID = NULL, payGrade = NULL;
-- UPDATE Department SET headID = NULL;
-- UPDATE Contract SET employeeID = NULL, insuranceID = NULL; 
DELETE FROM Employee;
DELETE FROM Contract;
DELETE FROM Insurance; -- Clear Insurance
DELETE FROM Position;
DELETE FROM Department;
DELETE FROM Role;
DELETE FROM LeavePolicy;
DELETE FROM LeaveType;
DELETE FROM TaxForm;
DELETE FROM SalaryType;
DELETE FROM PayGrade;
DELETE FROM SigningBonusConfiguration;
DELETE FROM Currency;
DELETE FROM Skill;

-- Reseed
DBCC CHECKIDENT ('Department', RESEED, 0);
DBCC CHECKIDENT ('Position', RESEED, 0);
DBCC CHECKIDENT ('Role', RESEED, 0);
DBCC CHECKIDENT ('Employee', RESEED, 0);
DBCC CHECKIDENT ('Contract', RESEED, 0);
DBCC CHECKIDENT ('Insurance', RESEED, 0);
DBCC CHECKIDENT ('LeaveType', RESEED, 0);
DBCC CHECKIDENT ('LeavePolicy', RESEED, 0);
DBCC CHECKIDENT ('ShiftSchedule', RESEED, 0);
DBCC CHECKIDENT ('Notification', RESEED, 0);
DBCC CHECKIDENT ('LeaveRequest', RESEED, 0);
DBCC CHECKIDENT ('Mission', RESEED, 0);

-- Departments
SET IDENTITY_INSERT Department ON;
INSERT INTO Department (departmentID, name, purpose, headID) VALUES
(1, 'Executive', 'Leadership', NULL),
(2, 'Human Resources', 'HR', NULL),
(3, 'Engineering', 'Dev', NULL),
(4, 'Finance', 'Finance', NULL),
(5, 'Marketing', 'Marketing', NULL),
(6, 'Operations', 'Ops', NULL);
SET IDENTITY_INSERT Department OFF;

-- Positions
SET IDENTITY_INSERT Position ON;
INSERT INTO Position (positionID, title, status) VALUES
(1, 'CEO', 'Active'), (2, 'HR Director', 'Active'), (3, 'HR Manager', 'Active'),
(5, 'Engineering Director', 'Active'), (6, 'Senior Developer', 'Active'),
(7, 'Software Developer', 'Active'), (8, 'Junior Developer', 'Active'),
(13, 'Operations Manager', 'Active'); -- For Fake/Good Manager
SET IDENTITY_INSERT Position OFF;

-- Roles
SET IDENTITY_INSERT Role ON;
INSERT INTO Role (roleID, name, purpose) VALUES
(1, 'SystemAdministrator', 'Admin'), (2, 'HRAdministrator', 'HR'), (3, 'LineManager', 'Manager'), (4, 'Employee', 'Basic');
SET IDENTITY_INSERT Role OFF;

-- Employees
SET IDENTITY_INSERT Employee ON;
INSERT INTO Employee (employeeID, firstName, middleName, lastName, nationalID, birthDate, birthCountry, phoneNumber, emailAddress, address, emergencyContactName, emergencyContactPhone, relationship, biography, employmentStatus, hireDate, isActive, IsFlagged, profileCompletion, departmentID, positionID, managerID, contractID) VALUES
(1, 'Alice', 'S', 'Admin', 'NAT001', '1980-01-01', 'USA', '+1000000001', 'admin@hrms.com', '1 Admin Way', 'Spouse', '+1999999999', 'Spouse', 'System Admin', 'Active', '2020-01-01', 1, 0, 100, 1, 1, 1, NULL),
(2, 'Harry', 'H', 'HR', 'NAT002', '1985-02-02', 'UK', '+1000000002', 'hr@hrms.com', '2 HR Lane', 'Partner', '+1999999998', 'Partner', 'HR Admin', 'Active', '2021-01-01', 1, 0, 100, 2, 2, 2, NULL),
(3, 'Dan', 'D', 'Director', 'NAT003', '1975-03-03', 'Canada', '+1000000003', 'director@hrms.com', '3 Dir Blvd', 'Spouse', '+1999999997', 'Spouse', 'Engineering Director', 'Active', '2019-01-01', 1, 0, 100, 3, 5, 1, NULL),
(4, 'Mary', 'M', 'Manager', 'NAT004', '1990-04-04', 'USA', '+1000000004', 'manager@hrms.com', '4 Mgr St', 'Parent', '+1999999996', 'Parent', 'Eng Manager', 'Active', '2022-01-01', 1, 0, 90, 3, 13, 3, NULL),
(5, 'Steve', 'S', 'Senior', 'NAT005', '1992-05-05', 'USA', '+1000000005', 'steve@hrms.com', '5 Dev Dr', 'Parent', '+1999999995', 'Parent', 'Senior Dev', 'Active', '2023-01-01', 1, 0, 80, 3, 6, 4, NULL),
(6, 'Joe', 'J', 'Junior', 'NAT006', '1998-06-06', 'USA', '+1000000006', 'joe@hrms.com', '6 Jun Ct', 'Parent', '+1999999994', 'Parent', 'Junior Dev', 'Active', '2024-01-01', 1, 0, 60, 3, 8, 4, NULL),
(7, 'Ian', 'I', 'Irregular', 'NAT007', '1993-07-07', 'USA', '+1000000007', 'ian@hrms.com', '7 Err Ln', 'Sibling', '+1999999993', 'Sibling', 'Irregular Dev', 'Active', '2023-06-01', 1, 1, 70, 3, 7, 4, NULL),
(8, 'Frank', 'F', 'Fake', 'NAT008', '1988-08-08', 'USA', '+1000000008', 'frank@hrms.com', '8 Fake Pl', 'Sibling', '+1999999992', 'Sibling', 'Fake Manager (Not Line Manager)', 'Active', '2022-05-01', 1, 0, 85, 3, 13, 3, NULL),
(12, 'Good', 'Y', 'Manager', 'NAT012', '1985-01-01', 'USA', '+1111111113', 'good.manager@company.com', '123 Good St', 'Contact', '+1111111114', 'Self', 'Good Manager (Line Manager)', 'Active', '2024-01-01', 1, 0, 100, 6, 13, 1, NULL);
SET IDENTITY_INSERT Employee OFF;

-- Insurance (To fix Unique Key Constraint on Contract.InsuranceID)
SET IDENTITY_INSERT Insurance ON;
INSERT INTO Insurance (insuranceId, contributionRate, coverage) VALUES
(1, 0.05, 'Basic'), (2, 0.05, 'Basic'), (3, 0.10, 'Premium'),
(4, 0.05, 'Basic'), (5, 0.05, 'Basic'), (6, 0.05, 'Basic'),
(7, 0.05, 'Basic'), (8, 0.05, 'Basic'), (12, 0.10, 'Premium');
SET IDENTITY_INSERT Insurance OFF;

-- Contracts
SET IDENTITY_INSERT Contract ON;
INSERT INTO Contract (contractID, type, startDate, endDate, currentState, employeeID, insuranceID) VALUES
(1, 'FullTime', '2020-01-01', '2030-12-31', 'Active', 1, 1),
(2, 'FullTime', '2021-01-01', '2030-12-31', 'Active', 2, 2),
(3, 'FullTime', '2019-01-01', '2029-12-31', 'Active', 3, 3),
(4, 'FullTime', '2022-01-01', '2027-12-31', 'Active', 4, 4),
(5, 'FullTime', '2023-01-01', '2028-12-31', 'Active', 5, 5),
(6, 'FullTime', '2024-01-01', '2025-01-01', 'Active', 6, 6),
(7, 'FullTime', '2023-06-01', '2028-06-01', 'Active', 7, 7),
(8, 'FullTime', '2022-05-01', '2027-04-30', 'Active', 8, 8),
(12, 'FullTime', '2024-01-01', '2029-01-01', 'Active', 12, 12);
SET IDENTITY_INSERT Contract OFF;

-- Update Employee Contracts
UPDATE Employee SET contractID = employeeID WHERE employeeID IN (1,2,3,4,5,6,7,8,12);

-- Update Dept Heads
UPDATE Department SET headID = 1 WHERE departmentID = 1;
UPDATE Department SET headID = 2 WHERE departmentID = 2;
UPDATE Department SET headID = 3 WHERE departmentID = 3;

-- Roles
INSERT INTO fulfills_role (employeeID, roleID, assignedDate) VALUES
(1, 1, '2020-01-01'), (1, 3, '2020-01-01'),
(2, 2, '2021-01-01'),
(3, 3, '2019-01-01'),
(4, 3, '2022-01-01'), (4, 4, '2022-01-01'),
(5, 4, '2023-01-01'), (6, 4, '2024-01-01'), (7, 4, '2023-06-01'), (8, 4, '2022-05-01'),
(12, 3, '2024-01-01'), (12, 4, '2024-01-01');

-- LineManagers
INSERT INTO LineManager (employeeID, teamSize, supervisedDepartments, approvalLimit) VALUES
(1, 10, 'All', 100000.00),
(3, 20, 'Engineering', 50000.00),
(4, 5, 'Engineering', 5000.00),
(12, 5, 'Operations', 50000.00);

-- Other Roles
INSERT INTO SystemAdministrator (employeeID, systemPrivilegeLevel, configurableFields, auditVisibilityScope) VALUES (1, 'Full', 'All', 'Global');
INSERT INTO HRAdministrator (employeeID, approvalLevel, recordAccessScope, documentValidationRights) VALUES (2, 2, 'All Employees', 'Full');

-- Hierarchy
INSERT INTO establishes_hierarchy (managerID, employeeID, hierarchyLevel) VALUES
(3, 4, 1), (3, 8, 1),
(4, 5, 2), (4, 6, 2), (4, 7, 2);

-- Leave Types
SET IDENTITY_INSERT LeaveType ON;
INSERT INTO LeaveType (leaveTypeID, typeName, description) VALUES
(1, 'Annual Leave', 'Standard'), (2, 'Sick Leave', 'Sick');
SET IDENTITY_INSERT LeaveType OFF;

-- Leave Policies
SET IDENTITY_INSERT LeavePolicy ON;
INSERT INTO LeavePolicy (leavePolicyID, name, purpose, noticePeriod, resetOnNewYear) VALUES
(1, 'Annual Pol', 'Desc', 7, 1);
SET IDENTITY_INSERT LeavePolicy OFF;

-- Leave Entitlement
INSERT INTO LeaveEntitlement (employeeID, leaveTypeID, entitlement) VALUES
(1,1,30), (2,1,25), (3,1,25), (4,1,20), (5,1,20), (6,1,15), (7,1,15), (8,1,20), (12,1,20);

-- Requests
SET IDENTITY_INSERT LeaveRequest ON;
INSERT INTO LeaveRequest (leaveRequestID, employeeID, leaveTypeID, justification, duration, startDate, status) VALUES
(1, 5, 1, 'Vacation', 5, '2025-06-01', 'Pending'),
(2, 6, 2, 'Sick', 2, '2025-01-10', 'Approved'),
(3, 7, 1, 'Irregular check', 3, '2025-03-01', 'Pending');
SET IDENTITY_INSERT LeaveRequest OFF;

-- Enable FKs
EXEC sp_MSforeachtable 'ALTER TABLE ? WITH CHECK CHECK CONSTRAINT ALL';

/*
================================================================================
DATA SEED DOCUMENTATION & USER STORIES
================================================================================

1. SYSTEM ADMINISTRATOR
   - Name: Alice Admin (ID: 1)
   - Login: admin@hrms.com
   - Role: System Admin + Line Manager (Head of Exec)
   - Capabilities: Full access to all system settings.

2. HR ADMINISTRATOR
   - Name: Harry HR (ID: 2)
   - Login: hr@hrms.com
   - Role: HR Administrator
   - Capabilities: Manage employees, contracts, policies.

3. HIERARCHY & LEAVE REQUEST TEST
   - Dan Director (ID: 3): Head of Engineering. Reports to no one (in this scope).
   - Mary Manager (ID: 4): Engineering Manager. Reports to Dan.
     - Has pending mission to "New York Office".
   - Steve Senior (ID: 5): Developer. Reports to Mary.
     - Has PENDING "Annual Leave" request (Verify Mary can see this).
   - Joe Junior (ID: 6): Developer. Reports to Mary. 
     - Has APPROVED "Sick Leave" request.
   - Ian Irregular (ID: 7): Developer. Reports to Mary.
     - IS FLAGGED (IsFlagged = 1). Verify "Unflag" option is visible to Mary.
     - Has PENDING "Annual Leave".

4. "MANAGER" DEFINITION TEST (Strict Line Manager Filtering)
   - Frank Fake (ID: 8):
     - Title: "Fake Manager (Not Line Manager)"
     - Is LineManager? NO.
     - Result: Should NOT appear in "Assign Mission" or "Edit Profile" manager dropdowns.
   - Good Manager (ID: 12):
     - Title: "Good Manager (Line Manager)"
     - Is LineManager? YES (in LineManager table).
     - Result: SHOULD appear in manager dropdowns.

================================================================================
*/
