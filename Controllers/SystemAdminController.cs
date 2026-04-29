using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Services.Profile;
using Services.General;
using Services.Configuration;
using Services.Mission;
using HRMS.Exceptions;
using DTOs;

namespace HRMS.Controllers
{
    [Authorize(Roles = "SystemAdministrator")]
    public class SystemAdminController : Controller
    {
        private readonly IRoleAssignmentService roleAssignmentService;
        private readonly IEmployeeProfileService employeeProfileService;
        private readonly IAccountService accountService;
        private readonly IShiftConfigurationService shiftConfigurationService;
        private readonly IShiftSchedulingService shiftSchedulingService;
        private readonly Services.Mission.ILeaveService leaveService;
        private readonly Services.Attendance.IAttendanceService attendanceService;

        public SystemAdminController(
            IRoleAssignmentService roleAssignmentService, 
            IEmployeeProfileService employeeProfileService,
            IAccountService accountService,
            IShiftConfigurationService shiftConfigurationService,
            IShiftSchedulingService shiftSchedulingService,
            Services.Mission.ILeaveService leaveService,
            Services.Attendance.IAttendanceService attendanceService)
        {
            this.roleAssignmentService = roleAssignmentService;
            this.employeeProfileService = employeeProfileService;
            this.accountService = accountService;
            this.shiftConfigurationService = shiftConfigurationService;
            this.shiftSchedulingService = shiftSchedulingService;
            this.leaveService = leaveService;
            this.attendanceService = attendanceService;
        }

