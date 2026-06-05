using AutoMapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManagerPro.Application.DTOs.Notification;
using TaskManagerPro.Application.Exceptions;
using TaskManagerPro.Application.Interfaces;
using TaskManagerPro.Application.Interfaces.Repositories;
using TaskManagerPro.Domain.Entities;
namespace TaskManagerPro.Application.Services;

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
        // TODO: add pagination here, users can accumulate a lot of notifications over time
        var notifications = await _uow.Notifications.GetByUserIdAsync(userId);
        return _mapper.Map<IEnumerable<NotificationResponseDto>>(notifications);
    }

    public async Task<int> GetUnreadCountAsync(string userId)
    {
        return await _uow.Notifications.GetUnreadCountByUserIdAsync(userId);
    }

    public async Task MarkAsReadAsync(int notificationId, string userId)
    {
        var notification = await _uow.Notifications.GetByIdAndUserIdAsync(notificationId, userId);

        if (notification == null)
            throw new NotFoundException("Notification not found");

        notification.IsRead = true;
        _uow.Notifications.Update(notification);
        await _uow.SaveChangesAsync();
    }

    public async Task MarkAllAsReadAsync(string userId)
    {
        var notifications = await _uow.Notifications.GetUnreadByUserIdAsync(userId);

        // FIXME: individual updates instead of a single bulk UPDATE — fine for now but won't scale
        foreach (var notification in notifications)
        {
            notification.IsRead = true;
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

    // thought about using this to deduplicate similar notifications, never implemented it
    private static string NotificationKey(string userId, string type, int? taskId) =>
        $"{userId}:{type}:{taskId}";
}
