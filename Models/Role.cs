using System;
using System.Collections.Generic;

namespace HRMS.Models;

public partial class Role
{
    public int RoleId { get; set; }

    public string Name { get; set; } = null!;

    public string? Purpose { get; set; }

    public virtual ICollection<FulfillsRole> FulfillsRoles { get; set; } = new List<FulfillsRole>();

    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
