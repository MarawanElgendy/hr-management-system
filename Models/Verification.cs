using System;
using System.Collections.Generic;

namespace HRMS.Models;

public partial class Verification
{
    public int VerificationId { get; set; }

    public string? Type { get; set; }

    public string? Issuer { get; set; }

    public DateOnly? IssueDate { get; set; }

    public int? ExpiryPeriod { get; set; }

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
