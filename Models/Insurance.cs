using System;
using System.Collections.Generic;

namespace HRMS.Models;

public partial class Insurance
{
    public int InsuranceId { get; set; }

    public decimal ContributionRate { get; set; }

    public string? Coverage { get; set; }

    public virtual ICollection<Contract> Contracts { get; set; } = new List<Contract>();
}
