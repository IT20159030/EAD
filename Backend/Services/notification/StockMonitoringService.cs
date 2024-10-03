/**
* This service is responsible for monitoring the stock levels of products in the inventory.
* It checks the inventory collection for products that are low on stock and sends a notification to the vendor.
* It uses the Inventory and Notification models to interact with the database.
*/

using Backend.Hubs;
using Backend.Models;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Driver;

namespace Backend.Services.notification
{
    public class StockMonitoringService
    {
        private readonly IMongoCollection<Notification> _notifications;
        private readonly IMongoCollection<Product> _products;
        private readonly ILogger<StockMonitoringService> _logger;
        private readonly IHubContext<NotificationHub> _hubContext;

        public StockMonitoringService(IMongoCollection<Notification> notifications,
                                      IMongoCollection<Product> products,
                                      ILogger<StockMonitoringService> logger,
                                      IHubContext<NotificationHub> hubContext)
        {
            _notifications = notifications;
            _products = products;
            _logger = logger;
            _hubContext = hubContext;
        }

        public async Task MonitorStockLevelsAsync()
        {
            // Fetch products where stock is less than or equal to 10
            var lowStockProducts = await _products
                .Find(p => p.Stock <= 10)
                .ToListAsync();

            foreach (var product in lowStockProducts)
            {
                var productName = product.Name ?? "Unknown Product";

                // Check if a low stock notification already exists for this product and vendor
                var existingNotification = await _notifications
                    .Find(n => n.MessageID == product.Id &&
                               n.RecipientId == product.VendorId &&
                               n.Type == "LowStock" &&
                               !n.IsRead)
                    .FirstOrDefaultAsync();

                // If no unread notification exists, create and send a new notification
                if (existingNotification == null)
                {
                    var notification = new Notification
                    {
                        RecipientId = product.VendorId,
                        Role = "vendor",
                        Message = $"{productName} is low on stock",
                        MessageID = product.Id,
                        CreatedAt = DateTime.UtcNow,
                        Type = "LowStock",
                        IsRead = false
                    };

                    // Insert the notification into the database
                    await _notifications.InsertOneAsync(notification);

                    // Send notification via SignalR
                    await _hubContext.Clients.User(product.VendorId).SendAsync("ReceiveNotification", notification.Message);

                    _logger.LogInformation($"Low stock notification sent for Product {productName}");
                }
                else
                {
                    _logger.LogInformation($"Notification already exists for Product {productName} and Vendor {product.VendorId}. Skipping...");
                }
            }
        }
    }
}
