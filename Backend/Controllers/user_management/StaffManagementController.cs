using System.Net;
using Backend.Dtos;
using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

/*
*  Controller for staff management
* Only admin can access this controller
* Functionalities include 
* - Get all staff
* - Create staff
* - Update staff
* - Delete staff
* - Update staff account status
*/

[ApiController]
[Authorize(Roles = "admin")]
[Route("api/v1/staff-management")]
[Produces("application/json")]
public class StaffManagementController : ControllerBase
{
  private readonly ILogger<StaffManagementController> _logger;
  private readonly StaffManagementService _staffManagementService;

  public StaffManagementController(ILogger<StaffManagementController> logger, StaffManagementService staffManagementService)
  {
    _logger = logger;
    _staffManagementService = staffManagementService;
  }

  [HttpGet("", Name = "Get all staff")]
  [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(GetAllStaffResponse))]
  public async Task<IActionResult> GetAllStaff()
  {
    try
    {
      var result = await _staffManagementService.GetAllStaffAsync();

      return result.IsSuccess ? Ok(result) : BadRequest(result);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error getting all staff");
      return BadRequest(ex.Message);
    }
  }

  [HttpPost("", Name = "Create staff")]
  [ProducesResponseType((int)HttpStatusCode.Created, Type = typeof(CreateStaffResponse))]
  public async Task<IActionResult> CreateStaff([FromBody] CreateStaffRequest request)
  {
    try
    {
      var result = await _staffManagementService.CreateStaffAsync(request);

      return result.IsSuccess ? Ok(result) : BadRequest(result);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error creating staff");
      return BadRequest(ex.Message);
    }
  }

  [HttpPut("{id}", Name = "Update staff")]
  [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(UpdateStaffResponse))]
  public async Task<IActionResult> UpdateStaff([FromRoute] string id, [FromBody] UpdateStaffRequest request)
  {
    try
    {
      var result = await _staffManagementService.UpdateStaffAsync(id, request);

      return result.IsSuccess ? Ok(result) : BadRequest(result);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error updating staff");
      return BadRequest(ex.Message);
    }
  }

  [HttpDelete("{id}", Name = "Delete staff")]
  [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(DeleteStaffResponse))]
  public async Task<IActionResult> DeleteStaff([FromRoute] string id)
  {
    try
    {
      var result = await _staffManagementService.DeleteStaffAsync(id);

      return result.IsSuccess ? Ok(result) : BadRequest(result);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error deleting staff");
      return BadRequest(ex.Message);
    }

  }

  [HttpPut("{id}/account-status", Name = "Update staff account status")]
  [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(UpdateStaffAccountStatusResponse))]
  public async Task<IActionResult> UpdateStaffAccountStatus([FromRoute] string id, [FromBody] UpdateStaffAccountStatusRequest request)
  {
    try
    {
      var newStatus = Enum.Parse<AccountStatus>(request.Status);
      var result = await _staffManagementService.UpdateStaffAccountStatusAsync(id, newStatus);

      return result.IsSuccess ? Ok(result) : BadRequest(result);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error updating staff account status");
      return BadRequest(ex.Message);
    }
  }


}
