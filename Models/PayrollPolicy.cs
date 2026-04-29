using System;
using System.Collections.Generic;

namespace HRMS.Models;

public partial class PayrollPolicy
{
    public int PayrollPolicyId { get; set; }

    public DateOnly? EffectiveDate { get; set; }

    public string? Description { get; set; }

    public string? Status { get; set; }

    public int? ApprovedBy { get; set; }

    public virtual Employee? ApprovedByNavigation { get; set; }

    public virtual BonusPolicy? BonusPolicy { get; set; }

    public virtual DeductionPolicy? DeductionPolicy { get; set; }

    public virtual LatenessPolicy? LatenessPolicy { get; set; }

    public virtual OvertimePolicy? OvertimePolicy { get; set; }

    public virtual ICollection<Payroll> Payrolls { get; set; } = new List<Payroll>();
}
