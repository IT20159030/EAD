using System.Runtime.InteropServices;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Backend.Models;

public class Vendor
{
    [BsonId]
    public Guid Id { get; set; }

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
    public float VendorRating { get; set; } = 0;

    [BsonElement("VendorRatingCount")]
    public int VendorRatingCount { get; set; } = 0;
}