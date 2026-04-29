using System;
using System.Collections.Generic;

namespace HRMS.Models;

public partial class ReplacementRequest
{
    public int RequestId { get; set; }

    public int? EmployeeId { get; set; }

    public string? Reason { get; set; }

    public DateTime? RequestDate { get; set; }

    public string? Status { get; set; }

    public virtual Employee? Employee { get; set; }
}
