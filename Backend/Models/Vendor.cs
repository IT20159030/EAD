using System.Runtime.InteropServices;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Backend.Models;

public class Vendor
{
    [BsonId]
    public Guid Id { get; set; }

    [BsonElement("VendorName")]
    public string VendorName { get; set; } = string.Empty;

    [BsonElement("VendorEmail")]
    public string VendorEmail { get; set; } = string.Empty;

    [BsonElement("VendorPhone")]
    public string VendorPhone { get; set; } = string.Empty;

    [BsonElement("VendorAddress")]
    public string VendorAddress { get; set; } = string.Empty;

    [BsonElement("VendorCity")]
    public string VendorCity { get; set; } = string.Empty;

    // cumulative rating of the vendor
    [BsonElement("VendorRating")]
    public required int VendorRating { get; set; }

    [BsonElement("VendorRatingCount")]
    public required int VendorRatingCount { get; set; }

    [BsonElement("Reviews")]
    public required List<Review> Reviews { get; set; }
}

public class Review
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("reviewerId")]
    public required string ReviewerId { get; set; }

    [BsonElement("reviewerName")]
    public required string ReviewerName { get; set; }

    [BsonElement("reviewRating")]
    public required int ReviewRating { get; set; }

    [BsonElement("reviewText")]
    public string? ReviewText { get; set; }
}