using System;
using System.Collections.Generic;

namespace HRMS.Models;

public partial class PayGrade
{
    public string PayGrade1 { get; set; } = null!;

    public string? GradeName { get; set; }

    public decimal? MinSalary { get; set; }

    public decimal? MaxSalary { get; set; }

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
