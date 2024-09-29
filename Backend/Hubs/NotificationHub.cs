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
        // Method to send notifications to the client
        public async Task SendNotification(string userId, string message)
        {
            await Clients.User(userId).SendAsync("ReceiveNotification", message);
        }
    }
}
