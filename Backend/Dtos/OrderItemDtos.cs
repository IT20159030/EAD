/*
    This file contains the Data Transfer Objects (DTOs) for the OrderItem model.
    It is used to represent the order item object stored inside the order in the application.
    It contains the following properties:
        Id: the unique identifier of the order item.
        OrderItemId: the unique identifier of the order to which the order item belongs.
        ProductId: the unique identifier of the product associated with the order item.
        ProductName: the name of the product associated with the order item.
        Quantity: the quantity of the product in the order item.
        Price: the price of the product in the order item.
        Status: the status of the order item (Pending, Approved, Rejected, Completed).
*/

using System.ComponentModel.DataAnnotations;
using Backend.Models;

namespace Backend.Dtos;

public class OrderItemDto
{
    [Required]
    public string Id { get; set; } = string.Empty;
    [Required]
    public string OrderItemId { get; set; } = string.Empty;

    [Required]
    public string ProductId { get; set; } = string.Empty;

    [Required]
    public string ProductName { get; set; } = string.Empty;

    [Required]
    public int Quantity { get; set; }

    [Required]
    public decimal Price { get; set; }

    [Required]
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
}

public class CreateOrderItemRequestDto
{
    [Required]
    public string OrderItemId { get; set; } = string.Empty;

    [Required]
    public string ProductId { get; set; } = string.Empty;

    [Required]
    public string ProductName { get; set; } = string.Empty;

    [Required]
    public int Quantity { get; set; }

    [Required]
    public decimal Price { get; set; }

    [Required]
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
}

public class UpdateOrderItemRequestDto
{
    [Required]
    public string Id { get; set; } = string.Empty;

    [Required]
    public string OrderItemId { get; set; } = string.Empty;

    [Required]
    public string ProductId { get; set; } = string.Empty;

    [Required]
    public string ProductName { get; set; } = string.Empty;

    [Required]
    public int Quantity { get; set; }

    [Required]
    public decimal Price { get; set; }

    [Required]
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
}