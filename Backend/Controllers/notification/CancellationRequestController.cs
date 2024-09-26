/**
* CancellationRequestController.cs
* This file contains the controller for the CancellationRequest model.
* It contains the CRUD operations for the CancellationRequest model.
*/

using System.Net;
using Backend.Dtos;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers.notification;

[ApiController]
[Route("api/v1/cancellations")]
[Produces("application/json")]
public class CancellationRequestController : ControllerBase
{
  private readonly ILogger<CancellationRequestController> _logger;
  private readonly CancellationRequestService _cancellationRequestService;

  public CancellationRequestController(ILogger<CancellationRequestController> logger, CancellationRequestService cancellationRequestService)
  {
    _logger = logger;
    _cancellationRequestService = cancellationRequestService;
  }

  [HttpGet("requests", Name = "Get All Cancellation Requests")]
  [ProducesResponseType(typeof(IEnumerable<CancellationRequestDto>), (int)HttpStatusCode.OK)]
  public async Task<ActionResult<IEnumerable<CancellationRequestDto>>> GetAllRequests()
  {
    var requests = await _cancellationRequestService.GetAllRequestsAsync();
    return Ok(requests);
  }

  [HttpPost("requests/create", Name = "Create Cancellation Request")]
  [ProducesResponseType(typeof(CancellationRequestDto), (int)HttpStatusCode.Created)]
  [ProducesResponseType((int)HttpStatusCode.BadRequest)]
  public async Task<ActionResult<CancellationRequestDto>> CreateRequest([FromBody] CreateCancellationRequest createRequest)
  {
    try
    {
      var newRequest = await _cancellationRequestService.CreateRequestAsync(createRequest);
      return CreatedAtAction(nameof(GetAllRequests), new { id = newRequest.Id }, newRequest);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error creating cancellation request");
      return BadRequest(ex.Message);
    }

  }

  [HttpPut("requests/{orderId}/process", Name = "Process Cancellation Request")]
  [ProducesResponseType((int)HttpStatusCode.NoContent)]
  [ProducesResponseType((int)HttpStatusCode.NotFound)]
  public async Task<ActionResult> ProcessRequest(string orderId, [FromBody] ProcessCancellationRequest processRequest)
  {
    try
    {
      await _cancellationRequestService.ProcessRequestAsync(orderId, processRequest);
      return NoContent();
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error processing cancellation request");
      return NotFound(ex.Message);
    }
  }
}