using System;
using System.Collections.Generic;

namespace HRMS.Models;

public partial class AllowanceAndDeduction
{
    public int AllowanceAndDeductionId { get; set; }

    public int? PayrollId { get; set; }

    public int? EmployeeId { get; set; }

    public decimal Amount { get; set; }

    public int Duration { get; set; }

    public int? TimeZone { get; set; }

    public int? CurrencyId { get; set; }

    public virtual Currency? Currency { get; set; }

    public virtual Employee? Employee { get; set; }

    public virtual Payroll? Payroll { get; set; }
}
