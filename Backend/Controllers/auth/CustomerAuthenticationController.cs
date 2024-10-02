/**
* CustomerAuthenticationController.cs provides the endpoints for the customer authentication for the mobile application.
*/

using System.Net;
using System.Security.Claims;
using Backend.Dtos;
using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Authorize(Roles = "customer")]
[Route("api/v1/customer-auth")]
[Produces("application/json")]
public class CustomerAuthenticationController : ControllerBase
{
  private readonly ILogger<CustomerAuthenticationController> _logger;
  private readonly IConfiguration _configuration;
  private readonly MobileUserAuthService _userAuthService;


  public CustomerAuthenticationController(ILogger<CustomerAuthenticationController> logger, IConfiguration configuration, MobileUserAuthService userAuthService)
  {
    _logger = logger;
    _configuration = configuration;
    _userAuthService = userAuthService;
  }


  [AllowAnonymous]
  [HttpPost("login", Name = "Mobile Login")]
  [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(LoginResponse))]
  public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
  {
    try
    {
      var result = await _userAuthService.LoginAsync(loginRequest.Email, loginRequest.Password);

      return result.IsSuccess ? Ok(result) : BadRequest(result);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error logging in user");
      return BadRequest(ex.Message);
    }

  }



  [AllowAnonymous]
  [HttpPost("register", Name = "Mobile Register")]
  [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(MRegisterResponse))]
  public async Task<IActionResult> Register([FromBody] MRegisterRequest registerRequest)
  {
    try
    {
      var result = await _userAuthService.RegisterUserAsync(registerRequest);

      _logger.LogInformation("User registered successfully");
      return result.IsSuccess ? Ok(result) : BadRequest(result);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error registering user");
      return BadRequest(ex.Message);
    }
  }

  [HttpGet("user", Name = "Mobile Get User")]
  [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(MUserResponse))]
  public async Task<IActionResult> GetUser()
  {
    try
    {
      var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

      if (string.IsNullOrEmpty(userId))
      {
        return BadRequest("User not found");
      }

      var result = await _userAuthService.UserDetailsAsync(userId);

      return result.IsSuccess ? Ok(result) : BadRequest(result);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error getting user");
      return BadRequest(ex.Message);
    }
  }

  // deactivate user
  [HttpPut("deactivate", Name = "Mobile Deactivate User")]
  [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(MDeactivateResponse))]
  public async Task<IActionResult> DeactivateUser()
  {
    try
    {
      var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

      if (string.IsNullOrEmpty(userId))
      {
        return BadRequest("User not found");
      }

      var result = await _userAuthService.DeactivateUserAsync(userId);

      return result.IsSuccess ? Ok(result) : BadRequest(result);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error deactivating user");
      return BadRequest(ex.Message);
    }
  }

  // [HttpPost("change-password", Name = "Mobile Change Password")]
  // [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(MChangePasswordResponse))]
  // public async Task<IActionResult> ChangePassword([FromBody] MChangePasswordRequest changePasswordRequest)
  // {
  //   try
  //   {
  //     var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

  //     if (string.IsNullOrEmpty(userId))
  //     {
  //       return BadRequest("User not found");
  //     }

  //     var result = await _userAuthService.ChangePasswordAsync(userId, changePasswordRequest);

  //     return result.IsSuccess ? Ok(result) : BadRequest(result.Message);
  //   }
  //   catch (Exception ex)
  //   {
  //     _logger.LogError(ex, "Error changing password");
  //     return BadRequest(ex.Message);
  //   }
  // }

}

