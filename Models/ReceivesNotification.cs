using System;
using System.Collections.Generic;

namespace HRMS.Models;

public partial class ReceivesNotification
{
    public int EmployeeId { get; set; }

    public int NotificationId { get; set; }

    public string? DeliveryStatus { get; set; }

    public DateTime? DeliveredAt { get; set; }

    public virtual Employee Employee { get; set; } = null!;

    public virtual Notification Notification { get; set; } = null!;
}
