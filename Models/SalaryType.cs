using System;
using System.Collections.Generic;

namespace HRMS.Models;

public partial class SalaryType
{
    public int SalaryTypeId { get; set; }

    public int? PaymentFrequency { get; set; }

    public int? CurrencyId { get; set; }

    public virtual ContractSalaryType? ContractSalaryType { get; set; }

    public virtual Currency? Currency { get; set; }

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

    public virtual HourlySalaryType? HourlySalaryType { get; set; }

    public virtual MonthlySalaryType? MonthlySalaryType { get; set; }
}
