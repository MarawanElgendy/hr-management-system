using System;
using System.Collections.Generic;

namespace HRMS.Models;

public partial class PartTimeContract
{
    public int ContractId { get; set; }

    public int? WorkingHours { get; set; }

    public decimal? HourlyRate { get; set; }

    public virtual Contract Contract { get; set; } = null!;
}
