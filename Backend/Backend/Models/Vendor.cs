using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Backend.Models;

public class Vendor
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("vendorId")]
    public required string VendorId { get; set; }

    [BsonElement("VendorName")]
    public required string VendorName { get; set; }

    [BsonElement("VendorEmail")]
    public required string VendorEmail { get; set; }

    [BsonElement("VendorPhone")]
    public required string VendorPhone { get; set; }

    [BsonElement("VendorAddress")]
    public required string VendorAddress { get; set; }

    [BsonElement("VendorCity")]
    public required string VendorCity { get; set; }

    [BsonElement("VendorRating")]
    public required float VendorRating { get; set; }

    [BsonElement("VendorRatingCount")]
    public required int VendorRatingCount { get; set; }
}