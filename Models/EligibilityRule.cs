using System;
using System.Collections.Generic;

namespace HRMS.Models;

public partial class EligibilityRule
{
    public int? LeavePolicyId { get; set; }

    public int EligibilityRuleId { get; set; }

    public string? EligibilityRule1 { get; set; }

    public virtual LeavePolicy? LeavePolicy { get; set; }
}
