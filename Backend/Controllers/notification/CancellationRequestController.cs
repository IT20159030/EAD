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
            _cancellationRequests = mongoDBService.Database.GetCollection<CancellationRequest>("CancellationRequests");
        }

        private CancellationRequestDto ConvertToDto(CancellationRequest request)
        {
            return new CancellationRequestDto
            {
                Id = request.Id ?? ObjectId.Empty.ToString(),
                OrderId = request.OrderId,
                CustomerId = request.CustomerId,
                RequestDate = request.RequestDate,
                Status = request.Status,
                Reason = request.Reason,
                ProcessedBy = request.ProcessedBy,
                ProcessedDate = request.ProcessedDate,
                DecisionNote = request.DecisionNote
            };
        }

        private CancellationRequest ConvertToModel(CreateCancellationRequestDto dto)
        {
            return new CancellationRequest
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

            return Ok(ConvertToDto(request));
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
        // [Authorize(Roles = "Admin, CSR")]
        public async Task<IActionResult> ProcessCancellationRequest(string id, [FromBody] ProcessCancellationRequestDto dto)
        {
            // if (!ModelState.IsValid)
            //     return BadRequest(ModelState);

            var result = await _cancellationRequests.FindOneAndUpdateAsync(
                r => r.Id == id,
                Builders<CancellationRequest>.Update
                    .Set(r => r.Status, dto.Status)
                    .Set(r => r.ProcessedBy, User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty)
                    .Set(r => r.ProcessedDate, DateTime.UtcNow)
                    .Set(r => r.DecisionNote, dto.DecisionNote)
            );

            if (result == null)
                return NotFound("No cancellation request found with the specified ID.");

            return Ok(result);
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

        // GET: api/v1/cancellation-request/order/{orderId}
        [HttpGet("order/{orderId}", Name = "GetRequestByOrderId")]
        // [Authorize(Roles = "Admin, CSR")]
        public async Task<IActionResult> GetRequestByOrderId(string orderId)
        {
            var request = await _cancellationRequests.Find(o => o.OrderId == orderId).FirstOrDefaultAsync();
            if (request == null)
                return NotFound("No cancellation request found for the specified order.");

            return Ok(request);
        }
    }
}
