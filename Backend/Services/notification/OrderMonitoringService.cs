/**
* This service is responsible for monitoring the orders placed by customers.
* It checks the orders collection for orders that are Pending, Ready, Approved, Rejected, Completed, CancelRequested, Cancelled and sends a notification to the vendor.
* It uses the Order and Notification models to interact with the database.
*/

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Backend.Models;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace Backend.Services.notification
{
    public class OrderMonitoringService
    {
        private readonly IMongoCollection<Order> _orders;
        private readonly IMongoCollection<Notification> _notifications;
        private readonly IMongoClient _mongoClient;
        private readonly IConfiguration _configuration;

        public OrderMonitoringService(IMongoClient mongoClient, IConfiguration configuration)
        {
            _mongoClient = mongoClient ?? throw new ArgumentNullException(nameof(mongoClient));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            var database = _mongoClient.GetDatabase(
                new MongoUrl(_configuration.GetConnectionString("MongoDB")).DatabaseName
            );

            _orders = database.GetCollection<Order>("orders");
            _notifications = database.GetCollection<Notification>("Notification");
        }

        public async Task MonitorOrderStatusAsync()
        {
            // Find orders with different statuses
            var orders = await _orders.Find(o => o.Status == OrderStatus.Pending || o.Status == OrderStatus.Ready || o.Status == OrderStatus.Approved || o.Status == OrderStatus.Rejected || o.Status == OrderStatus.Completed || o.Status == OrderStatus.CancelRequested || o.Status == OrderStatus.Cancelled).ToListAsync();

            foreach (var order in orders)
            {
                var existingNotification = await _notifications.Find(n =>
                        n.RecipientId == order.CustomerId &&
                        n.Message == $"Order {order.OrderId} is {order.Status}" &&
                        n.Type == "OrderStatus" &&
                        !n.IsRead)
                    .FirstOrDefaultAsync();

                // If no such notification exists, create and save a new one
                if (existingNotification == null)
                {
                    var notification = new Notification
                    {
                        RecipientId = order.CustomerId,
                        Role = "customer",
                        Message = $"Order {order.OrderId} is {order.Status}",
                        MessageID = order.OrderId,
                        CreatedAt = DateTime.UtcNow,
                        Type = "OrderStatus",
                        IsRead = false
                    };

                    await _notifications.InsertOneAsync(notification);
                }
            }
        }
    }
}