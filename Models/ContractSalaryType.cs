using System;
using System.Collections.Generic;

namespace HRMS.Models;

public partial class ContractSalaryType
{
    public int SalaryTypeId { get; set; }

    public decimal ContractValue { get; set; }

    public string? Installment { get; set; }

    public virtual SalaryType SalaryType { get; set; } = null!;
}
