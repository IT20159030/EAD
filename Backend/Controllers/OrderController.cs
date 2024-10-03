/*
* This controller is responsible for handling all the order related routes.
* It contains the following routes:
*   [customer] 
*   1. POST /api/v1/Order: Create a new order.
*   2. GET /api/v1/Order: Get all orders.
*   3. POST /api/v1/Order/cancel/{orderId}: Create a cancel request for an order.
*   4. GET /api/v1/Order/{id}: Get an order by id.
*   5. GET /api/v1/Order/GetByCustomerId/{customerId}: Get all orders by customer id.
*   [vendor, csr, admin]
*   6. PUT /api/v1/Order/{id}: Update an order by id.
*   7. DELETE /api/v1/Order/{id}: Delete an order by id.
*   8. PUT /api/v1/Order/ready/{orderId}: Mark an order as ready for pickup.
*   9. PUT /api/v1/Order/delivered/{orderId}: Mark an order as completed.
*   10. PUT /api/v1/Order/reject/{orderId}: Mark an order as rejected.
*   11. GET /api/v1/Order/GetByVendorId/{vendorId}: Get all orders by vendor id.
*/

// TODO: @Navod&Dinushka: When calling order cancellation, the order status should be updated to CancelRequested.
// TODO: Call both APIs from frontend to update the order status and create a cancellation request.
using System.Security.Claims;
using Backend.Dtos;
using Backend.Models;
using Backend.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Backend.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize(Roles = "admin,vendor,csr,customer")]
public class OrderController : ControllerBase
{
    private readonly IMongoCollection<Order> _orders;
    private readonly IMongoCollection<CancellationRequest> _cancellationRequests;
    private readonly ILogger<OrderController> _logger;

    public OrderController(ILogger<OrderController> logger, MongoDBService mongoDBService)
    {
        _logger = logger;
        _orders = mongoDBService.Database.GetCollection<Order>("Orders");
        _cancellationRequests = mongoDBService.Database.GetCollection<CancellationRequest>("CancellationRequests");
    }

    private OrderDto ConvertToDto(Order order) => new OrderDto
    {
        Id = order.Id!,
        OrderId = order.OrderId,
        Status = order.Status,
        OrderDate = order.OrderDate,
        OrderItems = order.OrderItems.Select(item => new OrderItemDto
        {
            Id = item.Id!,
            OrderItemId = item.OrderItemId,
            ProductId = item.ProductId,
            ProductName = item.ProductName,
            Quantity = item.Quantity,
            Price = item.Price,
            Status = item.Status
        }).ToList(),
        TotalPrice = order.TotalPrice,
        CustomerId = order.CustomerId,
        CustomerName = order.CustomerName ?? string.Empty
    };

    private Order ConvertToModel(CreateOrderRequestDto dto) => new Order
    {
        Id = ObjectId.GenerateNewId().ToString(),
        OrderId = dto.OrderId,
        Status = dto.Status,
        OrderDate = dto.OrderDate,
        OrderItems = dto.OrderItems.Select(item => new OrderItem
        {
            Id = ObjectId.GenerateNewId().ToString(),
            OrderItemId = dto.OrderId,
            ProductId = item.ProductId,
            ProductName = item.ProductName,
            Quantity = item.Quantity,
            Price = item.Price,
            Status = item.Status
        }).ToList(),
        TotalPrice = dto.OrderItems.Sum(item => item.Price * item.Quantity),
        CustomerId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty,
        CustomerName = User.FindFirstValue(ClaimTypes.Email) ?? string.Empty
    };

    private Order ConvertToModel(UpdateOrderRequestDto dto) => new Order
    {
        Id = dto.Id,
        OrderId = dto.OrderId,
        Status = dto.Status,
        OrderDate = dto.OrderDate,
        OrderItems = dto.OrderItems.Select(item => new OrderItem
        {
            Id = item.Id,
            OrderItemId = dto.OrderId,
            ProductId = item.ProductId,
            ProductName = item.ProductName,
            Quantity = item.Quantity,
            Price = item.Price,
            Status = item.Status
        }).ToList(),
        TotalPrice = dto.OrderItems.Sum(item => item.Price * item.Quantity),
        CustomerId = dto.CustomerId,
        CustomerName = dto.CustomerName ?? string.Empty
    };

    private CancellationRequest ConvertToModel(CreateCancellationRequestDto dto) => new CancellationRequest
    {
        Id = ObjectId.GenerateNewId().ToString(),
        OrderId = dto.OrderId,
        ProcessedBy = "Unprocessed",
        CustomerId = dto.CustomerId ?? string.Empty,
        RequestDate = DateTime.Now,
        Status = "Pending",
        Reason = dto.Reason
    };

