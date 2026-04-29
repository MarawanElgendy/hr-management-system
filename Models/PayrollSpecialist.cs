using System;
using System.Collections.Generic;

namespace HRMS.Models;

public partial class PayrollSpecialist
{
    public int EmployeeId { get; set; }

    public string? AssignedRegion { get; set; }

    public string? ProcessingFrequency { get; set; }

    public string? LastProcessedPeriod { get; set; }

    public virtual Employee Employee { get; set; } = null!;
}
