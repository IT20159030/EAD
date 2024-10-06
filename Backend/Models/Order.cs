using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

/*
    The model for an order and order item.
    An order is a collection of order items. Each order item is a product that the customer has ordered.
*/

namespace Backend.Models;

public class Order
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("orderId")]
    public string OrderId { get; set; } = string.Empty;

    [BsonElement("status")]
    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    [BsonElement("orderDate")]
    public DateTime OrderDate { get; set; }

    [BsonElement("deliveryAddress")]
    public string DeliveryAddress { get; set; } = string.Empty;

    [BsonElement("orderItems")]
    public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    [BsonElement("totalPrice")]
    public decimal TotalPrice { get; set; }

    [BsonElement("customerId")]
    public string CustomerId { get; set; } = string.Empty;

    [BsonElement("customerName")]
    public string? CustomerName { get; set; }
}

public class OrderItem
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("orderItemId")]
    public string OrderItemId { get; set; } = string.Empty;

    [BsonElement("productId")]
    public string ProductId { get; set; } = string.Empty;

    [BsonElement("productName")]
    public string ProductName { get; set; } = string.Empty;

    [BsonElement("quantity")]
    public int Quantity { get; set; }

    [BsonElement("price")]
    public decimal Price { get; set; }

    [BsonElement("status")]
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
}