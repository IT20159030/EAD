/*
* This class contains the Notification model which is used to store the notification details.
*/

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Backend.Models;

public class Notification
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("RecipientId")]
    public string? RecipientId { get; set; } // Vendor, CSR, Customer, or Admin ID

    [BsonElement("Role")]
    public string? Role { get; set; } // Vendor, CSR, Customer, or Admin

    [BsonElement("Message")]
    public string? Message { get; set; }

    [BsonElement("CreatedAt")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [BsonElement("Type")] // Inventory Alert, Order Status, Account Approval
    public string? Type { get; set; }

    [BsonElement("IsRead")]
    public bool IsRead { get; set; } = false;
}