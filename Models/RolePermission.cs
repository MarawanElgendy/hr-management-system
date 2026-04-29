using System;
using System.Collections.Generic;

namespace HRMS.Models;

public partial class RolePermission
{
    public int RolePermissionId { get; set; }

    public int RoleId { get; set; }

    public string PermissionName { get; set; } = null!;

    public string? AllowedAction { get; set; }

    public virtual Role Role { get; set; } = null!;
}
