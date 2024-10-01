/*
* This file contains the controller for user management.
*/
using System.Net;
using Backend.Dtos;
using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers.user_management;

[ApiController]
[Authorize(Roles = "admin")]
[Route("api/v1/user-management")]
[Produces("application/json")]
public class UserManagementController : ControllerBase
{
    private readonly ILogger<UserManagementController> _logger;
    private readonly UserManagementService _userManagementService;

    public UserManagementController(ILogger<UserManagementController> logger, UserManagementService userManagementService)
    {
        _logger = logger;
        _userManagementService = userManagementService;
    }


    [HttpPost("create-user", Name = "Create Account")]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(CreateUserResponse))]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest registerRequest)
    {
        try
        {
            var result = await _userManagementService.CreateUserAsync(registerRequest);

            return result.IsSuccess ? Ok(result) : BadRequest(result.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user");
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{userId}", Name = "Get User")]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(GetUserResponse))]
    public async Task<IActionResult> GetUser(string userId)
    {
        try
        {
            var result = await _userManagementService.GetUserAsync(userId);

            return result.IsSuccess ? Ok(result) : BadRequest(result.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user");
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{userId}", Name = "Update User")]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(UpdateUserResponse))]
    public async Task<IActionResult> UpdateUser(string userId, [FromBody] UpdateUserRequest updateRequest)
    {
        try
        {
            var result = await _userManagementService.UpdateUserAsync(userId, updateRequest);

            return result.IsSuccess ? Ok(result) : BadRequest(result.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user");
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{userId}", Name = "Delete User")]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(DeleteUserResponse))]
    public async Task<IActionResult> DeleteUser(string userId)
    {
        try
        {
            var result = await _userManagementService.DeleteUserAsync(userId);

            return result.IsSuccess ? Ok(result) : BadRequest(result.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user");
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("get-users/{role}", Name = "Get Users By Role")]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(GetUsersResponse))]
    public async Task<IActionResult> GetUsers(string role)
    {
        try
        {
            var result = await _userManagementService.GetUsersAsync(role);

            return result.IsSuccess ? Ok(result) : BadRequest(result.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting users");
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("update-user-status/{userId}", Name = "Update User Status")]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(UpdateUserStatusResponse))]
    public async Task<IActionResult> UpdateUserStatus(string userId, [FromBody] UpdateUserStatusRequest updateRequest)
    {
        try
        {
            var result = await _userManagementService.UpdateUserStatusAsync(userId, updateRequest);

            return result.IsSuccess ? Ok(result) : BadRequest(result.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user status");
            return BadRequest(ex.Message);
        }
    }
}