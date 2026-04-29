using System;
using System.Collections.Generic;

namespace HRMS.Models;

public partial class Exception
{
    public int ExceptionId { get; set; }

    public string? Name { get; set; }

    public string? Category { get; set; }

    public DateOnly? Date { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
