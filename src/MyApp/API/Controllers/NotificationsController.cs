using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.DTOs.Notification;
using MyApp.Application.Interfaces;
using System.Security.Claims;

namespace MyApp.Api.Controllers;

/// <summary>
/// Manages user notifications for task events.
/// </summary>
[ApiController]
[Route("api/v1/notifications")]
[Produces("application/json")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationsController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    /// <summary>
    /// Retrieves all notifications for the current user.
    /// </summary>
    [HttpGet]
    [ProducesResponseType<IEnumerable<NotificationResponseDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new InvalidOperationException("User ID not found in token");
        var notifications = await _notificationService.GetAllAsync(userId);
        return Ok(notifications);
    }

    /// <summary>
    /// Gets the count of unread notifications for the current user.
    /// </summary>
    [HttpGet("unread")]
    [ProducesResponseType<UnreadCountDto>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUnreadCount()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new InvalidOperationException("User ID not found in token");
        var count = await _notificationService.GetUnreadCountAsync(userId);
        return Ok(new UnreadCountDto { Count = count });
    }

    /// <summary>
    /// Marks a specific notification as read.
    /// </summary>
    /// <param name="id">The notification ID.</param>
    [HttpPatch("{id:int}/read")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new InvalidOperationException("User ID not found in token");
        await _notificationService.MarkAsReadAsync(id, userId);
        return NoContent();
    }

    /// <summary>
    /// Marks all notifications as read for the current user.
    /// </summary>
    [HttpPatch("read-all")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> MarkAllAsRead()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new InvalidOperationException("User ID not found in token");
        await _notificationService.MarkAllAsReadAsync(userId);
        return NoContent();
    }
}
