/**
* This controller is responsible for handling cancellation requests.
* It contains the CRUD operations for the CancellationRequest model.
*/

using System.Security.Claims;
using Backend.Dtos;
using Backend.Models;
using Backend.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/v1/cancellation-request")]
    [Authorize]
    public class CancellationRequestController : ControllerBase
    {
        private readonly IMongoCollection<CancellationRequest> _cancellationRequests;

        private readonly ILogger<CancellationRequestController> _logger;

        public CancellationRequestController(ILogger<CancellationRequestController> logger, MongoDBService mongoDBService)
        {
            _logger = logger;
            _cancellationRequests = mongoDBService.Database.GetCollection<CancellationRequest>("cancellationRequests");
        }

        // POST: api/v1/cancellation-request
        [HttpPost(Name = "CreateCancellationRequest")]
        public async Task<IActionResult> CreateCancellationRequest([FromBody] CreateCancellationRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var newRequest = new CancellationRequest
            {
                OrderId = dto.OrderId,
                CustomerId = dto.CustomerId,
                RequestDate = DateTime.UtcNow,
                Status = "Pending",
                Reason = dto.Reason,
                ProcessedBy = string.Empty,
                ProcessedDate = null,
                DecisionNote = null
            };

            await _cancellationRequests.InsertOneAsync(newRequest);
            return CreatedAtAction(nameof(GetById), new { id = newRequest.Id }, newRequest);
        }

        // GET: api/v1/cancellation-request/{id}
        [HttpGet("{id}", Name = "GetCancellationRequestById")]
        public async Task<IActionResult> GetById(string id)
        {
            var request = await _cancellationRequests.Find(x => x.Id == id).FirstOrDefaultAsync();

            if (request == null)
                return NotFound();

            return Ok(new CancellationRequestDto
            {
                Id = request.Id!,
                OrderId = request.OrderId,
                CustomerId = request.CustomerId,
                RequestDate = request.RequestDate,
                Status = request.Status,
                Reason = request.Reason,
                ProcessedBy = request.ProcessedBy,
                ProcessedDate = request.ProcessedDate,
                DecisionNote = request.DecisionNote
            });
        }

        // GET: api/v1/cancellation-request
        [HttpGet(Name = "GetAllRequests")]
        public async Task<IActionResult> GetAll()
        {
            var requests = await _cancellationRequests.Find(_ => true).ToListAsync();
            return Ok(requests);
        }

        // PUT: api/v1/cancellation-request/{id}
        [HttpPut("{id}", Name = "ProcessCancellationRequest")]
        [Authorize(Roles = "Admin, CSR")]
        public async Task<IActionResult> ProcessCancellationRequest(string id, [FromBody] ProcessCancellationRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updateDefinition = Builders<CancellationRequest>.Update
                .Set(r => r.Status, dto.Status)
                .Set(r => r.ProcessedBy, dto.ProcessedBy)
                .Set(r => r.ProcessedDate, DateTime.UtcNow)
                .Set(r => r.DecisionNote, dto.DecisionNote);

            var result = await _cancellationRequests.UpdateOneAsync(r => r.Id == id, updateDefinition);

            if (result.MatchedCount == 0)
                return NotFound();

            return NoContent();
        }

        // DELETE: api/v1/cancellation-request/{id}
        [HttpDelete("{id}", Name = "DeleteCancellationRequest")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCancellationRequest(string id)
        {
            var result = await _cancellationRequests.DeleteOneAsync(r => r.Id == id);

            if (result.DeletedCount == 0)
                return NotFound();

            return NoContent();
        }
    }
}
