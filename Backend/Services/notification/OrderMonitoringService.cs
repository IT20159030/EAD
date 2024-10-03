/**
* This service is responsible for monitoring the orders placed by customers.
* It checks the orders collection for orders that are Pending, Ready, Approved, Rejected, Completed, CancelRequested, Cancelled and sends a notification to the vendor.
* It uses the Order and Notification models to interact with the database.
*/

using Backend.Hubs;
using Backend.Models;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Driver;

namespace Backend.Services.notification
{
    public class OrderMonitoringService
    {
        private readonly IMongoCollection<Order> _orders;
        private readonly IMongoCollection<Notification> _notifications;
        private readonly IMongoCollection<Product> _products;
        private readonly ILogger<OrderMonitoringService> _logger;
        private readonly IHubContext<NotificationHub> _hubContext;

        public OrderMonitoringService(IMongoCollection<Order> orders,
                                      IMongoCollection<Notification> notifications,
                                      IMongoCollection<Product> products,
                                      ILogger<OrderMonitoringService> logger,
                                      IHubContext<NotificationHub> hubContext)
        {
            _orders = orders;
            _notifications = notifications;
            _products = products;
            _logger = logger;
            _hubContext = hubContext;
        }

        public async Task MonitorOrderStatusesAsync()
        {
            // Fetch all orders that have recently changed status
            var recentOrders = await _orders
                .Find(order => order.Status == OrderStatus.Ready ||
                               order.Status == OrderStatus.Approved ||
                               order.Status == OrderStatus.CancelRequested)
                .ToListAsync();

            foreach (var order in recentOrders)
            {
                // Loop through each OrderItem to get the vendor IDs
                foreach (var item in order.OrderItems)
                {
                    // Fetch the product to get the vendor ID
                    var product = await _products
                        .Find(p => p.Id == item.ProductId)
                        .FirstOrDefaultAsync();

                    if (product != null)
                    {
                        var vendorId = product.VendorId;
                        var orderDetails = order.Id.ToString();
                        var message = string.Empty;

                        // Create a message based on the order status
                        switch (order.Status)
                        {
                            case OrderStatus.Ready:
                                message = $"Order {orderDetails} is ready";
                                break;
                            case OrderStatus.Approved:
                                message = $"Order {orderDetails} has been approved";
                                break;
                            case OrderStatus.CancelRequested:
                                message = $"Order {orderDetails} has a cancellation request";
                                break;
                        }

                        // Check if a notification for this order status already exists
                        var existingNotification = await _notifications
                            .Find(n => n.MessageID == order.Id &&
                                       n.RecipientId == vendorId &&
                                       n.Type == order.Status.ToString() &&
                                       !n.IsRead)
                            .FirstOrDefaultAsync();

                        // If no unread notification exists, create and send a new notification
                        if (existingNotification == null)
                        {
                            var notification = new Notification
                            {
                                RecipientId = vendorId,
                                Role = "vendor",
                                Message = message,
                                MessageID = order.Id,
                                CreatedAt = DateTime.UtcNow,
                                Type = order.Status.ToString(),
                                IsRead = false
                            };

                            // Insert the notification into the database
                            await _notifications.InsertOneAsync(notification);

                            // Send notification via SignalR
                            await _hubContext.Clients.User(vendorId).SendAsync("ReceiveNotification", notification.Message);

                            _logger.LogInformation($"Order status notification sent for Order {orderDetails}");
                        }
                        else
                        {
                            _logger.LogInformation($"Notification already exists for Order {orderDetails} and Vendor {vendorId}. Skipping...");
                        }
                    }
                    else
                    {
                        _logger.LogWarning($"Product not found for OrderItem with ProductId: {item.ProductId}");
                    }
                }
            }
        }
    }
}
