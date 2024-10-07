using System.Net;
using System.Security.Claims;
using Backend.Dtos;
using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

/**
* CustomerAuthenticationController.cs provides the endpoints for the customer authentication for the mobile application.
*/

[ApiController]
[Authorize(Roles = "customer")]
[Route("api/v1/customer-auth")]
[Produces("application/json")]
public class MobileAuthController : ControllerBase
{
  private readonly ILogger<MobileAuthController> _logger;
  private readonly IConfiguration _configuration;
  private readonly MobileUserAuthService _userAuthService;


  public MobileAuthController(ILogger<MobileAuthController> logger, IConfiguration configuration, MobileUserAuthService userAuthService)
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

  [HttpPut("update", Name = "Mobile Update User")]
  [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(MUpdateUserResponse))]
  public async Task<IActionResult> UpdateUser([FromBody] MUpdateUserRequest updateUserRequest)
  {
    try
    {
      var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

      if (string.IsNullOrEmpty(userId))
      {
        return BadRequest("User not found");
      }

      var result = await _userAuthService.UpdateUserAsync(userId, updateUserRequest);

      return result.IsSuccess ? Ok(result) : BadRequest(result);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error updating user");
      return BadRequest(ex.Message);
    }
  }

  [HttpGet("address", Name = "Mobile Get Address")]
  [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(MAddressResponse))]
  public async Task<IActionResult> GetAddress()
  {
    try
    {
      var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

      if (string.IsNullOrEmpty(userId))
      {
        return BadRequest("User not found");
      }

      var result = await _userAuthService.GetAddressAsync(userId);

      return result.IsSuccess ? Ok(result) : BadRequest(result);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error getting address");
      return BadRequest(ex.Message);
    }
  }
  [HttpPut("address", Name = "Mobile Update Address")]
  [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(MAddressResponse))]
  public async Task<IActionResult> UpdateAddress([FromBody] MAddAddressRequest updateAddressRequest)
  {
    try
    {
      var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

      if (string.IsNullOrEmpty(userId))
      {
        return BadRequest("User not found");
      }

      var result = await _userAuthService.UpdateAddressAsync(userId, updateAddressRequest);

      return result.IsSuccess ? Ok(result) : BadRequest(result);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error updating address");
      return BadRequest(ex.Message);
    }
  }
}

