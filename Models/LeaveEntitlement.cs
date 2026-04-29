using System;
using System.Collections.Generic;

namespace HRMS.Models;

public partial class LeaveEntitlement
{
    public int EmployeeId { get; set; }

    public int LeaveTypeId { get; set; }

    public int Entitlement { get; set; }

    public virtual Employee Employee { get; set; } = null!;

    public virtual LeaveType LeaveType { get; set; } = null!;
}
