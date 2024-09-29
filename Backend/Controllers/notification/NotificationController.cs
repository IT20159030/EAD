/**
* This is the Notification Controller.
* It contains the CRUD operations for the Notification model
* and is responsible for handling all requests related to Notifications.
*/

using System.Security.Claims;
using Backend.Dtos;
using Backend.Models;
using Backend.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Backend.Controllers
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
            var notifications = await _notifications.Find(n => n.RecipientId == User.FindFirstValue(ClaimTypes.NameIdentifier)).ToListAsync();
#pragma warning disable CS8601 // Possible null reference assignment.
            return notifications.Select(n => new NotificationDto
            {
                Id = n.Id!,
                RecipientId = n.RecipientId,
                Role = n.Role,
                Message = n.Message,
                CreatedAt = n.CreatedAt,
                Type = n.Type,
                IsRead = n.IsRead
            });
#pragma warning restore CS8601 // Possible null reference assignment.
        }

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
                CreatedAt = notification.CreatedAt,
                Type = notification.Type,
                IsRead = notification.IsRead
            });
#pragma warning restore CS8601 // Possible null reference assignment.
        }

        [HttpPost(Name = "CreateNotification")]
        public async Task<IActionResult> Post([FromBody] CreateNotificationDto dto)
        {
            var notification = new Notification
            {
                RecipientId = dto.RecipientId,
                Role = dto.Role,
                Message = dto.Message,
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
                CreatedAt = notification.CreatedAt,
                Type = notification.Type,
                IsRead = notification.IsRead
            });
        }

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
                CreatedAt = notification.CreatedAt,
                Type = notification.Type,
                IsRead = notification.IsRead
            });
#pragma warning restore CS8601 // Possible null reference assignment.
        }

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

        [HttpPost("lowStock", Name = "CreateLowStockNotification")]
        public async Task<IActionResult> CreateLowStockNotification([FromBody] CreateLowStockNotificationDto dto)
        {
            var notification = new Notification
            {
                RecipientId = dto.RecipientId,
                Role = "Vendor",
                Message = $"Product {dto.ProductId} is low on stock. Current quantity: {dto.CurrentQuantity}.",
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
                CreatedAt = notification.CreatedAt,
                Type = notification.Type,
                IsRead = notification.IsRead
            });
        }


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