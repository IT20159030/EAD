using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Backend.Dtos;
using Backend.Models;
using MongoDB.Driver;

namespace Backend.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IMongoCollection<Notification> _notifications;
        private readonly IMapper _mapper;

        public NotificationService(IMongoClient mongoClient, IMapper mapper)
        {
            var database = mongoClient.GetDatabase("ecommerce_ead");
            _notifications = database.GetCollection<Notification>("notifications");
            _mapper = mapper;
        }

        public async Task<IEnumerable<NotificationDto>> GetAllNotificationsAsync()
        {
            var notifications = await _notifications.Find(_ => true).ToListAsync();
            return _mapper.Map<IEnumerable<NotificationDto>>(notifications);
        }

        public async Task<IEnumerable<NotificationDto>> GetNotificationsByRecipientAsync(string recipientId)
        {
            var notifications = await _notifications.Find(n => n.RecipientId == recipientId).ToListAsync();
            return _mapper.Map<IEnumerable<NotificationDto>>(notifications);
        }

        public async Task<IEnumerable<NotificationDto>> GetUnreadNotificationsByRecipientAsync(string recipientId)
        {
            var notifications = await _notifications.Find(n => n.RecipientId == recipientId && !n.IsRead).ToListAsync();
            return _mapper.Map<IEnumerable<NotificationDto>>(notifications);
        }

        public async Task<IEnumerable<NotificationDto>> GetReadNotificationsByRecipientAsync(string recipientId)
        {
            var notifications = await _notifications.Find(n => n.RecipientId == recipientId && n.IsRead).ToListAsync();
            return _mapper.Map<IEnumerable<NotificationDto>>(notifications);
        }

        public async Task<NotificationDto> CreateNotificationAsync(CreateNotificationDto createNotificationDto)
        {
            var notification = _mapper.Map<Notification>(createNotificationDto);
            notification.CreatedAt = DateTime.UtcNow;
            await _notifications.InsertOneAsync(notification);
            return _mapper.Map<NotificationDto>(notification);
        }

        public async Task MarkNotificationAsReadAsync(string notificationId)
        {
            var filter = Builders<Notification>.Filter.Eq(n => n.Id, notificationId);
            var update = Builders<Notification>.Update.Set(n => n.IsRead, true);
            await _notifications.UpdateOneAsync(filter, update);
        }

        public async Task DeleteNotificationAsync(string notificationId)
        {
            await _notifications.DeleteOneAsync(n => n.Id == notificationId);
        }

        public async Task DeleteAllNotificationsAsync()
        {
            await _notifications.DeleteManyAsync(_ => true);
        }
    }
}
