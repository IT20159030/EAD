/** 
* This is the Notification Hub.
* It contains the SignalR Hub for the Notification model
* and is responsible for handling all real-time notifications.
*/

using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Backend.Hubs
{
    public class NotificationHub : Hub
    {
        public async Task SendLowStockNotification(string recipientId, string productId, int quantity)
        {
            var message = $"Product {productId} is low on stock. Current quantity: {quantity}.";
            await Clients.User(recipientId).SendAsync("ReceiveNotification", message);
        }
    }
}
