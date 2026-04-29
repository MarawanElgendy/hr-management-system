using System;
using System.Collections.Generic;

namespace HRMS.Models;

public partial class Hradministrator
{
    public int EmployeeId { get; set; }

    public int? ApprovalLevel { get; set; }

    public string? RecordAccessScope { get; set; }

    public string? DocumentValidationRights { get; set; }

    public virtual Employee Employee { get; set; } = null!;
}
