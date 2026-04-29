using System;
using System.Collections.Generic;

namespace HRMS.Models;

public partial class DeductionPolicy
{
    public int PayrollPolicyId { get; set; }

    public string? DeductionReasons { get; set; }

    public bool CalculationMode { get; set; }

    public virtual PayrollPolicy PayrollPolicy { get; set; } = null!;
}
