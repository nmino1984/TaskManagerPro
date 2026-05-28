using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManagerPro.Domain.Entities;

namespace TaskManagerPro.Application.Interfaces.Repositories;

public interface INotificationRepository : IRepository<Notification>
{
    Task<List<Notification>> GetByUserIdAsync(string userId);
    Task<int> GetUnreadCountByUserIdAsync(string userId);
    Task<Notification?> GetByIdAndUserIdAsync(int notificationId, string userId);
    Task<List<Notification>> GetUnreadByUserIdAsync(string userId);
}
