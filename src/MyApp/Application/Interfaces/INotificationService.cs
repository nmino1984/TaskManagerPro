using MyApp.Application.DTOs.Notification;

namespace MyApp.Application.Interfaces;

public interface INotificationService
{
    Task<IEnumerable<NotificationResponseDto>> GetAllAsync(string userId);
    Task<int> GetUnreadCountAsync(string userId);
    Task MarkAsReadAsync(int notificationId, string userId);
    Task MarkAllAsReadAsync(string userId);
    Task CreateAsync(string userId, string title, string message, string type, int? relatedTaskId = null);
}
