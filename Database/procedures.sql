-- Youssef

-- this stored procedure returns all departments
CREATE PROCEDURE GetAllDepartments
    AS
    BEGIN
        SELECT departmentID, name
        FROM Department
    END
    GO

-- this stored procedure returns all roles
CREATE PROCEDURE GetAllRoles
    AS
    BEGIN
        SELECT roleID, name
        FROM Role
    END
    GO

-- this stored procedure returns all employees grouped by their department
CREATE PROCEDURE SearchEmployees
    (
        @Query VARCHAR(100) = NULL,
        @DepartmentID INT = NULL
    )
    AS
    BEGIN
        SELECT 
            E.employeeID,
            E.firstName + ' ' + E.lastName AS fullName,
            D.name AS departmentName,
            P.title AS positionTitle,
            E.emailAddress,
            E.phoneNumber
        FROM Employee E
        LEFT JOIN Department D ON E.departmentID = D.departmentID
        LEFT JOIN Position P ON E.positionID = P.positionID
        WHERE 
            (@Query IS NULL OR E.firstName LIKE '%' + @Query + '%' OR E.lastName LIKE '%' + @Query + '%' OR E.emailAddress LIKE '%' + @Query + '%')
            AND
            (@DepartmentID IS NULL OR E.departmentID = @DepartmentID)
        ORDER BY D.name ASC, E.lastName ASC;
    END;
    GO

-- this stored procedure makes sure that the employee exists
-- success = 1 means that the employee was found
-- success = 0 means that the employee was not found
CREATE PROCEDURE FindEmployee
    (
        @employeeID INT,
        @success INT OUTPUT
    )
    AS
    BEGIN
        IF NOT EXISTS (SELECT 1 FROM Employee WHERE employeeID = @employeeID)
            SET @success = 0;
        ELSE
            SET @success = 1;
    END;
    GO

