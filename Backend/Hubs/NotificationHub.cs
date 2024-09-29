/** 
* This is the Notification Hub.
* It contains the SignalR Hub for the Notification model
* and is responsible for handling all real-time notifications.
*/

using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Backend.Hubs
{
    public class NotificationHub : Hub
    {
        // Method to send notifications to all clients
        public async Task SendNotification(string message)
        {
            await Clients.All.SendAsync("ReceiveNotification", message);
        }

        // Alternatively, if you want to send to specific users as well
        public async Task SendNotificationToUser(string userId, string message)
        {
            await Clients.User(userId).SendAsync("ReceiveNotification", message);
        }
    }
}
