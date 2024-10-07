using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Backend.Dtos;
using Backend.Models;
using Backend.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;

namespace Backend.Services;

/*
*  Web user authentication service
* This service provides the functionality for the web user authentication
*/
public class WebUserAuthService : IWebUserAuthService
{

  private readonly UserManager<User> _userManager;

  private readonly IMongoCollection<Vendor> _vendors;
  private readonly IConfiguration _configuration;


  public WebUserAuthService(UserManager<User> userManager, MongoDBService mongoDBService, IConfiguration configuration)
  {
    _userManager = userManager;
    _vendors = mongoDBService.Database.GetCollection<Vendor>("Vendors");
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
      Profile = new Profile
      {
        UserId = user.Id.ToString(),
        Name = user.Name,
        Email = user.Email,
        Role = roles.FirstOrDefault() ?? "",
        NIC = user.NIC
      }
    };

  }

  public async Task<ProfileResponse> GetProfileAsync(string userId)
  {
    var user = await _userManager.FindByIdAsync(userId);
    if (user == null)
    {
      return new ProfileResponse
      {
        IsSuccess = false,
        Message = "User not found"
      };
    }

    var roles = await _userManager.GetRolesAsync(user);

    return new ProfileResponse
    {
      IsSuccess = true,
      Profile = new Profile
      {
        UserId = user.Id.ToString(),
        Name = user.Name,
        Email = user.Email ?? "",
        Role = roles.FirstOrDefault() ?? "",
        NIC = user.NIC,
        CreatedAt = user?.CreatedAt.ToString() ?? ""
      }
    };
  }

  public async Task<ProfileResponse> UpdateProfileAsync(string userId, UpdateProfileRequest request)
  {
    var user = await _userManager.FindByIdAsync(userId);
    if (user == null)
    {
      return new ProfileResponse
      {
        IsSuccess = false,
        Message = "User not found"
      };
    }

    user.Name = request.Name;
    user.Email = request.Email;
    user.NIC = request.NIC;

    var result = await _userManager.UpdateAsync(user);

    if (!result.Succeeded)
    {
      return new ProfileResponse
      {
        IsSuccess = false,
        Message = "Failed to update profile"
      };
    }

    var roles = await _userManager.GetRolesAsync(user);

    return new ProfileResponse
    {
      IsSuccess = true,
      Profile = new Profile
      {
        UserId = user.Id.ToString(),
        Name = user.Name,
        Email = user.Email,
        Role = roles.FirstOrDefault() ?? "",
        NIC = user.NIC,
        CreatedAt = user.CreatedAt.ToString()
      }
    };
  }

  public async Task<VendorInfoResponse> GetVendorInfoAsync(string userId)
  {

    var vendor = await _vendors.Find(v => v.Id == Guid.Parse(userId)).FirstOrDefaultAsync();
    if (vendor == null)
    {
      return new VendorInfoResponse
      {
        IsSuccess = false,
        Message = "Vendor not found"
      };
    }

    return new VendorInfoResponse
    {
      IsSuccess = true,
      VendorDetails = new VendorDetails
      {
        VendorName = vendor.VendorName,
        VendorEmail = vendor.VendorEmail,
        VendorPhone = vendor.VendorPhone,
        VendorAddress = vendor.VendorAddress,
        VendorCity = vendor.VendorCity
      }
    };
  }

  public async Task<UpdateVendorResponse> UpdateVendorAsync(string id, VendorDetails request)
  {
    var vendor = await _vendors.Find(v => v.Id == Guid.Parse(id)).FirstOrDefaultAsync();

    if (vendor == null)
    {
      return new UpdateVendorResponse
      {
        IsSuccess = false,
        Message = "Vendor not found",
      };
    }

    vendor.VendorName = request.VendorName;
    vendor.VendorEmail = request.VendorEmail;
    vendor.VendorPhone = request.VendorPhone;
    vendor.VendorAddress = request.VendorAddress;
    vendor.VendorCity = request.VendorCity;

    await _vendors.ReplaceOneAsync(v => v.Id == Guid.Parse(id), vendor);

    return new UpdateVendorResponse
    {
      IsSuccess = true,
      Message = "Vendor updated successfully"
    };
  }
}

