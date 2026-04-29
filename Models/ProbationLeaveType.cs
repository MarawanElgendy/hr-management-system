using System;
using System.Collections.Generic;

namespace HRMS.Models;

public partial class ProbationLeaveType
{
    public int LeaveTypeId { get; set; }

    public int StandardProbationDays { get; set; }

    public virtual LeaveType LeaveType { get; set; } = null!;
}
