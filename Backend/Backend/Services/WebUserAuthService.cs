

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Backend.Dtos;
using Backend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace Backend.Services;
public class WebUserAuthService : IWebUserAuthService
{
  private readonly UserManager<WebUser> _userManager;
  private readonly IConfiguration _configuration;

  public WebUserAuthService(UserManager<WebUser> userManager, RoleManager<ApplicationRole> roleManager, IConfiguration configuration)
  {
    _userManager = userManager;
    _configuration = configuration;
  }




  public async Task<RegisterResponse> RegisterUserAsync(RegisterRequest request)
  {
    var user = await _userManager.FindByEmailAsync(request.Email);
    if (user != null)
    {
      return new RegisterResponse
      {
        IsSuccess = false,
        Message = "User already exists"
      };
    }

    var newUser = new WebUser
    {
      Name = request.Name,
      Email = request.Email,
      UserName = request.Email,
      UpdatedAt = DateTime.Now,
      CreatedAt = DateTime.Now,
      ConcurrencyStamp = Guid.NewGuid().ToString()
    };

    var result = await _userManager.CreateAsync(newUser, request.Password);

    if (!result.Succeeded)
    {
      return new RegisterResponse
      {
        IsSuccess = false,
        Message = $"User creation failed: {result.Errors.FirstOrDefault()?.Description}"
      };
    }

    var addUserToRoleResult = await _userManager.AddToRoleAsync(newUser, request.Role);
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

  public async Task<LoginResponse> LoginAsync(string email, string password)
  {

    var user = await _userManager.FindByEmailAsync(email);
    if (user == null || user.Email == null || !await _userManager.CheckPasswordAsync(user, password))
    {
      return new LoginResponse
      {
        IsSuccess = false,
        Message = "Invalid email or password"
      };
    }

    var claims = new List<Claim>
  {
    new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
    new(ClaimTypes.Email, user.Email),
    new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
    new(ClaimTypes.NameIdentifier, user.Id.ToString())
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