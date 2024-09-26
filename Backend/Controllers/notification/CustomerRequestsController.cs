using Backend.Dtos;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers.notification
{
    [Route("api/v1/customers")]
    [ApiController]
    public class CustomerRequestsController : ControllerBase
    {
        private readonly ILogger<CustomerRequestsController> _logger;
        private readonly ICustomerAccountRequestService _customerAccountRequestService;

        public CustomerRequestsController(ILogger<CustomerRequestsController> logger, ICustomerAccountRequestService customerAccountRequestService)
        {
            _logger = logger;
            _customerAccountRequestService = customerAccountRequestService;
        }

        // GET /api/v1/customers/requests
        [HttpGet("requests")]
        public async Task<ActionResult<IEnumerable<CustomerAccountRequestDto>>> GetAllRequests()
        {
            var requests = await _customerAccountRequestService.GetAllRequestsAsync();
            return Ok(requests);
        }

        // POST /api/v1/customers/requests/create
        [HttpPost("requests/create")]
        public async Task<ActionResult<CustomerAccountRequestDto>> CreateRequest([FromBody] CreateCustomerAccountRequestDto createRequestDto)
        {
            try
            {
                var newRequest = await _customerAccountRequestService.CreateRequestAsync(createRequestDto);
                return CreatedAtAction(nameof(GetAllRequests), new { id = newRequest.Id }, newRequest);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating customer account request");
                return BadRequest(ex.Message);
            }
        }

        // PUT /api/v1/customers/{customerId}/process
        [HttpPut("{customerId}/process")]
        public async Task<IActionResult> ProcessRequest(string customerId, [FromBody] ProcessCustomerAccountRequestDto processRequestDto)
        {
            try
            {
                await _customerAccountRequestService.ProcessRequestAsync(customerId, processRequestDto);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing customer account request for CustomerId {customerId}");
                return BadRequest(ex.Message);
            }
        }
    }
}
