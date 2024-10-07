using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Backend.Dtos;
using Backend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace Backend.Services;

/*
*  Mobile user authentication service
* This service provides the functionality for the mobile user authentication
*/
public class MobileUserAuthService : IMobileUserAuthService
{
  private readonly UserManager<User> _userManager;
  private readonly IConfiguration _configuration;

  public MobileUserAuthService(UserManager<User> userManager, IConfiguration configuration)
  {
    _userManager = userManager;
    _configuration = configuration;
  }

  public async Task<MRegisterResponse> RegisterUserAsync(MRegisterRequest request)
  {
    var user = await _userManager.FindByEmailAsync(request.Email);
    if (user != null)
    {
      return new MRegisterResponse
      {
        IsSuccess = false,
        Message = "Email already in use"
      };
    }

    var newUser = new User
    {
      Name = $"{request.FirstName} {request.LastName}",
      NIC = request.NIC,
      Email = request.Email,
      UserName = request.Email,
      UpdatedAt = DateTime.Now,
      CreatedAt = DateTime.Now,
      ConcurrencyStamp = Guid.NewGuid().ToString()
    };

    var result = await _userManager.CreateAsync(newUser, request.Password);

    if (!result.Succeeded)
    {
      return new MRegisterResponse
      {
        IsSuccess = false,
        Message = $"User creation failed: {result.Errors.FirstOrDefault()?.Description}"
      };
    }

    var addUserToRoleResult = await _userManager.AddToRoleAsync(newUser, "customer");
    if (!addUserToRoleResult.Succeeded)
    {
      return new MRegisterResponse
      {
        IsSuccess = false,
        Message = $"User creation failed: {addUserToRoleResult.Errors.FirstOrDefault()?.Description}"
      };
    }

    return new MRegisterResponse
    {
      IsSuccess = result.Succeeded,
      Message = "User created successfully"
    };
  }

  public async Task<MLoginResponse> LoginAsync(string email, string password)
  {

    var user = await _userManager.FindByEmailAsync(email);
    if (user == null || user.Email == null || !await _userManager.CheckPasswordAsync(user, password))
    {
      return new MLoginResponse
      {
        IsSuccess = false,
        Message = "Invalid email or password"
      };
    }

    if (user.Status == AccountStatus.Unapproved || user.Status == AccountStatus.Deactivated || user.Status == AccountStatus.Rejected)
    {
      return new MLoginResponse
      {
        IsSuccess = false,
        Message = user.Status == AccountStatus.Unapproved ? "Account not approved" : user.Status == AccountStatus.Deactivated ? "Account deactivated" : "Account rejected"
      };
    }

    var isInRole = await _userManager.IsInRoleAsync(user, "customer");
    if (!isInRole)
    {
      return new MLoginResponse
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

    return new MLoginResponse
    {
      IsSuccess = true,
      AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
      Message = "Login successful",
      Email = user.Email,
      UserId = user.Id.ToString()
    };

  }

  public async Task<MUserResponse> UserDetailsAsync(string userId)
  {
    var user = await _userManager.FindByIdAsync(userId);
    if (user == null)
    {
      return new MUserResponse
      {
        IsSuccess = false,
        Message = "User not found"
      };
    }

    return new MUserResponse
    {
      IsSuccess = true,
      Data = new UserInfoDto
      {
        UserId = user.Id.ToString(),
        Name = user.Name,
        Email = user.Email ?? string.Empty,
        NIC = user.NIC ?? string.Empty
      }
    };
  }

  // deactivation of user account
  public async Task<MUserResponse> DeactivateUserAsync(string userId)
  {
    var user = await _userManager.FindByIdAsync(userId);
    if (user == null)
    {
      return new MUserResponse
      {
        IsSuccess = false,
        Message = "User not found"
      };
    }

    user.Status = AccountStatus.Deactivated;
    var result = await _userManager.UpdateAsync(user);

    return new MUserResponse
    {
      IsSuccess = result.Succeeded,
      Message = result.Succeeded ? "User account deactivated" : "Failed to deactivate user account"
    };
  }

  public async Task<MUpdateUserResponse> UpdateUserAsync(string userId, MUpdateUserRequest request)
  {
    var user = await _userManager.FindByIdAsync(userId);
    if (user == null)
    {
      return new MUpdateUserResponse
      {
        IsSuccess = false,
        Message = "User not found"
      };
    }

    user.Name = $"{request.FirstName} {request.LastName}";
    user.NIC = request.NIC;

    user.UpdatedAt = DateTime.Now;

    var result = await _userManager.UpdateAsync(user);

    return new MUpdateUserResponse
    {
      IsSuccess = result.Succeeded,
      Message = result.Succeeded ? "User updated successfully" : "Failed to update user"
    };
  }

  public async Task<MAddressResponse> GetAddressAsync(string userId)
  {
    var user = await _userManager.FindByIdAsync(userId);
    if (user == null)
    {
      return new MAddressResponse
      {
        IsSuccess = false,
        Message = "User not found"
      };
    }

    return new MAddressResponse
    {
      IsSuccess = true,
      Data = new AddressDto
      {
        Line1 = user.Address.Line1,
        Line2 = user.Address.Line2,
        City = user.Address.City,
        PostalCode = user.Address.PostalCode
      }
    };
  }


  public async Task<MAddressResponse> UpdateAddressAsync(string userId, MAddAddressRequest request)
  {
    var user = await _userManager.FindByIdAsync(userId);
    if (user == null)
    {
      return new MAddressResponse
      {
        IsSuccess = false,
        Message = "User not found"
      };
    }

    user.Address = new Address
    {
      Line1 = request.Line1,
      Line2 = request.Line2,
      City = request.City,
      PostalCode = request.PostalCode
    };

    user.UpdatedAt = DateTime.Now;

    var result = await _userManager.UpdateAsync(user);

    return new MAddressResponse
    {
      IsSuccess = result.Succeeded,
      Message = result.Succeeded ? "Address updated successfully" : "Failed to update address",
      Data = new AddressDto
      {
        Line1 = user.Address.Line1,
        Line2 = user.Address.Line2,
        City = user.Address.City,
        PostalCode = user.Address.PostalCode
      }
    };
  }
}
