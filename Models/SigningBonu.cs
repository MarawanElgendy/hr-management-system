using System;
using System.Collections.Generic;

namespace HRMS.Models;

public partial class SigningBonu
{
    public int SigningBonusId { get; set; }

    public int? EmployeeId { get; set; }

    public decimal BonusAmount { get; set; }

    public DateOnly EffectiveDate { get; set; }

    public virtual Employee? Employee { get; set; }
}
