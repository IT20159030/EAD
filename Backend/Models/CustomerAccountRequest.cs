/*
* This class represents a customer account request made by a customer.
*/

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Backend.Models;
public class CustomerAccountRequest
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("CustomerId")]
    public required string CustomerId { get; set; }

    [BsonElement("RequestDate")]
    public DateTime RequestDate { get; set; }

    [BsonElement("Status")] // Unapproved, Active, Rejected, Deactivated
    public string? Status { get; set; }

    [BsonElement("ProcessedBy")]
    public string? ProcessedBy { get; set; }

    [BsonElement("ProcessedDate")]
    public DateTime? ProcessedDate { get; set; }
}
