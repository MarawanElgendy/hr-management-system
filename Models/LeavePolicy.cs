using System;
using System.Collections.Generic;

namespace HRMS.Models;

public partial class LeavePolicy
{
    public int LeavePolicyId { get; set; }

    public string? Name { get; set; }

    public string? Purpose { get; set; }

    public int NoticePeriod { get; set; }

    public string? SpecialLeaveType { get; set; }

    public bool? ResetOnNewYear { get; set; }

    public virtual ICollection<EligibilityRule> EligibilityRules { get; set; } = new List<EligibilityRule>();

    public virtual ICollection<LeaveType> LeaveTypes { get; set; } = new List<LeaveType>();
}