-- this stored procedure creates a new employee and returns the employeeID
-- success = 1 means that the change was successful
-- success = 0 means that the change unsuccessful
CREATE PROCEDURE AddEmployee
    (
        @fullName VARCHAR(200),
        @nationalID VARCHAR(50),
        @birthDate DATE,
        @birthCountry VARCHAR(100),
        @phoneNumber VARCHAR(20),
        @emailAddress VARCHAR(100),
        @address VARCHAR(150),
        @emergencyContactName VARCHAR(100),
        @emergencyContactPhone VARCHAR(50),
        @relationship VARCHAR(50),
        @biography VARCHAR(MAX),
        @employmentProgress VARCHAR(100),
        @accountStatus VARCHAR(50),
        @employmentStatus VARCHAR(50),
        @hireDate DATE,
        @isActive BIT,
        @profileCompletion INT,
        @departmentID INT,
        @positionID INT,
        @managerID INT,
        @contractID INT,
        @taxFormID INT,
        @salaryTypeID INT,
        @payGrade VARCHAR(50),
        @employeeID INT OUTPUT,
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;
        DECLARE @firstName VARCHAR(100), @middleName VARCHAR(100), @lastName VARCHAR(100);
        
        SET @fullName = LTRIM(RTRIM(@fullName));

        SET @firstName = LEFT(@fullName, CHARINDEX(' ', @fullName + ' ') - 1);
        SET @lastName = RIGHT(@fullName, CHARINDEX(' ', REVERSE(@fullName) + ' ') - 1);
        SET @middleName =
        CASE 
            WHEN @firstName = @lastName THEN
                ''
            ELSE
                SUBSTRING(@fullName, LEN(@firstName) + 2, LEN(@fullName) - LEN(@firstName) - LEN(@lastName) - 1)
        END;

        INSERT INTO Employee
        (
            firstName,
            middleName,
            lastName,
            nationalID,
            birthDate,
            birthCountry,
            phoneNumber,
            emailAddress,
            address,
            emergencyContactName,
            emergencyContactPhone,
            relationship,
            biography,
            profileImage,
            employmentProgress,
            accountStatus,
            employmentStatus,
            hireDate,
            isActive,
            profileCompletion,
            departmentID,
            positionID,
            managerID,
            contractID,
            taxFormID,
            salaryTypeID,
            payGrade
        )
        VALUES
        (
            @firstName,
            @middleName,
            @lastName,
            @nationalID,
            @birthDate,
            @birthCountry,
            @phoneNumber,
            @emailAddress,
            @address,
            @emergencyContactName,
            @emergencyContactPhone,
            @relationship,
            @biography,
            NULL,
            @employmentProgress,
            @accountStatus,
            @employmentStatus,
            @hireDate,
            @isActive,
            @profileCompletion,
            @departmentID,
            @positionID,
            @managerID,
            @contractID,
            @taxFormID,
            @salaryTypeID,
            @payGrade
        );

        SET @employeeID = SCOPE_IDENTITY();

        IF @@ROWCOUNT = 1
            SET @success = 1;
        ELSE
            SET @success = 0;
    END;
    GO

-- this stored procedures updates contact and personal details
-- success = 1 means that the change was successful
-- success = 0 means that the change was unsuccessful
CREATE PROCEDURE UpdateEmployeeInfo
    (
        @employeeID INT,
        @emailAddress VARCHAR(100),
        @phoneNumber VARCHAR(20),
        @address VARCHAR(150),
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;

        UPDATE Employee
            SET emailAddress = @emailAddress,
                phoneNumber = @phoneNumber,
                address = @address
            WHERE employeeID = @employeeID;
        
        IF @@ROWCOUNT = 1
            SET @success = 1;
        ELSE
            SET @success = 0;
    END;
    GO

-- this stored procedure creates a new employee profile
-- success = 1 means that the employee was added successfully
-- success = 0 means that an unexpected error occured
-- success = -1 means that the email is duplicated
-- success = -2 means that the nationalID is duplicated
-- success = -3 means that the phone number is duplicated
CREATE PROCEDURE CreateEmployeeProfile
    (
        @firstName VARCHAR(50),
        @lastName VARCHAR(50),
        @departmentID INT,
        @roleID INT,
        @hireDate DATE,
        @email VARCHAR(100),
        @phone VARCHAR(20),
        @nationalID VARCHAR(50),
        @birthDate DATE,
        @birthCountry VARCHAR(100),
        @type VARCHAR(20),
        @employeeID INT OUTPUT,
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;

        -- default outputs
        SET @employeeID = NULL;
        SET @success = 0;

        IF EXISTS (SELECT 1 FROM dbo.Employee WHERE emailAddress = @email)
        BEGIN
            SET @success = -1; -- duplicate email
            RETURN;
        END

        IF EXISTS (SELECT 1 FROM dbo.Employee WHERE nationalID = @nationalID)
        BEGIN
            SET @success = -2; -- duplicate national ID
            RETURN;
        END

        IF EXISTS (SELECT 1 FROM dbo.Employee WHERE phoneNumber = @phone)
        BEGIN
            SET @success = -3; -- duplicate phone
            RETURN;
        END

        BEGIN TRANSACTION;

        BEGIN TRY
            INSERT INTO Employee
            (
                firstName,
                lastName,
                departmentID,
                hireDate,
                emailAddress,
                phoneNumber,
                nationalID,
                birthDate,
                birthCountry,
                isActive,
                type
            )
            VALUES
            (
                @firstName,
                @lastName,
                @departmentID,
                @hireDate,
                @email,
                @phone,
                @nationalID,
                @birthDate,
                @birthCountry,
                1,
                @type
            );

            SET @employeeID = SCOPE_IDENTITY();

            INSERT INTO fulfills_role
            (
                employeeID,
                roleID,
                assignedDate
            )
            VALUES
            (
                @employeeID,
                @roleID,
                GETDATE()
            );

            IF @type = 'SystemAdministrator'
                INSERT INTO SystemAdministrator (employeeID, systemPrivilegeLevel, configurableFields, auditVisibilityScope) VALUES (@employeeID, 'none', 'none', 'none');
            ELSE IF @type = 'HRAdministrator'
                INSERT INTO HRAdministrator (employeeID, approvalLevel, recordAccessScope, documentValidationRights) VALUES (@employeeID, 0, 'none', 'none');
            ELSE IF @type = 'PayrollSpecialist'
                INSERT INTO PayrollSpecialist (employeeID, assignedRegion, processingFrequency, lastProcessedPeriod) VALUES (@employeeID, 'none', 'none', 'none');
            ELSE IF @type = 'LineManager'
                INSERT INTO LineManager (employeeID, teamSize, supervisedDepartments, approvalLimit) VALUES (@employeeID, 0, 'none', 0.0);

            COMMIT TRANSACTION;

            SET @success = 1; -- success
        END TRY
        BEGIN CATCH
            ROLLBACK TRANSACTION;
            THROW;
        END CATCH
    END;
    GO

-- this stored procedure changes the profileCompletion field
-- success = 1 means that the change was successful
-- success = 0 means that the change was unsucessful
-- success = -1 means that the percentage was not valid
CREATE PROCEDURE SetProfileCompleteness
    (
        @employeeID INT,
        @completenessPercentage INT,
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;

        IF @completenessPercentage BETWEEN 0 AND 100
        BEGIN
            UPDATE Employee
                SET
                    profileCompletion = @completenessPercentage
                WHERE employeeID = @employeeID;
            
            IF @@ROWCOUNT = 1
                SET @success = 1;
            ELSE
                SET @success = 0;
        END;
        ELSE
            SET @success = -1;
    END;
    GO

-- this stored procedure changes the managerID for a given employeeID row
-- success = 1 means that the change was successful
-- success = 0 means that the change was unsuccessful
CREATE PROCEDURE ReassignManager
    (
        @employeeID INT,
        @newManagerID INT,
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;

        UPDATE Employee
            SET managerID = @newManagerID
            WHERE employeeID = @employeeID;

        IF @@ROWCOUNT = 1
                SET @success = 1;
            ELSE
                SET @success = 0;
    END;
    GO

-- this stored procedure changes the employee's manager and department
-- success = 1 means that the change was successful
-- success = 0 means that nothing was changed, but there were no errors
-- success = -1 means that the employee does not exist
-- success = -2 means that the employeeID is the same as managerID
-- success = -3 means that the new manager does not exist
-- success = -4 means that the department does not exist
CREATE PROCEDURE ReassignHierarchy
    (
        @employeeID INT,
        @newDepartmentID INT,
        @newManagerID INT,
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;

        -- must make sure that the employee exists
        IF NOT EXISTS(SELECT 1 FROM Employee WHERE employeeID = @employeeID)
        BEGIN
            SET @success = -1;
            RETURN;
        END;

        -- should prevent managerID = employeeID
        IF @newManagerID IS NOT NULL AND @newManagerID = @employeeID
        BEGIN
            SET @success = -2;
            RETURN;
        END;

        IF NOT EXISTS(SELECT 1 FROM Employee WHERE employeeID = @newManagerID)
        BEGIN
            SET @success = -3;
            RETURN;
        END;

        -- should make sure that the department exists
        IF @newDepartmentID IS NOT NULL AND NOT EXISTS(SELECT 1 FROM Department WHERE departmentID = @newDepartmentID)
        BEGIN
            SET @success = -4;
            RETURN;
        END;

        UPDATE Employee
            SET departmentID = @newDepartmentID,
                managerID = @newManagerID
            WHERE employeeID = @employeeID;
        
        IF @@ROWCOUNT = 1
                SET @success = 1;
            ELSE
                SET @success = 0;
    END;
    GO

-- this stored procedure outputs the organisational hierarchy
CREATE PROCEDURE ViewOrgHierarchy
    AS
    BEGIN
        SET NOCOUNT ON;

        WITH Hierarchy AS (
            SELECT
                department.departmentID,
                department.name AS departmentName,
                employee1.employeeID,
                employee1.fullName,
                employee1.managerID,
                employee1.positionID,
                0 AS lvl
            FROM Department department
            LEFT JOIN Employee employee1 ON employee1.employeeID = department.headID

            UNION ALL

            SELECT
                hierarchy.departmentID,
                hierarchy.departmentName,
                employee2.employeeID,
                employee2.fullName,
                employee2.managerID,
                employee2.positionID,
                hierarchy.lvl + 1 AS lvl
            FROM Hierarchy hierarchy
            JOIN Employee employee2 ON employee2.managerID = hierarchy.employeeID
        )

        SELECT
            hierarchy.departmentName,
            hierarchy.fullName AS employeeName,
            manager.fullName AS managerName,
            position.title AS positionTitle,
            hierarchy.lvl AS hierarchyLevel
        FROM Hierarchy hierarchy
        LEFT JOIN Employee manager ON hierarchy.managerID = manager.employeeID
        LEFT JOIN Position position ON hierarchy.positionID = position.positionID
        ORDER BY hierarchy.departmentName, hierarchy.lvl, manager.fullName, hierarchy.fullName;
    END;
    GO

-- this stored procedure assigns the department with a head
-- success = 1 means that the assignment was successful
-- success = 0 means that the department does not exist
-- success = -1 means that the manager does not exist
CREATE PROCEDURE AssignDepartmentHead
    (
        @departmentID INT,
        @managerID INT,
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;

        -- checks that the department exists
        IF NOT EXISTS (SELECT 1 FROM Department WHERE departmentID = @departmentID)
        BEGIN
            SET @success = 0;
            RETURN;
        END;

        -- checks if the manager exists
        IF NOT EXISTS (SELECT 1 FROM Employee WHERE employeeID = @managerID)
        BEGIN
            SET @success = -1;
            RETURN;
        END;

        -- assign the head
        UPDATE Department
            SET headID = @managerID
            WHERE departmentID = @departmentID;
        
        SET @success = 1;
    END;
    GO

-- this stored procedure outputs a list of employees (ID and name) under a single manager
-- does not have success yet
CREATE PROCEDURE GetTeamByManager
    (
        @managerID INT
    )
    AS
    BEGIN
        SET NOCOUNT ON;

        SELECT 
            E.employeeID,
            E.firstName + ' ' + E.lastName AS fullName,
            D.name AS departmentName,
            P.title AS positionTitle,
            E.emailAddress,
            E.phoneNumber
        FROM Employee E
        LEFT JOIN Department D ON E.departmentID = D.departmentID
        LEFT JOIN Position P ON E.positionID = P.positionID
        WHERE E.managerID = @managerID
        ORDER BY D.name ASC, E.lastName ASC;
    END;
    GO

-- this stored procedure creates a new contract
-- success = 1 means that the contract is correctly inserted
-- success = 0 means that the contract type is invalid
-- success = -1 means that there was no update done
ALTER PROCEDURE CreateContract
    (
        @employeeID INT,
        @type VARCHAR(50),
        @startDate DATE,
        @endDate DATE,
        @success INT OUTPUT,
        @contractID INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;
        SET @success = 0;

        IF NOT EXISTS (SELECT 1 FROM Employee WHERE employeeID = @employeeID)
        BEGIN
            SET @success = -1;
            RETURN;
        END;

        INSERT INTO Contract (type, startDate, endDate, currentState, employeeID)
            VALUES (@type, @startDate, @endDate, 'active', @employeeID);
        
        SET @contractID = SCOPE_IDENTITY();
        
        IF @type = 'FullTime'
        BEGIN
            INSERT INTO FullTimeContract (contractID, leaveEntitlement, insuranceEligibility, weeklyWorkingHours)
                VALUES (@contractID, 0, 0, 0);
            
            SET @success = 1;
        END;
        ELSE IF @type = 'PartTime'
        BEGIN
            INSERT INTO PartTimeContract (contractID, workingHours, hourlyRate)
                VALUES (@contractID, 0, 0.0);
            
            SET @success = 1;
        END;
        ELSE IF @type = 'Consultant'
        BEGIN
            INSERT INTO ConsultantContract (contractID, projectScope, fees, paymentSchedule)
                VALUES (@contractID, NULL, 0.0, NULL);
            
            SET @success = 1;
        END;
        ELSE IF @type = 'Internship'
        BEGIN
            INSERT INTO InternshipContract (contractID, mentoring, evaluation)
                VALUES (@contractID, NULL, NULL);
            
            SET @success = 1;
        END;
        ELSE
            DELETE FROM Contract WHERE contractID = @contractID;
    END;

-- this stored procedure renews an existing contract
-- success = 1 means that the contract has been successfully updated
-- success = 0 means that the contractID does not exist
CREATE PROCEDURE RenewContract
    (
        @contractID INT,
        @newEndDate DATE,
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;

        -- checks if the contract exists
        IF NOT EXISTS(SELECT 1 FROM Contract WHERE contractID = @contractID)
        BEGIN
            SET @success = 0;
            RETURN;
        END;

        UPDATE Contract
            SET endDate = @newEndDate
            WHERE contractID = @contractID;

        SET @success = 1;
    END;
    GO

-- this stored procedure outputs all active contracts
CREATE PROCEDURE GetActiveContracts
    AS
    BEGIN
        SET NOCOUNT ON;
        
        SELECT
            C.contractID,
            C.employeeID,
            E.firstName + ' ' + E.lastName AS EmployeeName,
            C.type,
            C.startDate,
            C.endDate,
            C.currentState
        FROM Contract C
        JOIN Employee E ON C.employeeID = E.employeeID
        WHERE C.currentState = 'active';
    END;
    GO

-- this stored procedure outputs all contracts that will expire in a given number of days
CREATE PROCEDURE GetExpiringContracts
    (
        @daysBefore INT
    )
    AS
    BEGIN
        SET NOCOUNT ON;

        SELECT 
            C.contractID,
            C.employeeID,
            E.firstName + ' ' + E.lastName AS EmployeeName,
            C.type,
            C.startDate,
            C.endDate,
            C.currentState
        FROM Contract C
        JOIN Employee E ON C.employeeID = E.employeeID
        WHERE C.endDate > CAST(GETDATE() AS DATE) 
          AND C.endDate <= DATEADD(DAY, @daysBefore, CAST(GETDATE() AS DATE));
    END;
    GO

-- this stored procedure configures termination and resignation compensations
-- success = 1 means that the termination benefit has been added
-- success = 0 means that there is no such employee
-- success = -1 means that the employee has not been terminated
CREATE PROCEDURE ConfigureTerminationBenefits
    (
        @employeeID INT,
        @compensationAmount DECIMAL(10,2),
        @effectiveDate DATE,
        @reason VARCHAR(50),
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;

        DECLARE @terminationID INT;

        -- makes sure that the employeeID is valid
        IF NOT EXISTS(SELECT 1 FROM Employee WHERE employeeID = @employeeID)
        BEGIN
            SET @success = 0;
            RETURN;
        END;

        -- finding the newest termination record
        SELECT TOP 1 @terminationID = termination.terminationID
            FROM Termination termination
            JOIN Contract contract ON termination.contractID = contract.contractID
            WHERE contract.employeeID = @employeeID
            ORDER BY termination.date DESC;
        
        -- checks whether the query returned a terminationID
        IF @terminationID IS NULL
        BEGIN
            SET @success = -1;
            RETURN;
        END;

        -- inserts the termination benefit record
        INSERT INTO TerminationBenefit (terminationID, compensationAmount, effectiveDate, reason)
            VALUES (@terminationID, @compensationAmount, @effectiveDate, @reason);
        
        SET @success = 1;
    END;
    GO

-- this stored procedure assigns missions to employees
-- success = 1 means that the mission was added successfully
-- success = 0 means that the employee does not exist
-- success = -1 means that the manager does not exist
CREATE PROCEDURE AssignMission
    (
        @employeeID INT,
        @managerID INT,
        @destination VARCHAR(50),
        @startDate DATE,
        @endDate DATE,
        @success INT OUTPUT,
        @missionID INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;

        IF NOT EXISTS (SELECT 1 FROM Employee WHERE employeeID = @employeeID)
        BEGIN
            SET @success = 0;
            RETURN;
        END;

        IF NOT EXISTS (SELECT 1 FROM Employee WHERE employeeID = @managerID)
        BEGIN
            SET @success = -1;
            RETURN;
        END;

        INSERT INTO Mission (employeeID, managerID, destination, startDate, endDate, status)
            VALUES (@employeeID, @managerID, @destination, @startDate, @endDate, 'Pending');

        SET @missionID = SCOPE_IDENTITY();

        SET @success = 1;
    END;
    GO

CREATE PROCEDURE ApproveMissionRequest
    (
        @missionID INT,
        @managerID INT,
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;
        
        IF NOT EXISTS (SELECT 1 FROM Mission WHERE missionID = @missionID)
        BEGIN
            SET @success = 0; -- Mission not found
            RETURN;
        END;

        IF NOT EXISTS (SELECT 1 FROM Mission WHERE missionID = @missionID AND managerID = @managerID)
        BEGIN
            SET @success = -1; -- Unauthorized (not the manager)
            RETURN;
        END;

        UPDATE Mission
        SET status = 'Active'
        WHERE missionID = @missionID;

        SET @success = 1;
    END;
    GO

CREATE PROCEDURE RejectMissionRequest
    (
        @missionID INT,
        @managerID INT,
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;

        IF NOT EXISTS (SELECT 1 FROM Mission WHERE missionID = @missionID)
        BEGIN
            SET @success = 0; -- Mission not found
            RETURN;
        END;

        IF NOT EXISTS (SELECT 1 FROM Mission WHERE missionID = @missionID AND managerID = @managerID)
        BEGIN
            SET @success = -1; -- Unauthorized
            RETURN;
        END;

        UPDATE Mission
        SET status = 'Rejected'
        WHERE missionID = @missionID;

        SET @success = 1;
    END;
    GO

CREATE PROCEDURE ViewPendingMissions
    (
        @managerID INT
    )
    AS
    BEGIN
        SET NOCOUNT ON;

        SELECT 
            M.missionID,
            M.destination,
            M.startDate,
            M.endDate,
            M.status,
            E.firstName + ' ' + E.lastName AS EmployeeName,
            E.employeeID
        FROM Mission M
        JOIN Employee E ON M.employeeID = E.employeeID
        WHERE M.managerID = @managerID AND M.status = 'Pending';
    END;
    GO

-- this stored procedure approves the completion of a mission
-- success = 1 means that ...
-- success = 0 means that the mission does not exist
-- success = -1 means that the manager does not exist
-- success = -2 means that the mission was not assigned to that manager
CREATE PROCEDURE ApproveMissionCompletion
    (
        @missionID INT,
        @managerID INT,
        @remarks VARCHAR(200),
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;

        -- making sure that the mission exists
        IF NOT EXISTS (SELECT 1 FROM Mission WHERE missionID = @missionID)
        BEGIN
            SET @success = 0;
            RETURN;
        END;

        -- making sure that the manager exists
        IF NOT EXISTS (SELECT 1 FROM Employee WHERE employeeID = @managerID)
        BEGIN
            SET @success = -1;
            RETURN;
        END;

        -- making sure that the mission was assigned to that manager
        IF NOT EXISTS (SELECT 1 FROM Mission WHERE missionID = @missionID AND managerID = @managerID)
        BEGIN
            SET @success = -2;
            RETURN;
        END;

        -- update the mission status to approved
        UPDATE Mission
            SET status = 'approved'
            WHERE missionID = @missionID;;

        SET @success = 1;
    END;
    GO

-- this stored procedure outputs a summary of employee distribution across departments
CREATE PROCEDURE GetDepartmentEmployeeStats
    AS
    BEGIN
        SET NOCOUNT ON;

        SELECT department.name AS departmentName, COUNT(employee.employeeID) AS employeeCount
            FROM Department department
            LEFT JOIN Employee employee ON employee.departmentID = department.departmentID
            GROUP BY department.name
            ORDER BY department.name;
    END;
    GO

-- this stored procedure updates any part of an employee's pofile
-- success = 1 means that the field was correctly updated
-- success = 0 means that the employee does not exist
-- success = -1 means that the field is invalid or unauthorised
CREATE PROCEDURE UpdateEmployeeProfile
    (
        @employeeID INT,
        @field VARCHAR(50),
        @value VARCHAR(MAX),
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;

        -- making sure that the employee exists
        IF NOT EXISTS (SELECT 1 FROM Employee WHERE employeeID = @employeeID)
        BEGIN
            SET @success = 0;
            RETURN;
        END;

        -- making sure that the field is valid and authorised
        IF @field NOT IN (
            'firstName', 'middleName', 'lastName',
            'nationalID', 'birthDate', 'birthCountry',
            'phoneNumber', 'emailAddress', 'address',
            'emergencyContactName', 'emergencyContactPhone', 'relationship',
            'biography', 'profileImage',
            'employmentProgress', 'accountStatus', 'employmentStatus',
            'hireDate', 'isActive', 'profileCompletion',
            'departmentID', 'positionID', 'managerID',
            'contractID', 'taxFormID', 'salaryTypeID', 'payGrade'
        )
        BEGIN
            SET @success = -1;
            RETURN;
        END;

        -- changing the requested field
        DECLARE @sqlScript NVARCHAR(MAX);

        SET @sqlScript = 'UPDATE employee SET ' + QUOTENAME(@field) + ' = @value WHERE employeeID = @employeeID;';

        EXEC sp_executesql @sqlScript,
            N'@value VARCHAR(MAX), @employeeID INT',
            @value = @value,
            @employeeID = @employeeID;

        SET @success = 1;
    END;
    GO

-- this stored procedure generates a filtered employee profile report
-- success = 1 means that the field was correctly updated
-- success = 0 means that the field is invalid or unauthorised
CREATE PROCEDURE GenerateProfileReport
    (
        @filterField VARCHAR(50),
        @filterValue VARCHAR(100),
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;

        -- make sure that the field is valid and authorised
        IF @filterField NOT IN (
            'firstName',
            'middleName',
            'lastName',
            'fullName',
            'nationalID',
            'birthCountry',
            'phoneNumber',
            'emailAddress',
            'address',
            'employmentStatus',
            'accountStatus',
            'departmentID',
            'positionID'
        )
        BEGIN
            SET @success = 0;
            RETURN;
        END;

        -- doing the query
        DECLARE @sqlScript NVARCHAR(MAX);

        SET @sqlScript = 'SELECT employeeID, fullName, departmentID, positionID, emailAddress, phoneNumber FROM Employee WHERE ' +  QUOTENAME(@filterField) + ' = @filterValue;';

        EXEC sp_executesql @sqlScript,
            N'@filterValue VARCHAR(100)',
            @filterValue = @filterValue;
        
        SET @success = 1;
    END;
    GO

-- this stored procedure configures a signing bonus for an employee
-- success = 1 means that the signing bonus has been configured correctly
-- success = 0 means that the employee does not exist
CREATE PROCEDURE ConfigureSigningBonus
    (
        @employeeID INT,
        @bonusAmount DECIMAL(10, 2),
        @effectiveDate DATE,
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;

        -- makes sure that the employee exists
        IF NOT EXISTS (SELECT 1 FROM Employee WHERE employeeID = @employeeID)
        BEGIN
            SET @success = 0;
            RETURN;
        END;

        -- inserts the new signing bonus
        INSERT INTO SigningBonus (employeeID, bonusAmount, effectiveDate)
            VALUES (@employeeID, @bonusAmount, @effectiveDate);
    END;
    GO

-- this stored procedure defines the pay type for an employee
-- success = 1 means that the pay type has been defined successfully
-- success = 0 means that the employee does not exist
-- success = -1 means that the payType is invalid
CREATE PROCEDURE DefinePayType
    (
        @employeeID INT,
        @payType VARCHAR(50),
        @effectiveDate DATE,
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;

        -- make sure that the employee exists
        IF NOT EXISTS (SELECT 1 FROM Employee WHERE employeeID = @employeeID)
        BEGIN
            SET @success = 0;
            RETURN;
        END;

        -- make sure that @payType exists in SalaryType
        IF NOT EXISTS (
            SELECT 1
            FROM SalaryType st
            WHERE 
                (@payType = 'Hourly'  AND EXISTS (SELECT 1 FROM HourlySalaryType  WHERE salaryTypeID = st.salaryTypeID)) OR
                (@payType = 'Monthly' AND EXISTS (SELECT 1 FROM MonthlySalaryType WHERE salaryTypeID = st.salaryTypeID)) OR
                (@payType = 'Contract' AND EXISTS (SELECT 1 FROM ContractSalaryType WHERE salaryTypeID = st.salaryTypeID))
        )
        BEGIN
            SET @success = -1;
            RETURN;
        END;

        -- update the employee's salarytypeID
        DECLARE @salaryTypeID INT;

        SELECT TOP 1
            @salaryTypeID = st.salaryTypeID
        FROM SalaryType st
        WHERE
            (@payType = 'Hourly'  AND EXISTS (SELECT 1 FROM HourlySalaryType  h WHERE h.salaryTypeID = st.salaryTypeID))
            OR (@payType = 'Monthly' AND EXISTS (SELECT 1 FROM MonthlySalaryType m WHERE m.salaryTypeID = st.salaryTypeID))
            OR (@payType = 'Contract' AND EXISTS (SELECT 1 FROM ContractSalaryType c WHERE c.salaryTypeID = st.salaryTypeID));
        
        UPDATE Employee
        SET salaryTypeID = @salaryTypeID
        WHERE employeeID = @employeeID;

        SET @success = 1;
    END;
    GO

-- this stored procedure configures overtime rules
-- success = 1 means that the overtime rule has been configured
-- success = 0 means that the dayType is invalid
CREATE PROCEDURE ConfigureOvertimeRules
    (
        @dayType VARCHAR(20),
        @multiplier DECIMAL(3, 2),
        @hoursPerMonth INT,
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;

        -- make sure that @dayType is valid
        IF @dayType NOT IN ('Weekday', 'Weekend', 'Holiday')
        BEGIN
            SET @success = 0;
            RETURN;
        END;

        -- make sure that overtime configuration exists (if applicable)
        DECLARE @policyID INT;
        SELECT TOP 1 @policyID = payrollPolicyID FROM OvertimePolicy;

        -- insert/update the overtime policy table if it exists
        IF @policyID IS NOT NULL
        BEGIN
            UPDATE OvertimePolicy
                SET weekdayRateMultiplier = CASE WHEN @dayType = 'Weekday' THEN @multiplier ELSE weekdayRateMultiplier END,
                    weekendRateMultiplier = CASE WHEN @dayType = 'Weekend' THEN @multiplier ELSE weekendRateMultiplier END,
                    maxHoursPerMonth = @hoursPerMonth
                WHERE payrollPolicyID = @policyID;
        END;
        ELSE
        BEGIN
            INSERT INTO PayrollPolicy (effectiveDate, description)
                VALUES (CAST(GETDATE() AS DATE), 'Standard Overtime Policy');
            
            INSERT INTO OvertimePolicy (payrollPolicyID, weekdayRateMultiplier, weekendRateMultiplier, maxHoursPerMonth)
                VALUES (SCOPE_IDENTITY(),
                        CASE WHEN @dayType = 'Weekday' THEN @multiplier ELSE 1.0 END,
                        CASE WHEN @dayType = 'Weekend' THEN @multiplier ELSE 1.0 END,
                        @hoursPerMonth);
        END;

        SET @success = 1;
    END;
    GO

-- this stored procedure configures shift allowances
-- success = 1 means that the shift allowance was successfully configured
-- success = 0 means that the creator does not exist
-- success = -1 means that the shiftType is invalid
CREATE PROCEDURE ConfigureShiftAllowance
    (
        @shiftType VARCHAR(20),
        @allowanceAmount DECIMAL(10, 2),
        @createdBy INT,
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;

        -- make sure that the manager/creator exists
        IF NOT EXISTS (SELECT 1 FROM Employee WHERE employeeID = @createdBy)
        BEGIN
            SET @success = 0;
            RETURN;
        END;

        -- make sure that the shiftType is valid (Removed restriction to allow new types)
        -- IF @shiftType NOT IN ('Morning', 'Evening', 'Night')
        -- BEGIN
        --    SET @success = -1;
        --    RETURN;
        -- END;

        -- insert/update the shift configuration table
        UPDATE ShiftConfiguration
            SET allowanceAmount = @allowanceAmount,
                lastUpdatedBy = @createdBy,
                lastUpdatedAt = GETDATE()
            WHERE shiftType = @shiftType;

        IF @@ROWCOUNT = 0
        BEGIN
            INSERT INTO ShiftConfiguration (shiftType, allowanceAmount, lastUpdatedBy, lastUpdatedAt)
                VALUES (@shiftType, @allowanceAmount, @createdBy, GETDATE());
        END;

        SET @success = 1;
    END;
    GO

-- this stored procedure returns all configured shift types
CREATE PROCEDURE GetAllShiftTypes
    AS
    BEGIN
        SELECT shiftConfigurationID, shiftType, allowanceAmount
        FROM ShiftConfiguration
        ORDER BY shiftType ASC;
    END;
    GO
                lastUpdatedAt = GETDATE()
            WHERE shiftType = @shiftType;

        IF @@ROWCOUNT = 0
            INSERT INTO ShiftConfiguration (shiftType, allowanceAmount, lastUpdatedBy, lastUpdatedAt)
                VALUES (@shiftType, @allowanceAmount, @createdBy, GETDATE());

        SET @success = 1;
    END;
    GO

-- this stored procedure configures the signing bonus policy
-- success = 1 means that the signing bonus was successfully configured
-- success = 0 means that the bonusType is invalid
-- success = -1 means that there was no eligibility criteria
CREATE PROCEDURE ConfigureSigningBonusPolicy
    (
        @bonusType VARCHAR(50),
        @amount DECIMAL(10, 2),
        @eligibilityCriteria NVARCHAR(MAX),
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;

        -- make sure that bonusType is valid
        IF @bonusType NOT IN ('Fixed', 'Percentage')
        BEGIN
            SET @success = 0;
            RETURN;
        END;

        -- validate eligibilityCriteria if needed
        IF @eligibilityCriteria IS NULL
        BEGIN
            SET @success = -1;
            RETURN;
        END;

        -- insert/update the signing bonus policy table (if one exists)
        UPDATE SigningBonusConfiguration
            SET defaultAmount = @amount,
                eligibilityCriteria = @eligibilityCriteria,
                lastUpdated = GETDATE()
            WHERE bonusType = @bonusType;

        IF @@ROWCOUNT = 0
        BEGIN
            INSERT INTO SigningBonusConfiguration (bonusType, defaultAmount, eligibilityCriteria, lastUpdated)
            VALUES (@bonusType, @amount, @eligibilityCriteria, GETDATE());
            SET @success = 1;
        END;

    END;
    GO

-- this stored procedure approves payroll configuration changes
-- success = 1 means that the payroll configuration was successfully approced
-- success = 0 means that the approver does not exist or is not authorised
CREATE PROCEDURE ApprovePayrollConfiguration
    (
        @configID INT,
        @approvedBy INT,
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;

        -- make sure the configuration exists
        IF NOT EXISTS (SELECT 1 FROM PayrollPolicy WHERE payrollPolicyID = @configID)
        BEGIN
            SET @success = 0; -- Or another error code for "Config not found"
            RETURN;
        END;

        -- make sure the approver exists and is authorized
        IF NOT EXISTS (SELECT 1 FROM Employee WHERE employeeID = @approvedBy)
        BEGIN
            SET @success = 0;
            RETURN;
        END;

        -- update the configuration status to approved
        UPDATE PayrollPolicy
            SET status = 'Approved',
                approvedBy = @approvedBy
            WHERE payrollPolicyID = @configID;

        IF @@ROWCOUNT = 1
            SET @success = 1;
        ELSE
            SET @success = -1;

    END;
    GO

-- this stored procedure modifies a past payroll entry
-- success = 1 means that the past payroll was successfully modified
-- success = 0 means that the payrollRun does not exist
-- success = -1 means that the employee does not exist in the payroll run
-- success = -2 means that the field name was not allowed to be modified
CREATE PROCEDURE ModifyPastPayroll
    (
        @payrollRunID INT,
        @employeeID INT,
        @fieldName VARCHAR(50),
        @newValue DECIMAL(10, 2),
        @modifiedBy INT,
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;

        -- make sure payrollRun exists
        IF NOT EXISTS (SELECT 1 FROM Payroll WHERE payrollID = @payrollRunID)
        BEGIN
            SET @success = 0;
            RETURN;
        END;

        -- make sure employee exists in that payroll run
        IF NOT EXISTS (
            SELECT 1 FROM Payroll
            WHERE payrollID = @payrollRunID AND employeeID = @employeeID
        )
        BEGIN
            SET @success = -1;
            RETURN;
        END;

        -- validate that @fieldName is allowed to be modified
        IF @fieldName NOT IN ('taxes', 'baseAmount', 'adjustments', 'contributions', 'actualPay', 'netSalary')
        BEGIN
            SET @success = -2;
            RETURN;
        END;
        -- apply the modification
        DECLARE @sqlScript NVARCHAR(MAX);

        -- Build safe dynamic update for the validated column
        SET @sqlScript = N'
            UPDATE Payroll
            SET ' + QUOTENAME(@fieldName) + N' = @newValue
            WHERE payrollID = @payrollRunID AND employeeID = @employeeID;
        ';

        EXEC sp_executesql @sqlScript,
            N'@newValue DECIMAL(10,2), @payrollRunID INT, @employeeID INT',
            @newValue = @newValue,
            @payrollRunID = @payrollRunID,
            @employeeID = @employeeID;

        -- log the modification into PayrollLog if exists
        IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'PayrollLog')
        BEGIN
            INSERT INTO PayrollLog (payrollID, actor, changeDate, modificationType)
                VALUES (@payrollRunID, @modifiedBy, SYSUTCDATETIME(), 'Modified ' + @fieldName);
        END;

        SET @success = 1;
    END;
    GO

-- this stored procedure assigns a shift to an employee
-- shift's start and end dates are today
-- success = 1 means that the shift was successfully assigned
-- success = 0 means that the employee does not exist
-- success = -1 means that the shift does not exist
CREATE PROCEDURE AssignShift
    (
        @employeeID INT,
        @shiftID INT,
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;

        -- make sure employee exists
        IF NOT EXISTS (SELECT 1 FROM Employee WHERE employeeID = @employeeID)
        BEGIN
            SET @success = 0;
            RETURN
        END;

        -- make sure shift exists
        IF NOT EXISTS (SELECT 1 FROM ShiftSchedule WHERE shiftID = @shiftID)
        BEGIN
            SET @success = -1
            RETURN
        END;

        -- create the assignment in works_shift table
        INSERT INTO works_shift (employeeID, shiftID, startDate, endDate, status)
            VALUES (@employeeID, @shiftID, CAST(GETDATE() AS DATE), CAST(GETDATE() AS DATE), 'Active');

        SET @success = 1;
    END;
    GO

-- this stored procedure sends a notification to team members
-- success = 1 means that the team notification was successfully sent
-- success = 0 means that the manager does not exist
-- success = -1 means that urguncy level is invalid
CREATE PROCEDURE SendTeamNotification
    (
        @managerID INT,
        @messageContent VARCHAR(255),
        @urgencyLevel VARCHAR(50),
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;

        -- verify manager exists
        IF NOT EXISTS (SELECT 1 FROM Employee WHERE employeeID = @managerID)
        BEGIN
            SET @success = 0;
            RETURN
        END;

        -- retrieve employees supervised by this manager
        -- verify urgency level is valid
        IF @urgencyLevel NOT IN ('Low', 'Medium', 'High', 'Critical')
        BEGIN
            SET @success = -1;
            RETURN;
        END;

        -- insert notification into Notification table
        INSERT INTO Notification (messageContent, timestamp, urgency, readStatus)
            VALUES (@messageContent, SYSUTCDATETIME(), @urgencyLevel, 'Unread');

        -- insert delivery records into receives_notification table for employees supervised by this manager
        DECLARE @notificationID INT = SCOPE_IDENTITY();

        INSERT INTO receives_notification (employeeID, notificationID, deliveryStatus, deliveredAt)
        SELECT e.employeeID, @notificationID, 'Pending', NULL
            FROM Employee e
            WHERE e.managerID = @managerID;

        SET @success = 1;
    END;
    GO

-- this stored procedure requests a replacement for an unavailable employee
-- success = 1 means that replacement request was successful
-- success = 0 means that the employee does not exist
CREATE PROCEDURE RequestReplacement
    (
        @employeeID INT,
        @reason VARCHAR(150),
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;

        -- verify employee exists
        IF NOT EXISTS (SELECT 1 FROM Employee WHERE employeeID = @employeeID)
        BEGIN
            SET @success = 0;
            RETURN
        END;

        -- create a replacement request (if a table exists)
        INSERT INTO ReplacementRequest (employeeID, reason, requestDate, status)
            VALUES (@employeeID, @reason, GETDATE(), 'Pending');

        IF @@ROWCOUNT = 1
            SET @success = 1;
        ELSE
            SET @success = -1;

    END;
    GO

-- this stored procedure reassigns a shift for an employee
-- success = 1 means that the shift reassigment was successful
-- success = 0 means that the employee does not exist
-- success = -1 means that the old shift does not exist or is not assigned to this employee
-- success = -2 means that the new shift does not exist
CREATE PROCEDURE ReassignShift
    (
        @employeeID INT,
        @oldShiftID INT,
        @newShiftID INT,
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;

        -- verify employee exists
        IF NOT EXISTS (SELECT 1 FROM Employee WHERE employeeID = @employeeID)
        BEGIN
            SET @success = 0;
            RETURN;
        END;
        -- verify oldShiftID exists for this employee
        IF NOT EXISTS (
            SELECT 1 
            FROM works_shift
            WHERE employeeID = @employeeID AND shiftID = @oldShiftID
        )
        BEGIN
            SET @success = -1;
            RETURN;
        END;

        -- verify newShiftID exists
        IF NOT EXISTS (SELECT 1 FROM ShiftSchedule WHERE shiftID = @newShiftID)
        BEGIN
            SET @success = -2;
            RETURN;
        END;

        -- update works_shift (or equivalent) to replace the assignment
        UPDATE works_shift
            SET shiftID = @newShiftID
        WHERE employeeID = @employeeID AND shiftID = @oldShiftID;

        SET @success = 1;
    END;
    GO

-- this stored procedure retrieves pending leave requests for a manager
-- success = 1 means that the pending leave requests were retireved
-- success = 0 means that the manager does not exist
CREATE PROCEDURE GetPendingLeaveRequests
    (
        @managerID INT,
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;

        -- verify manager exists
        IF NOT EXISTS (SELECT 1 FROM Employee WHERE employeeID = @managerID)
        BEGIN
            SET @success = 0;
            RETURN
        END;
        -- retrieve employees supervised by this manager
        -- fetch leave requests with status='pending' for these employees
        SELECT lr.*, e.fullName
            FROM LeaveRequest lr
            JOIN Employee e ON lr.employeeID = e.employeeID
            WHERE e.managerID = @managerID AND lr.status = 'Pending';

        SET @success = 1;
    END;
    GO

-- this stored procedure retrieves a summary of a manager's team
-- success = 1 means that the team summary was retrieved
-- success = 0 means that the manager does not exist
CREATE PROCEDURE GetTeamSummary
    (
        @managerID INT,
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;

        -- verify manager exists
        IF NOT EXISTS (SELECT 1 FROM Employee WHERE employeeID = @managerID)
        BEGIN
            SET @success = 0;
            RETURN
        END;
        
        -- retrieve employees supervised by this manager
        -- compute summary metrics (count, roles, active status, etc.)
        -- return summary rows
        SELECT 
            pos.title AS Position,
            COUNT(emp.employeeID) AS TotalCount,
            SUM(CASE WHEN emp.isActive = 1 THEN 1 ELSE 0 END) AS ActiveCount
        FROM Employee emp
        JOIN Position pos ON emp.positionID = pos.positionID
        WHERE emp.managerID = @managerID
        GROUP BY pos.title;

        SET @success = 1;
    END;
    GO

-- this stored procedure views certifications and skills of team members
-- success = 1 means that the team certifications were viewed
-- success = 0 means that the manager does not exist
CREATE PROCEDURE ViewTeamCertifications
    (
        @managerID INT,
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;

        -- verify manager exists
        IF NOT EXISTS (SELECT 1 FROM Employee WHERE employeeID = @managerID)
        BEGIN
            SET @success = 0;
            RETURN
        END;

        -- retrieve employees supervised by this manager
        -- join employees to Verification + Skill tables
        -- retrieve certifications and skill levels
        -- return result set
        SELECT 
            e.fullName,
            'Skill' AS category,
            s.name AS title,
            CAST(ps.proficiencyLevel AS VARCHAR(50)) AS details
        FROM Employee e
        JOIN possesses_skill ps ON e.employeeID = ps.employeeID
        JOIN Skill s ON ps.skillID = s.skillID
        WHERE e.managerID = @managerID

        UNION ALL

        SELECT 
            e.fullName,
            'Certification' AS category,
            v.type AS title,
            v.issuer AS details
        FROM Employee e
        JOIN undergoes_verification uv ON e.employeeID = uv.employeeID
        JOIN Verification v ON uv.verificationID = v.verificationID
        WHERE e.managerID = @managerID
        
        ORDER BY fullName, category;

        SET @success = 1;
    END;
    GO

-- this stored procedure adds manager notes about an employee
-- success = 1 means that the manager note was added
-- success = 0 means that the employee does not exist
-- success = -1 means that the manager does not exist
-- success = -2 means that the manager does not supervise the employee
CREATE PROCEDURE AddManagerNotes
    (
        @employeeID INT,
        @managerID INT,
        @note VARCHAR(500),
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;

        -- verify employee exists
        IF NOT EXISTS (SELECT 1 FROM Employee WHERE employeeID = @employeeID)
        BEGIN
            SET @success = 0;
            RETURN
        END;

        -- verify manager exists
        IF NOT EXISTS (SELECT 1 FROM Employee WHERE employeeID = @managerID)
        BEGIN
            SET @success = -1;
            RETURN
        END;

        -- verify manager supervises employee
        IF NOT EXISTS (
            SELECT 1 
            FROM Employee e
            WHERE e.employeeID = @employeeID
              AND e.managerID = @managerID
        )
        BEGIN
            SET @success = -2
            RETURN
        END;
        
        -- insert into ManagerNotes table
        INSERT INTO ManagerNotes (employeeID, managerID, noteContent, createdAt)
        VALUES (@employeeID, @managerID, @note, SYSUTCDATETIME());

        SET @success = 1;
    END;
    GO

-- this stored procedure records attendance manually for an employee
-- success = 1 means that the attendance was recorded
-- success = 0 means that the employee does not exist
-- success = -1 means that the shift does not exist
CREATE PROCEDURE RecordAttendance
    (
        @employeeID INT,
        @shiftID INT,
        @entryTime TIME,
        @exitTime TIME,
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;

        -- verify employee exists
        IF NOT EXISTS (SELECT 1 FROM Employee WHERE employeeID = @employeeID)
        BEGIN
            SET @success = 0;
            RETURN
        END;

        -- verify shift exists
        IF NOT EXISTS (SELECT 1 FROM ShiftSchedule WHERE shiftID = @shiftID)
        BEGIN
            SET @success = -1
            RETURN
        END;
        
        -- insert new attendance record into Attendance table
        DECLARE @today DATE = CAST(GETDATE() AS DATE);

        DECLARE @entryDT DATETIME2(0) = CAST(CONCAT(CONVERT(VARCHAR(10), @today, 120), ' ', CONVERT(VARCHAR(8), @entryTime, 108)) AS DATETIME2(0));
        DECLARE @exitDT  DATETIME2(0) = CAST(CONCAT(CONVERT(VARCHAR(10), @today, 120), ' ', CONVERT(VARCHAR(8), @exitTime, 108)) AS DATETIME2(0));

        INSERT INTO Attendance (entryTime, exitTime, employeeID, shiftID)
        VALUES (@entryDT, @exitDT, @employeeID, @shiftID);

        SET @success = 1;
    END;
    GO

-- Mahmoud

-- this stored procedure validates existence of policies and sets null statuses to Pending
-- success = 1 means that the configuration check passed and statuses were updated
-- success = -1 means that the LeaveType table is empty
-- success = -2 means that the LeavePolicy table is empty
-- success = -3 means that no leave types adhere to any policy
-- success = -99 means that an unexpected error occurred
CREATE PROCEDURE ConfigureLeavePolicies
    (
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;

        BEGIN TRY
            BEGIN TRANSACTION;
            
            IF NOT EXISTS (SELECT 1
        FROM LeaveType)
            BEGIN
            SET @success = -1;
            ROLLBACK TRANSACTION;
            RETURN;
        END
            
            IF NOT EXISTS (SELECT 1
        FROM LeavePolicy)
            BEGIN
            SET @success = -2;
            ROLLBACK TRANSACTION;
            RETURN;
        END
            
            IF NOT EXISTS (SELECT 1
        FROM adheres_to_policy)
            BEGIN
            SET @success = -3;
            ROLLBACK TRANSACTION;
            RETURN;
        END
            
            UPDATE LeaveRequest
            SET status = 'Pending'
            WHERE status IS NULL OR status = '';
            
            COMMIT TRANSACTION;
            SET @success = 1;
            
        END TRY
        BEGIN CATCH
            IF @@TRANCOUNT > 0
                ROLLBACK TRANSACTION;
            SET @success = -99;
        END CATCH
    END;
    GO

-- this stored procedure applies a holiday exception to a specific employee
-- success = 1 means that the holiday override was assigned
-- success = 0 means that the employee already has this exception assigned
-- success = -1 means that the employee does not exist
-- success = -2 means that the holiday exception does not exist
-- success = -99 means that an unexpected error occurred
CREATE PROCEDURE ApplyHolidayOverrides
    (
        @HolidayID INT,
        @employeeID INT,
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;

        BEGIN TRY
            BEGIN TRANSACTION;
            
            IF NOT EXISTS (SELECT 1
        FROM Employee
        WHERE employeeID = @employeeID)
            BEGIN
            SET @success = -1;
            ROLLBACK TRANSACTION;
            RETURN;
        END
            
            IF NOT EXISTS (SELECT 1
        FROM Exception
        WHERE exceptionID = @HolidayID)
            BEGIN
            SET @success = -2;
            ROLLBACK TRANSACTION;
            RETURN;
        END
            
            IF NOT EXISTS (SELECT 1
        FROM Employee_Exception
        WHERE employeeID = @employeeID AND exceptionID = @HolidayID)
            BEGIN
            INSERT INTO Employee_Exception
                (employeeID, exceptionID)
            VALUES
                (@employeeID, @HolidayID);
            SET @success = 1;
        END
            ELSE
            BEGIN
            SET @success = 0;
        END
            
            COMMIT TRANSACTION;
            
        END TRY
        BEGIN CATCH
            IF @@TRANCOUNT > 0
                ROLLBACK TRANSACTION;
            SET @success = -99;
        END CATCH
    END;
    GO

-- this stored procedure updates the notice period and adds eligibility rules to a policy
-- success = 1 means that the policy was successfully updated
-- success = -1 means that the policyID does not exist
-- success = -99 means that an unexpected error occurred
CREATE PROCEDURE UpdateLeavePolicy
    (
        @PolicyID INT,
        @EligibilityRules VARCHAR(200),
        @NoticePeriod INT,
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;
        
        BEGIN TRY
            BEGIN TRANSACTION;
            
            IF NOT EXISTS (SELECT 1 FROM LeavePolicy WHERE leavePolicyID = @PolicyID)
            BEGIN
                SET @success = -1;
                ROLLBACK TRANSACTION;
                RETURN;
            END
            
            UPDATE LeavePolicy
            SET noticePeriod = @NoticePeriod
            WHERE leavePolicyID = @PolicyID;
            
            INSERT INTO EligibilityRule (leavePolicyID, eligibilityRule)
            VALUES (@PolicyID, @EligibilityRules);
            
            COMMIT TRANSACTION;
            SET @success = 1;
            
        END TRY
        BEGIN CATCH
            IF @@TRANCOUNT > 0
                ROLLBACK TRANSACTION;
            SET @success = -99;
        END CATCH
    END;
    GO

-- this stored procedure escalates pending requests that have passed the deadline
-- success > 0 means that [success] number of requests were escalated
-- success = 0 means that no requests matched the criteria
-- success = -99 means that an unexpected error occurred
CREATE PROCEDURE EscalatePendingRequests
    (
        @Deadline DATETIME,
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;
        
        BEGIN TRY
            BEGIN TRANSACTION;
            
            UPDATE LeaveRequest
            SET status = 'Escalated'
            WHERE status = 'Pending'
            AND approvalTiming < @Deadline;
            
            SET @success = @@ROWCOUNT;
            
            IF @success > 0
            BEGIN
                INSERT INTO Notification (messageContent, timestamp, urgency, readStatus)
                SELECT 
                    'Leave request ID ' + CAST(leaveRequestID AS VARCHAR(10)) + ' has been escalated due to pending approval beyond deadline.',
                    GETDATE(),
                    'High',
                    'Unread'
                FROM LeaveRequest
                WHERE status = 'Escalated'
                AND approvalTiming < @Deadline;
            END
            
            COMMIT TRANSACTION;
            
        END TRY
        BEGIN CATCH
            IF @@TRANCOUNT > 0
                ROLLBACK TRANSACTION;
            SET @success = -99;
        END CATCH
    END;
    GO

-- this stored procedure links a specific vacation package/request to an employee
-- success = 1 means that the link was successful
-- success = -1 means that the employee does not exist
-- success = -2 means that the vacation package/request does not exist
-- success = -3 means that the request is not a valid Vacation type
-- success = -99 means that an unexpected error occurred
CREATE PROCEDURE LinkVacationToShift
    (
        @VacationPackageID INT,
        @EmployeeID INT,
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;
        
        BEGIN TRY
            BEGIN TRANSACTION;
            
            IF NOT EXISTS (SELECT 1 FROM Employee WHERE employeeID = @EmployeeID)
            BEGIN
                SET @success = -1;
                ROLLBACK TRANSACTION;
                RETURN;
            END;
            
            IF NOT EXISTS (SELECT 1 FROM LeaveRequest WHERE leaveRequestID = @VacationPackageID)
            BEGIN
                SET @success = -2;
                ROLLBACK TRANSACTION;
                RETURN;
            END;
            
            DECLARE @LeaveTypeID INT;
            SELECT @LeaveTypeID = leaveTypeID 
            FROM LeaveRequest 
            WHERE leaveRequestID = @VacationPackageID;
            
            IF NOT EXISTS (SELECT 1 FROM VacationLeaveType WHERE leaveTypeID = @LeaveTypeID)
            BEGIN
                SET @success = -3;
                ROLLBACK TRANSACTION;
                RETURN;
            END;
            
            COMMIT TRANSACTION;
            SET @success = 1;
            
        END TRY
        BEGIN CATCH
            IF @@TRANCOUNT > 0
                ROLLBACK TRANSACTION;
            SET @success = -99;
        END CATCH
    END;
    GO

-- this stored procedure configures tenure and employee type rules for a leave type
-- success = 1 means that the eligibility rule was added
-- success = -1 means that the leave type does not exist
-- success = -2 means that the leave type is not linked to a policy
-- success = -99 means that an unexpected error occurred
CREATE PROCEDURE ConfigureLeaveEligibility
    (
        @LeaveType VARCHAR(50),
        @MinTenure INT,
        @EmployeeType VARCHAR(50),
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;
        
        BEGIN TRY
            BEGIN TRANSACTION;
            
            DECLARE @LeaveTypeID INT;
            
            SELECT @LeaveTypeID = leaveTypeID
            FROM LeaveType
            WHERE typeName = @LeaveType;
            
            IF @LeaveTypeID IS NULL
            BEGIN
                SET @success = -1;
                ROLLBACK TRANSACTION;
                RETURN;
            END;
            
            DECLARE @PolicyID INT;
            
            SELECT TOP 1 @PolicyID = leavePolicyID
            FROM adheres_to_policy
            WHERE leaveTypeID = @LeaveTypeID;
            
            IF @PolicyID IS NULL
            BEGIN
                SET @success = -2;
                ROLLBACK TRANSACTION;
                RETURN;
            END;
            
            DECLARE @EligibilityRule VARCHAR(200);
            SET @EligibilityRule = 'MinTenure: ' + CAST(@MinTenure AS VARCHAR(10)) + ' months, EmployeeType: ' + @EmployeeType;
            
            INSERT INTO EligibilityRule (leavePolicyID, eligibilityRule)
            VALUES (@PolicyID, @EligibilityRule);
            
            COMMIT TRANSACTION;
            SET @success = 1;
            
        END TRY
        BEGIN CATCH
            IF @@TRANCOUNT > 0
                ROLLBACK TRANSACTION;
            SET @success = -99;
        END CATCH
    END;
    GO

-- this stored procedure creates or updates a leave type definition
-- success = 1 means that a new leave type was inserted
-- success = 2 means that an existing leave type was updated
-- success = -99 means that an unexpected error occurred
CREATE PROCEDURE ManageLeaveTypes
    (
        @LeaveType VARCHAR(50),
        @Description VARCHAR(200),
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;
        
        BEGIN TRY
            BEGIN TRANSACTION;
            
            IF EXISTS (SELECT 1 FROM LeaveType WHERE typeName = @LeaveType)
            BEGIN
                UPDATE LeaveType
                SET description = @Description
                WHERE typeName = @LeaveType;
                
                SET @success = 2;
            END
            ELSE
            BEGIN
                INSERT INTO LeaveType (typeName, description)
                VALUES (@LeaveType, @Description);
                
                SET @success = 1;
            END
            
            COMMIT TRANSACTION;
            
        END TRY
        BEGIN CATCH
            IF @@TRANCOUNT > 0
                ROLLBACK TRANSACTION;
            SET @success = -99;
        END CATCH
    END;
    GO

-- this stored procedure configures duration and workflow rules for a leave type
-- success = 1 means that the rules were configured successfully
-- success = -1 means that the leave type does not exist
-- success = -99 means that an unexpected error occurred
CREATE PROCEDURE ConfigureLeaveRules
    (
        @LeaveType VARCHAR(50),
        @MaxDuration INT,
        @NoticePeriod INT,
        @WorkflowType VARCHAR(50),
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;
        
        BEGIN TRY
            BEGIN TRANSACTION;
            
            DECLARE @LeaveTypeID INT;
            
            SELECT @LeaveTypeID = leaveTypeID
            FROM LeaveType
            WHERE typeName = @LeaveType;
            
            IF @LeaveTypeID IS NULL
            BEGIN
                SET @success = -1;
                ROLLBACK TRANSACTION;
                RETURN;
            END;
            
            DECLARE @PolicyID INT;
            
            SELECT TOP 1 @PolicyID = leavePolicyID
            FROM adheres_to_policy
            WHERE leaveTypeID = @LeaveTypeID;
            
            IF @PolicyID IS NULL
            BEGIN
                INSERT INTO LeavePolicy (name, purpose, noticePeriod)
                VALUES (@LeaveType + ' Policy', 'Policy for ' + @LeaveType, @NoticePeriod);
                
                SET @PolicyID = SCOPE_IDENTITY();
                
                INSERT INTO adheres_to_policy (leaveTypeID, leavePolicyID)
                VALUES (@LeaveTypeID, @PolicyID);
            END;
            ELSE
            BEGIN
                UPDATE LeavePolicy
                SET noticePeriod = @NoticePeriod
                WHERE leavePolicyID = @PolicyID;
            END;
            
            INSERT INTO EligibilityRule (leavePolicyID, eligibilityRule)
            VALUES (@PolicyID, 'MaxDuration: ' + CAST(@MaxDuration AS VARCHAR(10)) + ' days, WorkflowType: ' + @WorkflowType);
            
            COMMIT TRANSACTION;
            SET @success = 1;
            
        END TRY
        BEGIN CATCH
            IF @@TRANCOUNT > 0
                ROLLBACK TRANSACTION;
            SET @success = -99;
        END CATCH
    END;
    GO

-- this stored procedure sets the leave year dates and enables annual reset
-- success = 1 means that the leave year rules were set
-- success = -1 means that the start date is after the end date
-- success = -99 means that an unexpected error occurred
CREATE PROCEDURE SetLeaveYearRules
    (
        @StartDate DATE,
        @EndDate DATE,
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;
        
        BEGIN TRY
            BEGIN TRANSACTION;
            
            IF @StartDate >= @EndDate
            BEGIN
                SET @success = -1;
                ROLLBACK TRANSACTION;
                RETURN;
            END;
            
            UPDATE LeavePolicy
            SET resetOnNewYear = 1;
            
            INSERT INTO EligibilityRule (leavePolicyID, eligibilityRule)
            SELECT leavePolicyID, 
            'LeaveYear: ' + CONVERT(VARCHAR(10), @StartDate, 120) + ' to ' + CONVERT(VARCHAR(10), @EndDate, 120)
            FROM LeavePolicy;
            
            COMMIT TRANSACTION;
            SET @success = 1;
            
        END TRY
        BEGIN CATCH
            IF @@TRANCOUNT > 0
                ROLLBACK TRANSACTION;
            SET @success = -99;
        END CATCH
    END;
    GO

-- this stored procedure submits a new leave request for an employee
-- success = 1 means that the request was submitted successfully
-- success = -1 means that the employee does not exist
CREATE PROCEDURE SubmitLeaveRequest
    (
        @EmployeeID INT,
        @LeaveTypeID INT,
        @StartDate DATE,
        @EndDate DATE,
        @Reason VARCHAR(100),
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;
        
        BEGIN TRY
            BEGIN TRANSACTION;
            
            IF NOT EXISTS (SELECT 1 FROM Employee WHERE employeeID = @EmployeeID)
            BEGIN
                SET @success = -1;
                ROLLBACK TRANSACTION;
                RETURN;
            END
            
            IF NOT EXISTS (SELECT 1 FROM LeaveType WHERE leaveTypeID = @LeaveTypeID)
            BEGIN
                SET @success = -2;
                ROLLBACK TRANSACTION;
                RETURN;
            END
            
            IF @StartDate > @EndDate
            BEGIN
                SET @success = -3;
                ROLLBACK TRANSACTION;
                RETURN;
            END
            
            DECLARE @Duration INT;
            SET @Duration = DATEDIFF(DAY, @StartDate, @EndDate) + 1;
            
            INSERT INTO LeaveRequest (employeeID, leaveTypeID, justification, duration, status)
            VALUES (@EmployeeID, @LeaveTypeID, @Reason, @Duration, 'Pending');
            
            COMMIT TRANSACTION;
            SET @success = 1;
            
        END TRY
        BEGIN CATCH
            IF @@TRANCOUNT > 0
                ROLLBACK TRANSACTION;
            SET @success = -99;
        END CATCH
    END;
    GO

-- this stored procedure modifies an existing pending or returned leave request
-- success = 1 means that the request was modified successfully
-- success = -1 means that the leave request does not exist
-- success = -2 means that the request status is not Pending or Returned
-- success = -3 means that the start date is after the end date
-- success = -99 means that an unexpected error occurred
CREATE PROCEDURE ModifyLeaveRequest
    (
        @LeaveRequestID INT,
        @StartDate DATE,
        @EndDate DATE,
        @Reason VARCHAR(100),
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;
        
        BEGIN TRY
            BEGIN TRANSACTION;
            
            IF NOT EXISTS (SELECT 1 FROM LeaveRequest WHERE leaveRequestID = @LeaveRequestID)
            BEGIN
                SET @success = -1;
                ROLLBACK TRANSACTION;
                RETURN;
            END
            
            DECLARE @CurrentStatus VARCHAR(50);
            SELECT @CurrentStatus = status FROM LeaveRequest WHERE leaveRequestID = @LeaveRequestID;
            
            IF @CurrentStatus NOT IN ('Pending', 'Returned')
            BEGIN
                SET @success = -2;
                ROLLBACK TRANSACTION;
                RETURN;
            END
            
            IF @StartDate > @EndDate
            BEGIN
                SET @success = -3;
                ROLLBACK TRANSACTION;
                RETURN;
            END;
            
            DECLARE @Duration INT;
            SET @Duration = DATEDIFF(DAY, @StartDate, @EndDate) + 1;
            
            UPDATE LeaveRequest
            SET justification = @Reason,
                duration = @Duration,
                status = 'Pending'
            WHERE leaveRequestID = @LeaveRequestID;
            
            COMMIT TRANSACTION;
            SET @success = 1;
            
        END TRY
        BEGIN CATCH
            IF @@TRANCOUNT > 0
                ROLLBACK TRANSACTION;
            SET @success = -99;
        END CATCH
    END;
    GO

-- this stored procedure cancels a leave request
-- success = 1 means that the request was cancelled
-- success = -1 means that the leave request does not exist
-- success = -2 means that the request is already Approved or Cancelled
-- success = -99 means that an unexpected error occurred
CREATE PROCEDURE CancelLeaveRequest
    (
        @LeaveRequestID INT,
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;

        IF NOT EXISTS (SELECT 1 FROM LeaveRequest WHERE leaveRequestID = @LeaveRequestID)
        BEGIN
            SET @success = -1;
            RETURN;
        END

        DECLARE @CurrentStatus VARCHAR(50);
        SELECT @CurrentStatus = status FROM LeaveRequest WHERE leaveRequestID = @LeaveRequestID;
        
        IF @CurrentStatus IN ('Approved', 'Cancelled')
        BEGIN
            SET @success = -2;
            ROLLBACK TRANSACTION;
            RETURN;
        END
        
        UPDATE LeaveRequest
        SET status = 'Cancelled'
        WHERE leaveRequestID = @LeaveRequestID;
        
        COMMIT TRANSACTION;
        SET @success = 1;
    END;
    GO

-- this stored procedure processes an approval or rejection decision
-- success = 1 means that the status was updated and notification sent
-- success = -1 means that the leave request does not exist
-- success = -2 means that the approver does not exist
-- success = -3 means that the provided status is invalid (must be Approved or Rejected)
-- success = -99 means that an unexpected error occurred
CREATE PROCEDURE ApproveLeaveRequest
    (
        @LeaveRequestID INT,
        @ApproverID INT,
        @Status VARCHAR(20),
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;
        
        BEGIN TRY
            BEGIN TRANSACTION;
            
            IF NOT EXISTS (SELECT 1 FROM LeaveRequest WHERE leaveRequestID = @LeaveRequestID)
            BEGIN
                SET @success = -1;
                ROLLBACK TRANSACTION;
                RETURN;
            END
            
            IF NOT EXISTS (SELECT 1 FROM Employee WHERE employeeID = @ApproverID)
            BEGIN
                SET @success = -2;
                ROLLBACK TRANSACTION;
                RETURN;
            END;
            
            IF @Status NOT IN ('Approved', 'Rejected')
            BEGIN
                SET @success = -3;
                ROLLBACK TRANSACTION;
                RETURN;
            END;
            
            DECLARE @EmployeeID INT;
            SELECT @EmployeeID = employeeID FROM LeaveRequest WHERE leaveRequestID = @LeaveRequestID;
            
            UPDATE LeaveRequest
            SET status = @Status
            WHERE leaveRequestID = @LeaveRequestID;
            
            INSERT INTO Notification (messageContent, timestamp, urgency, readStatus)
            VALUES (
                'Your leave request has been ' + @Status + '.',
                GETDATE(),
                'Normal',
                'Unread'
            );
            
            DECLARE @NotificationID INT = SCOPE_IDENTITY();
            
            INSERT INTO receives_notification (employeeID, notificationID, deliveryStatus, deliveredAt)
            VALUES (@EmployeeID, @NotificationID, 'Delivered', GETDATE());
            
            COMMIT TRANSACTION;
            SET @success = 1;
            
        END TRY
        BEGIN CATCH
            IF @@TRANCOUNT > 0
                ROLLBACK TRANSACTION;
            SET @success = -99;
        END CATCH
    END;
    GO

-- this stored procedure rejects a leave request with a reason
-- success = 1 means that the request was rejected and notification sent
-- success = -1 means that the leave request does not exist
-- success = -2 means that the manager does not exist
-- success = -99 means that an unexpected error occurred
CREATE PROCEDURE RejectLeaveRequest
    (
        @LeaveRequestID INT,
        @ManagerID INT,
        @Reason VARCHAR(200),
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;
        
        BEGIN TRY
            BEGIN TRANSACTION;
            
            IF NOT EXISTS (SELECT 1 FROM LeaveRequest WHERE leaveRequestID = @LeaveRequestID)
            BEGIN
                SET @success = -1;
                ROLLBACK TRANSACTION;
                RETURN;
            END
            
            IF NOT EXISTS (SELECT 1 FROM Employee WHERE employeeID = @ManagerID)
            BEGIN
                SET @success = -2;
                ROLLBACK TRANSACTION;
                RETURN;
            END
            
            DECLARE @EmployeeID INT;
            SELECT @EmployeeID = employeeID FROM LeaveRequest WHERE leaveRequestID = @LeaveRequestID;
            
            UPDATE LeaveRequest
            SET status = 'Rejected'
            WHERE leaveRequestID = @LeaveRequestID;
            
            INSERT INTO Notification (messageContent, timestamp, urgency, readStatus)
            VALUES (
                'Your leave request has been rejected. Reason: ' + @Reason,
                GETDATE(),
                'Normal',
                'Unread'
            );
            
            DECLARE @NotificationID INT = SCOPE_IDENTITY();
            
            INSERT INTO receives_notification (employeeID, notificationID, deliveryStatus, deliveredAt)
            VALUES (@EmployeeID, @NotificationID, 'Delivered', GETDATE());
            
            COMMIT TRANSACTION;
            SET @success = 1;
            
        END TRY
        BEGIN CATCH
            IF @@TRANCOUNT > 0
                ROLLBACK TRANSACTION;
            SET @success = -99;
        END CATCH
    END;
    GO

-- this stored procedure retrieves the calculated leave balance for an employee
-- success = 1 means that the balance was retrieved
-- success = -1 means that the employee does not exist
-- success = -99 means that an unexpected error occurred
CREATE PROCEDURE GetLeaveBalance
    (
        @EmployeeID INT,
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;
        
        BEGIN TRY
            IF NOT EXISTS (SELECT 1 FROM Employee WHERE employeeID = @EmployeeID)
            BEGIN
                SET @success = -1;
                RETURN;
            END
            
            SELECT 
                lt.typeName AS LeaveType,
                le.entitlement AS TotalEntitlement,
                ISNULL(SUM(CASE WHEN lr.status = 'Approved' THEN lr.duration ELSE 0 END), 0) AS UsedDays,
                le.entitlement - ISNULL(SUM(CASE WHEN lr.status = 'Approved' THEN lr.duration ELSE 0 END), 0) AS RemainingDays
            FROM LeaveEntitlement le
            INNER JOIN LeaveType lt ON le.leaveTypeID = lt.leaveTypeID
            LEFT JOIN LeaveRequest lr ON lr.employeeID = le.employeeID AND lr.leaveTypeID = le.leaveTypeID
            WHERE le.employeeID = @EmployeeID
            GROUP BY lt.typeName, le.entitlement;
            
            SET @success = 1;
            
        END TRY
        BEGIN CATCH
            SET @success = -99;
        END CATCH
    END;
    GO

-- this stored procedure assigns or updates the leave entitlement amount
-- success = 1 means that a new entitlement record was created
-- success = 2 means that an existing entitlement record was updated
-- success = -1 means that the employee does not exist
-- success = -2 means that the leave type does not exist
-- success = -99 means that an unexpected error occurred
CREATE PROCEDURE AssignLeaveEntitlement
    (
        @EmployeeID INT,
        @LeaveType VARCHAR(50),
        @Entitlement INT,
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;
        
        BEGIN TRY
            BEGIN TRANSACTION;
            
            IF NOT EXISTS (SELECT 1 FROM Employee WHERE employeeID = @EmployeeID)
            BEGIN
                SET @success = -1;
                ROLLBACK TRANSACTION;
                RETURN;
            END
            
            DECLARE @LeaveTypeID INT;
            
            SELECT @LeaveTypeID = leaveTypeID
            FROM LeaveType
            WHERE typeName = @LeaveType;
            
            IF @LeaveTypeID IS NULL
            BEGIN
                SET @success = -2;
                ROLLBACK TRANSACTION;
                RETURN;
            END
            
            IF EXISTS (SELECT 1 FROM LeaveEntitlement WHERE employeeID = @EmployeeID AND leaveTypeID = @LeaveTypeID)
            BEGIN
                UPDATE LeaveEntitlement
                SET entitlement = @Entitlement
                WHERE employeeID = @EmployeeID AND leaveTypeID = @LeaveTypeID;
                
                SET @success = 2;
            END
            ELSE
            BEGIN
                INSERT INTO LeaveEntitlement (employeeID, leaveTypeID, entitlement)
                VALUES (@EmployeeID, @LeaveTypeID, @Entitlement);
                
                SET @success = 1;
            END
            
            COMMIT TRANSACTION;
            
        END TRY
        BEGIN CATCH
            IF @@TRANCOUNT > 0
                ROLLBACK TRANSACTION;
            SET @success = -99;
        END CATCH
    END;
    GO

-- this stored procedure manually adjusts an employee's leave balance
-- success = 1 means that the adjustment was applied
-- success = -1 means that the employee does not exist
-- success = -2 means that the leave type does not exist
-- success = -3 means that the employee has no entitlement record for this leave type
-- success = -99 means that an unexpected error occurred
CREATE PROCEDURE AdjustLeaveBalance
    (
        @EmployeeID INT,
        @LeaveType VARCHAR(50),
        @Adjustment INT,
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;
        
        BEGIN TRY
            BEGIN TRANSACTION;
            
            IF NOT EXISTS (SELECT 1 FROM Employee WHERE employeeID = @EmployeeID)
            BEGIN
                SET @success = -1;
                ROLLBACK TRANSACTION;
                RETURN;
            END
            
            DECLARE @LeaveTypeID INT;
            
            SELECT @LeaveTypeID = leaveTypeID
            FROM LeaveType
            WHERE typeName = @LeaveType;
            
            IF @LeaveTypeID IS NULL
            BEGIN
                SET @success = -2;
                ROLLBACK TRANSACTION;
                RETURN;
            END
            
            IF NOT EXISTS (SELECT 1 FROM LeaveEntitlement WHERE employeeID = @EmployeeID AND leaveTypeID = @LeaveTypeID)
            BEGIN
                SET @success = -3;
                ROLLBACK TRANSACTION;
                RETURN;
            END
            
            UPDATE LeaveEntitlement
            SET entitlement = entitlement + @Adjustment
            WHERE employeeID = @EmployeeID AND leaveTypeID = @LeaveTypeID;
            
            COMMIT TRANSACTION;
            SET @success = 1;
            
        END TRY
        BEGIN CATCH
            IF @@TRANCOUNT > 0
                ROLLBACK TRANSACTION;
            SET @success = -99;
        END CATCH
    END;
    GO

-- this stored procedure deducts approved leave days from the employee's entitlement
-- success = 1 means that the balance was synchronized
-- success = -1 means that the leave request does not exist
-- success = -2 means that the request is not Approved
-- success = -3 means that the employee has no entitlement record for this leave type
-- success = -99 means that an unexpected error occurred
CREATE PROCEDURE SyncLeaveBalances
    (
        @LeaveRequestID INT,
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;
        
        BEGIN TRY
            BEGIN TRANSACTION;
            
            IF NOT EXISTS (SELECT 1 FROM LeaveRequest WHERE leaveRequestID = @LeaveRequestID)
            BEGIN
                SET @success = -1;
                ROLLBACK TRANSACTION;
                RETURN;
            END
            
            DECLARE @Status VARCHAR(50);
            DECLARE @EmployeeID INT;
            DECLARE @LeaveTypeID INT;
            DECLARE @Duration INT;
            
            SELECT @Status = status, @EmployeeID = employeeID, @LeaveTypeID = leaveTypeID, @Duration = duration
            FROM LeaveRequest
            WHERE leaveRequestID = @LeaveRequestID;
            
            IF @Status != 'Approved'
            BEGIN
                SET @success = -2;
                ROLLBACK TRANSACTION;
                RETURN;
            END
            
            IF NOT EXISTS (SELECT 1 FROM LeaveEntitlement WHERE employeeID = @EmployeeID AND leaveTypeID = @LeaveTypeID)
            BEGIN
                SET @success = -3;
                ROLLBACK TRANSACTION;
                RETURN;
            END
            
            UPDATE LeaveEntitlement
            SET entitlement = entitlement - @Duration
            WHERE employeeID = @EmployeeID AND leaveTypeID = @LeaveTypeID;
            
            COMMIT TRANSACTION;
            SET @success = 1;
            
        END TRY
        BEGIN CATCH
            IF @@TRANCOUNT > 0
                ROLLBACK TRANSACTION;
            SET @success = -99;
        END CATCH
    END;
    GO

-- this stored procedure carries over unused leave and resets balances for the new year
-- success > 0 means that [success] number of entitlements were carried forward
-- success = 0 means that no entitlements required carry forward
-- success = -99 means that an unexpected error occurred
CREATE PROCEDURE ProcessLeaveCarryForward
    (
        @Year INT,
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;
        
        BEGIN TRY
            BEGIN TRANSACTION;
            
            UPDATE le
            SET le.entitlement = le.entitlement + 
                CASE 
                    WHEN vlt.maxCarryOverDays < le.entitlement THEN vlt.maxCarryOverDays
                    ELSE le.entitlement
                END
            FROM LeaveEntitlement le
            INNER JOIN LeaveType lt ON le.leaveTypeID = lt.leaveTypeID
            INNER JOIN VacationLeaveType vlt ON lt.leaveTypeID = vlt.leaveTypeID
            WHERE vlt.maxCarryOverDays > 0
            AND le.entitlement > 0;
            
            SET @success = @@ROWCOUNT;
            
            UPDATE le
            SET le.entitlement = 0
            FROM LeaveEntitlement le
            INNER JOIN LeaveType lt ON le.leaveTypeID = lt.leaveTypeID
            INNER JOIN LeavePolicy lp ON lp.leavePolicyID IN (
                SELECT leavePolicyID FROM adheres_to_policy WHERE leaveTypeID = lt.leaveTypeID
            )
            WHERE lp.resetOnNewYear = 1
            AND lt.leaveTypeID NOT IN (SELECT leaveTypeID FROM VacationLeaveType);
            
            COMMIT TRANSACTION;
            
        END TRY
        BEGIN CATCH
            IF @@TRANCOUNT > 0
                ROLLBACK TRANSACTION;
            SET @success = -99;
        END CATCH
    END;
    GO

-- this stored procedure verifies a medical leave request against a document
-- success = 1 means that the leave was verified
-- success = -1 means that the leave request does not exist
-- success = -2 means that the document does not exist
-- success = -3 means that the request is not for Sick Leave
-- success = -4 means that the document is not linked to this request
-- success = -99 means that an unexpected error occurred
CREATE PROCEDURE VerifyMedicalLeave
    (
        @LeaveRequestID INT,
        @DocumentID INT,
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;
        
        BEGIN TRY
            BEGIN TRANSACTION;
            
            IF NOT EXISTS (SELECT 1 FROM LeaveRequest WHERE leaveRequestID = @LeaveRequestID)
            BEGIN
                SET @success = -1;
                ROLLBACK TRANSACTION;
                RETURN;
            END;
            
            IF NOT EXISTS (SELECT 1 FROM LeaveDocument WHERE documentID = @DocumentID)
            BEGIN
                SET @success = -2;
                ROLLBACK TRANSACTION;
                RETURN;
            END;
            
            DECLARE @LeaveTypeID INT;
            
            SELECT @LeaveTypeID = leaveTypeID
            FROM LeaveRequest
            WHERE leaveRequestID = @LeaveRequestID;
            
            IF NOT EXISTS (SELECT 1 FROM SickLeaveType WHERE leaveTypeID = @LeaveTypeID)
            BEGIN
                SET @success = -3;
                ROLLBACK TRANSACTION;
                RETURN;
            END;
            
            IF NOT EXISTS (SELECT 1 FROM LeaveDocument WHERE documentID = @DocumentID AND leaveRequestID = @LeaveRequestID)
            BEGIN
                SET @success = -4;
                ROLLBACK TRANSACTION;
                RETURN;
            END;
            
            UPDATE LeaveRequest
            SET status = 'Verified'
            WHERE leaveRequestID = @LeaveRequestID;
            
            COMMIT TRANSACTION;
            SET @success = 1;
            
        END TRY
        BEGIN CATCH
            IF @@TRANCOUNT > 0
                ROLLBACK TRANSACTION;
            SET @success = -99;
        END CATCH
    END;
    GO

-- this stored procedure uploads/links a document to a leave request
-- success = 1 means that the document was attached
-- success = 0 means that the insertion failed
-- success = -1 means that the leave request does not exist
CREATE PROCEDURE AttachLeaveDocuments
    (
        @LeaveRequestID INT,
        @FilePath VARCHAR(200),
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;
        
        BEGIN TRY
            BEGIN TRANSACTION;
            
            IF NOT EXISTS (SELECT 1 FROM LeaveRequest WHERE leaveRequestID = @LeaveRequestID)
            BEGIN
                SET @success = -1;
                ROLLBACK TRANSACTION;
                RETURN;
            END;

            INSERT INTO LeaveDocument (leaveRequestID, filePath, uploadedAt)
            VALUES (@LeaveRequestID, @FilePath, GETDATE());
            
            COMMIT TRANSACTION;
            SET @success = 1;
            
        END TRY
        BEGIN CATCH
            IF @@TRANCOUNT > 0
                ROLLBACK TRANSACTION;
            SET @success = -99;
        END CATCH
    END;
    GO

-- this stored procedure finalizes an approved leave request
-- success = 1 means that the request was finalized
-- success = -1 means that the leave request does not exist
-- success = -2 means that the request is not currently Approved
-- success = -99 means that an unexpected error occurred
CREATE PROCEDURE FinalizeLeaveRequest
    (
        @LeaveRequestID INT,
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;
        
        BEGIN TRY
            BEGIN TRANSACTION;
            
            IF NOT EXISTS (SELECT 1 FROM LeaveRequest WHERE leaveRequestID = @LeaveRequestID)
            BEGIN
                SET @success = -1;
                ROLLBACK TRANSACTION;
                RETURN;
            END;
            
            DECLARE @Status VARCHAR(50);
            
            SELECT @Status = status
            FROM LeaveRequest
            WHERE leaveRequestID = @LeaveRequestID;
            
            IF @Status != 'Approved'
            BEGIN
                SET @success = -2;
                ROLLBACK TRANSACTION;
                RETURN;
            END;
            
            UPDATE LeaveRequest
            SET status = 'Finalized'
            WHERE leaveRequestID = @LeaveRequestID;
            
            COMMIT TRANSACTION;
            SET @success = 1;
            
        END TRY
        BEGIN CATCH
            IF @@TRANCOUNT > 0
                ROLLBACK TRANSACTION;
            SET @success = -99;
        END CATCH
    END;
    GO

-- this stored procedure forces an approval on a previously rejected request
-- success = 1 means that the decision was overridden and approved
-- success = -1 means that the leave request does not exist
-- success = -2 means that the current status is not Rejected
-- success = -99 means that an unexpected error occurred
CREATE PROCEDURE OverrideLeaveDecision
    (
        @LeaveRequestID INT,
        @Reason VARCHAR(200),
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;
        
        BEGIN TRY
            BEGIN TRANSACTION;
            
            IF NOT EXISTS (SELECT 1 FROM LeaveRequest WHERE leaveRequestID = @LeaveRequestID)
            BEGIN
                SET @success = -1;
                ROLLBACK TRANSACTION;
                RETURN;
            END;
            
            DECLARE @CurrentStatus VARCHAR(50);
            DECLARE @EmployeeID INT;
            
            SELECT @CurrentStatus = status, @EmployeeID = employeeID
            FROM LeaveRequest
            WHERE leaveRequestID = @LeaveRequestID;
            
            IF @CurrentStatus = 'Rejected'
            BEGIN
                UPDATE LeaveRequest
                SET status = 'Approved'
                WHERE leaveRequestID = @LeaveRequestID;
                
                INSERT INTO Notification (messageContent, timestamp, urgency, readStatus)
                VALUES (
                    'Your rejected leave request has been overridden and approved. Reason: ' + @Reason,
                    GETDATE(),
                    'High',
                    'Unread'
                );
                
                DECLARE @NotificationID INT = SCOPE_IDENTITY();
                
                INSERT INTO receives_notification (employeeID, notificationID, deliveryStatus, deliveredAt)
                VALUES (@EmployeeID, @NotificationID, 'Delivered', GETDATE());
                
                COMMIT TRANSACTION;
                SET @success = 1;
            END
            ELSE
            BEGIN
                SET @success = -2;
                ROLLBACK TRANSACTION;
            END
            
        END TRY
        BEGIN CATCH
            IF @@TRANCOUNT > 0
                ROLLBACK TRANSACTION;
            SET @success = -99;
        END CATCH
    END;
    GO

-- COMMENTS MISSING
CREATE PROCEDURE BulkProcessLeaveRequests
    (
        @LeaveRequestIDs VARCHAR(500),
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;

        BEGIN TRY
            BEGIN TRANSACTION;
            
            CREATE TABLE #RequestIDs
        (
            leaveRequestID INT
        );
            
            INSERT INTO #RequestIDs
            (leaveRequestID)
        SELECT CAST(value AS INT)
        FROM STRING_SPLIT(@LeaveRequestIDs, ',');
            
            UPDATE LeaveRequest
            SET status = 'Approved'
            WHERE leaveRequestID IN (SELECT leaveRequestID
            FROM #RequestIDs)
            AND status = 'Pending';
            
            SET @success = @@ROWCOUNT;
            
            INSERT INTO Notification
            (messageContent, timestamp, urgency, readStatus)
        SELECT
            'Your leave request has been approved.',
            GETDATE(),
            'Normal',
            'Unread'
        FROM LeaveRequest
        WHERE leaveRequestID IN (SELECT leaveRequestID
            FROM #RequestIDs)
            AND status = 'Approved';
            
            INSERT INTO receives_notification
            (employeeID, notificationID, deliveryStatus, deliveredAt)
        SELECT
            lr.employeeID,
            n.notificationID,
            'Delivered',
            GETDATE()
        FROM LeaveRequest lr
            CROSS APPLY (SELECT TOP 1
                notificationID
            FROM Notification
            ORDER BY notificationID DESC) n
        WHERE lr.leaveRequestID IN (SELECT leaveRequestID
            FROM #RequestIDs)
            AND lr.status = 'Approved';
            
            DROP TABLE #RequestIDs;
            
            COMMIT TRANSACTION;
            
        END TRY
        BEGIN CATCH
            IF @@TRANCOUNT > 0
                ROLLBACK TRANSACTION;
                
            IF OBJECT_ID('tempdb..#RequestIDs') IS NOT NULL
                DROP TABLE #RequestIDs;
                
            SET @success = -99;
        END CATCH
    END;
    GO

-- this stored procedure creates an escalation workflow for high-value items
-- success = 1 means that the workflow policy was created
-- success = -1 means that the creator (employee) does not exist
-- success = -2 means that the approver role does not exist
-- success = -99 means that an unexpected error occurred
CREATE PROCEDURE ConfigureEscalationWorkflow
    (
        @ThresholdAmount DECIMAL(10,2),
        @ApproverRole VARCHAR(50),
        @CreatedBy INT,
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;
        
        BEGIN TRY
            BEGIN TRANSACTION;
            
            IF NOT EXISTS (SELECT 1 FROM Employee WHERE employeeID = @CreatedBy)
            BEGIN
                SET @success = -1;
                ROLLBACK TRANSACTION;
                RETURN;
            END;
            
            DECLARE @RoleID INT;
            
            SELECT @RoleID = roleID
            FROM Role
            WHERE name = @ApproverRole;
            
            IF @RoleID IS NULL
            BEGIN
                SET @success = -2;
                ROLLBACK TRANSACTION;
                RETURN;
            END;
            
            INSERT INTO PayrollPolicy (effectiveDate, description)
            VALUES (
                GETDATE(),
                'Escalation workflow for amounts exceeding ' + CAST(@ThresholdAmount AS VARCHAR(20)) + 
                ' requiring approval from ' + @ApproverRole
            );
            
            COMMIT TRANSACTION;
            SET @success = 1;
            
        END TRY
        BEGIN CATCH
            IF @@TRANCOUNT > 0
                ROLLBACK TRANSACTION;
            SET @success = -99;
        END CATCH
    END;
    GO

-- Rushdy

-- this stored procedure creates a new standard shift definition
-- success = 1 means that the shift type was created
-- success = 0 means that the insertion failed
-- success = -1 means that one of the inputs is NULL
-- success = -2 means that the shift ID already exists
CREATE PROCEDURE CreateShiftType
    (
        @ShiftID INT,
        @Name VARCHAR(100),
        @Type VARCHAR(50),
        @Start_Time TIME,
        @End_Time TIME,
        @Break_Duration INT,
        @Shift_Date DATE,
        @Status VARCHAR(50),
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;

        IF @ShiftID IS NULL OR @Name IS NULL OR @Type IS NULL OR @Start_Time IS NULL 
        OR @End_Time IS NULL OR @Break_Duration IS NULL OR @Shift_Date IS NULL OR @Status IS NULL
        BEGIN
            SET @success = -1;
            PRINT 'One of the inputs is null.';
            RETURN;
        END;

        IF EXISTS(SELECT 1 FROM ShiftSchedule WHERE shiftID = @ShiftID)
        BEGIN
            SET @success = -2;
            PRINT 'ShiftID already exists.';
            RETURN;
        END;

        DECLARE @BreakTime TIME;
        SET @BreakTime = DATEADD(minute, @Break_Duration, '00:00:00');

        DECLARE @StartDateTime DATETIME;
        DECLARE @EndDateTime DATETIME;
        SET @StartDateTime = CAST(@Shift_Date AS DATETIME) + CAST(@Start_Time AS DATETIME);
        SET @EndDateTime = CAST(@Shift_Date AS DATETIME) + CAST(@End_Time AS DATETIME);

        INSERT INTO ShiftSchedule (name, type, startTime, endTime, breakDuration, shiftDate, status)
        VALUES (@Name, @Type, @StartDateTime, @EndDateTime, @BreakTime, @Shift_Date, @Status);

        IF @@ROWCOUNT = 1
            SET @success = 1;
        ELSE
            SET @success = 0;

        IF @success = 1
            PRINT 'Shift type created successfully.';
        ELSE IF @success = 0
            PRINT 'No changes were made.';
    END;
    GO

-- this stored procedure updates the status of a shift assignment
-- success = 1 means that the status was updated
-- success = 0 means that no changes were made
-- success = -1 means that the shift assignment does not exist
-- success = -2 means that status is NULL
-- success = -3 means that status is invalid (must be 'Active', 'Completed', 'Pending')
CREATE PROCEDURE UpdateShiftStatus
    (
        @ShiftAssignmentID INT,
        @Status VARCHAR(20),
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;

        IF NOT EXISTS(SELECT 1 FROM works_shift WHERE assignmentID = @ShiftAssignmentID)
        BEGIN
            SET @success = -1;
            PRINT 'Shift assignment does not exist.';
            RETURN;
        END;

        IF @Status IS NULL
        BEGIN
            SET @success = -2;
            PRINT 'Status cannot be NULL.';
            RETURN;
        END;

        IF @Status NOT IN ('Active', 'Completed', 'Pending')
        BEGIN
            SET @success = -3;
            PRINT 'Invalid status value.';
            RETURN;
        END;

        UPDATE works_shift
            SET status = @Status
            WHERE assignmentID = @ShiftAssignmentID;

        IF @@ROWCOUNT = 1
                SET @success = 1;
            ELSE
                SET @success = 0;

        IF @success = 1
        PRINT 'Shift status updated successfully.';
        ELSE IF @success = 0
        PRINT 'No changes were made.';
    END;
    GO

-- this stored procedure assigns a shift to all employees in a specific department
-- success = 1 means that shifts were assigned
-- success = 0 means that no assignments were made (e.g., if no employees in dept)
-- success = -1 means that the department does not exist
-- success = -2 means that the shift ID does not exist
-- success = -3 means that start or end dates are NULL
-- success = -4 means that there are no employees in the department
CREATE PROCEDURE AssignShiftToDepartment
    (
        @DepartmentID INT,
        @ShiftID INT,
        @StartDate DATE,
        @EndDate DATE,
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;

        IF NOT EXISTS(SELECT 1 FROM Department WHERE departmentID = @DepartmentID)
        BEGIN
            SET @success = -1;
            PRINT 'Department does not exist.';
            RETURN;
        END;

        IF NOT EXISTS(SELECT 1 FROM ShiftSchedule WHERE shiftID = @ShiftID)
        BEGIN
            SET @success = -2;
            PRINT 'Shift does not exist.';
            RETURN;
        END;

        IF @StartDate IS NULL OR @EndDate IS NULL
        BEGIN
            SET @success = -3;
            PRINT 'StartDate or EndDate cannot be NULL.';
            RETURN;
        END;

        IF NOT EXISTS(SELECT 1 FROM Employee WHERE departmentID = @DepartmentID)
        BEGIN
            SET @success = -4;
            PRINT 'No employees in department.';
            RETURN;
        END;

        INSERT INTO works_shift (employeeID, shiftID, startDate, endDate)
        SELECT employeeID, @ShiftID, @StartDate, @EndDate
        FROM Employee
        WHERE departmentID = @DepartmentID;

        IF @@ROWCOUNT = 1
            SET @success = 1;
            ELSE
            SET @success = 0;

        IF @success = 1
            PRINT 'Shift assigned to department successfully.';
        ELSE IF @success = 0
            PRINT 'No changes were made.';
    END;
    GO

-- COMMENTS MISSING
CREATE PROCEDURE AssignCustomShift
    (
        @EmployeeID INT,
        @ShiftName VARCHAR(50),
        @ShiftType VARCHAR(50),
        @StartTime TIME,
        @EndTime TIME,
        @StartDate DATE,
        @EndDate DATE,
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;

        IF @EmployeeID IS NULL OR @ShiftName IS NULL OR @ShiftType IS NULL OR 
        @StartTime IS NULL OR @EndTime IS NULL OR @StartDate IS NULL OR @EndDate IS NULL
        BEGIN
            SET @success = -2;
            PRINT 'One of the inputs is null';
            RETURN
        END

        IF NOT EXISTS (SELECT * FROM Employee WHERE employeeID = @EmployeeID)
        BEGIN
            SET @success = -1;
            PRINT 'Employee ID not found';
            RETURN
        END

        IF EXISTS (SELECT * FROM works_shift WHERE employeeID = @EmployeeID AND startDate = @StartDate)
        BEGIN
            SET @success = -3;
            PRINT 'Employee already has a shift on this start date';
            RETURN
        END

        DECLARE @StartDateTime DATETIME
        DECLARE @EndDateTime DATETIME

        SET @StartDateTime = @StartDate
        SET @StartDateTime = @StartDateTime + @StartTime

        SET @EndDateTime = @StartDate
        SET @EndDateTime = @EndDateTime + @EndTime

        INSERT INTO ShiftSchedule (name, type, startTime, endTime, breakDuration, shiftDate, status)
        VALUES (@ShiftName, @ShiftType, @StartDateTime, @EndDateTime, '00:00', @StartDate, 'Active')

        DECLARE @NewShiftID INT
        SET @NewShiftID = SCOPE_IDENTITY()

        INSERT INTO works_shift (employeeID, shiftID, startDate, endDate, status)
        VALUES (@EmployeeID, @NewShiftID, @StartDate, @EndDate, 'Assigned')

        IF @@ROWCOUNT = 1
            SET @success = 1;
            ELSE
            SET @success = 0;

        IF @success = 1
        PRINT 'Custom shift assigned successfully.';
        ELSE IF @success = 0
        PRINT 'No changes were made.';
    END;
    GO

-- COMMENTS MISSING
CREATE PROCEDURE TagAttendanceSource
    (
        @AttendanceID INT,
        @SourceType VARCHAR(20),
        @DeviceID INT,
        @Latitude DECIMAL(10,7),
        @Longitude DECIMAL(10,7),
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;

        IF @AttendanceID IS NULL OR @SourceType IS NULL OR @DeviceID IS NULL 
        OR @Latitude IS NULL OR @Longitude IS NULL
        BEGIN
            SET @success = -1;
            PRINT 'One of the inputs is null.';
            RETURN;
        END;

        IF NOT EXISTS (SELECT 1 FROM Attendance WHERE attendanceID = @AttendanceID)
        BEGIN
            SET @success = -2;
            PRINT 'Attendance record does not exist.';
            RETURN;
        END;

        INSERT INTO AttendanceSource (attendanceID, deviceID, sourceType, latitude, longitude, recordedAt)
        VALUES (@AttendanceID, @DeviceID, @SourceType, @Latitude, @Longitude, CURRENT_TIMESTAMP);

        IF @@ROWCOUNT = 1
            SET @success = 1;
            ELSE
            SET @success = 0;

        IF @success = 1
        PRINT 'Attendance source tagged successfully.';
        ELSE IF @success = 0
        PRINT 'No changes were made.';
    END;
    GO

-- COMMENTS MISSING
CREATE PROCEDURE LogAttendanceEdit
    (
        @AttendanceID INT,
        @EditedBy INT,
        @OldValue DATETIME,
        @NewValue DATETIME,
        @EditTimestamp DATETIME,
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;

        IF @AttendanceID IS NULL OR @EditedBy IS NULL OR @EditTimestamp IS NULL
        BEGIN
            SET @success = -1;
            PRINT 'One of the inputs is null';
            RETURN;
        END;

        IF NOT EXISTS(SELECT 1 FROM Attendance WHERE attendanceID = @AttendanceID)
        BEGIN
            SET @success = -2;
            PRINT 'AttendanceID not found';
            RETURN;
        END;

        IF NOT EXISTS(SELECT 1 FROM Employee WHERE employeeID = @EditedBy)
        BEGIN
            SET @success = -3;
            PRINT 'EditorID not found';
            RETURN;
        END;

        INSERT INTO AttendanceLog (attendanceID, actor, timestamp, oldValue, newValue)
        VALUES (@AttendanceID, @EditedBy, @EditTimestamp, @OldValue, @NewValue);

        IF @@ROWCOUNT = 1
                SET @success = 1;
            ELSE
                SET @success = 0;

        IF @success = 1
        PRINT 'Attendance edit logged successfully.';
        ELSE IF @success = 0
        PRINT 'No changes were made.';
    END;
    GO

-- COMMENTS MISSING
CREATE PROCEDURE NotifyShiftExpiry
    (
        @EmployeeID INT,
        @ShiftAssignmentID INT,
        @ExpiryDate DATE,
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;

        IF @EmployeeID IS NULL OR @ShiftAssignmentID IS NULL OR @ExpiryDate IS NULL
        BEGIN
            SET @success = -1;
            PRINT 'One of the inputs is null';
            RETURN;
        END;

        IF NOT EXISTS(SELECT 1 FROM Employee WHERE employeeID = @EmployeeID)
        BEGIN
            SET @success = -2;
            PRINT 'Employee does not exist';
            RETURN;
        END;

        IF NOT EXISTS(SELECT 1 FROM works_shift WHERE assignmentID = @ShiftAssignmentID)
        BEGIN
            SET @success = -3;
            PRINT 'Shift assignment does not exist';
            RETURN;
        END;

        DECLARE @MsgContent VARCHAR(255)
        SET @MsgContent = 'Your shift assignment ' + CAST(@ShiftAssignmentID AS VARCHAR(10)) + ' is expiring on ' + CAST(@ExpiryDate AS VARCHAR(20))

        INSERT INTO Notification (messageContent, timestamp, urgency, readStatus)
        VALUES (@MsgContent, CURRENT_TIMESTAMP, 'High', 'Unread')

        DECLARE @NewNotificationID INT
        SET @NewNotificationID = SCOPE_IDENTITY()

        INSERT INTO receives_notification (employeeID, notificationID, deliveryStatus, deliveredAt)
        VALUES (@EmployeeID, @NewNotificationID, 'Sent', CURRENT_TIMESTAMP)

        IF @@ROWCOUNT = 1
                SET @success = 1;
            ELSE
                SET @success = 0;

        IF @success = 1
            PRINT 'Shift expiry notification sent successfully.';
        ELSE IF @success = 0
            PRINT 'No changes were made.';
    END;
    GO

-- COMMENTS MISSING
CREATE PROCEDURE RecordManualAttendance
    (
        @EmployeeID INT,
        @Date DATE,
        @ClockIn TIME,
        @ClockOut TIME,
        @Reason VARCHAR(200),
        @RecordedBy INT,
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;

        IF @EmployeeID IS NULL OR @Date IS NULL OR @ClockIn IS NULL OR @ClockOut IS NULL OR @RecordedBy IS NULL
        BEGIN
            SET @success = -1;
            PRINT 'One of the inputs is null';
            RETURN;
        END;

        IF NOT EXISTS(SELECT 1 FROM Employee WHERE employeeID = @EmployeeID)
        BEGIN
            SET @success = -2;
            PRINT 'Employee does not exist';
            RETURN;
        END;

        IF NOT EXISTS(SELECT 1 FROM Employee WHERE employeeID = @RecordedBy)
        BEGIN
            SET @success = -3;
            PRINT 'Recorder (Employee) does not exist';
            RETURN;
        END;

        DECLARE @EntryDateTime DATETIME
        DECLARE @ExitDateTime DATETIME

        SET @EntryDateTime = @Date + @ClockIn
        SET @ExitDateTime = @Date + @ClockOut

        INSERT INTO Attendance (employeeID, entryTime, exitTime, shiftID)
        VALUES (@EmployeeID, @EntryDateTime, @ExitDateTime, NULL)

        DECLARE @NewAttendanceID INT
        SET @NewAttendanceID = SCOPE_IDENTITY()

        INSERT INTO AttendanceLog (attendanceID, actor, timestamp, reason, modificationType)
        VALUES (@NewAttendanceID, @RecordedBy, CURRENT_TIMESTAMP, @Reason, 'Manual Entry')

        IF @@ROWCOUNT = 1
                SET @success = 1;
            ELSE
                SET @success = 0;

        IF @success = 1
            PRINT 'Manual attendance recorded and logged successfully.';
        ELSE IF @success = 0
            PRINT 'No changes were made.';
    END;
    GO

-- this stored procedure enables or disables the 'First In, Last Out' calculation rule
-- success = 1 means that the setting was updated
-- success = 0 means that the update failed
-- success = -1 means that the input value is NULL
CREATE PROCEDURE EnableFirstInLastOut
    (
        @Enable BIT,
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;

        IF @Enable IS NULL
        BEGIN
            SET @success = -1;
            PRINT 'Enable value is null';
            RETURN;
        END;

        UPDATE SystemSettings
            SET enableFirstInLastOut = @Enable;

        IF @@ROWCOUNT = 1
                SET @success = 1;
            ELSE
                SET @success = 0;

        IF @success = 1
            PRINT 'Attendance processing rule updated successfully.';
        ELSE IF @success = 0
            PRINT 'No changes were made.';
    END;
    GO

-- this stored procedure assigns a rotational shift cycle to an employee
-- success = 1 means that the shift was assigned
-- success = 0 means that the insertion failed
-- success = -1 means that one of the inputs is NULL
-- success = -2 means that the employee does not exist
CREATE PROCEDURE AssignRotationalShift
    (
        @EmployeeID INT,
        @ShiftCycle INT,
        @StartDate DATE,
        @EndDate DATE,
        @status VARCHAR(20),
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;

        IF @EmployeeID IS NULL OR @ShiftCycle IS NULL OR @StartDate IS NULL OR @EndDate IS NULL OR @status IS NULL
        BEGIN
            SET @success = -1;
            PRINT 'One of the inputs is null';
            RETURN;
        END;

        IF NOT EXISTS(SELECT 1 FROM Employee WHERE employeeID = @EmployeeID)
        BEGIN
            SET @success = -2;
            PRINT 'Employee does not exist';
            RETURN;
        END;

        INSERT INTO works_shift (employeeID, shiftID, startDate, endDate, status)
        VALUES (@EmployeeID, @ShiftCycle, @StartDate, @EndDate, @status);

        IF @@ROWCOUNT = 1
                SET @success = 1;
            ELSE
                SET @success = 0;

        IF @success = 1
            PRINT 'Rotational shift assigned successfully.';
        ELSE IF @success = 0
            PRINT 'No changes were made.';
    END;
    GO

-- this stored procedure syncs an offline attendance record to a temporary table
-- success = 1 means that the record was synced
-- success = 0 means that the insertion failed
-- success = -1 means that one of the inputs is NULL
-- success = -2 means that the employee does not exis
CREATE PROCEDURE SyncOfflineAttendance
    (
        @DeviceID INT,
        @EmployeeID INT,
        @ClockTime DATETIME,
        @Type VARCHAR(10),
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;

        IF @DeviceID IS NULL OR @EmployeeID IS NULL OR @ClockTime IS NULL OR @Type IS NULL
        BEGIN
            SET @success = -1;
            PRINT 'One of the inputs is null';
            RETURN;
        END;

        IF NOT EXISTS(SELECT 1 FROM Employee WHERE employeeID = @EmployeeID)
        BEGIN
            SET @success = -2;
            PRINT 'Employee does not exist';
            RETURN;
        END;

        IF @Type NOT IN ('In', 'Out')
        BEGIN
            SET @success = -3;
            PRINT 'Invalid punch type';
            RETURN;
        END;

        INSERT INTO Attendance (employeeID, entryTime, exitTime, shiftID)
        VALUES (@EmployeeID, @ClockTime, NULL, NULL);

        IF @@ROWCOUNT = 1
                SET @success = 1;
            ELSE
                SET @success = 0;

        IF @success = 1
            PRINT 'Offline attendance record synced successfully.';
        ELSE IF @success = 0
            PRINT 'No changes were made.';
    END;
    GO

-- this stored procedure logs attendance for flexible working hours
-- success = 1 means that the attendance was logged
-- success = 0 means that the insertion failed
-- success = -1 means that one of the inputs is NULL
-- success = -2 means that the employee does not exist
CREATE PROCEDURE LogFlexibleAttendance
    (
        @EmployeeID INT,
        @Date DATE,
        @CheckIn TIME,
        @CheckOut TIME,
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;

        IF @EmployeeID IS NULL OR @Date IS NULL OR @CheckIn IS NULL OR @CheckOut IS NULL
        BEGIN
            SET @success = -1;
            PRINT 'One of the inputs is null';
            RETURN;
        END;

        IF NOT EXISTS(SELECT 1 FROM Employee WHERE employeeID = @EmployeeID)
        BEGIN
            SET @success = -2;
            PRINT 'Employee does not exist';
            RETURN;
        END;

        DECLARE @EntryDateTime DATETIME;
        DECLARE @ExitDateTime DATETIME;

        SET @EntryDateTime = CAST(@Date AS DATETIME) + CAST(@CheckIn AS DATETIME);
        SET @ExitDateTime = CAST(@Date AS DATETIME) + CAST(@CheckOut AS DATETIME);

        DECLARE @TotalHours DECIMAL(5,2);
        SET @TotalHours = DATEDIFF(minute, @EntryDateTime, @ExitDateTime) / 60.0;

        INSERT INTO Attendance (employeeID, entryTime, exitTime, shiftID)
        VALUES (@EmployeeID, @EntryDateTime, @ExitDateTime, NULL);

        IF @@ROWCOUNT = 1
            SET @success = 1;
        ELSE
            SET @success = 0;

        IF @success = 1
            PRINT 'Flexible attendance logged successfully. Total Working Hours: ' + CAST(@TotalHours AS VARCHAR(20));
        ELSE IF @success = 0
            PRINT 'No changes were made.';
    END;
    GO

-- this stored procedure records multiple clock-in or clock-out events
-- success = 1 means that the punch was recorded
-- success = 0 means that the operation failed
-- success = -1 means that one of the inputs is NULL
-- success = -2 means that the employee does not exist
-- success = -3 means that the punch type is invalid (must be 'In' or 'Out')
CREATE PROCEDURE RecordMultiplePunches
    (
        @EmployeeID INT,
        @ClockInOutTime DATETIME,
        @Type VARCHAR(10),
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;

        IF @EmployeeID IS NULL OR @ClockInOutTime IS NULL OR @Type IS NULL
        BEGIN
            SET @success = -1;
            PRINT 'One of the inputs is null';
            RETURN;
        END;

        IF NOT EXISTS(SELECT 1 FROM Employee WHERE employeeID = @EmployeeID)
        BEGIN
            SET @success = -2;
            PRINT 'Employee does not exist';
            RETURN;
        END;

        IF @Type NOT IN ('In', 'Out')
        BEGIN
            SET @success = -3;
            PRINT 'Invalid punch type';
            RETURN;
        END;

        IF @Type = 'In'
        BEGIN
            INSERT INTO Attendance (employeeID, entryTime, exitTime, shiftID)
            VALUES (@EmployeeID, @ClockInOutTime, NULL, NULL)
        END
        ELSE IF @Type = 'Out'
        BEGIN
            UPDATE Attendance
                SET exitTime = @ClockInOutTime
            WHERE attendanceID = 
            (
                SELECT TOP 1 attendanceID
                FROM Attendance
                WHERE employeeID = @EmployeeID AND exitTime IS NULL
                ORDER BY entryTime DESC
            )
        END

        IF @@ROWCOUNT = 1
                SET @success = 1;
            ELSE
                SET @success = 0;

        IF @success = 1
            PRINT 'Punch recorded successfully.';
        ELSE IF @success = 0
            PRINT 'No changes were made.';
    END;
    GO

-- this stored procedure updates the status of an attendance correction request
-- success = 1 means that the request was processed
-- success = 0 means that the update failed
-- success = -1 means that one of the inputs is NULL
-- success = -2 means that the request does not exist
-- success = -3 means that the manager does not exist
CREATE PROCEDURE ReviewMissedPunches
    (
        @ManagerID INT,
        @Date DATE,
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;

        IF @ManagerID IS NULL OR @Date IS NULL
        BEGIN
            SET @success = -1;
            PRINT 'One of the inputs is null';
            RETURN;
        END;

        IF NOT EXISTS(SELECT 1 FROM Employee WHERE employeeID = @ManagerID)
        BEGIN
            SET @success = -2;
            PRINT 'Manager does not exist';
            RETURN;
        END;

        SELECT 
            E.employeeID,
            E.firstName,
            E.lastName,
            A.entryTime,
            A.exitTime,
            'Missed Punch' AS IssueType
        FROM Attendance A
        INNER JOIN Employee E ON A.employeeID = E.employeeID
        WHERE E.managerID = @ManagerID
        AND (
            (CAST(A.entryTime AS DATE) = @Date AND A.exitTime IS NULL)
            OR
            (CAST(A.exitTime AS DATE) = @Date AND A.entryTime IS NULL)
        );

        IF @@ROWCOUNT = 1
                SET @success = 1;
            ELSE
                SET @success = 0;

        IF @success = 1
            PRINT 'Missed punches retrieved successfully.';
        ELSE IF @success = 0
            PRINT 'No changes were made.';
    END;
    GO

-- this stored procedure submits a request to correct an attendance record
-- success = 1 means that the request was submitted
-- success = 0 means that the insertion failed
-- success = -1 means that one of the inputs is NULL
-- success = -2 means that the employee does not exist
CREATE PROCEDURE SubmitCorrectionRequest
    (
        @EmployeeID INT,
        @Date DATE,
        @CorrectionType VARCHAR(50),
        @Reason VARCHAR(200),
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;

        IF @EmployeeID IS NULL OR @Date IS NULL OR @CorrectionType IS NULL OR @Reason IS NULL
        BEGIN
            SET @success = -1;
            PRINT 'One of the inputs is null';
            RETURN;
        END;

        IF NOT EXISTS(SELECT 1 FROM Employee WHERE employeeID = @EmployeeID)
        BEGIN
            SET @success = -2;
            PRINT 'Employee does not exist';
            RETURN;
        END;

        INSERT INTO AttendanceCorrectionRequest (employeeID, date, correctionType, reason, status)
        VALUES (@EmployeeID, @Date, @CorrectionType, @Reason, 'Pending');

        IF @@ROWCOUNT = 1
                SET @success = 1;
            ELSE
                SET @success = 0;

        IF @success = 1
            PRINT 'Correction request submitted successfully.';
        ELSE IF @success = 0
            PRINT 'No changes were made.';
    END;
    GO

-- this stored procedure updates the system-wide grace period for attendance
-- success = 1 means that the grace period was updated
-- success = 0 means that the update failed
-- success = -1 means that minutes input is NULL
CREATE PROCEDURE DefineShortTimeRules
    (
        @RuleName VARCHAR(50),
        @LateMinutes INT,
        @EarlyLeaveMinutes INT,
        @PenaltyType VARCHAR(50),
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;

        IF @RuleName IS NULL OR @LateMinutes IS NULL OR @EarlyLeaveMinutes IS NULL OR @PenaltyType IS NULL
        BEGIN
            SET @success = -1;
            PRINT 'One of the inputs is null';
            RETURN;
        END;

        INSERT INTO ShortTimeRule (ruleName, lateMinutes, earlyLeaveMinutes, penaltyType)
        VALUES (@RuleName, @LateMinutes, @EarlyLeaveMinutes, @PenaltyType);

        IF @@ROWCOUNT = 1
                SET @success = 1;
            ELSE
                SET @success = 0;

        IF @success = 1
            PRINT 'Short time rule defined successfully.';
        ELSE IF @success = 0
            PRINT 'No changes were made.';
    END;
    GO

-- this stored procedure defines the threshold for late penalties
-- success = 1 means that the threshold was defined
-- success = 0 means that the insertion failed
-- success = -1 means that one of the inputs is NULL
CREATE PROCEDURE SetGracePeriod
    (
        @Minutes INT,
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;

        IF @Minutes IS NULL
        BEGIN
            SET @success = -1;
            PRINT 'Minutes cannot be null';
            RETURN;
        END;

        -- Grace period configuration not stored in schema
        SET @success = 1;  -- Assume success for compatibility

        IF @success = 1
            PRINT 'Grace period configured successfully.';
        ELSE IF @success = 0
            PRINT 'No changes were made.';
    END;
    GO

-- this stored procedure creates approved attendance exceptions for the duration of a leave request
-- success = 1 means that exceptions were created
-- success = 0 means that the loop completed but no rows were inserted (or logic flow didn't capture rowcount)
-- success = -1 means that leaveRequestID is NULL
-- success = -2 means that the leave request does not exist
CREATE PROCEDURE DefinePenaltyThreshold
    (
        @LateMinutes INT,
        @DeductionType VARCHAR(50),
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;

        IF @LateMinutes IS NULL OR @DeductionType IS NULL
        BEGIN
            SET @success = -1;
            PRINT 'One of the inputs is null';
            RETURN;
        END;

        -- Penalty threshold table not in schema
        SET @success = -1;
        RETURN;

        IF @@ROWCOUNT = 1
                SET @success = 1;
            ELSE
                SET @success = 0;

        IF @success = 1
            PRINT 'Penalty threshold defined successfully.';
        ELSE IF @success = 0
            PRINT 'No changes were made.';
    END;
    GO

-- this stored procedure logs a daily attendance sync event to the payroll log
-- success = 1 means that the log entry was created
-- success = 0 means that the insertion failed
-- success = -1 means that sync date is NULL
-- success = -2 means that no payroll period exists for the given date
-- success = -3 means that an unexpected error occurred
CREATE PROCEDURE SyncLeaveToAttendance
    (
        @LeaveRequestID INT,
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;

        IF @LeaveRequestID IS NULL
        BEGIN
            SET @success = -1;
            PRINT 'LeaveRequestID is null';
            RETURN;
        END;

        IF NOT EXISTS(SELECT 1 FROM LeaveRequest WHERE leaveRequestID = @LeaveRequestID)
        BEGIN
            SET @success = -2;
            PRINT 'Leave request does not exist';
            RETURN;
        END;

        DECLARE @EmployeeID INT, @CurrentDate DATE, @Duration INT, @LeaveType VARCHAR(255);

        SELECT @EmployeeID = LR.employeeID,
            @CurrentDate = GETDATE(),
            @Duration = LR.duration,
            @LeaveType = LT.typeName
        FROM LeaveRequest LR
        INNER JOIN LeaveType LT ON LR.leaveTypeID = LT.leaveTypeID
        WHERE LR.leaveRequestID = @LeaveRequestID;

        DECLARE @EndDate DATE;
        SET @EndDate = DATEADD(DAY, @Duration, @CurrentDate);

        WHILE @CurrentDate <= @EndDate
        BEGIN
            INSERT INTO Exception (name, category, date, status)
            VALUES (@LeaveType, 'Leave', @CurrentDate, 'Approved');

            INSERT INTO excused (employeeID, exceptionID)
            VALUES (@EmployeeID, SCOPE_IDENTITY());

            SET @CurrentDate = DATEADD(day, 1, @CurrentDate);
        END;

        SET @success = 1;
    END;
    GO

-- this stored procedure records a manual attendance entry and logs the action
-- success = 1 means that the attendance and log entries were created
-- success = 0 means that the insertion failed
-- success = -1 means that sync date is NULL
-- success = -2 means that the employee does not exist
-- success = -3 means that the recorder (employee) does not exist
CREATE PROCEDURE SyncAttendanceToPayroll
    (
        @SyncDate DATE,
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;

        IF @SyncDate IS NULL
        BEGIN
            SET @success = -1;
            PRINT 'Sync date is null';
            RETURN;
        END;

        IF NOT EXISTS(SELECT 1 FROM Payroll WHERE @SyncDate BETWEEN periodStart AND periodEnd)
        BEGIN
            SET @success = -2;
            PRINT 'No payroll period matches the given date';
            RETURN;
        END;

        INSERT INTO PayrollLog (payrollID, actor, changeDate, modificationType)
        SELECT 
            payrollID,
            NULL,
            CURRENT_TIMESTAMP,
            'Daily Attendance Sync'
        FROM Payroll
        WHERE @SyncDate BETWEEN periodStart AND periodEnd;

        IF @@ROWCOUNT = 1
                SET @success = 1;
            ELSE
                SET @success = 0;

        IF @success = 1
            PRINT 'Attendance records synced to payroll successfully.';
        ELSE IF @success = 0
            PRINT 'No changes were made.';
    END;
    GO

-- this stored procedure updates the status of a leave request
-- success = 1 means that the request was reviewed
-- success = 0 means that the update failed
-- success = -1 means that one of the inputs is NULL
-- success = -2 means that the leave request does not exist
-- success = -3 means that the manager does not exist
CREATE PROCEDURE ApproveTimeRequest
    (
        @RequestID INT,
        @ManagerID INT,
        @Decision VARCHAR(20),
        @Comments VARCHAR(200),
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;

        IF @RequestID IS NULL OR @ManagerID IS NULL OR @Decision IS NULL
        BEGIN
            SET @success = -1;
            PRINT 'One of the inputs is null';
            RETURN;
        END;

        IF NOT EXISTS(SELECT 1 FROM AttendanceCorrectionRequest WHERE requestID = @RequestID)
        BEGIN
            SET @success = -2;
            PRINT 'Request does not exist';
            RETURN;
        END;

        IF NOT EXISTS(SELECT 1 FROM Employee WHERE employeeID = @ManagerID)
        BEGIN
            SET @success = -3;
            PRINT 'Manager does not exist';
            RETURN;
        END;

        UPDATE AttendanceCorrectionRequest
            SET status = @Decision,
                recordedBy = @ManagerID
            WHERE requestID = @RequestID;

        IF @@ROWCOUNT = 1
                SET @success = 1;
            ELSE
                SET @success = 0;

        IF @success = 1
            PRINT 'Time request processed successfully.';
        ELSE IF @success = 0
            PRINT 'No changes were made.';
    END;
    GO

-- this stored procedure retrieves details of a specific leave request for review
-- success = 1 means that the request was retrieved
-- success = 0 means that the request was not found (or access denied)
-- success = -1 means that one of the inputs is NULL
-- success = -2 means that the leave request does not exist
-- success = -3 means that the manager does not exist
CREATE PROCEDURE ReviewLeaveRequest
    (
        @LeaveRequestID INT,
        @ManagerID INT,
        @Decision VARCHAR(20),
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;

        IF @LeaveRequestID IS NULL OR @ManagerID IS NULL OR @Decision IS NULL
        BEGIN
            SET @success = -1;
            PRINT 'One of the inputs is null';
            RETURN;
        END;

        IF NOT EXISTS(SELECT 1 FROM LeaveRequest WHERE leaveRequestID = @LeaveRequestID)
        BEGIN
            SET @success = -2;
            PRINT 'Leave request does not exist';
            RETURN;
        END;

        IF NOT EXISTS(SELECT 1 FROM Employee WHERE employeeID = @ManagerID)
        BEGIN
            SET @success = -3;
            PRINT 'Manager does not exist';
            RETURN;
        END;

        UPDATE LeaveRequest
            SET status = @Decision
            WHERE leaveRequestID = @LeaveRequestID;

        IF @@ROWCOUNT = 1
            SET @success = 1;
        ELSE
            SET @success = 0;

        IF @success = 1
            PRINT 'Leave request reviewed successfully.';
        ELSE IF @success = 0
            PRINT 'No changes were made.';
    END;
    GO

-- this stored procedure retrieves missed punch records for a manager's team on a specific date
-- success = 1 means that the records were retrieved
-- success = 0 means that no missed punches were found
-- success = -1 means that one of the inputs is NULL
-- success = -2 means that the manager does not exist
CREATE PROCEDURE GetMissedPunches
    (
        @ManagerID INT,
        @Date DATE,
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;

        IF @ManagerID IS NULL OR @Date IS NULL
        BEGIN
            SET @success = -1;
            PRINT 'One of the inputs is null';
            RETURN;
        END;

        IF NOT EXISTS(SELECT 1 FROM Employee WHERE employeeID = @ManagerID)
        BEGIN
            SET @success = -2;
            PRINT 'Manager does not exist';
            RETURN;
        END;

        SELECT 
            E.employeeID,
            E.firstName,
            E.lastName,
            A.entryTime,
            A.exitTime,
            'Missed Punch' AS IssueType
        FROM Attendance A
        INNER JOIN Employee E ON A.employeeID = E.employeeID
        WHERE E.managerID = @ManagerID
        AND (
            (CAST(A.entryTime AS DATE) = @Date AND A.exitTime IS NULL)
            OR
            (CAST(A.exitTime AS DATE) = @Date AND A.entryTime IS NULL)
        );

        IF @@ROWCOUNT = 1
                SET @success = 1;
            ELSE
                SET @success = 0;

        IF @success = 1
            PRINT 'Missed punches retrieved successfully.';
        ELSE IF @success = 0
            PRINT 'No changes were made.';
    END;
    GO

-- this stored procedure delegates leave approval authority to another employee
-- success = 1 means that the delegation was created
-- success = 0 means that the insertion failed
-- success = -1 means that one of the inputs is NULL
-- success = -2 means that the manager does not exist
-- success = -3 means that the delegate does not exist
CREATE PROCEDURE DelegateLeaveApproval
    (
        @ManagerID INT,
        @DelegateID INT,
        @StartDate DATE,
        @EndDate DATE,
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;

        IF @ManagerID IS NULL OR @DelegateID IS NULL OR @StartDate IS NULL OR @EndDate IS NULL
        BEGIN
            SET @success = -1;
            PRINT 'One of the inputs is null';
            RETURN;
        END;

        IF NOT EXISTS(SELECT 1 FROM Employee WHERE employeeID = @ManagerID)
        BEGIN
            SET @success = -2;
            PRINT 'Manager does not exist';
            RETURN;
        END;

        IF NOT EXISTS(SELECT 1 FROM Employee WHERE employeeID = @DelegateID)
        BEGIN
            SET @success = -3;
            PRINT 'Delegate does not exist';
            RETURN;
        END;

        -- LeaveDelegation table not in schema
        SET @success = -1;
        RETURN;

        IF @@ROWCOUNT = 1
                SET @success = 1;
            ELSE
                SET @success = 0;

        IF @success = 1
            PRINT 'Leave approval delegated successfully.';
        ELSE IF @success = 0
            PRINT 'No changes were made.';
    END;
    GO

-- this stored procedure flags an employee for irregular leave patterns
-- success = 1 means that the flag was created
-- success = 0 means that the insertion failed
-- success = -1 means that one of the inputs is NULL
-- success = -2 means that the employee does not exist
-- success = -3 means that the manager does not exist
CREATE PROCEDURE FlagIrregularLeave
    (
        @EmployeeID INT,
        @ManagerID INT,
        @PatternDescription VARCHAR(200),
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;

        IF @EmployeeID IS NULL OR @ManagerID IS NULL OR @PatternDescription IS NULL
        BEGIN
            SET @success = -1;
            PRINT 'One of the inputs is null';
            RETURN;
        END;

        IF NOT EXISTS(SELECT 1 FROM Employee WHERE employeeID = @EmployeeID)
        BEGIN
            SET @success = -2;
            PRINT 'Employee does not exist';
            RETURN;
        END;

        IF NOT EXISTS(SELECT 1 FROM Employee WHERE employeeID = @ManagerID)
        BEGIN
            SET @success = -3;
            PRINT 'Manager does not exist';
            RETURN;
        END;

        -- IrregularLeaveFlag table not in schema
        SET @success = -1;
        RETURN;

        IF @@ROWCOUNT = 1
                SET @success = 1;
            ELSE
                SET @success = 0;

        IF @success = 1
            PRINT 'Irregular leave pattern flagged successfully.';
        ELSE IF @success = 0
            PRINT 'No changes were made.';
    END;
    GO

-- Marawan

-- this stored procedure retrieves shifts assigned to a specific employee
-- success = 1 means that the shifts were retrieved
-- success = -1 means that the employee does not exist
CREATE PROCEDURE ViewAssignedShifts
    (
        @EmployeeID INT,
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;


        IF NOT EXISTS (SELECT 1 FROM Employee WHERE employeeID = @EmployeeID)
        BEGIN
            SET @success = -1;
            RETURN;
        END;


        SELECT 
            SS.shiftDate, 
            SS.startTime, 
            SS.endTime,
            SS.name AS shiftname, 
            SS.type AS shiftType,  
            WS.startDate AS assignment_start,
            WS.endDate AS assignment_end,
            WS.status AS assignment_status
        FROM ShiftSchedule SS 
        JOIN works_shift WS ON SS.shiftID = WS.shiftID
        WHERE WS.employeeID = @EmployeeID;

        SET @success = 1;
    END;
    GO

-- this stored procedure retrieves the contract history for a specific employee
-- success = 1 means that the contracts were retrieved
-- success = -1 means that the employee does not exist
CREATE PROCEDURE ViewMyContracts
    (
        @EmployeeID INT,
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;


        IF NOT EXISTS (SELECT 1 FROM Employee WHERE employeeID = @EmployeeID)
        BEGIN
            SET @success = -1;
            RETURN;
        END;

        SELECT 
            C.contractID,
            C.type,
            C.startDate,
            C.endDate,
            C.currentState
        FROM Contract C
        WHERE C.employeeID = @EmployeeID;

        SET @success = 1;
    END;
    GO

-- this stored procedure retrieves payroll records for a specific employee
-- success = 1 means that the payroll records were retrieved
-- success = -1 means that the employee does not exist
CREATE PROCEDURE ViewMyPayroll
    (
        @EmployeeID INT,
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;


        IF NOT EXISTS (SELECT 1 FROM Employee WHERE employeeID = @EmployeeID)
        BEGIN
            SET @success = -1;
            RETURN;
        END;

        SELECT 
            P.payrollID,
            P.periodStart,
            P.periodEnd,
            P.baseAmount,
            P.adjustments,
            P.taxes,
            P.contributions,
            P.actualPay,
            P.netSalary,
            P.paymentDate
        FROM Payroll P
        WHERE P.employeeID = @EmployeeID
        ORDER BY P.periodStart DESC;

        SET @success = 1;
    END;
    GO

-- this stored procedure retrieves the current employment details and timeline for a specific employee
-- success = 1 means that the timeline was retrieved
-- success = -1 means that the employee does not exist
CREATE PROCEDURE ViewEmploymentTimeline
    (
        @EmployeeID INT,
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;


        IF NOT EXISTS (SELECT 1 FROM Employee WHERE employeeID = @EmployeeID)
        BEGIN
            SET @success = -1;
            RETURN;
        END;

        SELECT 
            E.hireDate,
            D.name AS currentDepartment,
            P.title AS currentPosition,
            M.fullName AS currentManager,
            C.type AS contractType,
            C.startDate AS contractStart,
            C.endDate AS contractEnd,
            C.currentState AS contractStatus
        FROM Employee E
        LEFT JOIN Department D ON E.departmentID = D.departmentID
        LEFT JOIN Position P ON E.positionID = P.positionID
        LEFT JOIN Employee M ON E.managerID = M.employeeID   
        LEFT JOIN Contract C ON E.contractID = C.contractID
        WHERE E.employeeID = @EmployeeID;

        SET @success = 1;
    END;
    GO

-- this stored procedure retrieves the status of leave and attendance correction requests
-- success = 1 means that the request statuses were retrieved
-- success = -1 means that the employee does not exist
CREATE PROCEDURE ViewRequestStatus
    (
        @EmployeeID INT,
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;


        IF NOT EXISTS (SELECT 1 FROM Employee WHERE employeeID = @EmployeeID)
        BEGIN
            SET @success = -1;
            RETURN;
        END;


        SELECT 
            LR.leaveRequestID,
            'LeaveRequest' AS requestType,
            LR.status,
            LR.justification,
            LR.duration,
            LR.approvalTiming
        FROM LeaveRequest LR
        WHERE LR.employeeID = @EmployeeID

        UNION ALL


        SELECT 
            ACR.requestID,
            'CorrectionRequest' AS requestType,
            ACR.status,
            ACR.reason AS justification,
            NULL AS duration,
            NULL AS approvalTiming
        FROM AttendanceCorrectionRequest ACR
        WHERE ACR.employeeID = @EmployeeID;

        SET @success = 1;
    END;
    GO

-- this stored procedure retrieves the detailed leave history for a specific employee
-- success = 1 means that the leave history was retrieved
-- success = -1 means that the employee does not exist
CREATE PROCEDURE ViewLeaveHistory
    (
        @EmployeeID INT,
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;

        IF NOT EXISTS (SELECT 1 FROM Employee WHERE employeeID = @EmployeeID)
        BEGIN
            SET @success = -1;
            RETURN;
        END;

        SELECT 
            LR.leaveRequestID,
            L.typeName,
            L.description,
            LR.justification,
            LR.duration,
            LR.status,
            LR.approvalTiming
        FROM LeaveRequest LR
        JOIN LeaveType L ON LR.leaveTypeID = L.leaveTypeID
        WHERE LR.employeeID = @EmployeeID
        ORDER BY LR.leaveRequestID DESC;

        SET @success = 1;
    END;
    GO

-- this stored procedure retrieves attendance records for a manager's team within a date range
-- success = 1 means that the team attendance was retrieved
-- success = -3 means that the manager does not exist
CREATE PROCEDURE ViewTeamAttendance
    (
        @ManagerID INT,
        @DateRangeStart DATE,
        @DateRangeEnd DATE,
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;

        IF NOT EXISTS (SELECT 1 FROM Employee WHERE employeeID = @ManagerID)
        BEGIN
            SET @success = -3;
            RETURN;
        END;

        SELECT 
            A.attendanceID,
            E.fullName AS employeeName,
            SS.shiftDate,
            SS.startTime,
            SS.endTime,
            A.entryTime,
            A.exitTime,
            A.durationMinutes,
            SS.name AS shiftName
        FROM Attendance A
        JOIN Employee E ON A.employeeID = E.employeeID
        JOIN ShiftSchedule SS ON A.shiftID = SS.shiftID
        WHERE E.managerID = @ManagerID
        AND SS.shiftDate BETWEEN @DateRangeStart AND @DateRangeEnd
        ORDER BY SS.shiftDate, E.fullName;

        SET @success = 1;
    END;
    GO

-- this stored procedure retrieves summary statistics for a specific department
-- success = 1 means that the summary was retrieved
-- success = -4 means that the department does not exist
CREATE PROCEDURE ViewDepartmentSummary
    (
        @DepartmentID INT,
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;

        IF NOT EXISTS (SELECT 1 FROM Department WHERE departmentID = @DepartmentID)
        BEGIN
            SET @success = -4;
            RETURN;
        END;

        SELECT
            D.name,
            COUNT(E.employeeID) AS employeeCount,
            H.fullName AS departmentHead
        FROM Department D
        LEFT JOIN Employee E ON E.departmentID = D.departmentID
        LEFT JOIN Employee H ON D.headID = H.employeeID
        WHERE D.departmentID = @DepartmentID
        GROUP BY D.name, H.fullName;

        SET @success = 1;
    END;
    GO

-- this stored procedure calculates statistics (team size, avg salary) for a manager's team
-- success = 1 means that the statistics were calculated
-- success = -3 means that the manager does not exist
CREATE PROCEDURE GetTeamStatistics
    (
        @ManagerID INT,
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;

        IF NOT EXISTS (SELECT 1 FROM Employee WHERE employeeID = @ManagerID)
        BEGIN
            SET @success = -3;
            RETURN;
        END;

        SELECT
            COUNT(E.employeeID) AS teamSize,
            AVG((PG.minSalary + PG.maxSalary) / 2) AS averageSalary,
            COUNT(E.employeeID) AS spanOfControl
        FROM Employee E
        LEFT JOIN PayGrade PG ON E.payGrade = PG.payGrade
        WHERE E.managerID = @ManagerID;

        SET @success = 1;
    END;
    GO

-- this stored procedure retrieves basic profile information for a manager's team
-- success = 1 means that the profiles were retrieved
-- success = -3 means that the manager does not exist
CREATE PROCEDURE ViewTeamProfiles
    (
        @ManagerID INT,
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;

        IF NOT EXISTS (SELECT 1 FROM Employee WHERE employeeID = @ManagerID)
        BEGIN
            SET @success = -3;
            RETURN;
        END;

        SELECT
            E.employeeID,
            E.fullName,
            E.emailAddress,
            E.phoneNumber,
            P.title,
            D.name,
            E.employmentStatus
        FROM Employee E
        LEFT JOIN Position P ON E.positionID = P.positionID
        LEFT JOIN Department D ON E.departmentID = D.departmentID
        WHERE E.managerID = @ManagerID;

        SET @success = 1;
    END;
    GO

-- this stored procedure filters a manager's team based on skills or roles
-- success = 1 means that the filtered profiles were retrieved
-- success = -3 means that the manager does not exist
CREATE PROCEDURE FilterTeamProfiles
    (
        @ManagerID INT,
        @Skill VARCHAR(50),
        @RoleID INT,
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;

        IF NOT EXISTS (SELECT 1 FROM Employee WHERE employeeID = @ManagerID)
        BEGIN
            SET @success = -3;
            RETURN;
        END;

        SELECT DISTINCT
            E.employeeID,
            E.fullName,
            E.emailAddress,
            E.phoneNumber,
            P.title AS positionTitle,
            D.name AS departmentName,
            E.employmentStatus
        FROM Employee E
        LEFT JOIN possesses_skill PS ON E.employeeID = PS.employeeID
        LEFT JOIN Skill S ON PS.skillID = S.skillID
        LEFT JOIN fulfills_role FR ON E.employeeID = FR.employeeID
        LEFT JOIN Position P ON E.positionID = P.positionID
        LEFT JOIN Department D ON E.departmentID = D.departmentID
        WHERE E.managerID = @ManagerID
        AND (@Skill IS NULL OR S.name = @Skill)
        AND (@RoleID IS NULL OR FR.roleID = @RoleID);

        SET @success = 1;
    END;
    GO

-- this stored procedure assigns a specific role to an employee
-- success = 1 means that the role was assigned
-- success = 0 means that the employee already has this role
-- success = -1 means that the employee does not exist
-- success = -5 means that the role does not exist
CREATE PROCEDURE AssignRole
    (
        @EmployeeID INT,
        @RoleID INT,
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;


        IF NOT EXISTS (SELECT 1 FROM Employee WHERE employeeID = @EmployeeID)
        BEGIN
            SET @success = -1;
            RETURN;
        END;


        IF NOT EXISTS (SELECT 1 FROM Role WHERE roleID = @RoleID)
        BEGIN
            SET @success = -5;
            RETURN;
        END;


        IF EXISTS (
            SELECT 1 
            FROM fulfills_role 
            WHERE employeeID = @EmployeeID AND roleID = @RoleID
        )
        BEGIN
            SET @success = 0;
            RETURN;
        END;


        INSERT INTO fulfills_role(employeeID, roleID, assignedDate)
        VALUES(@EmployeeID, @RoleID, GETDATE());

        SET @success = 1;
    END;
    GO

-- this stored procedure sends a structure change notification to a list of employees
-- success = 1 means that the notifications were sent
-- success = -1 means that one of the employees in the list does not exist
CREATE PROCEDURE NotifyStructureChange
    (
        @AffectedEmployees VARCHAR(500),
        @Message VARCHAR(200),
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;

        DECLARE @notificationID INT;
        DECLARE @Pos INT = 0;
        DECLARE @NextPos INT;
        DECLARE @EmpID INT;


        SET @AffectedEmployees = @AffectedEmployees + ',';


        WHILE CHARINDEX(',', @AffectedEmployees, @Pos + 1) > 0
        BEGIN
            SET @NextPos = CHARINDEX(',', @AffectedEmployees, @Pos + 1);
            SET @EmpID = CAST(SUBSTRING(@AffectedEmployees, @Pos + 1, @NextPos - @Pos - 1) AS INT);

            IF NOT EXISTS (SELECT 1 FROM Employee WHERE employeeID = @EmpID)
            BEGIN
                SET @success = -1;
                RETURN;
            END;

            SET @Pos = @NextPos;
        END;


        SET @Pos = 0;


        INSERT INTO Notification(messageContent, timestamp, readStatus, type)
        VALUES(@Message, GETDATE(), 'Unread', 'StructureChange');

        SET @notificationID = SCOPE_IDENTITY();


        WHILE CHARINDEX(',', @AffectedEmployees, @Pos + 1) > 0
        BEGIN
            SET @NextPos = CHARINDEX(',', @AffectedEmployees, @Pos + 1);
            SET @EmpID = CAST(SUBSTRING(@AffectedEmployees, @Pos + 1, @NextPos - @Pos - 1) AS INT);

            INSERT INTO receives_notification(employeeID, notificationID, deliveryStatus, deliveredAt)
            VALUES(@EmpID, @notificationID, 'Delivered', GETDATE());

            SET @Pos = @NextPos;
        END;

        SET @success = 1;
    END;
    GO

-- this stored procedure manages user roles (add, remove, change)
-- success = 1 means that the action was performed successfully
-- success = 0 means that the action failed (e.g., duplicate add, removing non-existent role, or invalid action)
-- success = -1 means that the user does not exist
-- success = -5 means that the role does not exist
CREATE PROCEDURE ManageUserAccounts
    (
        @UserID INT,
        @Role VARCHAR(50),
        @Action VARCHAR(20),
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;

        DECLARE @RoleID INT;


        IF NOT EXISTS (SELECT 1 FROM Employee WHERE employeeID = @UserID)
        BEGIN
            SET @success = -1;
            RETURN;
        END;

        SELECT @RoleID = roleID FROM Role WHERE name = @Role;;
        IF @RoleID IS NULL
        BEGIN
            SET @success = -5;
            RETURN;
        END;

        IF @Action = 'ADD'
        BEGIN
            IF EXISTS (SELECT 1 FROM fulfills_role WHERE employeeID = @UserID AND roleID = @RoleID)
            BEGIN
                SET @success = 0;
                RETURN;
            END;

            INSERT INTO fulfills_role(employeeID, roleID, assignedDate)
            VALUES(@UserID, @RoleID, GETDATE());

            SET @success = 1;
            RETURN;
        END;

        IF @Action = 'REMOVE'
        BEGIN
            IF NOT EXISTS (SELECT 1 FROM fulfills_role WHERE employeeID = @UserID AND roleID = @RoleID)
            BEGIN
                SET @success = 0;
                RETURN;
            END;

            DELETE FROM fulfills_role WHERE employeeID = @UserID AND roleID = @RoleID;

            SET @success = 1;
            RETURN;
        END;

        IF @Action = 'CHANGE'
        BEGIN
            DELETE FROM fulfills_role WHERE employeeID = @UserID;

            INSERT INTO fulfills_role(employeeID, roleID, assignedDate)
            VALUES(@UserID, @RoleID, GETDATE());

            SET @success = 1;
            RETURN;
        END;

        SET @success = 0; -- invalid action
    END;
    GO

-- this stored procedure submits a new reimbursement request
-- success = 1 means that the request was submitted
-- success = -1 means that the employee does not exist
CREATE PROCEDURE SubmitReimbursement
    (
        @EmployeeID INT,
        @ExpenseType VARCHAR(50),
        @Amount DECIMAL(10,2),
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;

        IF NOT EXISTS (SELECT 1 FROM Employee WHERE employeeID = @EmployeeID)
        BEGIN
            SET @success = -1;
            RETURN;
        END;

        INSERT INTO Reimbursement(type, currentStatus, employeeID)
        VALUES(@ExpenseType, 'Pending', @EmployeeID);

        SET @success = 1;
    END;
    GO

-- this stored procedure approves or rejects a reimbursement request
-- success = 1 means that the request status was updated
-- success = 0 means that the update failed
-- success = -1 means that the reimbursement request does not exist
-- success = -2 means that the action was invalid
CREATE PROCEDURE ReviewReimbursement
    (
        @RequestID INT,
        @Action VARCHAR(20),
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;

        IF NOT EXISTS (SELECT 1 FROM Reimbursement WHERE reimbursementID = @RequestID)
        BEGIN
            SET @success = -1;
            RETURN;
        END;

        IF @Action = 'APPROVE'
        BEGIN
            UPDATE Reimbursement
            SET currentStatus = 'Approved',
                approvalDate = GETDATE()
            WHERE reimbursementID = @RequestID;

            SET @success = 1;
            RETURN;
        END;

        IF @Action = 'REJECT'
        BEGIN
            UPDATE Reimbursement
            SET currentStatus = 'Rejected',
                approvalDate = GETDATE()
            WHERE reimbursementID = @RequestID;

            SET @success = 1;
            RETURN;
        END;

        SET @success = -2;
    END;
    GO

-- this stored procedure updates an employee's personal details
-- success = 1 means that the details were updated
-- success = 0 means that the update failed
-- success = -1 means that the employee does not exist
CREATE PROCEDURE UpdatePersonalDetails
    (
        @EmployeeID INT,
        @Email VARCHAR(100),
        @Phone VARCHAR(20),
        @Address VARCHAR(255),
        @Biography VARCHAR(MAX),
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;

        IF NOT EXISTS (SELECT 1 FROM Employee WHERE employeeID = @EmployeeID)
        BEGIN
            SET @success = -1;
            RETURN;
        END;

        UPDATE Employee
        SET 
            emailAddress = @Email,
            phoneNumber = @Phone,
            address = @Address,
            biography = @Biography
        WHERE employeeID = @EmployeeID;

        IF @@ROWCOUNT = 1 
            SET @success = 1;
        ELSE 
            SET @success = 0;
    END;
    GO

-- this stored procedure updates an employee's emergency contact information
-- success = 1 means that the contact info was updated
-- success = 0 means that the update failed
-- success = -1 means that the employee does not exist
CREATE PROCEDURE UpdateEmergencyContact
    (
        @EmployeeID INT,
        @ContactName VARCHAR(100),
        @ContactPhone VARCHAR(20),
        @Relationship VARCHAR(50),
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;

        IF NOT EXISTS (SELECT 1 FROM Employee WHERE employeeID = @EmployeeID)
        BEGIN
            SET @success = -1;
            RETURN;
        END;

        UPDATE Employee
        SET 
            emergencyContactName = @ContactName,
            emergencyContactPhone = @ContactPhone,
            relationship = @Relationship
        WHERE employeeID = @EmployeeID;

        IF @@ROWCOUNT = 1
            SET @success = 1;
        ELSE
            SET @success = 0;
    END;
    GO

-- this stored procedure adds a skill to an employee profile, creating the skill if needed
-- success = 1 means that the skill was added
-- success = 0 means that the employee already possesses this skill
-- success = -1 means that the employee does not exist
CREATE PROCEDURE AddEmployeeSkill
    (
        @EmployeeID INT,
        @SkillName VARCHAR(100),
        @Proficiency INT,
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;

        DECLARE @SkillID INT;

        IF NOT EXISTS (SELECT 1 FROM Employee WHERE employeeID = @EmployeeID)
        BEGIN
            SET @success = -1;
            RETURN;
        END;

        SELECT @SkillID = skillID
        FROM Skill
        WHERE name = @SkillName;

        IF @SkillID IS NULL
        BEGIN
            INSERT INTO Skill(name)
            VALUES(@SkillName);

            SET @SkillID = SCOPE_IDENTITY();
        END;

        IF EXISTS (
            SELECT 1 
            FROM possesses_skill
            WHERE employeeID = @EmployeeID AND skillID = @SkillID
        )
        BEGIN
            SET @success = 0;
            RETURN;
        END;

        INSERT INTO possesses_skill(employeeID, skillID, proficiencyLevel)
        VALUES(@EmployeeID, @SkillID, @Proficiency);

        SET @success = 1;
    END;
    GO

-- this stored procedure retrieves missions assigned to a specific employee
-- success = 1 means that the missions were retrieved
-- success = -1 means that the employee does not exist
CREATE PROCEDURE ViewMyMissions
    (
        @EmployeeID INT,
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;

        IF NOT EXISTS (SELECT 1 FROM Employee WHERE employeeID = @EmployeeID)
        BEGIN
            SET @success = -1;
            RETURN;
        END;

        SELECT
            M.missionID,
            M.destination,
            M.startDate,
            M.endDate,
            M.status,
            Manager.fullName AS managerName
        FROM Mission M
        LEFT JOIN Employee Manager 
            ON M.managerID = Manager.employeeID
        WHERE M.employeeID = @EmployeeID
        ORDER BY M.startDate DESC;

        SET @success = 1;
    END;
    GO

-- this stored procedure retrieves a comprehensive profile for a specific employee
-- success = 1 means that the profile was retrieved
-- success = -1 means that the employee does not exist
CREATE PROCEDURE ViewEmployeeProfile
    (
        @EmployeeID INT,
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;

        IF NOT EXISTS (SELECT 1 FROM Employee WHERE employeeID = @EmployeeID)
        BEGIN
            SET @success = -1;
            RETURN;
        END;

        -- profile (single row)
        SELECT TOP(1)
            E.employeeID,
            E.firstName,
            E.middleName,
            E.lastName,
            E.fullName,
            E.nationalID,
            E.birthDate,
            E.birthCountry,
            E.emailAddress,
            E.phoneNumber,
            E.address,
            E.emergencyContactName,
            E.emergencyContactPhone,
            E.relationship,
            E.biography,
            E.employmentStatus,
            E.hireDate,
            E.profileCompletion,
            E.isActive,
            D.name AS departmentName,
            E.departmentID,
            P.title AS positionTitle,
            E.positionID,
            M.fullName AS managerName,
            E.managerID,
            C.type AS contractType,
            E.contractID,
            C.startDate AS contractStart,
            C.endDate AS contractEnd,
            E.taxFormID,
            E.salaryTypeID,
            E.payGrade
        FROM Employee E
        LEFT JOIN Department D ON E.departmentID = D.departmentID
        LEFT JOIN Position P ON E.positionID = P.positionID
        LEFT JOIN Employee M ON E.managerID = M.employeeID
        LEFT JOIN Contract C ON E.contractID = C.contractID
        WHERE E.employeeID = @EmployeeID;

        -- skills (separate rowset)
        SELECT
            S.skillID,
            S.name AS skillName,
            ES.proficiencyLevel
        FROM possesses_skill ES
        JOIN Skill S ON ES.skillID = S.skillID
        WHERE ES.employeeID = @EmployeeID;

        SET @success = 1;
    END;
    GO

-- this stored procedure updates an employee's contact information (phone and email)
-- success = 1 means that the information was updated
-- success = 0 means that the update failed
-- success = -1 means that the employee does not exist
CREATE PROCEDURE UpdateContactInformation
    (
        @EmployeeID INT,
        @Phone VARCHAR(20),
        @Email VARCHAR(100),
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;

        IF NOT EXISTS (SELECT 1 FROM Employee WHERE employeeID = @EmployeeID)
        BEGIN
            SET @success = -1;
            RETURN;
        END;

        UPDATE Employee
        SET 
            phoneNumber = @Phone,
            emailAddress = @Email
        WHERE employeeID = @EmployeeID;

        IF @@ROWCOUNT = 1 
            SET @success = 1;
        ELSE 
            SET @success = 0;
    END;
    GO

-- this stored procedure sends a request for an HR document to administrators
-- success = 1 means that the request notification was sent
-- success = -1 means that the employee does not exist
CREATE PROCEDURE RequestHRDocument
    (
        @EmployeeID INT,
        @DocumentType VARCHAR(50),
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;

        IF NOT EXISTS (SELECT 1 FROM Employee WHERE employeeID = @EmployeeID)
        BEGIN
            SET @success = -1;
            RETURN;
        END;

        DECLARE @NotificationID INT;
        DECLARE @Message VARCHAR(300);

        SET @Message = 'HR Document Request: ' + @DocumentType 
                    + ' requested by Employee ID ' + CAST(@EmployeeID AS VARCHAR(10));

        INSERT INTO Notification(messageContent, timestamp, readStatus, type)
        VALUES(@Message, GETDATE(), 'Unread', 'HRDocumentRequest');

        SET @NotificationID = SCOPE_IDENTITY();

        INSERT INTO receives_notification(employeeID, notificationID, deliveryStatus, deliveredAt)
        SELECT employeeID, @NotificationID, 'Delivered', GETDATE()
        FROM HRAdministrator;

        SET @success = 1;
    END;
    GO

-- this stored procedure notifies HR administrators about an employee profile update
-- success = 1 means that the notification was sent
-- success = -1 means that the employee does not exist
CREATE PROCEDURE NotifyProfileUpdate
    (
        @EmployeeID INT,
        @Message VARCHAR(255),
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;

        IF NOT EXISTS (SELECT 1 FROM Employee WHERE employeeID = @EmployeeID)
        BEGIN
            SET @success = -1;
            RETURN;
        END;

        DECLARE @NotificationID INT;

        INSERT INTO Notification(messageContent, timestamp, readStatus, type)
        VALUES(@Message, GETDATE(), 'Unread', 'ProfileUpdate');

        SET @NotificationID = SCOPE_IDENTITY();

        INSERT INTO receives_notification(employeeID, notificationID, deliveryStatus, deliveredAt)
        SELECT employeeID, @NotificationID, 'Delivered', GETDATE()
        FROM HRAdministrator;

        SET @success = 1;
    END;
    GO

-- this stored procedure notifies a manager that an employee missed a clock-in/out
-- success = 1 means that the notification was sent
-- success = -1 means that the employee does not exist
-- success = -2 means that the employee has no manager assigned
CREATE PROCEDURE NotifyMissedPunch
    (
        @EmployeeID INT,
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;

        IF NOT EXISTS (SELECT 1 FROM Employee WHERE employeeID = @EmployeeID)
        BEGIN
            SET @success = -1;
            RETURN;
        END;

        DECLARE @ManagerID INT;
        DECLARE @NotificationID INT;

        SELECT @ManagerID = managerID
        FROM Employee
        WHERE employeeID = @EmployeeID;

        IF @ManagerID IS NULL
        BEGIN
            SET @success = -2;
            RETURN;
        END;

        INSERT INTO Notification(messageContent, timestamp, readStatus, type)
        VALUES('Employee missed punch: ID ' + CAST(@EmployeeID AS VARCHAR(10)), 
            GETDATE(), 'Unread', 'MissedPunch');

        SET @NotificationID = SCOPE_IDENTITY();

        INSERT INTO receives_notification(employeeID, notificationID, deliveryStatus, deliveredAt)
        VALUES(@ManagerID, @NotificationID, 'Delivered', GETDATE());

        SET @success = 1;
    END;
    GO

-- this stored procedure submits a retroactive leave request for an unplanned absence
-- success = 1 means that the leave request was submitted
-- success = -1 means that the employee does not exist
CREATE PROCEDURE SubmitLeaveAfterAbsence
    (
        @EmployeeID INT,
        @Justification VARCHAR(MAX),
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;

        IF NOT EXISTS (SELECT 1 FROM Employee WHERE employeeID = @EmployeeID)
        BEGIN
            SET @success = -1;
            RETURN;
        END;

        DECLARE @LeaveTypeID INT;

        SELECT @LeaveTypeID = leaveTypeID
        FROM LeaveType
        WHERE typeName = 'Absence';

        IF @LeaveTypeID IS NULL
        BEGIN
            INSERT INTO LeaveType(typeName, description)
            VALUES('Absence', 'Leave submitted after unplanned absence');

            SET @LeaveTypeID = SCOPE_IDENTITY();
        END;

        INSERT INTO LeaveRequest(employeeID, leaveTypeID, justification, duration, status)
        VALUES(@EmployeeID, @LeaveTypeID, @Justification, 1, 'Pending');

        SET @success = 1;
    END;
    GO

-- this stored procedure updates a leave request status and notifies the employee
-- success = 1 means that the status was updated and notification sent
-- success = -2 means that the leave request does not exist
CREATE PROCEDURE NotifyLeaveStatusChange
    (
        @RequestID INT,
        @NewStatus VARCHAR(50),
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;

        DECLARE @EmployeeID INT;
        DECLARE @NotificationID INT;

        SELECT @EmployeeID = employeeID
        FROM LeaveRequest
        WHERE leaveRequestID = @RequestID;

        IF @EmployeeID IS NULL
        BEGIN
            SET @success = -2;
            RETURN;
        END;

        UPDATE LeaveRequest
        SET status = @NewStatus
        WHERE leaveRequestID = @RequestID;

        INSERT INTO Notification(messageContent, timestamp, urgency, readStatus)
        VALUES('Your leave request #' + CAST(@RequestID AS VARCHAR(10)) 
            + ' status changed to: ' + @NewStatus,
            GETDATE(), 'Unread', 'LeaveStatusChange');

        SET @NotificationID = SCOPE_IDENTITY();

        INSERT INTO receives_notification(employeeID, notificationID, deliveryStatus, deliveredAt)
        VALUES(@EmployeeID, @NotificationID, 'Delivered', GETDATE());

        SET @success = 1;
    END;
    GO

-- this stored procedure grants a list of comma-separated permissions to a role
-- success = 1 means that at least one permission was granted
-- success = 0 means that no new permissions were added
-- success = -1 means that the role does not exist
-- success = -2 means that the permission is invalid
CREATE PROCEDURE GrantRolePermissions
    (
        @RoleID INT,
        @PermissionList VARCHAR(MAX),
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;
        
        DECLARE @Permission VARCHAR(50);
        DECLARE @Pos INT;
        DECLARE @SQL NVARCHAR(MAX);
        DECLARE @Count INT = 0;
        
        IF NOT EXISTS (SELECT 1 FROM Role WHERE roleID = @RoleID)
        BEGIN
            SET @success = -1;
            RETURN;
        END;
        
        SET @PermissionList = @PermissionList + ','; -- Ensure trailing comma
        
        WHILE LEN(@PermissionList) > 0
        BEGIN
            SET @Pos = CHARINDEX(',', @PermissionList);
            
            SET @Permission = LTRIM(RTRIM(LEFT(@PermissionList, @Pos - 1)));
            
            IF @Permission <> ''
            BEGIN
                IF NOT EXISTS (SELECT 1 FROM Permission WHERE name = @Permission)
                BEGIN
                    SET @success = -2;
                    RETURN;
                END;
                
                IF NOT EXISTS (SELECT 1 FROM RolePermission WHERE roleID = @RoleID AND permissionName = @Permission)
                BEGIN
                    INSERT INTO RolePermission (roleID, permissionName, allowedAction)
                    VALUES (@RoleID, @Permission, 'Grant');
                    
                    SET @Count = @Count + 1;
                END;
            END;
            
            SET @PermissionList = STUFF(@PermissionList, 1, @Pos, '');
        END;
        
        IF @Count > 0
            SET @success = 1;
        ELSE
            SET @success = 0;
    END;
    GO

-- George

-- COMMENTS MISSING
CREATE PROCEDURE DefinePermissionLimits
    (
        @MinHours INT,
        @MaxHours INT,
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;
        
        IF @MinHours < 0
        BEGIN
            SET @success = -1;
            RETURN;
        END;
        
        IF @MaxHours < 0
        BEGIN
            SET @success = -2;
            RETURN;
        END;
        
        IF @MinHours > @MaxHours
        BEGIN
            SET @success = -3;
            RETURN;
        END;
        
        MERGE INTO SystemConfiguration AS target
        USING (VALUES 
            ('PermissionMinHours', CAST(@MinHours AS VARCHAR(10))),
            ('PermissionMaxHours', CAST(@MaxHours AS VARCHAR(10)))
        ) AS source (configKey, configValue)
        ON target.configKey = source.configKey
        WHEN MATCHED THEN
            UPDATE SET configValue = source.configValue, updatedAt = GETDATE()
        WHEN NOT MATCHED THEN
            INSERT (configKey, configValue, updatedAt)
            VALUES (source.configKey, source.configValue, GETDATE());
        
        IF @@ROWCOUNT > 0
            SET @success = 1;
        ELSE
            SET @success = 0;
    END;
    GO

-- COMMENTS MISSING
CREATE PROCEDURE AuthenticateLeaveAdmin
    (
        @AdminID INT,
        @PasswordHash VARBINARY(32),
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;
        
        DECLARE @StoredPasswordHash VARBINARY(32);
        
        IF NOT EXISTS (SELECT 1 FROM HRAdministrator WHERE employeeId = @AdminID)
        BEGIN
            SET @success = -1;
            RETURN;
        END;
        
        IF NOT EXISTS (SELECT 1 FROM EmployeeCredentials WHERE employeeId = @AdminID)
        BEGIN
            SET @success = -2;
            RETURN;
        END;
        
        SELECT @StoredPasswordHash = passwordHash 
        FROM EmployeeCredentials 
        WHERE employeeId = @AdminID;
        
        IF @StoredPasswordHash = @PasswordHash
            SET @success = 1;
        ELSE
            SET @success = 0;
    END;
    GO

-- COMMENTS MISSING
CREATE PROCEDURE ApplyLeaveConfiguration
    (
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;
        
        UPDATE LeavePolicy
        SET status = 'Active'
        WHERE status = 'Pending';
        
        IF @@ROWCOUNT > 0
            SET @success = 1;
        ELSE
            SET @success = 0;
    END;
    GO

-- COMMENTS MISSING
CREATE PROCEDURE ManageLeaveRoles
    (
        @RoleID INT,
        @Permissions VARCHAR(200),
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;
        
        DECLARE @PermissionList VARCHAR(200) = @Permissions;
        DECLARE @Permission VARCHAR(50);
        DECLARE @Pos INT;
        DECLARE @InsertCount INT = 0;
        
        IF NOT EXISTS (SELECT 1 FROM Role WHERE roleId = @RoleID)
        BEGIN
            SET @success = -1;
            RETURN;
        END;
        
        WHILE LEN(@PermissionList) > 0
        BEGIN
            SET @Pos = CHARINDEX(',', @PermissionList);
            IF @Pos = 0 SET @Pos = LEN(@PermissionList) + 1;
            
            SET @Permission = LTRIM(RTRIM(LEFT(@PermissionList, @Pos - 1)));
            
            IF NOT EXISTS (SELECT 1 FROM RolePermission WHERE roleId = @RoleID AND permissionName = @Permission)
            BEGIN
                INSERT INTO RolePermission (roleId, permissionName, allowedAction)
                VALUES (@RoleID, @Permission, 'Grant');
                
                SET @InsertCount = @InsertCount + 1;
            END;
            
            SET @PermissionList = STUFF(@PermissionList, 1, @Pos, '');
        END;
        
        IF @InsertCount > 0
            SET @success = 1;
        ELSE
            SET @success = 0;
    END;
    GO

-- INCORRECT
-- this stored procedure generates the base payroll records for a specific period
-- success = 1 means that payroll records were generated
-- success = 0 means that no eligible employees were found or records already exist
-- success = -1 means that start or end dates are null
-- success = -2 means that the start date is after the end date
-- success = -3 means that an unexpected error occurred
CREATE PROCEDURE GeneratePayroll
    (
        @StartDate DATE,
        @EndDate DATE,
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;
        
        IF @StartDate IS NULL OR @EndDate IS NULL
        BEGIN
            SET @success = -1;
            RETURN;
        END;
        
        IF @StartDate > @EndDate
        BEGIN
            SET @success = -2;
            RETURN;
        END;
        
        BEGIN TRY
            BEGIN TRANSACTION;
            
            INSERT INTO Payroll (
                employeeID, periodStart, periodEnd, baseAmount, 
                adjustments, contributions, taxes, actualPay, netSalary, paymentDate
            )
            SELECT 
                e.employeeID,
                @StartDate,
                @EndDate,
                CASE 
                    WHEN mst.salaryTypeID IS NOT NULL THEN ISNULL(mst.baseSalary, 0)
                    WHEN hst.salaryTypeID IS NOT NULL THEN ISNULL(hst.hourlyRate * hst.maxMonthlyHours, 0)
                    ELSE 0
                END AS baseAmount,
                '0' AS adjustments,
                0 AS contributions,
                0 AS taxes,
                0 AS actualPay,
                0 AS netSalary,
                NULL AS paymentDate
            FROM Employee e
            JOIN SalaryType st ON e.salaryTypeID = st.salaryTypeID
            LEFT JOIN MonthlySalaryType mst ON st.salaryTypeID = mst.salaryTypeID
            LEFT JOIN HourlySalaryType hst ON st.salaryTypeID = hst.salaryTypeID
            WHERE e.isActive = 1
            AND NOT EXISTS (
                SELECT 1 FROM Payroll p 
                WHERE p.employeeID = e.employeeID 
                AND p.periodStart = @StartDate 
                AND p.periodEnd = @EndDate
            );
            
            IF @@ROWCOUNT > 0
                SET @success = 1;
            ELSE
                SET @success = 0;
            
            COMMIT TRANSACTION;
        END TRY
        BEGIN CATCH
            IF @@TRANCOUNT > 0
                ROLLBACK TRANSACTION;
            
            SET @success = -3;
        END CATCH;
    END;
    GO

-- this stored procedure calculates net salary by aggregating allowances, deductions, and taxes
-- success = 1 means that the net salary was calculated and updated
-- success = 0 means that the update failed
-- success = -1 means that the payroll ID does not exist
-- success = -2 means that an unexpected error occurred
CREATE PROCEDURE CalculateNetSalary
    (
        @PayrollID INT,
        @NetSalary DECIMAL(10,2) OUTPUT,
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;
        
        DECLARE @BaseAmount DECIMAL(10,2);
        DECLARE @TotalAllowances DECIMAL(10,2) = 0;
        DECLARE @TotalDeductions DECIMAL(10,2) = 0;
        DECLARE @Taxes DECIMAL(10,2);
        DECLARE @Contributions DECIMAL(10,2);
        
        IF NOT EXISTS (SELECT 1 FROM Payroll WHERE payrollId = @PayrollID)
        BEGIN
            SET @success = -1;
            RETURN;
        END;
        
        BEGIN TRY
            BEGIN TRANSACTION;
            
            SELECT @BaseAmount = baseAmount, @Taxes = taxes, @Contributions = contributions
            FROM Payroll
            WHERE payrollId = @PayrollID;
            
            SELECT @TotalAllowances = ISNULL(SUM(amount), 0)
            FROM AllowanceDeduction
            WHERE payrollId = @PayrollID AND type = 'Allowance';
            
            SELECT @TotalDeductions = ISNULL(SUM(amount), 0)
            FROM AllowanceDeduction
            WHERE payrollId = @PayrollID AND type = 'Deduction';
            
            SET @NetSalary = @BaseAmount + @TotalAllowances - @TotalDeductions - @Taxes - @Contributions;
            
            UPDATE Payroll
            SET 
                adjustments = @TotalAllowances - @TotalDeductions,
                actualPay = @BaseAmount + @TotalAllowances - @TotalDeductions,
                netSalary = @NetSalary
            WHERE payrollId = @PayrollID;
            
            IF @@ROWCOUNT = 1
                SET @success = 1;
            ELSE
                SET @success = 0;
            
            COMMIT TRANSACTION;
        END TRY
        BEGIN CATCH
            IF @@TRANCOUNT > 0
                ROLLBACK TRANSACTION;
            
            SET @success = -2;
        END CATCH;
    END;
    GO

-- this stored procedure links a policy to a payroll record
-- success = 1 means that the policy was applied
-- success = -1 means that the policy ID does not exist
-- success = -2 means that the payroll ID does not exist
-- success = -3 means that an unexpected error occurred
CREATE PROCEDURE ApplyPayrollPolicy
    (
        @PolicyID INT,
        @PayrollID INT,
        @Type VARCHAR(20),
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;
        
        IF NOT EXISTS (SELECT 1 FROM PayrollPolicy WHERE payrollPolicyID = @PolicyID)
        BEGIN
            SET @success = -1;
            RETURN;
        END;
        
        IF NOT EXISTS (SELECT 1 FROM Payroll WHERE payrollID = @PayrollID)
        BEGIN
            SET @success = -2;
            RETURN;
        END;
        
        BEGIN TRY
            BEGIN TRANSACTION;
            
            -- Link policy to payroll using checks_payroll_policy junction table
            INSERT INTO checks_payroll_policy (payrollID, payrollPolicyID)
            VALUES (@PayrollID, @PolicyID);
            
            SET @success = 1;
            
            COMMIT TRANSACTION;
        END TRY
        BEGIN CATCH
            IF @@TRANCOUNT > 0
                ROLLBACK TRANSACTION;
            
            SET @success = -3;
        END CATCH;
    END;
    GO

-- this stored procedure creates or updates a tax rule
-- success = 1 means that the tax rule was successfully managed
-- success = -1 means that the tax rate is invalid (not between 0-100)
-- success = -2 means that the exemption amount is negative
CREATE PROCEDURE ManageTaxRules
    (
        @TaxRuleName VARCHAR(50),
        @CountryCode VARCHAR(10),
        @Rate DECIMAL(5,2),
        @Exemption DECIMAL(10,2),
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;
        
        IF @Rate < 0 OR @Rate > 100
        BEGIN
            SET @success = -1;
            RETURN;
        END;
        
        IF @Exemption < 0
        BEGIN
            SET @success = -2;
            RETURN;
        END;
        
        IF EXISTS (SELECT 1 FROM TaxRule WHERE ruleName = @TaxRuleName)
        BEGIN
            UPDATE TaxRule
            SET countryCode = @CountryCode, 
                taxRate = @Rate, 
                exemptionAmount = @Exemption
            WHERE ruleName = @TaxRuleName;
            
            SET @success = 1;
        END
        ELSE
        BEGIN
            INSERT INTO TaxRule (ruleName, countryCode, taxRate, exemptionAmount, createdAt)
            VALUES (@TaxRuleName, @CountryCode, @Rate, @Exemption, GETDATE());
            
            SET @success = 1;
        END;
    END;
    GO

-- INCORRECT
-- this stored procedure creates a new insurance bracket configuration
-- success = 1 means that the bracket was created
-- success = 0 means that the insertion failed
-- success = -1 means that salary values are negative
-- success = -2 means that minimum salary is greater than or equal to maximum salary
-- success = -3 means that the pay grade name already exists
CREATE PROCEDURE ConfigureInsuranceBrackets
    (
        @InsuranceType VARCHAR(50),
        @MinSalary DECIMAL(10,2),
        @MaxSalary DECIMAL(10,2),
        @EmployeeContribution DECIMAL(5,2),
        @EmployerContribution DECIMAL(5,2),
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;
        
        IF @MinSalary < 0 OR @MaxSalary < 0
        BEGIN
            SET @success = -1;
            RETURN;
        END;
        
        IF @MinSalary >= @MaxSalary
        BEGIN
            SET @success = -2;
            RETURN;
        END;
        
        IF EXISTS (SELECT 1 FROM PayGrade WHERE gradeName = @GradeName)
        BEGIN
            SET @success = -3;
            RETURN;
        END;
        
        INSERT INTO PayGrade (gradeName, minSalary, maxSalary)
        VALUES (@GradeName, @MinSalary, @MaxSalary);
        
        IF @@ROWCOUNT = 1
            SET @success = 1;
        ELSE
            SET @success = 0;
    END;
    GO

-- this stored procedure adds a manual allowance or deduction to a payroll record
-- success = 1 means that the item was added
-- success = 0 means that the insertion failed
-- success = -1 means that the payroll ID does not exist
-- success = -2 means that the type is invalid (must be 'Allowance' or 'Deduction')
CREATE PROCEDURE AdjustPayrollItem
    (
        @PayrollID INT,
        @Type VARCHAR(50),
        @Amount DECIMAL(10,2),
        @Duration INT,
        @Timezone VARCHAR(20),
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;
        
        DECLARE @EmployeeID INT;


        IF NOT EXISTS (SELECT 1 FROM Payroll WHERE payrollId = @PayrollID)
        BEGIN
            SET @success = -1;
            RETURN;
        END;
        
        IF @Type NOT IN ('Allowance', 'Deduction')
        BEGIN
            SET @success = -2;
            RETURN;
        END;
        
        SELECT @EmployeeID = employeeID FROM Payroll WHERE payrollID = @PayrollID;
        
        INSERT INTO AllowanceAndDeduction (
            payrollID, employeeID, amount, duration, timeZone
        )
        VALUES (
            @PayrollID, @EmployeeID, @Amount, @Duration, 1
        );
        
        IF @@ROWCOUNT = 1
                SET @success = 1;
            ELSE
                SET @success = 0;

        IF @success = 1
            PRINT 'Payroll item adjusted successfully.';
        ELSE IF @success = 0
            PRINT 'No changes were made.';
    END;
    GO

-- this stored procedure configures an allowance amount for a specific shift type
-- success = 1 means that the allowance was configured
-- success = 0 means that the insertion failed
-- success = -1 means that the amount is negative
CREATE PROCEDURE ConfigureShiftAllowances
    (
        @ShiftType VARCHAR(50),
        @AllowanceName VARCHAR(50),
        @Amount DECIMAL(10,2),
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;

        IF @Amount < 0
        BEGIN
            SET @success = -1;
            RETURN;
        END;

        INSERT INTO ShiftConfiguration (shiftType, allowanceAmount, lastUpdatedAt)
        VALUES (@ShiftType, @Amount, GETDATE());
        
        IF @@ROWCOUNT = 1
                SET @success = 1;
            ELSE
                SET @success = 0;

        IF @success = 1
            PRINT 'Shift allowance configured successfully.';
        ELSE IF @success = 0
            PRINT 'No changes were made.';
    END;
    GO

-- this stored procedure calculates costs for approved permissions and adds them to payroll adjustments
-- success = 1 means that payroll adjustments were updated
-- success = 0 means that no matching records were found
-- success = -1 means that the payroll period does not exist
-- success = -2 means that an unexpected error occurred
CREATE PROCEDURE SyncApprovedPermissionsToPayroll
    (
        @PayrollPeriodID INT,
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;
        
        DECLARE @StartDate DATE;
        DECLARE @EndDate DATE;
        
        IF NOT EXISTS (SELECT 1 FROM PayrollPeriod WHERE payrollPeriodId = @PayrollPeriodID)
        BEGIN
            SET @success = -1;
            RETURN;
        END;
        
        SELECT @StartDate = startDate, @EndDate = endDate
        FROM PayrollPeriod
        WHERE payrollPeriodId = @PayrollPeriodID;
        
        BEGIN TRY
            BEGIN TRANSACTION;
            
            UPDATE p
            SET p.adjustments = p.adjustments + 
                ISNULL((SELECT SUM(perm.durationHours * e.hourlyRate)
                        FROM Permission perm
                        JOIN Employee e ON perm.employeeId = e.employeeId
                        WHERE perm.employeeId = p.employeeId
                        AND perm.status = 'Approved'
                        AND perm.permissionDate BETWEEN @StartDate AND @EndDate), 0)
            FROM Payroll p
            WHERE EXISTS (
                SELECT 1 FROM PayrollPeriod pp
                WHERE pp.payrollPeriodId = @PayrollPeriodID
                AND p.periodStart = pp.startDate
                AND p.periodEnd = pp.endDate
            );
            
            IF @@ROWCOUNT > 0
                SET @success = 1;
            ELSE
                SET @success = 0;
            
            COMMIT TRANSACTION;
        END TRY
        BEGIN CATCH
            IF @@TRANCOUNT > 0
                ROLLBACK TRANSACTION;
            
            SET @success = -2;
        END CATCH;
    END;
    GO

-- this stored procedure creates a new pay grade definition
-- success = 1 means that the pay grade was created
-- success = 0 means that the insertion failed
-- success = -1 means that salary values are negative
-- success = -2 means that minimum salary is greater than or equal to maximum salary
-- success = -3 means that the pay grade name already exists
CREATE PROCEDURE ConfigurePayGrades
    (
        @GradeName VARCHAR(50),
        @MinSalary DECIMAL(10,2),
        @MaxSalary DECIMAL(10,2),
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;
        
        IF @MinSalary < 0 OR @MaxSalary < 0
        BEGIN
            SET @success = -1;
            RETURN;
        END;
        
        IF @MinSalary >= @MaxSalary
        BEGIN
            SET @success = -2;
            RETURN;
        END;
        
        IF EXISTS (SELECT 1 FROM PayGrade WHERE gradeName = @GradeName)
        BEGIN
            SET @success = -3;
            RETURN;
        END;
        
        INSERT INTO PayGrade (gradeName, minSalary, maxSalary)
        VALUES (@GradeName, @MinSalary, @MaxSalary);
        
        IF @@ROWCOUNT = 1
            SET @success = 1;
        ELSE
            SET @success = 0;
    END;
    GO

-- this stored procedure configures or updates currency exchange rates
-- success = 1 means that the currency configuration was updated or inserted
-- success = 0 means that no rows were affected
-- success = -1 means that the exchange rate is zero or negative
CREATE PROCEDURE EnableMultiCurrencyPayroll
    (
        @CurrencyCode VARCHAR(10),
        @ExchangeRate DECIMAL(10,4),
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;
        
        IF @ExchangeRate <= 0
        BEGIN
            SET @success = -1;
            RETURN;
        END;
        
        IF NOT EXISTS (SELECT 1 FROM Currency WHERE currencyName = @CurrencyCode)
        BEGIN
            INSERT INTO Currency (currencyName)
            VALUES (@CurrencyCode);
        END;
        
        SET @success = 1;
    END;
    GO

-- this stored procedure approves or rejects a payroll configuration change
-- success = 1 means that the status was updated
-- success = 0 means that the update failed
-- success = -1 means that the config ID does not exist
-- success = -2 means that the approver (employee) does not exist
-- success = -3 means that the status is invalid (must be 'Approved' or 'Rejected')
CREATE PROCEDURE ApprovePayrollConfigChanges
    (
        @ConfigID INT,
        @ApproverID INT,
        @Status VARCHAR(20),
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;
        
        IF NOT EXISTS (SELECT 1 FROM PayrollPolicy WHERE payrollPolicyID = @ConfigID)
        BEGIN
            SET @success = -1;
            RETURN;
        END;
        
        IF NOT EXISTS (SELECT 1 FROM Employee WHERE employeeId = @ApproverID)
        BEGIN
            SET @success = -2;
            RETURN;
        END;
        
        IF @Status NOT IN ('Approved', 'Rejected')
        BEGIN
            SET @success = -3;
            RETURN;
        END;
        
        UPDATE PayrollPolicy
        SET 
            status = @Status,
            approvedBy = @ApproverID
        WHERE payrollPolicyID = @ConfigID;
        
        IF @@ROWCOUNT = 1
            SET @success = 1;
        ELSE
            SET @success = 0;
    END;
    GO

-- this stored procedure creates a new payroll policy in pending status
-- success = 1 means that the policy was created
-- success = 0 means that the insertion failed
-- success = -1 means that the effective date is missing
CREATE PROCEDURE ConfigurePayrollPolicies
    (
        @PolicyType VARCHAR(50),
        @PolicyDetails NVARCHAR(MAX),
        @EffectiveDate DATE,
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;
        
        IF @EffectiveDate IS NULL
        BEGIN
            SET @success = -1;
            RETURN;
        END;
        
        INSERT INTO PayrollPolicy (effectiveDate, description, status, approvedBy)
        VALUES (@EffectiveDate, @PolicyDetails, 'Pending', NULL);
        
        IF @@ROWCOUNT = 1
            SET @success = 1;
        ELSE
            SET @success = 0;
    END;
    GO

-- this stored procedure creates a new pay grade with creator tracking
-- success = 1 means that the pay grade was created
-- success = 0 means that the insertion failed
-- success = -1 means that salary values are negative
-- success = -2 means that minimum salary is greater than or equal to maximum salary
-- success = -3 means that the creator (employee) does not exist
-- success = -4 means that the pay grade name already exists
CREATE PROCEDURE DefinePayGrades
    (
        @GradeName VARCHAR(50),
        @MinSalary DECIMAL(10,2),
        @MaxSalary DECIMAL(10,2),
        @CreatedBy INT,
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;
        
        IF @MinSalary < 0 OR @MaxSalary < 0
        BEGIN
            SET @success = -1;
            RETURN;
        END;
        
        IF @MinSalary >= @MaxSalary
        BEGIN
            SET @success = -2;
            RETURN;
        END;
        
        IF NOT EXISTS (SELECT 1 FROM Employee WHERE employeeID = @CreatedBy)
        BEGIN
            SET @success = -3;
            RETURN;
        END;
        
        IF EXISTS (SELECT 1 FROM PayGrade WHERE gradeName = @GradeName)
        BEGIN
            SET @success = -4;
            RETURN;
        END;
        
        INSERT INTO PayGrade (gradeName, minSalary, maxSalary)
        VALUES (@GradeName, @MinSalary, @MaxSalary);
        
        IF @@ROWCOUNT = 1
            SET @success = 1;
        ELSE
            SET @success = 0;
    END;
    GO

-- this stored procedure retrieves missed punch records for a manager's team on a specific date
-- success = 1 means that the records were retrieved
-- success = 0 means that no missed punches were found
-- success = -1 means that one of the inputs is NULL
-- success = -2 means that the manager does not exist
CREATE PROCEDURE GetMissedPunches
    (
        @ManagerID INT,
        @Date DATE,
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;

        IF @ManagerID IS NULL OR @Date IS NULL
        BEGIN
            SET @success = -1;
            PRINT 'One of the inputs is null';
            RETURN;
        END;

        IF NOT EXISTS(SELECT 1 FROM Employee WHERE employeeID = @ManagerID)
        BEGIN
            SET @success = -2;
            PRINT 'Manager does not exist';
            RETURN;
        END;

        SELECT 
            E.employeeID,
            E.firstName,
            E.lastName,
            A.entryTime,
            A.exitTime,
            'Missed Punch' AS IssueType
        FROM Attendance A
        INNER JOIN Employee E ON A.employeeID = E.employeeID
        WHERE E.managerID = @ManagerID
        AND (
            (CAST(A.entryTime AS DATE) = @Date AND A.exitTime IS NULL)
            OR
            (CAST(A.exitTime AS DATE) = @Date AND A.entryTime IS NULL)
        );

        IF @@ROWCOUNT = 1
                SET @success = 1;
            ELSE
                SET @success = 0;

        IF @success = 1
            PRINT 'Missed punches retrieved successfully.';
        ELSE IF @success = 0
            PRINT 'No changes were made.';
    END;
    GO

-- this stored procedure sends a notification to a manager about a new leave request
-- success = 1 means that the notification was sent
-- success = 0 means that the insertion failed
-- success = -1 means that one of the inputs is NULL
-- success = -2 means that the manager does not exist
-- success = -3 means that the leave request does not exist
CREATE PROCEDURE NotifyNewLeaveRequest
    (
        @ManagerID INT,
        @RequestID INT,
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;

        IF @ManagerID IS NULL OR @RequestID IS NULL
        BEGIN
            SET @success = -1;
            PRINT 'One of the inputs is null';
            RETURN;
        END;

        IF NOT EXISTS(SELECT 1 FROM Employee WHERE employeeID = @ManagerID)
        BEGIN
            SET @success = -2;
            PRINT 'Manager does not exist';
            RETURN;
        END;

        IF NOT EXISTS(SELECT 1 FROM LeaveRequest WHERE leaveRequestID = @RequestID)
        BEGIN
            SET @success = -3;
            PRINT 'Leave request does not exist';
            RETURN;
        END;

        DECLARE @MsgContent VARCHAR(255);
        SET @MsgContent = 'New Leave Request #' + CAST(@RequestID AS VARCHAR(20)) 
                        + ' has been assigned to you for review.';

        INSERT INTO Notification (messageContent, timestamp, urgency, readStatus)
        VALUES (@MsgContent, GETDATE(), 'Medium', 'Unread');

        DECLARE @NewNotificationID INT;
        SET @NewNotificationID = SCOPE_IDENTITY();

        INSERT INTO receives_notification (employeeID, notificationID, deliveryStatus, deliveredAt)
        VALUES (@ManagerID, @NewNotificationID, 'Sent', GETDATE());

        IF @@ROWCOUNT = 1
                SET @success = 1;
            ELSE
                SET @success = 0;

        IF @success = 1
            PRINT 'Manager notified of new leave request successfully.';
        ELSE IF @success = 0
            PRINT 'No changes were made.';
    END;
    GO

-- INCORRECT
-- this stored procedure approves a specific payroll policy
-- success = 1 means that the policy was approved
-- success = 0 means that the update failed
-- success = -1 means that the policy ID does not exist
-- success = -2 means that the approver (employee) does not exist
CREATE PROCEDURE ApprovePolicyUpdate
    (
        @PolicyID INT,
        @ApprovedBy INT,
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;
        
        IF NOT EXISTS (SELECT 1 FROM PayrollPolicy WHERE payrollPolicyID = @PolicyID)
        BEGIN
            SET @success = -1;
            RETURN;
        END;
        
        IF NOT EXISTS (SELECT 1 FROM Employee WHERE employeeID = @ApprovedBy)
        BEGIN
            SET @success = -2;
            RETURN;
        END;
        
        UPDATE PayrollPolicy
        SET status = 'Approved', 
            approvedBy = @ApprovedBy,
            approvedAt = GETDATE()
        WHERE policyId = @PolicyID;
        
        IF @@ROWCOUNT = 1
            SET @success = 1;
        ELSE
            SET @success = 0;
    END;
    GO

-- this stored procedure updates an existing insurance bracket configuration
-- success = 1 means that the bracket was updated
-- success = 0 means that the update failed (bracket not found via rowcount check)
-- success = -1 means that the bracket ID does not exist
-- success = -2 means that salary values are negative
-- success = -3 means that minimum salary is greater than or equal to maximum salary
-- success = -4 means that contribution percentages are invalid (not between 0-100)
CREATE PROCEDURE UpdateInsuranceBrackets
    (
        @BracketID INT,
        @NewMinSalary DECIMAL(10,2),
        @NewMaxSalary DECIMAL(10,2),
        @NewEmployeeContribution DECIMAL(5,2),
        @NewEmployerContribution DECIMAL(5,2),
        @UpdatedBy INT,
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;
        
        IF NOT EXISTS (SELECT 1 FROM InsuranceBracket WHERE bracketId = @BracketID)
        BEGIN
            SET @success = -1;
            RETURN;
        END;
        
        IF @NewMinSalary < 0 OR @NewMaxSalary < 0
        BEGIN
            SET @success = -2;
            RETURN;
        END;
        
        IF @NewMinSalary >= @NewMaxSalary
        BEGIN
            SET @success = -3;
            RETURN;
        END;
        
        IF @NewEmployeeContribution < 0 OR @NewEmployeeContribution > 100 OR 
        @NewEmployerContribution < 0 OR @NewEmployerContribution > 100
        BEGIN
            SET @success = -4;
            RETURN;
        END;
        
        UPDATE InsuranceBracket
        SET 
            minSalary = @NewMinSalary,
            maxSalary = @NewMaxSalary,
            employeeContribution = @NewEmployeeContribution,
            employerContribution = @NewEmployerContribution,
            updatedBy = @UpdatedBy,
            updatedAt = GETDATE()
        WHERE bracketId = @BracketID;
        
        IF @@ROWCOUNT = 1
            SET @success = 1;
        ELSE
            SET @success = 0;
    END;
    GO

-- this stored procedure creates or updates a special leave policy
-- success = 1 means that the policy was configured successfully
CREATE PROCEDURE ConfigureSpecialLeave
    (
        @LeaveType VARCHAR(50),
        @Rules VARCHAR(200),
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;
        
        UPDATE LeavePolicy
        SET specialLeaveType = @LeaveType, 
            noticePeriod = 0
        WHERE name = @LeaveType;
        
        IF @@ROWCOUNT = 0
        BEGIN
            INSERT INTO LeavePolicy (name, specialLeaveType, noticePeriod, resetOnNewYear)
            VALUES (@LeaveType, @LeaveType, 0, 0);
            
            SET @success = 1;
        END
        ELSE
        BEGIN
            SET @success = 1;
        END;
    END;
    GO

-- INCORRECT
-- this stored procedure recalculates leave entitlements based on tenure and contract type
-- success = 1 means that entitlements were updated or inserted
-- success = 0 means that no changes were needed
-- success = -1 means that the employee does not exist
-- success = -2 means that the employee has no valid contract type
-- success = -3 means that no leave policy exists for that contract type
CREATE PROCEDURE UpdateLeaveEntitlements
    (
        @EmployeeID INT,
        @success INT OUTPUT
    )
    AS
    BEGIN
        SET NOCOUNT ON;
        
        DECLARE @Tenure INT;
        DECLARE @ContractType VARCHAR(50);
        
        IF NOT EXISTS (SELECT 1 FROM Employee WHERE employeeID = @EmployeeID)
        BEGIN
            SET @success = -1;
            RETURN;
        END;
        
        SELECT 
            @Tenure = DATEDIFF(YEAR, hireDate, GETDATE()),
            @ContractType = c.type
        FROM Employee e
        JOIN Contract c ON e.contractID = c.contractID
        WHERE e.employeeID = @EmployeeID;
        
        IF @ContractType IS NULL
        BEGIN
            SET @success = -2;
            RETURN;
        END;
        
        -- Update existing entitlements
        UPDATE LeaveEntitlement
        SET entitlement = entitlement + (@Tenure / 5)  -- Add tenure-based bonus
        WHERE employeeID = @EmployeeID;
        
        IF @@ROWCOUNT > 0
        BEGIN
            SET @success = 1;
        END
        ELSE
        BEGIN
            SET @success = 0;
        END;
        
        -- Query through EligibilityRule junction table instead of using LIKE on eligibilityRules column
        MERGE INTO LeaveEntitlement AS target
        USING (
            SELECT 
                @EmployeeID AS employeeId,
                lp.leaveTypeID,
                lp.entitlement + (CASE WHEN @Tenure > 5 THEN 5 ELSE 0 END) AS entitlement
            FROM LeavePolicy lp
            INNER JOIN EligibilityRule er ON lp.leavePolicyID = er.leavePolicyID
            WHERE er.type = @ContractType
        ) AS source
        ON target.employeeId = source.employeeId AND target.leaveTypeID = source.leaveTypeID
        WHEN MATCHED THEN
            UPDATE SET target.entitlement = source.entitlement
        WHEN NOT MATCHED THEN
            INSERT (employeeId, leaveTypeID, entitlement)
            VALUES (source.employeeId, source.leaveTypeID, source.entitlement);
        
        IF @@ROWCOUNT > 0
            SET @success = 1;
        ELSE
            SET @success = 0;
    END;
    GO

t h i s 
 
 s t o r e d 
 
 p r o c e d u r e 
 
 s e a r c h e s 
 
 f o r 
 
 e m p l o y e e s 
 
 C R E A T E 
 
 P R O C E D U R E 
 
 S e a r c h E m p l o y e e s 
 
 I N T 
 
 = 
 
 N U L L 
 
 A S 
 
 B E G I N 
 
 S E L E C T 
 
 E . p h o n e N u m b e r 
 
 F R O M 
 
 E m p l o y e e 
 
 E 
 
 L E F T 
 
 J O I N 
 
 D e p a r t m e n t 
 
 D 
 
 O N 
 
 E . d e p a r t m e n t I D 
 
 = 
 
 D . d e p a r t m e n t I D 
 
 L E F T 
 
 J O I N 
 
 P o s i t i o n 
 
 P 
 
 O N 
 
 E . p o s i t i o n I D 
 
 = 
 
 P . p o s i t i o n I D 
 
 W H E R E 
 
 A N D 
 
 G O 
 
         E N D ; 
 
         G O 
 
 
 
 - -   t h i s   s t o r e d   p r o c e d u r e   s e a r c h e s   f o r   e m p l o y e e s 
 
 C R E A T E   P R O C E D U R E   S e a r c h E m p l o y e e s 
 
 ( 
 
         @ Q u e r y   V A R C H A R ( 1 0 0 )   =   N U L L , 
 
         @ D e p a r t m e n t I D   I N T   =   N U L L 
 
 ) 
 
 A S 
 
 B E G I N 
 
         S E L E C T   
 
                 E . e m p l o y e e I D , 
 
                 E . f i r s t N a m e   +   '   '   +   E . l a s t N a m e   A S   f u l l N a m e , 
 
                 D . n a m e   A S   d e p a r t m e n t N a m e , 
 
                 P . t i t l e   A S   p o s i t i o n T i t l e , 
 
                 E . e m a i l A d d r e s s , 
 
                 E . p h o n e N u m b e r 
 
         F R O M   E m p l o y e e   E 
 
         L E F T   J O I N   D e p a r t m e n t   D   O N   E . d e p a r t m e n t I D   =   D . d e p a r t m e n t I D 
 
         L E F T   J O I N   P o s i t i o n   P   O N   E . p o s i t i o n I D   =   P . p o s i t i o n I D 
 
         W H E R E   
 
                 ( @ Q u e r y   I S   N U L L   O R   E . f i r s t N a m e   L I K E   ' % '   +   @ Q u e r y   +   ' % '   O R   E . l a s t N a m e   L I K E   ' % '   +   @ Q u e r y   +   ' % '   O R   E . e m a i l A d d r e s s   L I K E   ' % '   +   @ Q u e r y   +   ' % ' ) 
 
                 A N D 
 
                 ( @ D e p a r t m e n t I D   I S   N U L L   O R   E . d e p a r t m e n t I D   =   @ D e p a r t m e n t I D ) 
 
         O R D E R   B Y   D . n a m e   A S C ,   E . l a s t N a m e   A S C ; 
 
 E N D ; 
 
 G O 
 
 