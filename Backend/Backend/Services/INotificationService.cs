using Backend.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend.Services
{
    public interface INotificationService
    {
        Task<IEnumerable<NotificationDto>> GetAllNotificationsAsync();
        Task<IEnumerable<NotificationDto>> GetNotificationsByRecipientAsync(string recipientId);
        Task<IEnumerable<NotificationDto>> GetUnreadNotificationsByRecipientAsync(string recipientId);
        Task<IEnumerable<NotificationDto>> GetReadNotificationsByRecipientAsync(string recipientId);
        Task<NotificationDto> CreateNotificationAsync(CreateNotificationDto createNotificationDto);
        Task MarkNotificationAsReadAsync(string notificationId);
        Task DeleteNotificationAsync(string notificationId);
        Task DeleteAllNotificationsAsync();
    }
}
