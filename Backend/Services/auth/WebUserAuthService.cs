

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
  private readonly UserManager<User> _userManager;
  private readonly IConfiguration _configuration;

  public WebUserAuthService(UserManager<User> userManager, IConfiguration configuration)
  {
    _userManager = userManager;
    _configuration = configuration;
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

    if (user.Status == AccountStatus.Unapproved || user.Status == AccountStatus.Deactivated || user.Status == AccountStatus.Rejected)
    {
      return new LoginResponse
      {
        IsSuccess = false,
        Message = "Account is not active"
      };
    }

    var roles = await _userManager.GetRolesAsync(user);
    var allowedRoles = new[] { "admin", "vendor", "csr" };

    if (!roles.Any(role => allowedRoles.Contains(role)))
    {
      return new LoginResponse
      {
        IsSuccess = false,
        Message = "You do not have the required role to access this system"
      };
    }

    var claims = new List<Claim>
  {
    new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
    new(ClaimTypes.Email, user.Email),
    new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
    new(ClaimTypes.NameIdentifier, user.Id.ToString())
  };
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
      Name = user.Name,
      Email = user.Email,
      Role = roles.FirstOrDefault() ?? string.Empty,
      UserId = user.Id.ToString()
    };

  }

}