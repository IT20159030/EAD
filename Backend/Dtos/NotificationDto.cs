using System;
using System.ComponentModel.DataAnnotations;

namespace Backend.Dtos
{
    // DTO for creating a new notification
    public class CreateNotificationDto
    {
        [Required]
        public string RecipientId { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = string.Empty;

        [Required]
        public string Message { get; set; } = string.Empty;

        [Required]
        public string MessageID { get; set; } = string.Empty;

        public string Type { get; set; } = string.Empty;

        public bool IsRead { get; set; } = false;
    }

    // DTO for displaying a notification
    public class NotificationDto
    {
        public string Id { get; set; } = string.Empty;

        public string RecipientId { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public string MessageID { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public string Type { get; set; } = string.Empty;

        public bool IsRead { get; set; }
    }

    // DTO for updating a notification
    public class UpdateNotificationDto
    {
        public bool IsRead { get; set; }
    }

    // DTO for deleting a notification
    public class DeleteNotificationDto
    {
        public string Id { get; set; } = string.Empty;
    }

    // DTO for getting a list of notifications
    public class GetNotificationsDto
    {
        public string RecipientId { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        public string Type { get; set; } = string.Empty;

        public bool IsRead { get; set; }
    }

    // DTO for getting a list of notifications by recipient ID
    public class GetNotificationsByRecipientDto
    {
        public string RecipientId { get; set; } = string.Empty;
    }

    // DTO for getting a list of notifications by role
    public class GetNotificationsByRoleDto
    {
        public string Role { get; set; } = string.Empty;
    }

    // DTO for getting a list of notifications by type
    public class GetNotificationsByTypeDto
    {
        public string Type { get; set; } = string.Empty;
    }

    // DTO for getting a list of notifications by read status
    public class GetNotificationsByIsReadDto
    {
        public bool IsRead { get; set; }
    }

    // DTO for getting a list of notifications by recipient ID and role
    public class GetNotificationsByRecipientAndRoleDto
    {
        public string RecipientId { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;
    }

    // DTO for CreateLowStockNotificationDto
    public class CreateLowStockNotificationDto
    {
        public string RecipientId { get; set; } = null!;
        public string ProductId { get; set; } = null!;
        public int CurrentQuantity { get; set; }
    }
}
