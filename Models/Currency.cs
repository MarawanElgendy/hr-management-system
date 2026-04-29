using System;
using System.Collections.Generic;

namespace HRMS.Models;

public partial class Currency
{
    public int CurrencyId { get; set; }

    public string? CurrencyName { get; set; }

    public virtual ICollection<AllowanceAndDeduction> AllowanceAndDeductions { get; set; } = new List<AllowanceAndDeduction>();

    public virtual ICollection<SalaryType> SalaryTypes { get; set; } = new List<SalaryType>();
}
