using System;
using System.Collections.Generic;

namespace HRMS.Models;

public partial class StipendFeature
{
    public int InternshipContractId { get; set; }

    public int StipendFeatureId { get; set; }

    public string Feature { get; set; } = null!;

    public virtual InternshipContract InternshipContract { get; set; } = null!;
}
