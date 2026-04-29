using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Services.General;

namespace HRMS.Controllers;

[AllowAnonymous]
public class AccountController : Controller
{
    private readonly IAccountService accountService;

    public AccountController(IAccountService accountService)
    {
        this.accountService = accountService;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Login(int employeeID)
    {
        bool exists = await accountService.LoginAsync(employeeID);

        if (!exists)
        {
            ModelState.AddModelError("", "Invalid employee ID");
            return View();
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, employeeID.ToString())
        };

        var type = await accountService.GetEmployeeTypeAsync(employeeID);
        if (!string.IsNullOrEmpty(type))
        {
            claims.Add(new Claim(ClaimTypes.Role, type));
        }

        var identity = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme
        );

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity)
        );

        // NOTHING AFTER THIS CAN THROW
        return RedirectToAction("Index", "Home");
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme
        );

        return RedirectToAction("Login");
    }

    [HttpGet]
    public async Task<IActionResult> Register()
    {
        ViewBag.Departments = await accountService.GetDepartmentsAsync();
        ViewBag.Roles = await accountService.GetRolesAsync();

        return View();
    }

    [HttpGet]
    public IActionResult RegisterSuccess(int employeeID)
    {
        return View(employeeID);
    }

    [HttpPost]
    public async Task<IActionResult> Register
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

        return RedirectToAction("RegisterSuccess", new { employeeID });
    }
}
