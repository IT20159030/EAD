/**
* NotificationsController.cs
* This file contains the controller for the Notifications model.
* It contains the CRUD operations for the Notifications model.
*/

using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace Backend.Controllers;

[Route("api/notifications")]
[ApiController]
public class NotificationsController : ControllerBase
{
    private readonly ILogger<NotificationsController> _logger;
    private readonly IMongoCollection<Notification> _notifications;

    public NotificationsController(ILogger<NotificationsController> logger, MongoDBService mongoDbService)
    {
        _logger = logger;
        _notifications = mongoDbService.Database?.GetCollection<Notification>("NOTIFICATIONS") ?? throw new InvalidOperationException("Cannot read MongoDB connection settings");
    }

    // GET /api/notifications
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Notification>>> GetAllNotifications()
    {
        var notifications = await _notifications.Find(notification => true).ToListAsync();
        return Ok(notifications);
    }

    // POST /api/notifications/account-approval
    [HttpPost("account-approval")]
    public async Task<IActionResult> CreateAccountApprovalNotification([FromBody] Notification notification)
    {
        notification.Type = "Account Approval";
        await _notifications.InsertOneAsync(notification);
        return CreatedAtAction(nameof(GetAllNotifications), notification);
    }

    // POST /api/notifications/order-status
    [HttpPost("order-status")]
    public async Task<IActionResult> CreateOrderStatusNotification([FromBody] Notification notification)
    {
        notification.Type = "Order Status";
        await _notifications.InsertOneAsync(notification);
        return CreatedAtAction(nameof(GetAllNotifications), notification);
    }

    // POST /api/notifications/inventory-alert
    [HttpPost("inventory-alert")]
    public async Task<IActionResult> CreateInventoryAlertNotification([FromBody] Notification notification)
    {
        notification.Type = "Inventory Alert";
        await _notifications.InsertOneAsync(notification);
        return CreatedAtAction(nameof(GetAllNotifications), notification);
    }

    // PUT /api/notifications/{notificationId}/mark-read
    [HttpPut("{notificationId}/mark-read")]
    public async Task<IActionResult> MarkNotificationAsRead(string notificationId)
    {
        var notification = await _notifications.Find(n => n.Id == notificationId).FirstOrDefaultAsync();

        if (notification == null) return NotFound();

        notification.IsRead = true;

        await _notifications.ReplaceOneAsync(n => n.Id == notificationId, notification);

        return NoContent();
    }

    // DELETE /api/notifications/{notificationId}
    [HttpDelete("{notificationId}")]
    public async Task<IActionResult> DeleteNotification(string notificationId)
    {
        var notification = await _notifications.Find(n => n.Id == notificationId).FirstOrDefaultAsync();

        if (notification == null) return NotFound();

        await _notifications.DeleteOneAsync(n => n.Id == notificationId);

        return NoContent();
    }

    // DELETE /api/notifications
    [HttpDelete]
    public async Task<IActionResult> DeleteAllNotifications()
    {
        await _notifications.DeleteManyAsync(notification => true);
        return NoContent();
    }

    // GET /api/notifications/{recipientId}
    [HttpGet("{recipientId}")]
    public async Task<ActionResult<IEnumerable<Notification>>> GetNotificationsByRecipient(string recipientId)
    {
        var notifications = await _notifications.Find(notification => notification.RecipientId == recipientId).ToListAsync();
        return Ok(notifications);
    }

    // GET /api/notifications/{recipientId}/unread
    [HttpGet("{recipientId}/unread")]
    public async Task<ActionResult<IEnumerable<Notification>>> GetUnreadNotificationsByRecipient(string recipientId)
    {
        var notifications = await _notifications.Find(notification => notification.RecipientId == recipientId && !notification.IsRead).ToListAsync();
        return Ok(notifications);
    }

    // GET /api/notifications/{recipientId}/read
    [HttpGet("{recipientId}/read")]
    public async Task<ActionResult<IEnumerable<Notification>>> GetReadNotificationsByRecipient(string recipientId)
    {
        var notifications = await _notifications.Find(notification => notification.RecipientId == recipientId && notification.IsRead).ToListAsync();
        return Ok(notifications);
    }

    // GET /api/notifications/{recipientId}/type/{type}
    [HttpGet("{recipientId}/type/{type}")]
    public async Task<ActionResult<IEnumerable<Notification>>> GetNotificationsByRecipientAndType(string recipientId, string type)
    {
        var notifications = await _notifications.Find(notification => notification.RecipientId == recipientId && notification.Type == type).ToListAsync();
        return Ok(notifications);
    }

    // GET /api/notifications/{recipientId}/type/{type}/unread
    [HttpGet("{recipientId}/type/{type}/unread")]
    public async Task<ActionResult<IEnumerable<Notification>>> GetUnreadNotificationsByRecipientAndType(string recipientId, string type)
    {
        var notifications = await _notifications.Find(notification => notification.RecipientId == recipientId && notification.Type == type && !notification.IsRead).ToListAsync();
        return Ok(notifications);
    }

    // GET /api/notifications/{recipientId}/type/{type}/read
    [HttpGet("{recipientId}/type/{type}/read")]
    public async Task<ActionResult<IEnumerable<Notification>>> GetReadNotificationsByRecipientAndType(string recipientId, string type)
    {
        var notifications = await _notifications.Find(notification => notification.RecipientId == recipientId && notification.Type == type && notification.IsRead).ToListAsync();
        return Ok(notifications);
    }

    // GET /api/notifications/{recipientId}/type/{type}/unread/count
    [HttpGet("{recipientId}/type/{type}/unread/count")]
    public async Task<ActionResult<int>> GetUnreadNotificationCountByRecipientAndType(string recipientId, string type)
    {
        var count = await _notifications.Find(notification => notification.RecipientId == recipientId && notification.Type == type && !notification.IsRead).CountDocumentsAsync();
        return Ok(count);
    }

    // GET /api/notifications/{recipientId}/type/{type}/read/count
    [HttpGet("{recipientId}/type/{type}/read/count")]
    public async Task<ActionResult<int>> GetReadNotificationCountByRecipientAndType(string recipientId, string type)
    {
        var count = await _notifications.Find(notification => notification.RecipientId == recipientId && notification.Type == type && notification.IsRead).CountDocumentsAsync();
        return Ok(count);
    }

    // GET /api/notifications/{recipientId}/unread/count
    [HttpGet("{recipientId}/unread/count")]
    public async Task<ActionResult<int>> GetUnreadNotificationCountByRecipient(string recipientId)
    {
        var count = await _notifications.Find(notification => notification.RecipientId == recipientId && !notification.IsRead).CountDocumentsAsync();
        return Ok(count);
    }

    // GET /api/notifications/{recipientId}/read/count
    [HttpGet("{recipientId}/read/count")]
    public async Task<ActionResult<int>> GetReadNotificationCountByRecipient(string recipientId)
    {
        var count = await _notifications.Find(notification => notification.RecipientId == recipientId && notification.IsRead).CountDocumentsAsync();
        return Ok(count);
    }

    // GET /api/notifications/{recipientId}/count
    [HttpGet("{recipientId}/count")]
    public async Task<ActionResult<int>> GetNotificationCountByRecipient(string recipientId)
    {
        var count = await _notifications.Find(notification => notification.RecipientId == recipientId).CountDocumentsAsync();
        return Ok(count);
    }
}