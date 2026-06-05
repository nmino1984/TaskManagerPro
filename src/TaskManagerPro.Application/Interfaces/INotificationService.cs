using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManagerPro.Application.DTOs.Notification;
namespace TaskManagerPro.Application.Interfaces;

public interface INotificationService
{
    Task<IEnumerable<NotificationResponseDto>> GetAllAsync(string userId);
    Task<int> GetUnreadCountAsync(string userId);
    Task MarkAsReadAsync(int notificationId, string userId);
    Task MarkAllAsReadAsync(string userId);
    Task CreateAsync(string userId, string title, string message, string type, int? relatedTaskId = null);
}

