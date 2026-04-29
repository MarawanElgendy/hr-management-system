using System;
using System.Collections.Generic;

namespace HRMS.Models;

public partial class WorksShift
{
    public int AssignmentId { get; set; }

    public int EmployeeId { get; set; }

    public int? ShiftId { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public string? Status { get; set; }

    public virtual Employee Employee { get; set; } = null!;

    public virtual ShiftSchedule? Shift { get; set; }
}
