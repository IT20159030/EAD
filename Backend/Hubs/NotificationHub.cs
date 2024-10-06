/** 
* This is the Notification Hub.
* It contains the SignalR Hub for the Notification model
* and is responsible for handling all real-time notifications.
* It contains methods to send notifications to all clients or to specific users.
* The SendNotification method sends a notification to all clients.
* The SendNotificationToUser method sends a notification to a specific user.
* The ReceiveNotification method is the method that the client will call to receive the notification.
* The NotificationHub class inherits from the Hub class in the Microsoft.AspNetCore.SignalR namespace.
* The Hub class is the base class for all SignalR hubs.
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
