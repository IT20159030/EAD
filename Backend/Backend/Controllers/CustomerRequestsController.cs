/**
* CustomerRequestsController.cs
* This file contains the controller for the CustomerRequest model.
* It contains the CRUD operations for the CustomerRequest model.
*/

using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace Backend.Controllers;


[Route("api/customers")]
[ApiController]
public class CustomerRequestsController : ControllerBase
{
    private readonly ILogger<CustomerRequestsController> _logger;
    private readonly IMongoCollection<CustomerAccountRequest> _customerRequests;

    public CustomerRequestsController(ILogger<CustomerRequestsController> logger, MongoDBService mongoDbService)
    {
        _logger = logger;
        _customerRequests = mongoDbService.Database?.GetCollection<CustomerAccountRequest>("CUSTOMER_ACCOUNT_REQUESTS") ?? throw new InvalidOperationException("Cannot read MongoDB connection settings");
    }

    // GET /api/customers/requests
    [HttpGet("requests")]
    public async Task<ActionResult<IEnumerable<CustomerAccountRequest>>> GetAllRequests()
    {
        var requests = await _customerRequests.Find(request => true).ToListAsync();
        return Ok(requests);
    }

    // PUT /api/customers/{customerId}/process
    [HttpPut("{customerId}/process")]
    public async Task<IActionResult> ProcessRequest(string customerId, [FromBody] CustomerAccountRequest request)
    {
        var existingRequest = await _customerRequests.Find(r => r.CustomerId == customerId).FirstOrDefaultAsync();

        if (existingRequest == null) return NotFound();

        existingRequest.Status = request.Status;
        existingRequest.ProcessedBy = request.ProcessedBy;
        existingRequest.ProcessedDate = DateTime.Now;

        await _customerRequests.ReplaceOneAsync(r => r.CustomerId == customerId, existingRequest);

        return NoContent();
    }
}