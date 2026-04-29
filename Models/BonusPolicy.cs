using System;
using System.Collections.Generic;

namespace HRMS.Models;

public partial class BonusPolicy
{
    public int PayrollPolicyId { get; set; }

    public string? BonusType { get; set; }

    public int EligibiltyCriteria { get; set; }

    public virtual PayrollPolicy PayrollPolicy { get; set; } = null!;
}
