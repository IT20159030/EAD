/*
* This file contains the controller for customer management.
*/

using System.Net;
using Backend.Dtos;
using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Authorize(Roles = "admin")]
[Route("api/v1/customer-management")]
[Produces("application/json")]
public class CustomerManagementController : ControllerBase
{
  private readonly ILogger<CustomerManagementController> _logger;
  private readonly CustomerManagementService _customerManagementService;

  public CustomerManagementController(ILogger<CustomerManagementController> logger, CustomerManagementService customerManagementService)
  {
    _logger = logger;
    _customerManagementService = customerManagementService;
  }

  [HttpGet("", Name = "Get all customer")]
  [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(GetAllCustomerResponse))]
  public async Task<IActionResult> GetAllCustomer()
  {
    try
    {
      var result = await _customerManagementService.GetAllCustomerAsync();

      return result.IsSuccess ? Ok(result) : BadRequest(result);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error getting all customer");
      return BadRequest(ex.Message);
    }
  }

  [HttpGet("{id}", Name = "Get customer by id")]
  [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(GetCustomerByIdResponse))]
  public async Task<IActionResult> GetCustomerById([FromRoute] string id)
  {
    try
    {
      var result = await _customerManagementService.GetCustomerByIdAsync(id);

      return result.IsSuccess ? Ok(result) : BadRequest(result);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error getting customer by id");
      return BadRequest(ex.Message);
    }
  }

  [HttpPost("", Name = "Create customer")]
  [ProducesResponseType((int)HttpStatusCode.Created, Type = typeof(CreateCustomerResponse))]
  public async Task<IActionResult> CreateCustomer([FromBody] CreateCustomerRequest request)
  {
    try
    {
      var result = await _customerManagementService.CreateCustomerAsync(request);

      return result.IsSuccess ? Ok(result) : BadRequest(result);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error creating customer");
      return BadRequest(ex.Message);
    }
  }

  [HttpPut("{id}", Name = "Update customer")]
  [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(UpdateCustomerResponse))]
  public async Task<IActionResult> UpdateCustomer([FromRoute] string id, [FromBody] UpdateCustomerRequest request)
  {
    try
    {
      var result = await _customerManagementService.UpdateCustomerAsync(id, request);

      return result.IsSuccess ? Ok(result) : BadRequest(result);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error updating customer");
      return BadRequest(ex.Message);
    }
  }

  [HttpDelete("{id}", Name = "Delete customer")]
  [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(DeleteCustomerResponse))]
  public async Task<IActionResult> DeleteCustomer([FromRoute] string id)
  {
    try
    {
      var result = await _customerManagementService.DeleteCustomerAsync(id);

      return result.IsSuccess ? Ok(result) : BadRequest(result);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error deleting customer");
      return BadRequest(ex.Message);
    }

  }

  [HttpPut("{id}/account-status", Name = "Update customer account status")]
  [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(UpdateCustomerAccountStatusResponse))]
  public async Task<IActionResult> UpdateCustomerAccountStatus([FromRoute] string id, [FromBody] UpdateCustomerAccountStatusRequest request)
  {
    try
    {
      var newStatus = Enum.Parse<AccountStatus>(request.Status);
      var result = await _customerManagementService.UpdateCustomerAccountStatusAsync(id, newStatus);

      return result.IsSuccess ? Ok(result) : BadRequest(result);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error updating customer account status");
      return BadRequest(ex.Message);
    }
  }


}
