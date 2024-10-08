/**
* This is the Notification Controller.
* It contains the CRUD operations for the Notification model
* and is responsible for handling all requests related to Notifications.
* It contains following routes:
* [GET] /api/v1/notification
* [GET] /api/v1/notification/{id}
* [POST] /api/v1/notification
* [PUT] /api/v1/notification/{id}
* [DELETE] /api/v1/notification/{id}
* [POST] /api/v1/notification/lowStock
* [PUT] /api/v1/notification/{id}/read
* [GET] /api/v1/notification/recipient/{recipientId}
* All routes are protected and can only be accessed by authenticated users.
* The controller uses MongoDB to store the Notification data.
* The controller also uses ILogger to log all the relevant information.
* The controller uses NotificationDto, CreateNotificationDto, UpdateNotificationDto, CreateLowStockNotificationDto
* to transfer data between the controller and the client.
*/

using System.Security.Claims;
using Backend.Dtos;
using Backend.Models;
using Backend.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Backend.Controllers.notification
{
    [ApiController]
    [Route("api/v1/notification")]
    [Authorize]
    public class NotificationController : ControllerBase
    {
        private readonly IMongoCollection<Notification> _notifications;
        private readonly ILogger<NotificationController> _logger;

        public NotificationController(ILogger<NotificationController> logger, MongoDBService mongoDBService)
        {
            _logger = logger;
            _notifications = mongoDBService.Database.GetCollection<Notification>("Notification");
        }

        [HttpGet(Name = "GetAllNotifications")]
        public async Task<IEnumerable<NotificationDto>> Get()
        {
            var notifications = await _notifications
                .Find(n => true)
                .Sort(Builders<Notification>.Sort.Descending(n => n.CreatedAt))
                .ToListAsync();

            if (notifications.Count == 0)
            {
                _logger.LogInformation("No notifications found.");
            }
            else
            {
                _logger.LogInformation($"{notifications.Count} notifications found.");
            }

            return notifications.Select(n => new NotificationDto
            {
                Id = n.Id!,
                RecipientId = n.RecipientId ?? string.Empty,
                Role = n.Role ?? string.Empty,
                Message = n.Message ?? string.Empty,
                MessageID = n.MessageID ?? string.Empty,
                CreatedAt = n.CreatedAt,
                Type = n.Type ?? string.Empty,
                IsRead = n.IsRead
            });
        }

        // Get notification by id 
        [HttpGet("{id}", Name = "GetNotificationById")]
        public async Task<IActionResult> Get(string id)
        {
            var notification = await _notifications.Find(n => n.Id == id).FirstOrDefaultAsync();
            if (notification == null)
            {
                return NotFound();
            }

#pragma warning disable CS8601 // Possible null reference assignment.
            return Ok(new NotificationDto
            {
                Id = notification.Id!,
                RecipientId = notification.RecipientId,
                Role = notification.Role,
                Message = notification.Message,
                MessageID = notification.MessageID,
                CreatedAt = notification.CreatedAt,
                Type = notification.Type,
                IsRead = notification.IsRead
            });
#pragma warning restore CS8601 // Possible null reference assignment.
        }

        // Create a new notification 
        [HttpPost(Name = "CreateNotification")]
        public async Task<IActionResult> Post([FromBody] CreateNotificationDto dto)
        {
            var notification = new Notification
            {
                RecipientId = dto.RecipientId,
                Role = dto.Role,
                Message = dto.Message,
                MessageID = dto.MessageID,
                Type = dto.Type,
                IsRead = dto.IsRead,
                CreatedAt = DateTime.UtcNow
            };

            await _notifications.InsertOneAsync(notification);

            return CreatedAtAction(nameof(Get), new { id = notification.Id }, new NotificationDto
            {
                Id = notification.Id!,
                RecipientId = notification.RecipientId,
                Role = notification.Role,
                Message = notification.Message,
                MessageID = notification.MessageID,
                CreatedAt = notification.CreatedAt,
                Type = notification.Type,
                IsRead = notification.IsRead
            });
        }

        // Update notification by id 
        [HttpPut("{id}", Name = "UpdateNotification")]
        public async Task<IActionResult> Put(string id, [FromBody] UpdateNotificationDto dto)
        {
            var notification = await _notifications.Find(n => n.Id == id).FirstOrDefaultAsync();
            if (notification == null)
            {
                return NotFound();
            }

            notification.IsRead = dto.IsRead;

            await _notifications.ReplaceOneAsync(n => n.Id == id, notification);

#pragma warning disable CS8601 // Possible null reference assignment.
            return Ok(new NotificationDto
            {
                Id = notification.Id!,
                RecipientId = notification.RecipientId,
                Role = notification.Role,
                Message = notification.Message,
                MessageID = notification.MessageID,
                CreatedAt = notification.CreatedAt,
                Type = notification.Type,
                IsRead = notification.IsRead
            });
#pragma warning restore CS8601 // Possible null reference assignment.
        }

        // Delete notification by id
        [HttpDelete("{id}", Name = "DeleteNotification")]
        public async Task<IActionResult> Delete(string id)
        {
            var notification = await _notifications.Find(n => n.Id == id).FirstOrDefaultAsync();
            if (notification == null)
            {
                return NotFound();
            }

            await _notifications.DeleteOneAsync(n => n.Id == id);

            return NoContent();
        }

        // Create a new low stock notification
        [HttpPost("lowStock", Name = "CreateLowStockNotification")]
        public async Task<IActionResult> CreateLowStockNotification([FromBody] CreateLowStockNotificationDto dto)
        {
            var notification = new Notification
            {
                RecipientId = dto.RecipientId,
                Role = "vendor",
                Message = $"{dto.ProductId} is low on stock. Current quantity: {dto.CurrentQuantity}.",
                MessageID = dto.ProductId,
                Type = "LowStock",
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            await _notifications.InsertOneAsync(notification);

            return CreatedAtAction(nameof(Get), new { id = notification.Id }, new NotificationDto
            {
                Id = notification.Id!,
                RecipientId = notification.RecipientId,
                Role = notification.Role,
                Message = notification.Message,
                MessageID = notification.MessageID,
                CreatedAt = notification.CreatedAt,
                Type = notification.Type,
                IsRead = notification.IsRead
            });
        }

        // Mark notification as read by notification id
        [HttpPut("{id}/read", Name = "MarkNotificationAsRead")]
        public async Task<IActionResult> MarkAsRead(string id)
        {
            var notification = await _notifications.Find(n => n.Id == id).FirstOrDefaultAsync();
            if (notification == null)
            {
                return NotFound();
            }

            notification.IsRead = true;

            await _notifications.ReplaceOneAsync(n => n.Id == id, notification);

#pragma warning disable CS8601 // Possible null reference assignment.
            return Ok(new NotificationDto
            {
                Id = notification.Id!,
                RecipientId = notification.RecipientId,
                Role = notification.Role,
                Message = notification.Message,
                MessageID = notification.MessageID,
                CreatedAt = notification.CreatedAt,
                Type = notification.Type,
                IsRead = notification.IsRead
            });
#pragma warning restore CS8601 // Possible null reference assignment.
        }

        // get notifications by recipient id
        [HttpGet("recipient/{recipientId}", Name = "GetNotificationsByRecipientId")]
        public async Task<IEnumerable<NotificationDto>> GetByRecipientId(string recipientId)
        {
            var notifications = await _notifications
                .Find(n => n.RecipientId == recipientId)
                .Sort(Builders<Notification>.Sort.Descending(n => n.CreatedAt))
                .ToListAsync();

            if (notifications.Count == 0)
            {
                _logger.LogInformation("No notifications found.");
            }
            else
            {
                _logger.LogInformation($"{notifications.Count} notifications found.");
            }

            return notifications.Select(n => new NotificationDto
            {
                Id = n.Id!,
                RecipientId = n.RecipientId ?? string.Empty,
                Role = n.Role ?? string.Empty,
                Message = n.Message ?? string.Empty,
                MessageID = n.MessageID ?? string.Empty,
                CreatedAt = n.CreatedAt,
                Type = n.Type ?? string.Empty,
                IsRead = n.IsRead
            });
        }

        // Convert Notification to NotificationDto
        private NotificationDto ConvertToDto(Notification notification)
        {
#pragma warning disable CS8601 // Possible null reference assignment.
            return new NotificationDto
            {
                Id = notification.Id!,
                RecipientId = notification.RecipientId,
                Role = notification.Role,
                Message = notification.Message,
                CreatedAt = notification.CreatedAt,
                Type = notification.Type,
                IsRead = notification.IsRead
            };
#pragma warning restore CS8601 // Possible null reference assignment.
        }

        // Convert CreateNotificationDto to Notification
        private Notification ConvertToModel(CreateNotificationDto dto)
        {
            return new Notification
            {
                RecipientId = dto.RecipientId,
                Role = dto.Role,
                Message = dto.Message,
                Type = dto.Type,
                IsRead = dto.IsRead
            };
        }
    }
}