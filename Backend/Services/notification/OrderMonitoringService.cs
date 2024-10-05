/**
* This service is responsible for monitoring the orders placed by customers.
* It checks the orders collection for orders that are Pending, Ready, Approved, Rejected, Completed, CancelRequested, Cancelled and sends a notification to the vendor and customer.
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
                               order.Status == OrderStatus.CancelRequested ||
                               order.Status == OrderStatus.Cancelled)
                .ToListAsync();

            foreach (var order in recentOrders)
            {
                foreach (var item in order.OrderItems)
                {
                    // Fetch the product to get the vendor ID
                    var product = await _products
                        .Find(p => p.Id == item.ProductId)
                        .FirstOrDefaultAsync();

                    if (product != null)
                    {
                        var vendorId = product.VendorId;
                        var orderDetails = order.OrderId;
                        var message = string.Empty;

                        switch (order.Status)
                        {
                            case OrderStatus.Approved:
                                message = $"Order {orderDetails} has been approved";
                                break;
                            case OrderStatus.CancelRequested:
                                message = $"Order {orderDetails} has a cancellation request";
                                break;
                            case OrderStatus.Cancelled:
                                message = $"Order {orderDetails} has been cancelled";
                                break;
                            case OrderStatus.Ready:
                                message = $"Order {orderDetails} is ready";
                                break;
                        }

                        var notificationType = $"OrderStatus";
                        var existingVendorNotification = await _notifications
                            .Find(n => n.MessageID == order.OrderId &&
                                       n.RecipientId == vendorId &&
                                       n.Type == notificationType &&
                                       !n.IsRead)
                            .FirstOrDefaultAsync();

                        if (existingVendorNotification == null)
                        {
                            var vendorNotification = new Notification
                            {
                                RecipientId = vendorId,
                                Role = "vendor",
                                Message = message,
                                MessageID = order.OrderId,
                                CreatedAt = DateTime.UtcNow,
                                Type = notificationType, // Unique notification type
                                IsRead = false
                            };

                            await _notifications.InsertOneAsync(vendorNotification);
                            await _hubContext.Clients.User(vendorId).SendAsync("ReceiveNotification", vendorNotification.Message);
                            _logger.LogInformation($"Order status notification sent to Vendor for Order {orderDetails}");
                        }
                        else
                        {
                            _logger.LogInformation($"Notification already exists for Order {orderDetails} and Vendor {vendorId}. Skipping...");
                        }

                        if (order.Status == OrderStatus.Ready || order.Status == OrderStatus.Cancelled)
                        {
                            var customerMessage = order.Status == OrderStatus.Ready
                                ? $"Your order {orderDetails} is ready"
                                : $"Your order {orderDetails} has been cancelled";

                            var existingCustomerNotification = await _notifications
                                .Find(n => n.MessageID == order.OrderId &&
                                           n.RecipientId == order.CustomerId &&
                                           n.Type == notificationType &&
                                           !n.IsRead)
                                .FirstOrDefaultAsync();

                            if (existingCustomerNotification == null)
                            {
                                var customerNotification = new Notification
                                {
                                    RecipientId = order.CustomerId,
                                    Role = "customer",
                                    Message = customerMessage,
                                    MessageID = order.OrderId,
                                    CreatedAt = DateTime.UtcNow,
                                    Type = notificationType, // Unique notification type
                                    IsRead = false
                                };

                                await _notifications.InsertOneAsync(customerNotification);
                                await _hubContext.Clients.User(order.CustomerId).SendAsync("ReceiveNotification", customerNotification.Message);
                                _logger.LogInformation($"Order status notification sent to Customer for Order {orderDetails}");
                            }
                            else
                            {
                                _logger.LogInformation($"Notification already exists for Order {orderDetails} and Customer {order.CustomerId}. Skipping...");
                            }
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
