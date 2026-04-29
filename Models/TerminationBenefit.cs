using System;
using System.Collections.Generic;

namespace HRMS.Models;

public partial class TerminationBenefit
{
    public int BenefitId { get; set; }

    public int? TerminationId { get; set; }

    public decimal CompensationAmount { get; set; }

    public DateOnly EffectiveDate { get; set; }

    public string Reason { get; set; } = null!;

    public virtual Termination? Termination { get; set; }
}
