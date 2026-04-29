using Microsoft.EntityFrameworkCore;
using Services.Attendance;
using Services.General;
using Services.Leave;
using Services.Mission;
using Services.Notification;
using Services.Profile;
using Microsoft.AspNetCore.Authentication.Cookies;

using HRMS.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Connect to the SQL server
builder.Services.AddDbContext<HrmsContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

// Set login redirect behaviour
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/Login";
});

// Register the services
builder.Services.AddScoped<IEmployeeProfileService, EmployeeProfileService>();
builder.Services.AddScoped<IRoleAssignmentService, RoleAssignmentService>();
builder.Services.AddScoped<IContractService, ContractService>();
builder.Services.AddScoped<IShiftService, ShiftService>();
builder.Services.AddScoped<Services.Configuration.IShiftConfigurationService, Services.Configuration.ShiftConfigurationService>();
builder.Services.AddScoped<Services.Mission.IShiftSchedulingService, Services.Mission.ShiftSchedulingService>();
builder.Services.AddScoped<IAttendanceService, AttendanceService>();
// New Service for Employee Leave Requests
builder.Services.AddScoped<Services.Mission.ILeaveService, Services.Mission.LeaveService>();
builder.Services.AddScoped<ILeaveRequestService, LeaveRequestService>();
builder.Services.AddScoped<ILeavePolicyService, LeavePolicyService>();
builder.Services.AddScoped<IMissionService, MissionService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();
builder.Services.AddScoped<IHierarchyService, HierarchyService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseExceptionHandler("/Error");
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
