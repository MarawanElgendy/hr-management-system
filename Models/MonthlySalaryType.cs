using System;
using System.Collections.Generic;

namespace HRMS.Models;

public partial class MonthlySalaryType
{
    public int SalaryTypeId { get; set; }

    public int TaxRule { get; set; }

    public string? ContributionScheme { get; set; }

    public virtual SalaryType SalaryType { get; set; } = null!;
}
