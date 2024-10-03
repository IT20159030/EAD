/*
* This file contains the controller for vendor management.
*/

using System.Net;
using Backend.Dtos;
using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Authorize(Roles = "admin")]
[Route("api/v1/vendor-management")]
[Produces("application/json")]
public class VendorManagementController : ControllerBase
{
  private readonly ILogger<VendorManagementController> _logger;
  private readonly VendorManagementService _vendorManagementService;

  public VendorManagementController(ILogger<VendorManagementController> logger, VendorManagementService vendorManagementService)
  {
    _logger = logger;
    _vendorManagementService = vendorManagementService;
  }

  [HttpGet("", Name = "Get all vendor")]
  [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(GetAllVendorResponse))]
  public async Task<IActionResult> GetAllVendor()
  {
    try
    {
      var result = await _vendorManagementService.GetAllVendorAsync();

      return result.IsSuccess ? Ok(result) : BadRequest(result);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error getting all vendor");
      return BadRequest(ex.Message);
    }
  }

  [HttpPost("", Name = "Create vendor")]
  [ProducesResponseType((int)HttpStatusCode.Created, Type = typeof(CreateVendorResponse))]
  public async Task<IActionResult> CreateVendor([FromBody] CreateVendorRequest request)
  {
    try
    {
      var result = await _vendorManagementService.CreateVendorAsync(request);

      return result.IsSuccess ? Ok(result) : BadRequest(result);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error creating vendor");
      return BadRequest(ex.Message);
    }
  }

  [HttpPut("{id}", Name = "Update vendor")]
  [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(UpdateVendorResponse))]
  public async Task<IActionResult> UpdateVendor([FromRoute] string id, [FromBody] UpdateVendorRequest request)
  {
    try
    {
      var result = await _vendorManagementService.UpdateVendorAsync(id, request);

      return result.IsSuccess ? Ok(result) : BadRequest(result);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error updating vendor");
      return BadRequest(ex.Message);
    }
  }

  [HttpDelete("{id}", Name = "Delete vendor")]
  [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(DeleteVendorResponse))]
  public async Task<IActionResult> DeleteVendor([FromRoute] string id)
  {
    try
    {
      var result = await _vendorManagementService.DeleteVendorAsync(id);

      return result.IsSuccess ? Ok(result) : BadRequest(result);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error deleting vendor");
      return BadRequest(ex.Message);
    }

  }

  [HttpPut("{id}/account-status", Name = "Update vendor account status")]
  [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(UpdateVendorAccountStatusResponse))]
  public async Task<IActionResult> UpdateVendorAccountStatus([FromRoute] string id, [FromBody] UpdateVendorAccountStatusRequest request)
  {
    try
    {
      var newStatus = Enum.Parse<AccountStatus>(request.Status);
      var result = await _vendorManagementService.UpdateVendorAccountStatusAsync(id, newStatus);

      return result.IsSuccess ? Ok(result) : BadRequest(result);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error updating vendor account status");
      return BadRequest(ex.Message);
    }
  }

}
