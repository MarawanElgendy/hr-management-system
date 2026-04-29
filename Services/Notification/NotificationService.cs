using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRMS.Models;
using Microsoft.EntityFrameworkCore;

namespace Services.Notification
{
    public class NotificationService : INotificationService
    {
        private readonly HrmsContext _context;

        public NotificationService(HrmsContext context)
        {
            _context = context;
        }

        public async Task SendNotificationAsync(int userId, string message, string type = "Info", string urgency = "Normal")
        {
            // 1. Create Notification Record
            var notification = new HRMS.Models.Notification
            {
                MessageContent = message,
                Type = type,
                Urgency = urgency,
                Timestamp = DateTime.Now,
                ReadStatus = "Unread" 
                // Note: The schema for Notification seems to store ReadStatus on the Notification itself? 
                // Checking ReceivesNotification table structure: likely M:N relationship.
                // Re-checking HrmsContext: ReceivesNotifications link Employee and Notification.
                // The Notification table has 'ReadStatus', which might be a legacy or global status?
                // Or maybe it's a simplification. Let's assume M:N for correctness.
            };
            
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            // 2. Link to User via ReceivesNotifications
            var link = new ReceivesNotification
            {
                EmployeeId = userId,
                NotificationId = notification.NotificationId,
                DeliveredAt = DateTime.Now,
                DeliveryStatus = "Delivered"
            };
            _context.ReceivesNotifications.Add(link);
            await _context.SaveChangesAsync();
        }

        public async Task<List<HRMS.Models.Notification>> GetUnreadNotificationsAsync(int userId)
        {
            // Join Notification and ReceivesNotification
            // Assuming we track 'read' state... 
            // The table ReceivesNotification doesn't seem to have 'IsRead', but Notification has 'ReadStatus'.
            // If Notification table has ReadStatus, it implies 1:1 or 1:Many (One notification for one user).
            // But ReceivesNotification implies Many-to-Many broadcast.
            // Let's check schema again. Notification has ReadStatus. ReceivesNotification has DeliveryStatus.
            // If we send a personalized notification, creating a new Notification row per user is safest usage of this schema.
            
           return await _context.Notifications
                .Where(n => _context.ReceivesNotifications.Any(rn => rn.NotificationId == n.NotificationId && rn.EmployeeId == userId)
                            && n.ReadStatus == "Unread")
                .OrderByDescending(n => n.Timestamp)
                .ToListAsync();
        }

        public async Task MarkAsReadAsync(int notificationId, int userId)
        {
            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.NotificationId == notificationId);
            
            // Validate ownership via link
            var isRecipient = await _context.ReceivesNotifications
                .AnyAsync(rn => rn.NotificationId == notificationId && rn.EmployeeId == userId);

            if (notification != null && isRecipient)
            {
                notification.ReadStatus = "Read";
                _context.Notifications.Update(notification);
                await _context.SaveChangesAsync();
            }
        }
    }
}
