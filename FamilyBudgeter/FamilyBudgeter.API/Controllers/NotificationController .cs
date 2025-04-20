using FamilyBudgeter.API.BLL.DTOs.NotificationDTOs;
using FamilyBudgeter.API.BLL.Interfaces;
using FamilyBudgeter.API.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamilyBudgeter.API.Controllers
{
    [Authorize]
    public class NotificationController : BaseApiController
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserNotifications()
        {
            try
            {
                var userId = GetCurrentUserId();
                var notifications = await _notificationService.GetUserNotificationsAsync(userId);
                return Ok(notifications);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("unread")]
        public async Task<IActionResult> GetUnreadNotifications()
        {
            try
            {
                var userId = GetCurrentUserId();
                var notifications = await _notificationService.GetUnreadNotificationsAsync(userId);
                return Ok(notifications);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetNotification(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var notification = await _notificationService.GetNotificationByIdAsync(id, userId);
                return Ok(notification);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPost("{id}/mark-read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _notificationService.MarkAsReadAsync(id, userId);
                return Ok(new { success = result });
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPost("mark-all-read")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _notificationService.MarkAllAsReadAsync(userId);
                return Ok(new { success = result });
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNotification(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _notificationService.DeleteNotificationAsync(id, userId);
                return Ok(new { success = result });
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("by-type/{type}")]
        public async Task<IActionResult> GetNotificationsByType(NotificationType type)
        {
            try
            {
                var userId = GetCurrentUserId();
                var notifications = await _notificationService.GetNotificationsByTypeAsync(userId, type);
                return Ok(notifications);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPost("filter")]
        public async Task<IActionResult> FilterNotifications([FromBody] NotificationFilterDto filterDto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var notifications = await _notificationService.FilterNotificationsAsync(filterDto, userId);
                return Ok(notifications);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
