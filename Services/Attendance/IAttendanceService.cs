namespace Services.Attendance;

using DTOs;

public interface IAttendanceService
{
    Task RecordAttendanceAsync(int employeeID, int shiftID, TimeSpan entryTime, TimeSpan exitTime);
    Task SubmitCorrectionRequestAsync(int employeeID, DateTime date, string correctionType, string reason);
    Task SyncLeaveToAttendanceAsync(int leaveRequestID);
    Task SyncOfflineAttendanceAsync(int deviceID, int employeeID, DateTime clockTime, string type);
    Task DefineShortTimeRulesAsync(string ruleName, int lateMinutes, int earlyLeaveMinutes, string penaltyType);
    Task SetGracePeriodAsync(int minutes);
    Task DefinePenaltyThresholdAsync(int lateMinutes, string deductionType);

    Task<List<TeamAttendanceDTO>> ViewTeamAttendanceAsync(int managerID, DateTime dateRangeStart, DateTime dateRangeEnd);
    
    Task<List<HRMS.Models.AttendanceRule>> GetAttendanceRulesAsync();
    
    // Employee Self-Service
    Task CheckInAsync(int employeeId);
    Task CheckOutAsync(int employeeId);
    Task<List<HRMS.Models.Attendance>> GetMyAttendanceAsync(int employeeId);
}
