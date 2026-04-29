using System;
using System.Collections.Generic;

namespace HRMS.Models;

public partial class ShiftSchedule
{
    public int ShiftId { get; set; }

    public string? Name { get; set; }

    public string? Type { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public TimeOnly BreakDuration { get; set; }

    public DateOnly ShiftDate { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();

    public virtual ICollection<WorksShift> WorksShifts { get; set; } = new List<WorksShift>();
}
