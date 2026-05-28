using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
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

        foreach (var notification in notifications)
            notification.IsRead = true;

        foreach (var notification in notifications)
            _uow.Notifications.Update(notification);

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



