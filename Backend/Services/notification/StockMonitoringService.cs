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
        private readonly IMongoCollection<Inventory> _inventoryCollection;
        private readonly IMongoCollection<Notification> _notifications;
        private readonly IMongoCollection<Product> _products;
        private readonly ILogger<StockMonitoringService> _logger;
        private readonly IHubContext<NotificationHub> _hubContext;


        public StockMonitoringService(IMongoCollection<Inventory> inventoryCollection,
                                   IMongoCollection<Notification> notifications,
                                   IMongoCollection<Product> products,
                                   ILogger<StockMonitoringService> logger,
                                   IHubContext<NotificationHub> hubContext)
        {
            _inventoryCollection = inventoryCollection;
            _notifications = notifications;
            _products = products;
            _logger = logger;
            _hubContext = hubContext;
        }

        public async Task MonitorStockLevelsAsync()
        {
            var lowStockItems = await _inventoryCollection
                .Find(i => i.Quantity <= i.AlertThreshold && !i.LowStockAlert)
                .ToListAsync();

            foreach (var item in lowStockItems)
            {
                // Fetch the product details
                var product = await _products
                    .Find(p => p.Id == item.ProductId)
                    .FirstOrDefaultAsync();

                string productName = product?.Name ?? "Unknown Product";

                var notification = new Notification
                {
                    RecipientId = item.VendorId,
                    Role = "Vendor",
                    Message = $"{productName} is low on stock",
                    MessageID = item.ProductId,
                    CreatedAt = DateTime.UtcNow,
                    Type = "LowStock",
                    IsRead = false
                };

                // Insert the notification into the database
                await _notifications.InsertOneAsync(notification);

                // Update the inventory to mark that the alert was sent
                var update = Builders<Inventory>.Update.Set(i => i.LowStockAlert, true);
                await _inventoryCollection.UpdateOneAsync(i => i.Id == item.Id, update);

                // Send notification via SignalR
                await _hubContext.Clients.User(item.VendorId).SendAsync("ReceiveNotification", notification.Message);

                _logger.LogInformation($"Low stock notification sent for Product {productName}");
            }
        }
    }
}
