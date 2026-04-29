using System;
using System.Collections.Generic;

namespace HRMS.Models;

public partial class Responsibility
{
    public int ResponsibilityId { get; set; }

    public int PositionId { get; set; }

    public string? Responsibility1 { get; set; }

    public virtual Position Position { get; set; } = null!;
}
