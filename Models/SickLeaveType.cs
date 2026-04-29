using System;
using System.Collections.Generic;

namespace HRMS.Models;

public partial class SickLeaveType
{
    public int LeaveTypeId { get; set; }

    public bool RequiresPhysicianCert { get; set; }

    public virtual LeaveType LeaveType { get; set; } = null!;
}
