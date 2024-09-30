/**
* This service is responsible for monitoring the user's activity and sending notifications to the user.
* User activity is monitored by checking the status of the user account and sending a notification to the user if the account is unapproved.
* Additionally, if the user account's status changes from Unapproved to Active, a notification is sent to the user.
*/

using System;
using System.Threading.Tasks;
using Backend.Models;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace Backend.Services.notification
{
    public class UserMonitoringService
    {
        private readonly IMongoCollection<User> _users;
        private readonly IMongoCollection<Notification> _notifications;
        private readonly IMongoClient _mongoClient;
        private readonly IConfiguration _configuration;

        public UserMonitoringService(IMongoClient mongoClient, IConfiguration configuration)
        {
            _mongoClient = mongoClient ?? throw new ArgumentNullException(nameof(mongoClient));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            var database = _mongoClient.GetDatabase(
                new MongoUrl(_configuration.GetConnectionString("MongoDB")).DatabaseName
            );

            _users = database.GetCollection<User>("users");
            _notifications = database.GetCollection<Notification>("Notification");
        }

        public async Task MonitorUserActivityAsync()
        {
            // Find users with an unapproved status 
            var users = await _users.Find(u => u.Status == AccountStatus.Unapproved).ToListAsync();

            foreach (var user in users)
            {
                if (user.Status == AccountStatus.Active)
                {
                    // Skip active users for this specific check
                    Console.WriteLine($"Skipping notification for active user {user.UserName}");
                    continue;
                }

                var existingNotification = await _notifications.Find(n =>
                        n.RecipientId == user.Id.ToString() &&
                        n.Message == $"{user.Name}'s account is still unapproved. Please complete any pending actions for approval" &&
                        n.Type == "AccountApproval")
                    .FirstOrDefaultAsync();

                // If no such notification exists, create and save a new one
                if (existingNotification == null)
                {
                    var notification = new Notification
                    {
                        RecipientId = user.Id.ToString(),
                        Role = "csr",
                        Message = $"{user.Name}'s account is still unapproved. Please complete any pending actions for approval",
                        MessageID = user.Id.ToString(),
                        CreatedAt = DateTime.UtcNow,
                        Type = "AccountApproval",
                        IsRead = false
                    };

                    // Insert the notification into the database
                    await _notifications.InsertOneAsync(notification);
                    Console.WriteLine($"Notification saved for user {user.UserName}: {notification.Message}");
                }
                else
                {
                    Console.WriteLine($"Notification for user {user.UserName} already exists. Skipping.");
                }
            }

            // Check for users whose accounts have changed to Active
            await CheckAndNotifyStatusChangeAsync();
        }

        private async Task CheckAndNotifyStatusChangeAsync()
        {
            // Find all users with an active status
            var activeUsers = await _users.Find(u => u.Status == AccountStatus.Active).ToListAsync();

            foreach (var user in activeUsers)
            {
                // Check if a notification for account activation has already been sent
                var existingNotification = await _notifications.Find(n =>
                        n.RecipientId == user.Id.ToString() &&
                        n.Message == $"{user.Name}'s account has been approved and is now active" &&
                        n.Type == "AccountActivated")
                    .FirstOrDefaultAsync();

                // If no such notification exists, create and save a new one
                if (existingNotification == null)
                {
                    var notification = new Notification
                    {
                        RecipientId = user.Id.ToString(),
                        Role = "csr",
                        Message = $"{user.Name}'s account has been approved and is now active",
                        MessageID = user.Id.ToString(),
                        CreatedAt = DateTime.UtcNow,
                        Type = "AccountActivated",
                        IsRead = false
                    };

                    // Insert the notification into the database
                    await _notifications.InsertOneAsync(notification);
                    Console.WriteLine($"Notification sent for account activation of user {user.UserName}: {notification.Message}");
                }
                else
                {
                    Console.WriteLine($"Activation notification for user {user.UserName} already exists. Skipping.");
                }
            }
        }
    }
}
