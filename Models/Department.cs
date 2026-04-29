using System;
using System.Collections.Generic;

namespace HRMS.Models;

public partial class Department
{
    public int DepartmentId { get; set; }

    public string Name { get; set; } = null!;

    public string? Purpose { get; set; }

    public int? HeadId { get; set; }

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

    public virtual Employee? Head { get; set; }
}
