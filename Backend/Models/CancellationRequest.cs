/*
* This class represents a cancellation request made by a customer.
*/

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Backend.Models;

public class CancellationRequest
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("OrderId")]
    public required string OrderId { get; set; }

    [BsonElement("CustomerId")]
    public required string CustomerId { get; set; }

    [BsonElement("RequestDate")]
    public DateTime RequestDate { get; set; }

    [BsonElement("Status")] // Pending, Approved, Denied
    public required string Status { get; set; } = "Pending";

    [BsonElement("Reason")]
    public required string Reason { get; set; }

    [BsonElement("ProcessedBy")]
    public required string ProcessedBy { get; set; } // CSR or Admin ID

    [BsonElement("ProcessedDate")]
    public DateTime? ProcessedDate { get; set; }

    [BsonElement("DecisionNote")]
    public string? DecisionNote { get; set; } // Reason for approval/denial
}