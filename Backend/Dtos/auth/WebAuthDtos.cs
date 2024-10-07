using System.ComponentModel.DataAnnotations;

namespace Backend.Dtos;

/*
*  Dtos for the web user authentication
*/
public class LoginRequest
{
  [Required, EmailAddress]
  public string Email { get; set; } = string.Empty;

  [Required, DataType(DataType.Password)]
  public string Password { get; set; } = string.Empty;
}

public class LoginResponse
{
  public bool IsSuccess { get; set; }
  public string Message { get; set; } = string.Empty;
  public string AccessToken { get; set; } = string.Empty;
  public Profile Profile { get; set; } = new();
}

public class ProfileResponse
{
  public bool IsSuccess { get; set; }
  public string Message { get; set; } = string.Empty;
  public Profile Profile { get; set; } = new();

}

public class Profile
{
  public string UserId { get; set; } = string.Empty;
  public string Name { get; set; } = string.Empty;
  public string Email { get; set; } = string.Empty;
  public string Role { get; set; } = string.Empty;
  public string NIC { get; set; } = string.Empty;

  public string CreatedAt { get; set; } = string.Empty;

}

public class UpdateProfileRequest
{
  [Required]
  public string Name { get; set; } = string.Empty;

  [Required, EmailAddress]
  public string Email { get; set; } = string.Empty;

  [Required, RegularExpression(@"^(\d{12}|\d{9}[vV])$", ErrorMessage = "NIC must be either a 12-digit number or a 9-digit number ending with 'v' or 'V'.")]
  public string NIC { get; set; } = string.Empty;
}

// ONLY FOR VENDOR ACCOUNT UPDATES
public class VendorInfoResponse
{
  public bool IsSuccess { get; set; }
  public string Message { get; set; } = string.Empty;
  public VendorDetails VendorDetails { get; set; } = new();
}

