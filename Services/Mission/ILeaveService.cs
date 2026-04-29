using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HRMS.Models;
using Microsoft.AspNetCore.Http;

namespace Services.Mission
{
    public interface ILeaveService
    {
        Task<List<LeaveType>> GetLeaveTypesAsync();
        Task SubmitLeaveRequestAsync(int employeeId, int leaveTypeId, DateTime start, DateTime end, string justification, IFormFile? attachment);
        
        Task<List<LeaveRequest>> GetLeaveHistoryAsync(int employeeId);
        Task<List<LeaveBalanceDto>> GetLeaveBalancesAsync(int employeeId);
        Task<List<LeaveRequest>> GetPendingTeamLeavesAsync(int managerId);
        Task<List<LeaveRequest>> GetApprovedLeavesAsync();
        Task ApproveRequestAsync(int requestId, int managerId);
        Task RejectRequestAsync(int requestId, int managerId);
        Task ToggleEmployeeFlagAsync(int employeeId, int managerId);

        // Admin Configuration
        Task AddLeaveTypeAsync(LeaveType leaveType);
        Task UpdateLeaveTypeAsync(LeaveType leaveType);
        Task DeleteLeaveTypeAsync(int leaveTypeId);
        
        Task<List<LeavePolicy>> GetLeavePoliciesAsync();
        Task<LeavePolicy?> GetLeavePolicyAsync(int id);
        Task AddLeavePolicyAsync(LeavePolicy policy);
        Task UpdateLeavePolicyAsync(LeavePolicy policy);
        Task DeleteLeavePolicyAsync(int policyId);

        // Entitlement Management
        Task<List<LeaveEntitlementDto>> GetEmployeeEntitlementsAsync(int employeeId);
        Task UpdateLeaveEntitlementAsync(int employeeId, int leaveTypeId, int entitlement);

        // Admin Override
        Task<List<LeaveRequest>> GetRecentLeaveRequestsAsync();
        Task OverrideLeaveStatusAsync(int requestId, string newStatus, int adminId, string reason);
    }

    public class LeaveBalanceDto
    {
        public string TypeName { get; set; }
        public int Entitlement { get; set; }
        public int Used { get; set; }
        public int Remaining => Entitlement - Used;
    }

    public class LeaveEntitlementDto
    {
        public int LeaveTypeId { get; set; }
        public string LeaveTypeName { get; set; } = string.Empty;
        public int CurrentEntitlement { get; set; }
    }
}
