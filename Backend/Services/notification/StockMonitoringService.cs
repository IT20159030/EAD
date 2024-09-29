/**
* This service is responsible for monitoring the stock levels of products in the inventory.
* It checks the inventory collection for products that are low on stock and sends a notification to the vendor.
* It uses the Inventory and Notification models to interact with the database.
*/

using Backend.Models;
using MongoDB.Driver;

namespace Backend.Services.notification
{
    public class StockMonitoringService
    {
        private readonly IMongoCollection<Inventory> _inventoryCollection;
        private readonly IMongoCollection<Notification> _notifications;
        private readonly ILogger<StockMonitoringService> _logger;

        public StockMonitoringService(IMongoCollection<Inventory> inventoryCollection,
                                      IMongoCollection<Notification> notifications,
                                      ILogger<StockMonitoringService> logger)
        {
            _inventoryCollection = inventoryCollection;
            _notifications = notifications;
            _logger = logger;
        }

        public async Task MonitorStockLevelsAsync()
        {
            var lowStockItems = await _inventoryCollection
                .Find(i => i.Quantity <= i.AlertThreshold && !i.LowStockAlert)
                .ToListAsync();

            foreach (var item in lowStockItems)
            {
                var notification = new Notification
                {
                    RecipientId = item.VendorId,
                    Role = "Vendor",
                    Message = $"Product {item.ProductId} is low on stock. Current quantity: {item.Quantity}.",
                    CreatedAt = DateTime.UtcNow,
                    Type = "LowStock",
                    IsRead = false
                };

                // Insert the notification into the database
                await _notifications.InsertOneAsync(notification);

                // Update the inventory to mark that the alert was sent
                var update = Builders<Inventory>.Update.Set(i => i.LowStockAlert, true);
                await _inventoryCollection.UpdateOneAsync(i => i.Id == item.Id, update);

                _logger.LogInformation($"Low stock notification sent for Product {item.ProductId}");
            }
        }
    }
}
