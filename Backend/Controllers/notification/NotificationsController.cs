using Backend.Dtos;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers.notification
{
    [Route("api/v1/notifications")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly ILogger<NotificationsController> _logger;
        private readonly INotificationService _notificationService;

        public NotificationsController(ILogger<NotificationsController> logger, INotificationService notificationService)
        {
            _logger = logger;
            _notificationService = notificationService;
        }

        // GET /api/v1/notifications
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NotificationDto>>> GetAllNotifications()
        {
            var notifications = await _notificationService.GetAllNotificationsAsync();
            return Ok(notifications);
        }

        // POST /api/v1/notifications
        [HttpPost]
        public async Task<ActionResult<NotificationDto>> CreateNotification([FromBody] CreateNotificationDto createNotificationDto)
        {
            var newNotification = await _notificationService.CreateNotificationAsync(createNotificationDto);
            return CreatedAtAction(nameof(GetAllNotifications), new { id = newNotification.Id }, newNotification);
        }

        // PUT /api/v1/notifications/{notificationId}/mark-read
        [HttpPut("{notificationId}/mark-read")]
        public async Task<IActionResult> MarkNotificationAsRead(string notificationId)
        {
            try
            {
                await _notificationService.MarkNotificationAsReadAsync(notificationId);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error marking notification {notificationId} as read.");
                return BadRequest(ex.Message);
            }
        }

        // DELETE /api/v1/notifications/{notificationId}
        [HttpDelete("{notificationId}")]
        public async Task<IActionResult> DeleteNotification(string notificationId)
        {
            try
            {
                await _notificationService.DeleteNotificationAsync(notificationId);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting notification {notificationId}.");
                return BadRequest(ex.Message);
            }
        }

        // DELETE /api/v1/notifications
        [HttpDelete]
        public async Task<IActionResult> DeleteAllNotifications()
        {
            try
            {
                await _notificationService.DeleteAllNotificationsAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting all notifications.");
                return BadRequest(ex.Message);
            }
        }

        // GET /api/v1/notifications/{recipientId}
        [HttpGet("{recipientId}")]
        public async Task<ActionResult<IEnumerable<NotificationDto>>> GetNotificationsByRecipient(string recipientId)
        {
            var notifications = await _notificationService.GetNotificationsByRecipientAsync(recipientId);
            return Ok(notifications);
        }

        // GET /api/v1/notifications/{recipientId}/unread
        [HttpGet("{recipientId}/unread")]
        public async Task<ActionResult<IEnumerable<NotificationDto>>> GetUnreadNotificationsByRecipient(string recipientId)
        {
            var notifications = await _notificationService.GetUnreadNotificationsByRecipientAsync(recipientId);
            return Ok(notifications);
        }

        // GET /api/v1/notifications/{recipientId}/read
        [HttpGet("{recipientId}/read")]
        public async Task<ActionResult<IEnumerable<NotificationDto>>> GetReadNotificationsByRecipient(string recipientId)
        {
            var notifications = await _notificationService.GetReadNotificationsByRecipientAsync(recipientId);
            return Ok(notifications);
        }
    }
}
