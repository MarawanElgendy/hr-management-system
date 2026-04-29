using System;
using System.Collections.Generic;

namespace HRMS.Models;

public partial class Position
{
    public int PositionId { get; set; }

    public string Title { get; set; } = null!;

    public string? Status { get; set; }

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

    public virtual ICollection<Responsibility> Responsibilities { get; set; } = new List<Responsibility>();
}
