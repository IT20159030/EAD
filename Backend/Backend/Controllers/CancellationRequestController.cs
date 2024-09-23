/**
* CancellationRequestController.cs
* This file contains the controller for the CancellationRequest model.
* It contains the CRUD operations for the CancellationRequest model.
*/

using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace Backend.Controllers;

[Route("api/cancellations")]
[ApiController]
public class CancellationRequestController : ControllerBase
{
    private readonly ILogger<CancellationRequestController> _logger;
    private readonly IMongoCollection<CancellationRequest> _cancellationRequests;

    public CancellationRequestController(ILogger<CancellationRequestController> logger, MongoDBService mongoDbService)
    {
        _logger = logger;
        _cancellationRequests = mongoDbService.Database?.GetCollection<CancellationRequest>("CANCELLATION_REQUESTS") ?? throw new InvalidOperationException("Cannot read MongoDB connection settings");
    }

    // GET /api/cancellations/requests
    [HttpGet("requests")]
    public async Task<ActionResult<IEnumerable<CancellationRequest>>> GetAllRequests()
    {
        var requests = await _cancellationRequests.Find(request => true).ToListAsync();
        return Ok(requests);
    }

    // PUT /api/cancellations/{orderId}/process
    [HttpPut("{orderId}/process")]
    public async Task<IActionResult> ProcessRequest(string orderId, [FromBody] CancellationRequest request)
    {
        var existingRequest = await _cancellationRequests.Find(r => r.OrderId == orderId).FirstOrDefaultAsync();

        if (existingRequest == null) return NotFound();

        existingRequest.Status = request.Status;
        existingRequest.ProcessedBy = request.ProcessedBy;
        existingRequest.ProcessedDate = DateTime.Now;
        existingRequest.DecisionNote = request.DecisionNote;

        await _cancellationRequests.ReplaceOneAsync(r => r.OrderId == orderId, existingRequest);

        return NoContent();
    }

    // POST /api/cancellations
    [HttpPost]
    public async Task<IActionResult> CreateCancellationRequest([FromBody] CancellationRequest request)
    {
        await _cancellationRequests.InsertOneAsync(request);
        return CreatedAtAction(nameof(GetAllRequests), request);
    }

    // DELETE /api/cancellations/{orderId}
    [HttpDelete("{orderId}")]
    public async Task<IActionResult> DeleteRequest(string orderId)
    {
        await _cancellationRequests.DeleteOneAsync(r => r.OrderId == orderId);
        return NoContent();
    }

    // GET /api/cancellations/{orderId}
    [HttpGet("{orderId}")]
    public async Task<ActionResult<CancellationRequest>> GetRequest(string orderId)
    {
        var request = await _cancellationRequests.Find(r => r.OrderId == orderId).FirstOrDefaultAsync();

        if (request == null) return NotFound();

        return Ok(request);
    }
}