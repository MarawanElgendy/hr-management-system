using System;
using System.Collections.Generic;

namespace HRMS.Models;

public partial class ShiftConfiguration
{
    public int ShiftConfigurationId { get; set; }

    public string ShiftType { get; set; } = null!;

    public decimal AllowanceAmount { get; set; }

    public int? LastUpdatedBy { get; set; }

    public DateTime? LastUpdatedAt { get; set; }

    public virtual Employee? LastUpdatedByNavigation { get; set; }
}
