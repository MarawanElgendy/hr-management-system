using System;
using System.Collections.Generic;

namespace HRMS.Models;

public partial class FulfillsRole
{
    public int EmployeeId { get; set; }

    public int RoleId { get; set; }

    public DateTime AssignedDate { get; set; }

    public virtual Employee Employee { get; set; } = null!;

    public virtual Role Role { get; set; } = null!;
}
