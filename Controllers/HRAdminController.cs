using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Services.Mission;
using Services.Profile;
using DTOs;
using HRMS.Exceptions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HRMS.Controllers
{
    [Authorize(Roles = "HRAdministrator")]
    public class HRAdminController : Controller
    {
        private readonly IEmployeeProfileService employeeProfileService;
        private readonly IContractService contractService;
        private readonly IMissionService missionService;
        private readonly IShiftSchedulingService shiftSchedulingService;
        private readonly Services.Attendance.IAttendanceService attendanceService;
        private readonly Services.Mission.ILeaveService leaveService;
        private readonly Services.Notification.INotificationService notificationService;

        public HRAdminController(
            IEmployeeProfileService employeeProfileService, 
            IContractService contractService, 
            IMissionService missionService, 
            IShiftSchedulingService shiftSchedulingService, 
            Services.Attendance.IAttendanceService attendanceService, 
            Services.Mission.ILeaveService leaveService,
            Services.Notification.INotificationService notificationService)
        {
            this.employeeProfileService = employeeProfileService;
            this.contractService = contractService;
            this.missionService = missionService;
            this.shiftSchedulingService = shiftSchedulingService;
            this.attendanceService = attendanceService;
            this.leaveService = leaveService;
            this.notificationService = notificationService;
        }

        [HttpGet]
        public async Task<IActionResult> DiversityReport()
        {
            var report = await employeeProfileService.GetDiversityReportAsync();
            return View(report);
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> FindEmployee(int id)
        {
            try
            {
                await employeeProfileService.FindEmployeeAsync(id);
                return RedirectToAction("EditProfile", new { id = id });
            }
            catch (EmployeeNotFoundException)
            {
                TempData["ErrorMessage"] = $"Employee with ID {id} not found. Please try again.";
                return RedirectToAction("Index");
            }
        }
        
        [HttpGet]
        public async Task<IActionResult> EditProfile(int id)
        {
            try 
            {
                var profile = await employeeProfileService.ViewEmployeeProfileAsync(id);
                ViewBag.Managers = await employeeProfileService.GetLineManagersAsync();
                return View(profile);
            }
            catch (EmployeeNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfileField(int employeeID, string field, string value)
        {
            try
            {
                // Basic null check for value, set to empty string if null to avoid SQL issues if SP doesn't handle DBNull
                value ??= string.Empty;

                if (field == "profileCompletion")
                {
                    if (int.TryParse(value, out int completeness))
                    {
                        await employeeProfileService.SetProfileCompletenessAsync(employeeID, completeness);
                    }
                    else
                    {
                        throw new ArgumentException("Profile completion must be a valid integer.");
                    }
                }
                else
                {
                    await employeeProfileService.UpdateEmployeeProfileAsync(employeeID, field, value);
                }

                TempData["SuccessMessage"] = $"{field} updated successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error updating {field}: {ex.Message}";
            }

            return RedirectToAction("EditProfile", new { id = employeeID });
        }
        [HttpGet]
        public async Task<IActionResult> CreateContract(int? employeeID)
        {
            try
            {
                if (employeeID.HasValue)
                {
                    await employeeProfileService.FindEmployeeAsync(employeeID.Value);
                    var model = new DTOs.CreateContractDTO 
                    { 
                        EmployeeID = employeeID.Value,
                        StartDate = DateTime.Today,
                        EndDate = DateTime.Today.AddYears(1)
                    };
                    return View(model);
                }
                
                // If no ID provided, load empty form. Dropdown will be populated in View or via ViewBag
                // We need to populate ViewBag.Employees for the dropdown
                ViewBag.Employees = await employeeProfileService.SearchEmployeesAsync(null, null);
                
                var emptyModel = new DTOs.CreateContractDTO 
                { 
                    StartDate = DateTime.Today,
                    EndDate = DateTime.Today.AddYears(1)
                };
                return View(emptyModel);
            }
            catch (EmployeeNotFoundException)
            {
                TempData["ErrorMessage"] = "Employee not found.";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateContract(DTOs.CreateContractDTO model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Employees = await employeeProfileService.SearchEmployeesAsync(null, null);
                return View(model);
            }

            try
            {
                int contractID = await employeeProfileService.CreateContractAsync(model);
                
                // Send Notification
                await notificationService.SendNotificationAsync(model.EmployeeID, "A new contract has been created for you.", "HR", "Normal");

                TempData["SuccessMessage"] = $"Contract created successfully! ID: {contractID}";
                return RedirectToAction("EditProfile", new { id = model.EmployeeID });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error creating contract: {ex.Message} (Received ID: {model.EmployeeID})";
                ViewBag.Employees = await employeeProfileService.SearchEmployeesAsync(null, null);
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Contracts(int days = 30)
        {
            var viewModel = new HRMS.ViewModels.ContractsDashboardViewModel
            {
                ActiveContracts = await contractService.GetActiveContractsAsync(),
                ExpiringContracts = await contractService.GetExpiringContractsAsync(days),
                ExpiringDaysFilter = days
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> RenewContract(int contractID, DateTime newEndDate, string returnUrl = null)
        {
            try
            {
                await contractService.RenewContractAsync(contractID, newEndDate);
                TempData["SuccessMessage"] = "Contract renewed successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to renew contract: " + ex.Message;
            }
            
            if (returnUrl == "Contracts") return RedirectToAction("Contracts");
            return RedirectToAction("Contracts"); // Default to dashboard
        }

        [HttpGet]
        public async Task<IActionResult> AssignMission()
        {
            var employees = await employeeProfileService.SearchEmployeesAsync(null, null);
            
            // Strictly fetch Line Managers as per requirement
            var managers = await employeeProfileService.GetLineManagersAsync();
            
            // Fallback if no line managers defined (unlikely but safe)
            if (!managers.Any()) managers = employees;

            ViewBag.Employees = employees;
            ViewBag.Managers = managers;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AssignMission(int employeeID, int managerID, string destination, DateTime startDate, DateTime endDate)
        {
            try
            {
                await missionService.AssignMissionAsync(employeeID, managerID, destination, startDate, endDate);
                TempData["SuccessMessage"] = "Mission assigned successfully.";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to assign mission: " + ex.Message;
                // Reload dropdowns
                var employees = await employeeProfileService.SearchEmployeesAsync(null, null);
                var managers = employees.Where(e => 
                    !string.IsNullOrEmpty(e.PositionTitle) && (
                    e.PositionTitle.Contains("Manager", StringComparison.OrdinalIgnoreCase) || 
                    e.PositionTitle.Contains("Head", StringComparison.OrdinalIgnoreCase) || 
                    e.PositionTitle.Contains("Director", StringComparison.OrdinalIgnoreCase) ||
                    e.PositionTitle.Contains("Lead", StringComparison.OrdinalIgnoreCase) ||
                    e.PositionTitle.Contains("Supervisor", StringComparison.OrdinalIgnoreCase)
                    )).ToList();
                if (!managers.Any()) managers = employees;

                ViewBag.Employees = employees;
                ViewBag.Managers = managers;
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> ShiftScheduler()
        {
            // ViewBag.Employees removed as we don't assign to employees anymore
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GenerateSplitSchedule(DateTime startDate, DateTime endDate, TimeSpan start1, TimeSpan end1, TimeSpan start2, TimeSpan end2)
        {
            try
            {
                await shiftSchedulingService.CreateSplitScheduleAsync(startDate, endDate, start1, end1, start2, end2);
                TempData["SuccessMessage"] = "Split Shift schedule created successfully.";
            }
            catch (System.Exception ex)
            {
                TempData["ErrorMessage"] = "Error creating split schedule: " + ex.Message;
            }
            return RedirectToAction("ShiftScheduler");
        }

        [HttpPost]
        public async Task<IActionResult> GenerateRotationalSchedule(DateTime startDate, DateTime endDate, int cycleLengthDays)
        {
            try
            {
                // Hardcoded types for now, or could come from form
                var cycleTypes = new string[] { "Morning", "Night", "Off" }; 
                await shiftSchedulingService.CreateRotationalScheduleAsync(startDate, endDate, cycleLengthDays, cycleTypes);
                TempData["SuccessMessage"] = "Rotational Shift schedule created successfully.";
            }
            catch (System.Exception ex)
            {
                TempData["ErrorMessage"] = "Error creating rotational schedule: " + ex.Message;
            }
            return RedirectToAction("ShiftScheduler");
        }
        [HttpGet]
        public async Task<IActionResult> AttendanceRules()
        {
            var rules = await attendanceService.GetAttendanceRulesAsync();
            return View(rules);
        }

        [HttpPost]
        public async Task<IActionResult> SetGracePeriod(int minutes)
        {
            try
            {
                await attendanceService.SetGracePeriodAsync(minutes);
                TempData["SuccessMessage"] = "Grace period updated.";
            }
            catch (Exception ex)
            {
                 TempData["ErrorMessage"] = "Error: " + ex.Message;
            }
            return RedirectToAction("AttendanceRules");
        }

        [HttpPost]
        public async Task<IActionResult> DefinePenaltyThreshold(int lateMinutes, string deductionType)
        {
             try
            {
                await attendanceService.DefinePenaltyThresholdAsync(lateMinutes, deductionType);
                TempData["SuccessMessage"] = "Penalty threshold updated.";
            }
            catch (Exception ex)
            {
                 TempData["ErrorMessage"] = "Error: " + ex.Message;
            }
            return RedirectToAction("AttendanceRules");
        }
        [HttpPost]
        public async Task<IActionResult> DefineShortTimeRules(string ruleName, int lateMinutes, int earlyLeaveMinutes, string penaltyType)
        {
             try
            {
                await attendanceService.DefineShortTimeRulesAsync(ruleName, lateMinutes, earlyLeaveMinutes, penaltyType);
                TempData["SuccessMessage"] = "Short-time rules updated.";
            }
            catch (Exception ex)
            {
                 TempData["ErrorMessage"] = "Error: " + ex.Message;
            }
            return RedirectToAction("AttendanceRules");
        }

        // Leave Types Management
        [HttpGet]
        public async Task<IActionResult> ManageLeaveTypes()
        {
            var types = await leaveService.GetLeaveTypesAsync();
            return View(types);
        }

        [HttpGet]
        public async Task<IActionResult> EditLeaveType(int? id)
        {
            if (id == null) return View(new HRMS.Models.LeaveType());
            
            var types = await leaveService.GetLeaveTypesAsync();
            var type = types.FirstOrDefault(t => t.LeaveTypeId == id);
            return View(type);
        }

        [HttpPost]
        public async Task<IActionResult> EditLeaveType(HRMS.Models.LeaveType leaveType)
        {
            try
            {
                if (leaveType.LeaveTypeId == 0)
                {
                    await leaveService.AddLeaveTypeAsync(leaveType);
                    TempData["SuccessMessage"] = "Leave type created.";
                }
                else
                {
                    await leaveService.UpdateLeaveTypeAsync(leaveType);
                    TempData["SuccessMessage"] = "Leave type updated.";
                }
                return RedirectToAction(nameof(ManageLeaveTypes));
            }
            catch (Exception ex)
            {
                 TempData["ErrorMessage"] = "Error: " + ex.Message;
                 return View(leaveType);
            }
        }
        
        [HttpPost]
        public async Task<IActionResult> DeleteLeaveType(int id)
        {
            await leaveService.DeleteLeaveTypeAsync(id);
             TempData["SuccessMessage"] = "Leave type deleted.";
            return RedirectToAction(nameof(ManageLeaveTypes));
        }

        // Leave Policies Management
        [HttpGet]
        public async Task<IActionResult> ManageLeavePolicies()
        {
            var policies = await leaveService.GetLeavePoliciesAsync();
            return View(policies);
        }

        [HttpGet]
        public async Task<IActionResult> EditLeavePolicy(int? id)
        {
            if (id == null) return View(new HRMS.Models.LeavePolicy());
            
            var policy = await leaveService.GetLeavePolicyAsync(id.Value);
            return View(policy);
        }

        [HttpPost]
        public async Task<IActionResult> EditLeavePolicy(HRMS.Models.LeavePolicy policy)
        {
             try
            {
                if (policy.LeavePolicyId == 0)
                {
                    await leaveService.AddLeavePolicyAsync(policy);
                    TempData["SuccessMessage"] = "Leave policy created.";
                }
                else
                {
                    await leaveService.UpdateLeavePolicyAsync(policy);
                    TempData["SuccessMessage"] = "Leave policy updated.";
                }
                return RedirectToAction(nameof(ManageLeavePolicies));
            }
            catch (Exception ex)
            {
                 TempData["ErrorMessage"] = "Error: " + ex.Message;
                 return View(policy);
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteLeavePolicy(int id)
        {
            await leaveService.DeleteLeavePolicyAsync(id);
            TempData["SuccessMessage"] = "Leave policy deleted.";
            return RedirectToAction(nameof(ManageLeavePolicies));
        }

        // Leave Entitlements
        [HttpGet]
        public async Task<IActionResult> ManageEntitlements()
        {
            // Use SearchEmployeesAsync(null, null) to get all employees as summaries
            var employees = await employeeProfileService.SearchEmployeesAsync(null, null);
            return View(employees);
        }

        [HttpGet]
        public async Task<IActionResult> EditEntitlements(int id)
        {
            var employee = await employeeProfileService.ViewEmployeeProfileAsync(id);
            if (employee == null) return NotFound();

            var entitlements = await leaveService.GetEmployeeEntitlementsAsync(id);
            
            // DTO has FirstName/LastName or FullName
            ViewBag.EmployeeName = $"{employee.FirstName} {employee.LastName}"; 
            ViewBag.EmployeeId = id;

            return View(entitlements);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateEntitlement(int employeeId, int leaveTypeId, int entitlement)
        {
            await leaveService.UpdateLeaveEntitlementAsync(employeeId, leaveTypeId, entitlement);
            return RedirectToAction("EditEntitlements", new { id = employeeId });
        }
        
        [HttpPost]
        public async Task<IActionResult> SaveEntitlements(int employeeId, Dictionary<int, int> entitlements)
        {
            foreach(var kvp in entitlements)
            {
                await leaveService.UpdateLeaveEntitlementAsync(employeeId, kvp.Key, kvp.Value);
            }
             TempData["SuccessMessage"] = "Entitlements updated.";
             return RedirectToAction("EditEntitlements", new { id = employeeId });
        }

        // Leave Request Override
        [HttpGet]
        public async Task<IActionResult> ManageLeaveRequests()
        {
            var requests = await leaveService.GetRecentLeaveRequestsAsync();
            return View(requests);
        }

        [HttpPost]
        public async Task<IActionResult> OverrideRequest(int requestId, string newStatus, string reason)
        {
            // Assuming current user is the admin. We can get ID from claims if needed, simplified for now.
            // In a real app, User.Identity.Name or Claims would give the ID. 
            // For now passing 0 or a placeholder as 'adminId' since tracking 'actor' might be separate.
            int adminId = 0; 
            
            try 
            {
                await leaveService.OverrideLeaveStatusAsync(requestId, newStatus, adminId, reason);
                TempData["SuccessMessage"] = "Request updated successfully.";
            }
            catch(Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction(nameof(ManageLeaveRequests));
        }
        [HttpGet]
        public async Task<IActionResult> DepartmentStats()
        {
            var stats = await employeeProfileService.GetDepartmentStatisticsAsync();
            return View(stats);
        }
    }
}
