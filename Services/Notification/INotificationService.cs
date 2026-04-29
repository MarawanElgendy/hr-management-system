namespace Services.Notification;

public interface INotificationService
{
    Task SendNotificationAsync(int userId, string message, string type = "Info", string urgency = "Normal");
    Task<List<HRMS.Models.Notification>> GetUnreadNotificationsAsync(int userId);
    Task MarkAsReadAsync(int notificationId, int userId);
}
