using System;
using System.Collections.Generic;

namespace HRMS.Models;

public partial class OvertimePolicy
{
    public int PayrollPolicyId { get; set; }

    public decimal WeekdayRateMultiplier { get; set; }

    public decimal WeekendRateMultiplier { get; set; }

    public int MaxHoursPerMonth { get; set; }

    public virtual PayrollPolicy PayrollPolicy { get; set; } = null!;
}