        [HttpGet]
        public async Task<IActionResult> CreateEmployee()
        {
            ViewBag.Departments = await accountService.GetDepartmentsAsync();
            ViewBag.Roles = await accountService.GetRolesAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateEmployee
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
            try 
            {
                int employeeID = await accountService.CreateEmployeeAsync
                (
                    firstName,
                    lastName,
                    departmentID,
                    roleID,
                    hireDate,
                    email,
                    phone,
                    nationalID,
                    birthDate,
                    birthCountry,
                    type
                );

                TempData["SuccessMessage"] = $"Employee created successfully with ID {employeeID}";
                return RedirectToAction("RegisterSuccess", "Account", new { employeeID });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error creating employee: " + ex.Message);
                ViewBag.Departments = await accountService.GetDepartmentsAsync();
                ViewBag.Roles = await accountService.GetRolesAsync();
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> AssignRole()
        {
            try
            {
                ViewBag.Roles = await employeeProfileService.GetRolesAsync();
                return View();
            }
            catch (Exception ex)
            {
                // Fallback if role service fails
                TempData["ErrorMessage"] = "Failed to load roles: " + ex.Message;
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> AssignRole(int employeeID, int roleID)
        {
            try
            {
                await roleAssignmentService.AssignRoleAsync(employeeID, roleID);
                TempData["SuccessMessage"] = $"Role assigned successfully to Employee {employeeID}.";
            }
            catch (EmployeeNotFoundException)
            {
                TempData["ErrorMessage"] = $"Employee with ID {employeeID} not found.";
            }
            catch (RoleNotFoundException)
            {
                TempData["ErrorMessage"] = "Selected Role not found.";
            }
            catch (RoleAlreadyAssignedException)
            {
                TempData["ErrorMessage"] = $"Employee {employeeID} already has this role.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred: " + ex.Message;
            }

            // Reload roles for the dropdown
            ViewBag.Roles = await employeeProfileService.GetRolesAsync();
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> ShiftTypes()
        {
            var shifts = await shiftConfigurationService.GetAllShiftTypesAsync();
            return View(shifts);
        }

        [HttpPost]
        public async Task<IActionResult> CreateShiftType(string shiftType, decimal allowanceAmount)
        {
            var userIdString = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdString, out int createdBy)) return RedirectToAction("Login", "Account");

            try
            {
                await shiftConfigurationService.ConfigureShiftAllowanceAsync(shiftType, allowanceAmount, createdBy);
                TempData["SuccessMessage"] = "Shift Type configured successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error configuring shift: " + ex.Message;
            }
            return RedirectToAction("ShiftTypes");
        }

        [HttpGet]
        public async Task<IActionResult> AssignShift()
        {
            ViewBag.Departments = await employeeProfileService.GetDepartmentsAsync();
            ViewBag.Employees = await employeeProfileService.SearchEmployeesAsync(null, null);
            // Use Actual Scheduled Shift Names
            ViewBag.ShiftNames = await shiftSchedulingService.GetScheduledShiftNamesAsync(); 
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AssignShift(DateTime startDate, DateTime endDate, string shiftType, string targetType, int? departmentID, int? employeeID)
        {
            try
            {
                if (targetType == "Department")
                {
                    if (departmentID == null) throw new System.Exception("Department is required.");
                    await shiftSchedulingService.AssignShiftsToDepartmentAsync(departmentID.Value, startDate, endDate, shiftType);
                    TempData["SuccessMessage"] = "Shifts assigned to Department successfully.";
                }
                else if (targetType == "Employee")
                {
                    if (employeeID == null) throw new System.Exception("Employee is required.");
                    await shiftSchedulingService.AssignShiftsToEmployeeAsync(employeeID.Value, startDate, endDate, shiftType);
                    TempData["SuccessMessage"] = "Shifts assigned to Employee successfully.";
                }
                else
                {
                    throw new System.Exception("Invalid Target Type.");
                }
            }
            catch (System.Exception ex)
            {
                TempData["ErrorMessage"] = "Error assigning shifts: " + ex.Message;
            }
            return RedirectToAction("AssignShift");
        }
        [HttpGet]
        public async Task<IActionResult> ManageEmployeeShifts(int? employeeId)
        {
            ViewBag.Employees = await employeeProfileService.SearchEmployeesAsync(null, null);
            ViewBag.ShiftNames = await shiftSchedulingService.GetScheduledShiftNamesAsync();
            
            if (employeeId.HasValue)
            {
                ViewBag.SelectedEmployeeId = employeeId;
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetEmployeeShifts(int employeeId, DateTime start, DateTime end)
        {
            var events = await shiftSchedulingService.GetEmployeeScheduleAsync(employeeId, start, end);
            return Json(events);
        }

        [HttpPost]
        public async Task<IActionResult> AddCustomShift(int employeeId, DateTime date, TimeSpan start, TimeSpan end, string name)
        {
            await shiftSchedulingService.AssignCustomShiftAsync(employeeId, date, start, end, name);
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> AssignExistingShift(int employeeId, DateTime date, string shiftType)
        {
            // Reusing the batch assignment but for single day
            await shiftSchedulingService.AssignShiftsToEmployeeAsync(employeeId, date, date, shiftType);
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAssignment(int assignmentId)
        {
            await shiftSchedulingService.DeleteAssignmentAsync(assignmentId);
            return Json(new { success = true });
        }
        [HttpGet]
        public async Task<IActionResult> ReassignEmployee()
        {
            // Populate dropdowns
            var employees = await employeeProfileService.SearchEmployeesAsync("", null); // Get all
            var departments = await employeeProfileService.GetDepartmentsAsync();
            
            // For managers, filter employees who have "Manager", "Head", "Director", or "Chief" in their title.
            // This is a heuristic until a verified "IsManager" flag is available.
            var potentialManagers = employees.Where(e => 
                !string.IsNullOrEmpty(e.PositionTitle) && 
                (e.PositionTitle.Contains("Manager", StringComparison.OrdinalIgnoreCase) || 
                 e.PositionTitle.Contains("Head", StringComparison.OrdinalIgnoreCase) || 
                 e.PositionTitle.Contains("Director", StringComparison.OrdinalIgnoreCase) || 
                 e.PositionTitle.Contains("Chief", StringComparison.OrdinalIgnoreCase) ||
                 e.PositionTitle.Contains("Lead", StringComparison.OrdinalIgnoreCase) ||
                 e.PositionTitle.Contains("Supervisor", StringComparison.OrdinalIgnoreCase) ||
                 e.PositionTitle.Contains("President", StringComparison.OrdinalIgnoreCase) ||
                 e.PositionTitle.Contains("VP", StringComparison.OrdinalIgnoreCase))
            ).ToList();
            
            ViewBag.Employees = employees;
            ViewBag.Departments = departments;
            ViewBag.Managers = potentialManagers;
            
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReassignEmployee(ReassignEmployeeDTO model)
        {
            if (model.Type == ReassignmentType.Department && model.NewDepartmentID == null)
            {
                 ModelState.AddModelError("NewDepartmentID", "Please select a department.");
            }
            // NewManagerID can be null (unassigned), so no mandatory check other than user intent.

            if (ModelState.IsValid)
            {
                try
                {
                    // 1. Fetch current profile to get missing piece
                    var currentProfile = await employeeProfileService.ViewEmployeeProfileAsync(model.EmployeeID);
                    
                    int targetDeptId = 0;
                    int? targetManagerId = null;

                    if (model.Type == ReassignmentType.Department)
                    {
                        targetDeptId = model.NewDepartmentID ?? 0; // Should be handled by validation above
                        targetManagerId = currentProfile.ManagerID;
                    }
                    else // Manager
                    {
                        targetDeptId = currentProfile.DepartmentID ?? 0; // Should assume active emp has dept, but handle safe
                        targetManagerId = model.NewManagerID;
                    }

                    await employeeProfileService.ReassignEmployeeAsync(model.EmployeeID, targetDeptId, targetManagerId);
                    TempData["SuccessMessage"] = "Employee reassigned successfully.";
                    return RedirectToAction(nameof(ReassignEmployee));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            // Reload dropdowns if validation fails
            var employees = await employeeProfileService.SearchEmployeesAsync("", null);
            var departments = await employeeProfileService.GetDepartmentsAsync();
            var potentialManagers = employees.Where(e => 
                !string.IsNullOrEmpty(e.PositionTitle) && 
                (e.PositionTitle.Contains("Manager", StringComparison.OrdinalIgnoreCase) || 
                 e.PositionTitle.Contains("Head", StringComparison.OrdinalIgnoreCase) || 
                 e.PositionTitle.Contains("Director", StringComparison.OrdinalIgnoreCase) || 
                 e.PositionTitle.Contains("Chief", StringComparison.OrdinalIgnoreCase) ||
                 e.PositionTitle.Contains("Lead", StringComparison.OrdinalIgnoreCase) ||
                 e.PositionTitle.Contains("Supervisor", StringComparison.OrdinalIgnoreCase) ||
                 e.PositionTitle.Contains("President", StringComparison.OrdinalIgnoreCase) ||
                 e.PositionTitle.Contains("VP", StringComparison.OrdinalIgnoreCase))
            ).ToList();

            ViewBag.Employees = employees;
            ViewBag.Departments = departments;
            ViewBag.Managers = potentialManagers;

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> SyncLeaveAttendance()
        {
            try
            {
                var approvedLeaves = await leaveService.GetApprovedLeavesAsync();
                int count = 0;
                foreach (var leave in approvedLeaves)
                {
                    await attendanceService.SyncLeaveToAttendanceAsync(leave.LeaveRequestId);
                    count++;
                }
                TempData["SuccessMessage"] = $"Successfully synced {count} leave records with attendance.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error syncing leaves: {ex.Message}";
            }
            
            return RedirectToAction("Attendance", "Home");
        }
    }
}
