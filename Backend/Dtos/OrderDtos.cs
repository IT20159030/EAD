/*
    This file contains the Data Transfer Objects (DTOs) for the Order model.
    It is used to represent the order object in the application.
    It contains the following properties:
        Id: the unique identifier of the order.
        OrderId: the unique identifier of the order.
        Status: the status of the order (Pending, Approved, Rejected, Completed).
        OrderDate: the date of the order.
        OrderItems: the list of order items associated with the order.
        TotalPrice: the total price of the order.
        CustomerId: the unique identifier of the customer who placed the order.
        CustomerName: the name of the customer who placed the order.
*/

using System.ComponentModel.DataAnnotations;
using Backend.Models;

namespace Backend.Dtos;

public class OrderDto
{
    [Required]
    public string Id { get; set; } = string.Empty;

    [Required]
    public string OrderId { get; set; } = string.Empty;

    [Required]
    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    [Required]
    public DateTime OrderDate { get; set; }

    [required]
    public List<OrderItemDto> OrderItems { get; set; } = new List<OrderItemDto>();

    [Required]
    public decimal TotalPrice { get; set; }

    [Required]
    public string CustomerId { get; set; } = string.Empty;

    public string? CustomerName { get; set; }
}

public class CreateOrderRequestDto
{
    [Required]
    public string OrderId { get; set; } = string.Empty;

    [Required]
    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    [Required]
    public DateTime OrderDate { get; set; }

    [required]
    public List<CreateOrderItemRequestDto> OrderItems { get; set; } = new List<CreateOrderItemRequestDto>();

    [Required]
    public decimal TotalPrice { get; set; }

    [Required]
    public string CustomerId { get; set; } = string.Empty;
}

public class UpdateOrderRequestDto
{
    [Required]
    public string Id { get; set; } = string.Empty;

    [Required]
    public string OrderId { get; set; } = string.Empty;

    [Required]
    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    [Required]
    public DateTime OrderDate { get; set; }

    [required]
    public List<UpdateOrderItemRequestDto> OrderItems { get; set; } = new List<UpdateOrderItemRequestDto>();

    [Required]
    public decimal TotalPrice { get; set; }

    [Required]
    public string CustomerId { get; set; } = string.Empty;
}