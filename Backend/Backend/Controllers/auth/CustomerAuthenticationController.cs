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



  [HttpPost("login", Name = "Mobile Login")]
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




  [HttpPost("register", Name = "Mobile Register")]
  [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(MRegisterResponse))]
  public async Task<IActionResult> Register([FromBody] MRegisterRequest registerRequest)
  {
    try
    {
      var result = await _userAuthService.RegisterUserAsync(registerRequest);

      return result.IsSuccess ? Ok(result) : BadRequest(result.Message);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error registering user");
      return BadRequest(ex.Message);
    }
  }



}

