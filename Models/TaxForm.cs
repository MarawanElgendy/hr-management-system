using System;
using System.Collections.Generic;

namespace HRMS.Models;

public partial class TaxForm
{
    public int TaxFormId { get; set; }

    public string? Jurisdiction { get; set; }

    public string? FormContent { get; set; }

    public DateOnly? ValidityStart { get; set; }

    public DateOnly? ValidityEnd { get; set; }

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
