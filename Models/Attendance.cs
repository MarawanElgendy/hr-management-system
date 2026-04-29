using System;
using System.Collections.Generic;

namespace HRMS.Models;

public partial class Attendance
{
    public int AttendanceId { get; set; }

    public DateTime? EntryTime { get; set; }

    public DateTime? ExitTime { get; set; }

    public int? DurationMinutes { get; set; }

    public int EmployeeId { get; set; }

    public int? ShiftId { get; set; }

    public virtual ICollection<AttendanceLog> AttendanceLogs { get; set; } = new List<AttendanceLog>();

    public virtual ICollection<AttendanceSource> AttendanceSources { get; set; } = new List<AttendanceSource>();

    public virtual Employee Employee { get; set; } = null!;

    public virtual ShiftSchedule? Shift { get; set; }
}
