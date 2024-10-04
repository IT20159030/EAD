using System.Runtime.InteropServices;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Backend.Models;

public class Vendor
{
    [BsonId]
    public Guid Id { get; set; }

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