/**
* UserController.cs
* This file contains the controller for the User model.
* It contains the CRUD operations for the User model.
*/

using System.Net;
using Backend.Dtos;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("api/v1/web-auth")]
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


}