    [HttpPost]
    [Authorize(Roles = "customer")]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequestDto request)
    {
        var order = ConvertToModel(request);

        if (order.CustomerId.IsNullOrEmpty())
        {
            return BadRequest("User not found");
        }

        await _orders.InsertOneAsync(order);
        return CreatedAtRoute("GetOrder", new { id = order.Id }, ConvertToDto(order));
    }

    [HttpGet]
    public async Task<IEnumerable<OrderDto>> GetAllOrders()
    {
        var customerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (customerId.IsNullOrEmpty())
        {
            return Enumerable.Empty<OrderDto>();
        }

        var orders = await _orders.Find(o => o.CustomerId == customerId).ToListAsync();
        return orders.Select(ConvertToDto);
    }

    [HttpPost("CancelRequest/{orderId}")]
    [Authorize(Roles = "customer")]
    public async Task<IActionResult> CancelRequestOrder(string orderId, [FromBody] CreateCancellationRequestDto request)
    {
        var order = await _orders.Find(o => o.Id == orderId).FirstOrDefaultAsync();
        var customerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (order == null)
        {
            return NotFound();
        }
        else if (customerId == null || customerId.IsNullOrEmpty())
        {
            return BadRequest("User not found");
        }
        else if (order.Status != OrderStatus.Pending)
        {
            return BadRequest("Order cannot be cancelled");
        }

        request.CustomerId = customerId;
        request.OrderId = orderId;

        var cancellationRequest = ConvertToModel(request);

        await _cancellationRequests.InsertOneAsync(cancellationRequest);

        order.Status = OrderStatus.CancelRequested;
        await _orders.ReplaceOneAsync(o => o.Id == orderId, order);
        return Ok(ConvertToDto(order));
    }

    [HttpPut("cancel/{id}")]
    [Authorize(Roles = "admin,vendor,csr")]
    public async Task<IActionResult> CancelOrder(string id)
    {
        var order = await _orders.Find(o => o.Id == id).FirstOrDefaultAsync();
        if (order == null)
        {
            return NotFound();
        }

        order.Status = OrderStatus.Cancelled;
        await _orders.ReplaceOneAsync(o => o.Id == id, order);
        return Ok(ConvertToDto(order));
    }

    [HttpGet("{id}", Name = "GetOrder")]
    public async Task<IActionResult> GetOrder(string id)
    {
        var order = await _orders.Find(o => o.Id == id).FirstOrDefaultAsync();
        if (order == null)
        {
            return NotFound();
        }

        return Ok(ConvertToDto(order));
    }

    [HttpGet("GetByCustomerId/{customerId}")]
    [Authorize(Roles = "customer")]
    public async Task<IEnumerable<OrderDto>> GetOrdersByCustomerId(string customerId)
    {
        var orders = await _orders.Find(o => o.CustomerId == customerId).ToListAsync();
        return orders.Select(ConvertToDto);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "admin,vendor,csr,customer")]
    public async Task<IActionResult> UpdateOrder(string id, [FromBody] UpdateOrderRequestDto request)
    {
        var order = ConvertToModel(request);
        var result = await _orders.ReplaceOneAsync(o => o.Id == id, order);
        if (result.MatchedCount == 0)
        {
            return NotFound();
        }

        return Ok(ConvertToDto(order));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "admin,vendor,csr")]
    public async Task<IActionResult> DeleteOrder(string id)
    {
        var result = await _orders.DeleteOneAsync(o => o.Id == id);
        if (result.DeletedCount == 0)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpPut("ready/{orderId}")]
    [Authorize(Roles = "vendor,csr,admin")]
    public async Task<IActionResult> MarkOrderAsReady(string orderId)
    {
        var order = await _orders.Find(o => o.Id == orderId).FirstOrDefaultAsync();
        if (order == null)
        {
            return NotFound();
        }

        order.Status = OrderStatus.Ready;
        await _orders.ReplaceOneAsync(o => o.Id == orderId, order);
        return Ok(ConvertToDto(order));
    }

    [HttpPut("delivered/{orderId}")]
    [Authorize(Roles = "vendor,csr,admin")]
    public async Task<IActionResult> MarkOrderAsDelivered(string orderId)
    {
        var order = await _orders.Find(o => o.Id == orderId).FirstOrDefaultAsync();
        if (order == null)
        {
            return NotFound();
        }

        order.Status = OrderStatus.Completed;
        await _orders.ReplaceOneAsync(o => o.Id == orderId, order);
        return Ok(ConvertToDto(order));
    }

    [HttpPut("reject/{orderId}")]
    [Authorize(Roles = "vendor,csr,admin")]
    public async Task<IActionResult> RejectOrder(string orderId)
    {
        var order = await _orders.Find(o => o.Id == orderId).FirstOrDefaultAsync();
        if (order == null)
        {
            return NotFound();
        }

        order.Status = OrderStatus.Rejected;
        await _orders.ReplaceOneAsync(o => o.Id == orderId, order);
        return Ok(ConvertToDto(order));
    }

    [HttpGet("GetByVendorId/{vendorId}")]
    [Authorize(Roles = "vendor")]
    public async Task<IEnumerable<OrderDto>> GetOrdersByVendorId(string vendorId)
    {
        var orders = await _orders.Find(o => o.OrderItems.Any(oi => oi.ProductId == vendorId)).ToListAsync();
        return orders.Select(ConvertToDto);
    }
}