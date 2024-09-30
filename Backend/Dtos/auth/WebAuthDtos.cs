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

  public string Name { get; set; } = string.Empty;
  public string Email { get; set; } = string.Empty;

  public string Role { get; set; } = string.Empty;
}
