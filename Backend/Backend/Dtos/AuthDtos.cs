using System.ComponentModel.DataAnnotations;

namespace Backend.Dtos;

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
  public string Email { get; set; } = string.Empty;
  public string UserId { get; set; } = string.Empty;
}


public class RegisterRequest
{
  [Required]
  public string Name { get; set; } = string.Empty;

  [Required, EmailAddress]
  public string Email { get; set; } = string.Empty;

  [Required, DataType(DataType.Password)]
  public string Password { get; set; } = string.Empty;

  // [Required, Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
  // public string ConfirmPassword { get; set; } = string.Empty;

  [Required]
  public string Role { get; set; } = string.Empty;

}

public class RegisterResponse
{
  public bool IsSuccess { get; set; }
  public string Message { get; set; } = string.Empty;
}


public class CreateRoleRequest
{
  [Required]
  public string Role { get; set; } = string.Empty;
}

public class CreateRoleResponse
{
  public bool IsSuccess { get; set; }
  public string Message { get; set; } = string.Empty;
}