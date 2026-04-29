using System;
using System.Collections.Generic;

namespace HRMS.Models;

public partial class EstablishesHierarchy
{
    public int EmployeeId { get; set; }

    public int? ManagerId { get; set; }

    public int HierarchyLevel { get; set; }

    public virtual Employee Employee { get; set; } = null!;

    public virtual Employee? Manager { get; set; }
}
