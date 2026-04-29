namespace Services.Leave;

using DTOs;

public interface ILeaveRequestService
{
    Task<List<RequestStatusDTO>> ViewRequestStatusAsync(int employeeId);
    Task ApproveRejectLeaveRequest(int managerId, int leaveRequestId, string newStatus);
    Task<List<PendingLeaveRequestDTO>> GetPendingLeaveRequestsAsync(int managerId);
    Task OverrideLeaveDecision(int hrAdminId, int leaveRequestId, string newStatus);
    Task SubmitLeaveAfterAbsenceAsync(int employeeID, string justification);
    Task SubmitLeaveRequestAsync(int employeeId, int leaveTypeId, DateTime startDate, DateTime endDate, string reason);
}
