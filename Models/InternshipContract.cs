using System;
using System.Collections.Generic;

namespace HRMS.Models;

public partial class InternshipContract
{
    public int ContractId { get; set; }

    public string? Mentoring { get; set; }

    public string? Evaluation { get; set; }

    public virtual Contract Contract { get; set; } = null!;

    public virtual ICollection<StipendFeature> StipendFeatures { get; set; } = new List<StipendFeature>();
}
