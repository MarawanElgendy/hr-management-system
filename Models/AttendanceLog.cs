using System;
using System.Collections.Generic;

namespace HRMS.Models;

public partial class AttendanceLog
{
    public int AttendanceLogId { get; set; }

    public int? AttendanceId { get; set; }

    public int? Actor { get; set; }

    public DateTime? Timestamp { get; set; }

    public string? Reason { get; set; }

    public virtual Employee? ActorNavigation { get; set; }

    public virtual Attendance? Attendance { get; set; }
}
