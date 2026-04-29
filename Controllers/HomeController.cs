namespace HRMS.Controllers;

using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using HRMS.Models;

[Authorize]
public class HomeController : Controller
{
    private readonly Services.Notification.INotificationService _notificationService;
    private readonly Services.Profile.IEmployeeProfileService _employeeProfileService;

    public HomeController(Services.Notification.INotificationService notificationService, Services.Profile.IEmployeeProfileService employeeProfileService)
    {
        _notificationService = notificationService;
        _employeeProfileService = employeeProfileService;
    }

    public async Task<IActionResult> Index()
    {
        int userId = 1; // Default to Admin for dev/demo if not authenticated? No, let's try to be precise.
        
        if(User.Identity.IsAuthenticated) {
            // Retrieve ID from Claims
            var claim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier); // Created in AccountController as NameIdentifier
            if (claim != null && int.TryParse(claim.Value, out int parsed)) 
            {
                userId = parsed;
            }
        }

        var notifications = await _notificationService.GetUnreadNotificationsAsync(userId);
        ViewBag.Notifications = notifications;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> MarkNotificationRead(int id)
    {
        int userId = 1;
        if(User.Identity.IsAuthenticated) {
            var claim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (claim != null && int.TryParse(claim.Value, out int parsed)) userId = parsed;
        }

        await _notificationService.MarkAsReadAsync(id, userId);
        return Ok();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
    public IActionResult Employees() => View();
    public IActionResult Hierarchy() => View();
    public IActionResult Shift() => View();
    public IActionResult Mission() => View();
    public IActionResult Attendance() => View();
    public IActionResult Leave() => View();
    public IActionResult Contract() => View();
    
    [HttpGet]
    public async Task<IActionResult> EmployeeDirectory(string? query, int? departmentId)
    {
        ViewBag.Departments = await _employeeProfileService.GetDepartmentsAsync();
        ViewBag.CurrentQuery = query;
        ViewBag.CurrentDept = departmentId;

        var employees = await _employeeProfileService.SearchEmployeesAsync(query, departmentId);
        return View(employees);
    }
}
