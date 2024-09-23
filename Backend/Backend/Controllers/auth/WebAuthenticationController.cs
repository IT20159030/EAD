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
  private readonly ILogger<UserController> _logger;
  private readonly WebUserAuthService _webUserAuthService;

  public WebAuthenticationController(ILogger<UserController> logger, IConfiguration configuration, WebUserAuthService webUserAuthService)
  {
    _logger = logger;
    _webUserAuthService = webUserAuthService;
  }

  [HttpPost("login", Name = "Login")]
  [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(LoginResponse))]
  public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
  {
    try
    {
      var result = await _webUserAuthService.LoginAsync(loginRequest.Email, loginRequest.Password);

      return result.IsSuccess ? Ok(result) : BadRequest(result.Message);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error logging in user");
      return BadRequest(ex.Message);
    }
  }

  [HttpPost("register", Name = "Register")]
  [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(CreateRoleResponse))]
  public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest)
  {
    try
    {
      var result = await _webUserAuthService.RegisterUserAsync(registerRequest);

      return result.IsSuccess ? Ok(result) : BadRequest(result.Message);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error registering user");
      return BadRequest(ex.Message);
    }
  }
}