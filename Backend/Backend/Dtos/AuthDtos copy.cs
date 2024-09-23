using System.ComponentModel.DataAnnotations;

namespace Backend.Dtos;

public class MLoginRequest
{
  [Required, EmailAddress]
  public string Email { get; set; } = string.Empty;

  [Required, DataType(DataType.Password)]
  public string Password { get; set; } = string.Empty;
}

public class MLoginResponse
{
  public bool IsSuccess { get; set; }
  public string Message { get; set; } = string.Empty;
  public string AccessToken { get; set; } = string.Empty;
  public string Email { get; set; } = string.Empty;
  public string UserId { get; set; } = string.Empty;
}


public class MRegisterRequest
{
  [Required]
  public string Name { get; set; } = string.Empty;

  [Required, EmailAddress]
  public string Email { get; set; } = string.Empty;

  [Required, DataType(DataType.Password)]
  public string Password { get; set; } = string.Empty;

  // [Required, Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
  // public string ConfirmPassword { get; set; } = string.Empty;

  // [Required] //TODO: Add role to register request
  // public string Role { get; set; } = string.Empty;

}

public class MRegisterResponse
{
  public bool IsSuccess { get; set; }
  public string Message { get; set; } = string.Empty;
}


public class MCreateRoleRequest
{
  [Required]
  public string Role { get; set; } = string.Empty;
}

public class MCreateRoleResponse
{
  public bool IsSuccess { get; set; }
  public string Message { get; set; } = string.Empty;
}