using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Backend.Models;

public class Inventory
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("productId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public required string ProductId { get; set; }

    [BsonElement("quantity")]
    public required int Quantity { get; set; }

    [BsonElement("alertThreshold")]
    public int AlertThreshold { get; set; } = 10;

    [BsonElement("lowStockAlert")]
    public bool LowStockAlert { get; set; } = false;
}
