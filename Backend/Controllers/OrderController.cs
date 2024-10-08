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
    private readonly IMongoCollection<Product> _products;
    private readonly ILogger<OrderController> _logger;

    // Inject the logger and MongoDB service into the controller
    public OrderController(ILogger<OrderController> logger, MongoDBService mongoDBService)
    {
        _logger = logger;
        _orders = mongoDBService.Database.GetCollection<Order>("Orders");
        _cancellationRequests = mongoDBService.Database.GetCollection<CancellationRequest>("CancellationRequests");
        _products = mongoDBService.Database.GetCollection<Product>("Products");
    }

    // Convert Order model to OrderDto
    private OrderDto ConvertToDto(Order order) => new OrderDto
    {
        Id = order.Id!,
        OrderId = order.OrderId,
        Status = order.Status,
        OrderDate = order.OrderDate,
        DeliveryAddress = order.DeliveryAddress,
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

    // Convert CreateOrderRequestDto to Order model
    private Order ConvertToModel(CreateOrderRequestDto dto) => new Order
    {
        Id = ObjectId.GenerateNewId().ToString(),
        OrderId = dto.OrderId,
        Status = dto.Status,
        OrderDate = dto.OrderDate,
        DeliveryAddress = dto.DeliveryAddress,
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
        TotalPrice = dto.TotalPrice,
        CustomerId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty,
        CustomerName = User.FindFirstValue(ClaimTypes.Email) ?? string.Empty
    };

    // Convert UpdateOrderRequestDto to Order model
    private Order ConvertToModel(UpdateOrderRequestDto dto) => new Order
    {
        Id = dto.Id,
        OrderId = dto.OrderId,
        Status = dto.Status,
        OrderDate = dto.OrderDate,
        DeliveryAddress = dto.DeliveryAddress,
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
        TotalPrice = dto.TotalPrice,
        CustomerId = dto.CustomerId,
        CustomerName = dto.CustomerName ?? string.Empty
    };

    // Convert CreateCancellationRequestDto to CancellationRequest model
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

    // POST: api/v1/Order
    [HttpPost]
    [Authorize(Roles = "customer")]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequestDto request)
    {
        var order = ConvertToModel(request);

        if (order.CustomerId.IsNullOrEmpty())
        {
            return BadRequest("User not found");
        }

        // update product stock
        foreach (var item in order.OrderItems)
        {
            var product = await _products.Find(p => p.Id == item.ProductId).FirstOrDefaultAsync();
            if (product == null)
            {
                return BadRequest("Product not found");
            }

            if (product.Stock < item.Quantity)
            {
                return BadRequest("Insufficient stock");
            }

            product.Stock -= item.Quantity;
            await _products.ReplaceOneAsync(p => p.Id == item.ProductId, product);
        }

        await _orders.InsertOneAsync(order);
        return CreatedAtRoute("GetOrder", new { id = order.Id }, ConvertToDto(order));
    }

    // GET: api/v1/Order
    [HttpGet]
    public async Task<IEnumerable<OrderDto>> GetAllOrders()
    {
        var orders = await _orders.Find(_ => true).ToListAsync();
        return orders.Select(ConvertToDto);
    }

    // POST: api/v1/Order/CancelRequest/{orderId}
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

    // GET: api/v1/Order/cancel/{id}
    [HttpPut("cancel/{id}")]
    [Authorize(Roles = "admin,vendor,csr")]
    public async Task<IActionResult> CancelOrder(string id)
    {
        var order = await _orders.Find(o => o.Id == id).FirstOrDefaultAsync();
        if (order == null)
        {
            return NotFound();
        }

        // update product stock
        foreach (var item in order.OrderItems)
        {
            var product = await _products.Find(p => p.Id == item.ProductId).FirstOrDefaultAsync();
            if (product == null)
            {
                return BadRequest("Product not found");
            }

            product.Stock += item.Quantity;
            await _products.ReplaceOneAsync(p => p.Id == item.ProductId, product);
        }

        order.Status = OrderStatus.Cancelled;
        await _orders.ReplaceOneAsync(o => o.Id == id, order);
        return Ok(ConvertToDto(order));
    }

    // GET: api/v1/Order/{id}
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

    // GET: api/v1/Order/GetByCustomerId
    [HttpGet("GetByCustomerId")]
    [Authorize(Roles = "customer")]
    public async Task<IEnumerable<OrderDto>> GetOrdersByCustomerId()
    {
        var customerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (customerId.IsNullOrEmpty())
        {
            return Enumerable.Empty<OrderDto>();
        }

        var orders = await _orders.Find(o => o.CustomerId == customerId).ToListAsync();
        return orders.Select(ConvertToDto);
    }

    // PUT: api/v1/Order/{id}
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

    // DELETE: api/v1/Order/{id}
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

    // PUT: api/v1/Order/ready/{orderId}
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

    // PUT: api/v1/Order/delivered/{orderId}
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

    // PUT: api/v1/Order/reject/{orderId}
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

    // GET: api/v1/Order/GetByVendorId/{vendorId}
    [HttpGet("GetByVendorId/{vendorId}")]
    [Authorize(Roles = "vendor")]
    public async Task<IEnumerable<OrderDto>> GetOrdersByVendorId(string vendorId)
    {
        // get product list of vendor
        var products = await _products.Find(p => p.VendorId == vendorId).ToListAsync();
        if (!products.Any())
        {
            return Enumerable.Empty<OrderDto>();
        }

        var productIds = products.Select(p => p.Id).ToList();

        // Use MongoDB filter to match OrderItems by ProductId
        var filter = Builders<Order>.Filter.ElemMatch(o => o.OrderItems, oi => productIds.Contains(oi.ProductId));

        // Find orders with products of the vendor
        var orders = await _orders.Find(filter).ToListAsync();

        // only return orders relevant to the vendor
        var filteredOrders = orders.Select(order => new Order
        {
            Id = order.Id,
            OrderId = order.OrderId,
            Status = order.Status,
            OrderDate = order.OrderDate,
            DeliveryAddress = order.DeliveryAddress,
            CustomerId = order.CustomerId,
            CustomerName = order.CustomerName,
            TotalPrice = order.TotalPrice,
            // Filter the OrderItems to include only items with a ProductId from the vendor's products
            OrderItems = order.OrderItems.Where(oi => productIds.Contains(oi.ProductId)).ToList()
        });
        return filteredOrders.Select(ConvertToDto);
    }
}