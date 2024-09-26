using System;
using System.ComponentModel.DataAnnotations;

namespace Backend.Dtos
{
    // DTO for creating a new notification
    public class CreateNotificationDto
    {
        [Required]
        public string RecipientId { get; set; }

        [Required]
        public string Message { get; set; }

        public string Type { get; set; }

        public bool IsRead { get; set; } = false;
    }

    // DTO for displaying a notification
    public class NotificationDto
    {
        public string Id { get; set; }

        public string RecipientId { get; set; }

        public string Message { get; set; }

        public DateTime CreatedAt { get; set; }

        public string Type { get; set; }

        public bool IsRead { get; set; }
    }
}
