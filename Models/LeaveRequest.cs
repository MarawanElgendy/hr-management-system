using System;
using System.Collections.Generic;

namespace HRMS.Models;

public partial class LeaveRequest
{
    public int LeaveRequestId { get; set; }

    public int EmployeeId { get; set; }

    public int LeaveTypeId { get; set; }

    public DateOnly StartDate { get; set; }

    public string Justification { get; set; } = null!;

    public int Duration { get; set; }

    public string? PhysicianName { get; set; }

    public string? MedicalCertification { get; set; }

    public int? ApprovalTiming { get; set; }

    public string? Status { get; set; }

    public virtual Employee Employee { get; set; } = null!;

    public virtual ICollection<LeaveDocument> LeaveDocuments { get; set; } = new List<LeaveDocument>();

    public virtual LeaveType LeaveType { get; set; } = null!;
}
