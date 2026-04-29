using System;
using System.Collections.Generic;

namespace HRMS.Models;

public partial class Notification
{
    public int NotificationId { get; set; }

    public string? MessageContent { get; set; }

    public DateTime? Timestamp { get; set; }

    public string? Urgency { get; set; }

    public string? ReadStatus { get; set; }

    public string? Type { get; set; }

    public virtual ICollection<ReceivesNotification> ReceivesNotifications { get; set; } = new List<ReceivesNotification>();
}
