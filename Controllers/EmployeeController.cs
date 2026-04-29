namespace HRMS.Controllers;

using Microsoft.AspNetCore.Mvc;
using Services.Profile;
using Services.Mission;
using Microsoft.AspNetCore.Authorization;
using Services.Attendance;
using System.Security.Claims;

[Authorize]
public class EmployeeController : Controller
{
    private readonly IEmployeeProfileService employeeProfileService;
    private readonly IMissionService missionService;
    private readonly ILeaveService leaveService;
    private readonly IAttendanceService attendanceService;

    public EmployeeController(
        IEmployeeProfileService employeeProfileService,
        IMissionService missionService,
        ILeaveService leaveService,
        IAttendanceService attendanceService)
    {
        this.employeeProfileService = employeeProfileService;
        this.missionService = missionService;
        this.leaveService = leaveService;
        this.attendanceService = attendanceService;
    }

    [HttpGet]
    public async Task<IActionResult> RequestLeave()
    {
        ViewBag.LeaveTypes = await leaveService.GetLeaveTypesAsync();
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> RequestLeave(int leaveTypeId, DateTime startDate, DateTime endDate, string justification, IFormFile? attachment)
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdString, out int employeeId)) return RedirectToAction("Login", "Account");

        try
        {
            await leaveService.SubmitLeaveRequestAsync(employeeId, leaveTypeId, startDate, endDate, justification, attachment);
            TempData["SuccessMessage"] = "Leave request submitted successfully.";
            return RedirectToAction("Index", "Home"); // Or wherever appropriate
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "Error submitting: " + ex.Message;
            ViewBag.LeaveTypes = await leaveService.GetLeaveTypesAsync();
            return View();
        }
    }

    [HttpGet]
    public async Task<IActionResult> LeaveHistory()
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdString, out int employeeId)) return RedirectToAction("Login", "Account");

        var model = new 
        {
            Balances = await leaveService.GetLeaveBalancesAsync(employeeId),
            History = await leaveService.GetLeaveHistoryAsync(employeeId)
        };

        return View(model);
    }

    [HttpGet]
    public IActionResult MyProfile()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (int.TryParse(userId, out int id))
        {
            return RedirectToAction("Profile", new { id = id });
        }
        return RedirectToAction("Login", "Account");
    }

    public async Task<IActionResult> Profile(int id)
    {
        var profile = await employeeProfileService.ViewEmployeeProfileAsync(id);

        var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (int.TryParse(userIdStr, out int userId))
        {
            bool isSelf = userId == id;
            bool isAdmin = User.IsInRole("SystemAdministrator") || User.IsInRole("HRAdministrator") || User.IsInRole("LineManager");
            bool isManager = profile.ManagerID == userId;

            if (!isSelf && !isAdmin && !isManager)
            {
                // Access Denied
                return RedirectToAction("Index", "Home");
            }
        }
        else
        {
            return RedirectToAction("Login", "Account");
        }

        return View(profile);
    }

    [HttpPost]
    public async Task<IActionResult> UpdatePersonalDetails(int employeeID, string field, string value)
    {
        // 1. Authentication Check: Ensure user is updating their OWN profile
        var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdStr, out int userId) || userId != employeeID)
        {
            return Forbid(); // Or Unauthorized
        }

        // 2. Field Whitelist: Strictly allow only personal details
        var allowedFields = new List<string>
        {
            "emailAddress", "phoneNumber", "address", 
            "emergencyContactName", "emergencyContactPhone", "relationship", 
            "biography"
        };

        if (!allowedFields.Contains(field))
        {
            TempData["ErrorMessage"] = "You are not authorized to edit this field.";
            return RedirectToAction("Profile", new { id = employeeID });
        }

        value ??= string.Empty;

        await employeeProfileService.UpdateEmployeeProfileAsync(employeeID, field, value);
        
        TempData["SuccessMessage"] = "Profile updated successfully.";

        return RedirectToAction("Profile", new { id = employeeID });
        }

        [HttpGet]
        public async Task<IActionResult> Missions()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int userId))
            {
                return Unauthorized();
            }

            try 
            {
                var missions = await missionService.ViewAssignedMissionsAsync(userId);
                return View(missions);
            }
            catch (Exception ex)
            {
                // Handle case where employee not found or generic error
                TempData["ErrorMessage"] = "Could not load missions: " + ex.Message;
                return View(new DTOs.MissionsDTO());
            }
        }


        [HttpGet]
        public async Task<IActionResult> Attendance()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int employeeId)) return RedirectToAction("Login", "Account");

            var history = await attendanceService.GetMyAttendanceAsync(employeeId);
            return View(history);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckIn()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int employeeId)) return RedirectToAction("Login", "Account");

            await attendanceService.CheckInAsync(employeeId);
            return RedirectToAction(nameof(Attendance));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckOut()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int employeeId)) return RedirectToAction("Login", "Account");

            await attendanceService.CheckOutAsync(employeeId);
            return RedirectToAction(nameof(Attendance));
        }
        [HttpGet]
        public IActionResult RequestCorrection()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RequestCorrection(DateTime date, string correctionType, string reason)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int employeeId)) return RedirectToAction("Login", "Account");

            try
            {
                await attendanceService.SubmitCorrectionRequestAsync(employeeId, date, correctionType, reason);
                TempData["SuccessMessage"] = "Correction request submitted successfully.";
                return RedirectToAction(nameof(Attendance));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error: " + ex.Message;
                return View();
            }
        }
        [HttpGet]
        public async Task<IActionResult> ViewRules()
        {
            var rules = await attendanceService.GetAttendanceRulesAsync();
            return View(rules);
        }
        [HttpGet]
        public async Task<IActionResult> OrgHierarchy()
        {
            var hierarchy = await employeeProfileService.GetOrgHierarchyAsync();
            return View(hierarchy);
        }


        [HttpPost]
        public async Task<IActionResult> SyncOfflineLogs([FromBody] List<OfflineLogRequest> logs)
        {
            var userIdString = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdString, out int employeeID)) return Unauthorized();

            // Sort by time just in case
            if (logs != null && logs.Any())
            {
                logs = logs.OrderBy(l => l.Timestamp).ToList();

                int successCount = 0;
                foreach (var log in logs)
                {
                    try
                    {
                        // DeviceID is optional, passing 0 or null if SP handles it. 
                        // Service method expects int deviceID. 0 is fine.
                        await attendanceService.SyncOfflineAttendanceAsync(0, employeeID, log.Timestamp, log.Type);
                        successCount++;
                    }
                    catch
                    {
                        // Ignore errors for individual logs (best effort)
                    }
                }
                return Json(new { success = true, count = successCount });
            }
            return Json(new { success = true, count = 0 });
        }

        public class OfflineLogRequest
        {
            public DateTime Timestamp { get; set; }
            public string Type { get; set; } // 'CheckIn' or 'CheckOut'
        }
    }

