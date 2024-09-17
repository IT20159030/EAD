/**
* UserController.cs
* This file contains the controller for the User model.
* It contains the CRUD operations for the User model.
*/

using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Net;
using System.Security.Claims;
using System.Text;
using Backend.Dtos;
using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Backend.Controllers;


[ApiController]
[Route("api/v1/[controller]")]
public class AuthenticationController : ControllerBase
{
  private readonly ILogger<UserController> _logger;
  private readonly IMongoCollection<User> _users;
  private readonly UserManager<WebApplicationUser> _userManager;
  private readonly RoleManager<ApplicationRole> _roleManager;
  private readonly IConfiguration _configuration;


  public AuthenticationController(ILogger<UserController> logger, MongoDBService mongoDBService, UserManager<WebApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, IConfiguration configuration)
  {
    _logger = logger;
    _users = mongoDBService.Database.GetCollection<User>("TEST_USERS");
    _userManager = userManager;
    _roleManager = roleManager;
    _configuration = configuration;

  }

  [HttpPost("role", Name = "Create Role")]
  public async Task<IActionResult> CreateRole([FromBody] CreateRoleRequest request)
  {
    try
    {
      var result = await _roleManager.CreateAsync(new ApplicationRole
      {
        Name = request.Role
      });

      return result.Succeeded ? Ok(result) : BadRequest("Role creation failed");
    }
    catch (Exception ex)
    {
      return BadRequest(ex.Message);
    }
  }


  public class CreateRoleRequest
  {
    public string Role { get; set; } = string.Empty;
  }


  [HttpPost("register", Name = "Register")]
  [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(RegisterResponse))]
  public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest)
  {
    try
    {
      var result = await RegisterAsync(registerRequest);

      return result.IsSuccess ? Ok(result) : BadRequest(result.Message);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error registering user");
      return BadRequest(ex.Message);
    }
  }

  //TODO: move this to a service
  private async Task<RegisterResponse> RegisterAsync(RegisterRequest registerRequest)
  {
    //TODO: add validation
    var user = await _userManager.FindByEmailAsync(registerRequest.Email);
    if (user != null)
    {
      return new RegisterResponse
      {
        IsSuccess = false,
        Message = "User already exists"
      };
    }

    var newUser = new WebApplicationUser
    {
      Email = registerRequest.Email,
      UserName = registerRequest.Email,
      ConcurrencyStamp = Guid.NewGuid().ToString()
    };

    var result = await _userManager.CreateAsync(newUser, registerRequest.Password);

    if (!result.Succeeded)
    {
      return new RegisterResponse
      {
        IsSuccess = false,
        Message = $"User creation failed: {result.Errors.FirstOrDefault()?.Description}"
      };
    }

    var addUserToRoleResult = await _userManager.AddToRoleAsync(newUser, registerRequest.Role);
    if (!addUserToRoleResult.Succeeded)
    {
      return new RegisterResponse
      {
        IsSuccess = false,
        Message = $"User creation failed: {addUserToRoleResult.Errors.FirstOrDefault()?.Description}"
      };
    }


    return new RegisterResponse
    {
      IsSuccess = result.Succeeded,
      Message = "User created successfully"
    };
  }




  [HttpPost("login", Name = "Login")]
  [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(LoginResponse))]
  public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
  {
    try
    {
      var result = await LoginAsync(loginRequest.Email, loginRequest.Password);

      return result.IsSuccess ? Ok(result) : BadRequest(result.Message);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error logging in user");
      return BadRequest(ex.Message);
    }

  }

  //TODO: move this to a service
  private async Task<LoginResponse> LoginAsync(string email, string password)
  {

    var user = await _userManager.FindByEmailAsync(email);
    if (user == null)
    {
      return new LoginResponse
      {
        IsSuccess = false,
        Message = "Invalid email or password"
      };
    }

    var claims = new List<Claim>
  {
    new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
    new Claim(ClaimTypes.Email, user.Email), //TODO: check on this
    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
  };
    var roles = await _userManager.GetRolesAsync(user);
    var roleClaims = roles.Select(role => new Claim(ClaimTypes.Role, role));
    claims.AddRange(roleClaims);

    var JwtKey = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key is missing in appsettings.json");
    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtKey));
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
    var expires = DateTime.Now.AddDays(30);

    var token = new JwtSecurityToken(
      _configuration["Frontend:Url"],
      _configuration["Frontend:Url"],
      claims,
      expires: expires,
      signingCredentials: creds
    );

    return new LoginResponse
    {
      IsSuccess = true,
      AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
      Message = "Login successful",
      Email = user.Email,
      UserId = user.Id.ToString()
    };

  }
}

