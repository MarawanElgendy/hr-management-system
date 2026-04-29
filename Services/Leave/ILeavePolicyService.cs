namespace Services.Leave;

using DTOs;

public interface ILeavePolicyService
{
    Task<List<LeaveHistoryDTO>> ViewLeaveHistoryAsync(int employeeId);
    Task<List<LeaveBalanceDTO>> GetLeaveBalanceAsync(int employeeId);
}
