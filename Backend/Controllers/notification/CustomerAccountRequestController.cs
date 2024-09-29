/**
* This is the Customer Account Requests Controller.
* It contains the CRUD operations for the CustomerAccountRequest model
* and is responsible for handling all requests related to Cancellation Requests.
* CreateCustomerAccountRequestDto, ProcessCustomerAccountRequestDto, and CustomerAccountRequestDto are used as Data Transfer Objects (DTOs).
*/

using System.Security.Claims;
using Backend.Dtos;
using Backend.Models;
using Backend.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Backend.Controllers.notification
{
    [ApiController]
    [Route("api/v1/customer-account-request")]
    [Authorize]
    public class CustomerAccountRequestController : ControllerBase
    {
        private readonly IMongoCollection<CustomerAccountRequest> _customerAccountRequests;
        private readonly ILogger<CustomerAccountRequestController> _logger;

        public CustomerAccountRequestController(ILogger<CustomerAccountRequestController> logger, MongoDBService mongoDBService)
        {
            _logger = logger;
            _customerAccountRequests = mongoDBService.Database.GetCollection<CustomerAccountRequest>("customerAccountRequests");
        }

        // POST: api/v1/customer-account-request
        [HttpPost(Name = "CreateCustomerAccountRequest")]
        public async Task<IActionResult> CreateCustomerAccountRequest([FromBody] CreateCustomerAccountRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var newRequest = new CustomerAccountRequest
            {
                CustomerId = dto.CustomerId,
                RequestDate = DateTime.UtcNow,
                Status = "Pending"
            };

            await _customerAccountRequests.InsertOneAsync(newRequest);
            return CreatedAtAction(nameof(GetById), new { id = newRequest.Id }, newRequest);
        }

        // GET: api/v1/customer-account-request/{id}
        [HttpGet("{id}", Name = "GetCustomerAccountRequestById")]
        public async Task<IActionResult> GetById(string id)

        {
            var request = await _customerAccountRequests.Find(x => x.Id == id).FirstOrDefaultAsync();

            if (request == null)
            {
                return NotFound();
            }
#pragma warning disable CS8601 // Possible null reference assignment.
            return Ok(new CustomerAccountRequestDto
            {
                Id = request.Id,
                CustomerId = request.CustomerId,
                RequestDate = request.RequestDate,
                Status = request.Status,
                ProcessedBy = request.ProcessedBy,
                ProcessedDate = request.ProcessedDate
            });
        }

        // PUT: api/v1/customer-account-request/{id}/process
        [HttpPut("{id}/process", Name = "ProcessCustomerAccountRequest")]
        public async Task<IActionResult> ProcessCustomerAccountRequest(string id, [FromBody] ProcessCustomerAccountRequestDto dto)
        {
            var request = await _customerAccountRequests.Find(x => x.Id == id).FirstOrDefaultAsync();

            if (request == null)
            {
                return NotFound();
            }

            if (request.Status != "Unapproved")
            {
                return BadRequest("Request has already been processed");
            }

            request.Status = dto.Status;
            request.ProcessedBy = dto.ProcessedBy;
            request.ProcessedDate = dto.ProcessedDate;

            await _customerAccountRequests.ReplaceOneAsync(x => x.Id == id, request);
            return Ok(request);
        }
    }
}