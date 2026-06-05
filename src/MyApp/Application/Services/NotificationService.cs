using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MyApp.Application.DTOs.Notification;
using MyApp.Application.Exceptions;
using MyApp.Application.Interfaces;
using MyApp.Application.Interfaces.Repositories;
using MyApp.Domain.Entities;

namespace MyApp.Application.Services;

public class NotificationService : INotificationService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public NotificationService(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<IEnumerable<NotificationResponseDto>> GetAllAsync(string userId)
    {
        var notifications = await _uow.Notifications.Query()
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();

        return _mapper.Map<IEnumerable<NotificationResponseDto>>(notifications);
    }

    public async Task<int> GetUnreadCountAsync(string userId)
    {
        return await _uow.Notifications.Query()
            .Where(n => n.UserId == userId && !n.IsRead)
            .CountAsync();
    }

    public async Task MarkAsReadAsync(int notificationId, string userId)
    {
        var notification = await _uow.Notifications.Query()
            .FirstOrDefaultAsync(n => n.NotificationId == notificationId && n.UserId == userId);

        if (notification == null)
        {
            throw new NotFoundException("Notification not found");
        }

        notification.IsRead = true;
        _uow.Notifications.Update(notification);
        await _uow.SaveChangesAsync();
    }

    public async Task MarkAllAsReadAsync(string userId)
    {
        var notifications = await _uow.Notifications.Query()
            .Where(n => n.UserId == userId && !n.IsRead)
            .ToListAsync();

        foreach (var notification in notifications)
        {
            notification.IsRead = true;
        }

        foreach (var notification in notifications)
        {
            _uow.Notifications.Update(notification);
        }

        await _uow.SaveChangesAsync();
    }

    public async Task CreateAsync(string userId, string title, string message, string type, int? relatedTaskId = null)
    {
        var notification = new Notification
        {
            UserId = userId,
            Title = title,
            Message = message,
            Type = type,
            RelatedTaskId = relatedTaskId,
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        await _uow.Notifications.AddAsync(notification);
        await _uow.SaveChangesAsync();
    }
}
