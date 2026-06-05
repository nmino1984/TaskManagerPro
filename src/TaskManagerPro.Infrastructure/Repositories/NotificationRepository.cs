using Microsoft.EntityFrameworkCore;
using TaskManagerPro.Application.Interfaces.Repositories;
using TaskManagerPro.Domain.Entities;
using TaskManagerPro.Infrastructure.Persistence;

namespace TaskManagerPro.Infrastructure.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly AppDbContext _context;

    public NotificationRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Notification>> GetByUserIdAsync(string userId)
    {
        return await _context.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
    }

    public async Task<int> GetUnreadCountByUserIdAsync(string userId)
    {
        return await _context.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .CountAsync();
    }

    public async Task<Notification?> GetByIdAndUserIdAsync(int notificationId, string userId)
    {
        return await _context.Notifications
            .FirstOrDefaultAsync(n => n.NotificationId == notificationId && n.UserId == userId);
    }

    public async Task<List<Notification>> GetUnreadByUserIdAsync(string userId)
    {
        return await _context.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .ToListAsync();
    }

    public async Task AddAsync(Notification entity)
    {
        await _context.Notifications.AddAsync(entity);
    }

    public void Update(Notification entity)
    {
        _context.Notifications.Update(entity);
    }

    public void Remove(Notification entity)
    {
        _context.Notifications.Remove(entity);
    }

    public async Task<Notification?> GetByIdAsync(object id)
    {
        return await _context.Notifications.FirstOrDefaultAsync(n => n.NotificationId == (int)id);
    }

    public IQueryable<Notification> Query()
    {
        return _context.Notifications;
    }
}
