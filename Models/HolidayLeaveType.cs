using System;
using System.Collections.Generic;

namespace HRMS.Models;

public partial class HolidayLeaveType
{
    public int LeaveTypeId { get; set; }

    public string? RegionalScope { get; set; }

    public string? OfficialRecognition { get; set; }

    public string? HolidayName { get; set; }

    public virtual LeaveType LeaveType { get; set; } = null!;
}
