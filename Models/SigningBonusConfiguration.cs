using System;
using System.Collections.Generic;

namespace HRMS.Models;

public partial class SigningBonusConfiguration
{
    public int ConfigId { get; set; }

    public string BonusType { get; set; } = null!;

    public decimal DefaultAmount { get; set; }

    public string? EligibilityCriteria { get; set; }

    public DateTime? LastUpdated { get; set; }
}
