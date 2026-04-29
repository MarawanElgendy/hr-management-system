using System;
using System.Collections.Generic;

namespace HRMS.Models;

public partial class LatenessPolicy
{
    public int PayrollPolicyId { get; set; }

    public int GracePeriodMins { get; set; }

    public decimal DeductionRate { get; set; }

    public virtual PayrollPolicy PayrollPolicy { get; set; } = null!;
}
