using System;
using System.Collections.Generic;

namespace HRMS.Models;

public partial class LeaveType
{
    public int LeaveTypeId { get; set; }

    public string TypeName { get; set; } = null!;

    public string? Description { get; set; }

    public virtual HolidayLeaveType? HolidayLeaveType { get; set; }

    public virtual ICollection<LeaveEntitlement> LeaveEntitlements { get; set; } = new List<LeaveEntitlement>();

    public virtual ICollection<LeaveRequest> LeaveRequests { get; set; } = new List<LeaveRequest>();

    public virtual ProbationLeaveType? ProbationLeaveType { get; set; }

    public virtual SickLeaveType? SickLeaveType { get; set; }

    public virtual VacationLeaveType? VacationLeaveType { get; set; }

    public virtual ICollection<LeavePolicy> LeavePolicies { get; set; } = new List<LeavePolicy>();
}
