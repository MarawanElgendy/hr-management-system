using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Services.Profile;
using Services.Mission;
using Services.Configuration;
using Services.Attendance;
using System.Security.Claims;

namespace HRMS.Controllers
{
    [Authorize]
    public class ManagerController : Controller
    {
        private readonly IEmployeeProfileService employeeProfileService;
        private readonly IMissionService missionService;
        private readonly IShiftSchedulingService shiftSchedulingService;
        private readonly Services.Configuration.IShiftConfigurationService shiftConfigurationService;
        private readonly ILeaveService leaveService;
        private readonly IAttendanceService attendanceService;
        private readonly Services.Notification.INotificationService notificationService;

        public ManagerController(
            IEmployeeProfileService employeeProfileService, 
            IMissionService missionService,
            IShiftSchedulingService shiftSchedulingService,
            Services.Configuration.IShiftConfigurationService shiftConfigurationService,
            ILeaveService leaveService,
            IAttendanceService attendanceService,
            Services.Notification.INotificationService notificationService)
        {
            this.employeeProfileService = employeeProfileService;
            this.missionService = missionService;
            this.shiftSchedulingService = shiftSchedulingService;
            this.shiftConfigurationService = shiftConfigurationService;
            this.leaveService = leaveService;
            this.attendanceService = attendanceService;
            this.notificationService = notificationService;
        }


        [HttpGet]
        public async Task<IActionResult> LeaveRequests()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int managerId))
            {
                return RedirectToAction("Login", "Account");
            }

            var requests = await leaveService.GetPendingTeamLeavesAsync(managerId);
            return View(requests);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveLeave(int id)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int managerId)) return RedirectToAction("Login", "Account");

            try
            {
                await leaveService.ApproveRequestAsync(id, managerId);
                TempData["Success"] = "Leave Request Approved.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(LeaveRequests));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectLeave(int id)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int managerId)) return RedirectToAction("Login", "Account");

            try
            {
                await leaveService.RejectRequestAsync(id, managerId);
                TempData["Success"] = "Leave Request Rejected.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(LeaveRequests));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleEmployeeFlag(int employeeId)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int managerId)) return RedirectToAction("Login", "Account");

            try
            {
                await leaveService.ToggleEmployeeFlagAsync(employeeId, managerId);
                TempData["Success"] = "Employee flag updated.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(LeaveRequests));
        }

        [HttpGet]
        public async Task<IActionResult> MyTeam()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int managerID))
            {
                return RedirectToAction("Login", "Account");
            }

            var teamMembers = await employeeProfileService.GetTeamMembersAsync(managerID);
            return View(teamMembers);
        }

        [HttpGet]
        public async Task<IActionResult> MissionRequests()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int managerID)) return RedirectToAction("Login", "Account");

            var requests = await missionService.GetPendingMissionsAsync(managerID);
            return View(requests);
        }

        [HttpPost]
        public async Task<IActionResult> ApproveMission(int missionID)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int managerID)) return RedirectToAction("Login", "Account");

            try
            {
                await missionService.ApproveMissionRequestAsync(managerID, missionID);
                TempData["SuccessMessage"] = "Mission approved successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error approving mission: " + ex.Message;
            }
            return RedirectToAction("MissionRequests");
        }

        [HttpPost]
        public async Task<IActionResult> RejectMission(int missionID)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int managerID)) return RedirectToAction("Login", "Account");

            try
            {
                await missionService.RejectMissionRequestAsync(managerID, missionID);
                TempData["SuccessMessage"] = "Mission rejected.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error rejecting mission: " + ex.Message;
            }
            return RedirectToAction("MissionRequests");
        }


        [HttpGet]
        public async Task<IActionResult> AssignTeamShift()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int managerID)) return RedirectToAction("Login", "Account");

            ViewBag.TeamMembers = await employeeProfileService.GetTeamMembersAsync(managerID);
            // Use Actual Scheduled Shift Names
            ViewBag.ShiftNames = await shiftSchedulingService.GetScheduledShiftNamesAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AssignTeamShift(DateTime startDate, DateTime endDate, string shiftType, int? employeeID)
        {
            try
            {
                // Note: Manager can only assign to team. Verify if Emp is in team? 
                // For simplicity assuming dropdown restriction + backend checks are sufficient for now, or trust Manager.
                // ideally checking if employeeID is in GetTeamMembersAsync results.
                
                if (employeeID == null) throw new System.Exception("Employee is required.");
                await shiftSchedulingService.AssignShiftsToEmployeeAsync(employeeID.Value, startDate, endDate, shiftType);
                TempData["SuccessMessage"] = "Shifts assigned to team member successfully.";
            }
            catch (System.Exception ex)
            {
                TempData["ErrorMessage"] = "Error assigning shifts: " + ex.Message;
            }
            return RedirectToAction("AssignTeamShift");
        }

        [HttpGet]
        public async Task<IActionResult> TeamAttendance(DateTime? startDate, DateTime? endDate)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int managerId)) return RedirectToAction("Login", "Account");

            var start = startDate ?? DateTime.Today.AddDays(-7);
            var end = endDate ?? DateTime.Today;

            var data = await attendanceService.ViewTeamAttendanceAsync(managerId, start, end);
            
            ViewBag.StartDate = start;
            ViewBag.EndDate = end;

            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> SendNotification()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int managerId)) return RedirectToAction("Login", "Account");

            var team = await employeeProfileService.GetTeamMembersAsync(managerId);
            
            // Get Distinct Departments
            var departments = team
                .Where(t => t.DepartmentID.HasValue && !string.IsNullOrEmpty(t.DepartmentName))
                .Select(t => new { Id = t.DepartmentID.Value, Name = t.DepartmentName })
                .Distinct()
                .ToList();

            ViewBag.TeamMembers = team;
            ViewBag.Departments = departments;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendNotification(string scope, int? departmentId, List<int> recipientIds, string message, string urgency)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int managerId)) return RedirectToAction("Login", "Account");

            if(string.IsNullOrWhiteSpace(message))
            {
                TempData["Error"] = "Message content is required.";
                return RedirectToAction(nameof(SendNotification));
            }

            try
            {
                var team = await employeeProfileService.GetTeamMembersAsync(managerId);
                var recipients = new List<int>();

                if (scope == "employee" && recipientIds != null && recipientIds.Any())
                {
                    // Validate Membership for ALL selected IDs
                    var invalidIds = recipientIds.Except(team.Select(t => t.EmployeeID)).ToList();
                    if(invalidIds.Any()) 
                        throw new Exception("One or more selected employees are not in your team.");
                    
                    recipients.AddRange(recipientIds);
                }
                else if (scope == "department" && departmentId.HasValue)
                {
                    var deptMembers = team.Where(t => t.DepartmentID == departmentId.Value).Select(t => t.EmployeeID).ToList();
                    if(!deptMembers.Any())
                         throw new Exception("No team members found in the selected department.");

                    recipients.AddRange(deptMembers);
                }
                else // scope == "all"
                {
                    recipients.AddRange(team.Select(t => t.EmployeeID));
                }

                if (!recipients.Any()) throw new Exception("No recipients selected.");

                foreach(var empId in recipients.Distinct())
                {
                     await notificationService.SendNotificationAsync(empId, message, "Manager", urgency ?? "Normal");
                }

                TempData["Success"] = $"Notification sent to {recipients.Distinct().Count()} team member(s).";
            }
            catch(Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(SendNotification));
        }
    }
}