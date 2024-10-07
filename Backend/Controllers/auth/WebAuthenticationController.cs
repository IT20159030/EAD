using System.Net;
using System.Security.Claims;
using Backend.Dtos;
using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Authorize(Roles = "admin, vendor, csr")]
[Route("api/v1/web-auth")]
/**
* WebAuthenticationController.cs provides the API endpoints for user authentication for the web application.
*/
public class WebAuthenticationController : ControllerBase
{
  private readonly ILogger<WebAuthenticationController> _logger;
  private readonly WebUserAuthService _userAuthService;

  public WebAuthenticationController(ILogger<WebAuthenticationController> logger, WebUserAuthService userAuthService)
  {
    _logger = logger;
    _userAuthService = userAuthService;
  }

  [HttpPost("login", Name = "Login")]
  [AllowAnonymous]
  [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(LoginResponse))]
  public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
  {
    try
    {
      var result = await _userAuthService.LoginAsync(loginRequest.Email, loginRequest.Password);

      return result.IsSuccess ? Ok(result) : BadRequest(result.Message);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error logging in user");
      return BadRequest(ex.Message);
    }
  }

  [HttpGet("profile", Name = "Get Profile")]
  [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ProfileResponse))]
  public async Task<IActionResult> GetProfile()
  {
    try
    {
      var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
      if (string.IsNullOrEmpty(userId))
      {
        return BadRequest("User not found");
      }
      var result = await _userAuthService.GetProfileAsync(userId);

      return result.IsSuccess ? Ok(result) : BadRequest(result.Message);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error getting user profile");
      return BadRequest(ex.Message);
    }
  }

  [HttpPut("profile", Name = "Update Profile")]
  [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ProfileResponse))]
  public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest updateProfileRequest)
  {
    try
    {
      var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
      if (string.IsNullOrEmpty(userId))
      {
        return BadRequest("User not found");
      }
      var result = await _userAuthService.UpdateProfileAsync(userId, updateProfileRequest);

      return result.IsSuccess ? Ok(result) : BadRequest(result.Message);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error updating user profile");
      return BadRequest(ex.Message);
    }
  }



}