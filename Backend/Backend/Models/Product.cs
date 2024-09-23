using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Backend.Models;

public class Product
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("name")]
    public required string Name { get; set; }

    [BsonElement("category")]
    [BsonRepresentation(BsonType.ObjectId)]
    public required string Category { get; set; }

    [BsonElement("description")]
    public required string Description { get; set; }

    [BsonElement("price")]
    [BsonRepresentation(BsonType.Decimal128)]
    public required decimal Price { get; set; }

    [BsonElement("isActive")]
    public bool IsActive { get; set; } = false;

    [BsonElement("vendorId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public required string VendorId { get; set; }
}
