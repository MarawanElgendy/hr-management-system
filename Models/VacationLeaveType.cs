using System;
using System.Collections.Generic;

namespace HRMS.Models;

public partial class VacationLeaveType
{
    public int LeaveTypeId { get; set; }

    public int MaxCarryOverDays { get; set; }

    public bool RequiresManagerApproval { get; set; }

    public virtual LeaveType LeaveType { get; set; } = null!;
}
