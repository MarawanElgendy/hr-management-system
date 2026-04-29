using System;
using System.Collections.Generic;
using System.IO; // Keep for SubmitLeaveRequestAsync
using System.Linq;
using System.Threading.Tasks;
using HRMS.Models;
using Microsoft.AspNetCore.Hosting; // Keep for SubmitLeaveRequestAsync
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Services.Mission
{
    public class LeaveService : ILeaveService
    {
        private readonly HrmsContext _context;
        private readonly IWebHostEnvironment _environment; // Keep for SubmitLeaveRequestAsync
        private readonly Services.Notification.INotificationService _notificationService;
        private readonly Services.Attendance.IAttendanceService _attendanceService;

        public LeaveService(
            HrmsContext context, 
            IWebHostEnvironment environment, 
            Services.Notification.INotificationService notificationService,
            Services.Attendance.IAttendanceService attendanceService)
        {
            _context = context;
            _environment = environment;
            _notificationService = notificationService;
            _attendanceService = attendanceService;
        }

        public async Task<List<LeaveType>> GetLeaveTypesAsync()
        {
            return await _context.LeaveTypes.ToListAsync();
        }

        public async Task AddLeaveTypeAsync(LeaveType leaveType)
        {
            _context.LeaveTypes.Add(leaveType);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateLeaveTypeAsync(LeaveType leaveType)
        {
            _context.LeaveTypes.Update(leaveType);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteLeaveTypeAsync(int leaveTypeId)
        {
            var type = await _context.LeaveTypes.FindAsync(leaveTypeId);
            if (type != null)
            {
                _context.LeaveTypes.Remove(type);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<LeavePolicy>> GetLeavePoliciesAsync()
        {
            return await _context.LeavePolicies.Include(p => p.EligibilityRules).ToListAsync();
        }

        public async Task<LeavePolicy?> GetLeavePolicyAsync(int id)
        {
             return await _context.LeavePolicies
                .Include(p => p.EligibilityRules)
                .FirstOrDefaultAsync(p => p.LeavePolicyId == id);
        }

        public async Task AddLeavePolicyAsync(LeavePolicy policy)
        {
            _context.LeavePolicies.Add(policy);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateLeavePolicyAsync(LeavePolicy policy)
        {
            // Update basic properties
            var existing = await _context.LeavePolicies
                .Include(p => p.EligibilityRules)
                .FirstOrDefaultAsync(p => p.LeavePolicyId == policy.LeavePolicyId);

            if (existing != null)
            {
                existing.Name = policy.Name;
                existing.Purpose = policy.Purpose;
                existing.NoticePeriod = policy.NoticePeriod;
                existing.SpecialLeaveType = policy.SpecialLeaveType;
                existing.ResetOnNewYear = policy.ResetOnNewYear;

                // Handle Eligibility Rules (Simple replacement strategy for now)
                _context.EligibilityRules.RemoveRange(existing.EligibilityRules);
                foreach(var rule in policy.EligibilityRules)
                {
                    existing.EligibilityRules.Add(rule);
                }
                
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteLeavePolicyAsync(int policyId)
        {
            var policy = await _context.LeavePolicies.FindAsync(policyId);
            if (policy != null)
            {
                _context.LeavePolicies.Remove(policy);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<LeaveEntitlementDto>> GetEmployeeEntitlementsAsync(int employeeId)
        {
            var allTypes = await _context.LeaveTypes.ToListAsync();
            var existingEntitlements = await _context.LeaveEntitlements
                .Where(e => e.EmployeeId == employeeId)
                .ToListAsync();

            var result = new List<LeaveEntitlementDto>();
            foreach (var type in allTypes)
            {
                var entitlementRecord = existingEntitlements.FirstOrDefault(e => e.LeaveTypeId == type.LeaveTypeId);
                result.Add(new LeaveEntitlementDto
                {
                    LeaveTypeId = type.LeaveTypeId,
                    LeaveTypeName = type.TypeName,
                    CurrentEntitlement = entitlementRecord?.Entitlement ?? 0
                });
            }
            return result;
        }

        public async Task UpdateLeaveEntitlementAsync(int employeeId, int leaveTypeId, int entitlement)
        {
            var record = await _context.LeaveEntitlements
                .FirstOrDefaultAsync(e => e.EmployeeId == employeeId && e.LeaveTypeId == leaveTypeId);

            if (record != null)
            {
                record.Entitlement = entitlement;
                _context.LeaveEntitlements.Update(record);
            }
            else
            {
                var newRecord = new LeaveEntitlement
                {
                    EmployeeId = employeeId,
                    LeaveTypeId = leaveTypeId,
                    Entitlement = entitlement
                };
                _context.LeaveEntitlements.Add(newRecord);
            }
            await _context.SaveChangesAsync();
        }

        public async Task SubmitLeaveRequestAsync(int employeeId, int leaveTypeId, DateTime start, DateTime end, string justification, IFormFile? attachment)
        {
            // 1. Create Leave Request
            var request = new LeaveRequest
            {
                EmployeeId = employeeId,
                LeaveTypeId = leaveTypeId,
                Justification = justification,
                Duration = (end - start).Days + 1, // Simple duration calculation
                StartDate = DateOnly.FromDateTime(start), // Using DateOnly
                Status = "Pending",
                // Assuming ApprovalTiming/PhysicianName/MedicalCertification mapping handled elsewhere or nullable
            };

            _context.LeaveRequests.Add(request);
            await _context.SaveChangesAsync(); // Save to get generated ID

            // 2. Handle Attachment
            if (attachment != null && attachment.Length > 0)
            {
                // Ensure directory exists
                string uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "leave_documents");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Unique filename
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(attachment.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Save file
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await attachment.CopyToAsync(fileStream);
                }

                // Create Document Record
                var document = new LeaveDocument
                {
                    LeaveRequestId = request.LeaveRequestId,
                    FilePath = $"/uploads/leave_documents/{uniqueFileName}"
                };

                _context.LeaveDocuments.Add(document);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<LeaveRequest>> GetLeaveHistoryAsync(int employeeId)
        {
            return await _context.LeaveRequests
                .Include(r => r.LeaveType)
                .Include(r => r.LeaveDocuments) // Assuming navigation property exists or will check
                .Where(r => r.EmployeeId == employeeId)
                .OrderByDescending(r => r.StartDate) // Use StartDate
                .ToListAsync();
        }

        public async Task<List<LeaveBalanceDto>> GetLeaveBalancesAsync(int employeeId)
        {
            // 1. Get Entitlements
            var entitlements = await _context.LeaveEntitlements
                .Include(e => e.LeaveType)
                .Where(e => e.EmployeeId == employeeId)
                .ToListAsync();
            
            // 2. Get Usage (Approved Requests only? Or all? Usually Approved + Pending)
            var requests = await _context.LeaveRequests
                .Where(r => r.EmployeeId == employeeId && r.Status != "Rejected")
                .ToListAsync();

            var balances = new List<LeaveBalanceDto>();

            foreach (var ent in entitlements)
            {
                var used = requests
                    .Where(r => r.LeaveTypeId == ent.LeaveTypeId)
                    .Sum(r => r.Duration);

                balances.Add(new LeaveBalanceDto
                {
                    TypeName = ent.LeaveType.TypeName,
                    Entitlement = ent.Entitlement,
                    Used = used
                });
            }

            return balances;
        }

        public async Task<List<LeaveRequest>> GetPendingTeamLeavesAsync(int managerId)
        {
            // 1. Get Direct Reports (Union of Hierarchy table and direct ManagerID reference)
            var hierarchyIds = await _context.EstablishesHierarchies
                .Where(h => h.ManagerId == managerId)
                .Select(h => h.EmployeeId)
                .ToListAsync();

            var directReportIds = await _context.Employees
                .Where(e => e.ManagerId == managerId)
                .Select(e => e.EmployeeId)
                .ToListAsync();
            
            var teamIds = hierarchyIds.Union(directReportIds).Distinct().ToList();

            if (!teamIds.Any()) return new List<LeaveRequest>();

            // 2. Get Pending Requests for Team
            return await _context.LeaveRequests
                .Include(r => r.Employee)
                .Include(r => r.LeaveType)
                .Include(r => r.LeaveDocuments)
                .Where(r => teamIds.Contains(r.EmployeeId) && r.Status == "Pending")
                .OrderBy(r => r.StartDate)
                .ToListAsync();
        }

        public async Task<List<LeaveRequest>> GetApprovedLeavesAsync()
        {
            return await _context.LeaveRequests
                .Where(r => r.Status == "Approved")
                .ToListAsync();
        }

        public async Task ApproveRequestAsync(int requestId, int managerId)
        {
            await UpdateRequestStatusAsync(requestId, managerId, "Approved");
            // Notifications handled inside UpdateRequestStatusAsync if we refactor, or here.
            // Let's notify here to be explicit about "Manager Approval".
            var req = await _context.LeaveRequests.Include(r => r.LeaveType).FirstOrDefaultAsync(r => r.LeaveRequestId == requestId);
            if(req != null) 
            {
                await _notificationService.SendNotificationAsync(req.EmployeeId, $"Your {req.LeaveType.TypeName} request has been Approved by Manager.", "Success", "Normal");
                
                // Auto-Sync with Attendance
                await _attendanceService.SyncLeaveToAttendanceAsync(requestId);
            }
        }

        public async Task RejectRequestAsync(int requestId, int managerId)
        {
            await UpdateRequestStatusAsync(requestId, managerId, "Rejected");
            var req = await _context.LeaveRequests.Include(r => r.LeaveType).FirstOrDefaultAsync(r => r.LeaveRequestId == requestId);
            if(req != null) 
                await _notificationService.SendNotificationAsync(req.EmployeeId, $"Your {req.LeaveType.TypeName} request has been Rejected by Manager.", "Alert", "High");
        }

        private async Task UpdateRequestStatusAsync(int requestId, int managerId, string status)
        {
            var request = await _context.LeaveRequests.FindAsync(requestId);
            if (request == null) throw new ArgumentException("Request not found");

            // Verify Hierarchy (Hierarchy table OR direct ManagerId)
            var isSubordinate = await _context.EstablishesHierarchies
                .AnyAsync(h => h.ManagerId == managerId && h.EmployeeId == request.EmployeeId);
            
            if (!isSubordinate)
            {
                 // Check Check direct report relationship
                 var isDirectReport = await _context.Employees
                    .AnyAsync(e => e.EmployeeId == request.EmployeeId && e.ManagerId == managerId);
                 
                 if (!isDirectReport) throw new UnauthorizedAccessException("Employee is not in your team");
            }

            request.Status = status;
            _context.LeaveRequests.Update(request);
            await _context.SaveChangesAsync();
        }

        public async Task<List<LeaveRequest>> GetRecentLeaveRequestsAsync()
        {
            return await _context.LeaveRequests
                .Include(r => r.Employee)
                .Include(r => r.LeaveType)
                .OrderByDescending(r => r.StartDate)
                .Take(50)
                .ToListAsync();
        }

        public async Task OverrideLeaveStatusAsync(int requestId, string newStatus, int adminId, string reason)
        {
            var request = await _context.LeaveRequests
                .Include(r => r.LeaveType)
                .FirstOrDefaultAsync(r => r.LeaveRequestId == requestId);
                
            if (request == null) throw new ArgumentException("Request not found");

            var oldStatus = request.Status;
            request.Status = newStatus;
            _context.LeaveRequests.Update(request);
            await _context.SaveChangesAsync();

            // Notify Employee
                string message = $"Your {request.LeaveType.TypeName} request from {request.StartDate} has been manually updated from '{oldStatus}' to '{newStatus}' by HR Admin. Reason: {reason}";
                await _notificationService.SendNotificationAsync(request.EmployeeId, message, "Alert", "High");


            // Auto-Sync if Approved
            if (newStatus == "Approved")
            {
                await _attendanceService.SyncLeaveToAttendanceAsync(requestId);
            }
        }

        public async Task ToggleEmployeeFlagAsync(int employeeId, int managerId)
        {
            // Verify Hierarchy (Hierarchy table OR direct ManagerId)
            var isSubordinate = await _context.EstablishesHierarchies
                .AnyAsync(h => h.ManagerId == managerId && h.EmployeeId == employeeId);
            
            if (!isSubordinate)
            {
                 var isDirectReport = await _context.Employees
                    .AnyAsync(e => e.EmployeeId == employeeId && e.ManagerId == managerId);
                 
                 if (!isDirectReport) throw new UnauthorizedAccessException("Employee is not in your team");
            }

            var employee = await _context.Employees.FindAsync(employeeId);
            if (employee == null) throw new ArgumentException("Employee not found");

            employee.IsFlagged = !employee.IsFlagged;
            await _context.SaveChangesAsync();
        }
    }
}
