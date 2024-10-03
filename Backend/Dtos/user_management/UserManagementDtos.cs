using System.ComponentModel.DataAnnotations;
using Backend.Models;
namespace Backend.Dtos;

public class CreateUserRequest
{
  [Required]
  public string Name { get; set; } = string.Empty;

  [Required]
  public string NIC { get; set; } = string.Empty;

  [Required, EmailAddress]
  public string Email { get; set; } = string.Empty;

  [Required, DataType(DataType.Password)]
  public string Password { get; set; } = string.Empty;

  [Required]
  public string Role { get; set; } = string.Empty;
}

public class CreateUserResponse
{
  public bool IsSuccess { get; set; }
  public string Message { get; set; } = string.Empty;
}


public class GetUserResponse
{
  public bool IsSuccess { get; set; }
  public string Message { get; set; } = string.Empty;
  public User User { get; set; } = new();
}



public class UpdateUserRequest
{
  public string Name { get; set; } = string.Empty;

  [EmailAddress]
  public string Email { get; set; } = string.Empty;

  [DataType(DataType.Password)]
  public string Password { get; set; } = string.Empty;

  public string Role { get; set; } = string.Empty;
}

public class UpdateUserResponse
{
  public bool IsSuccess { get; set; }
  public string Message { get; set; } = string.Empty;
}
public class DeleteUserResponse
{
  public bool IsSuccess { get; set; }
  public string Message { get; set; } = string.Empty;
}

public class GetUsersResponse
{
  public bool IsSuccess { get; set; }
  public string Message { get; set; } = string.Empty;
  public List<UserDto> Users { get; set; } = [];
}


public class UserDto
{
  public required string Id { get; set; }
  public required string Name { get; set; }
  public required string Email { get; set; }

  public required string Status { get; set; }
  public required DateTime CreatedAt { get; set; }
  // Add other properties as needed
}

public class UpdateUserStatusRequest
{
  public required string Status { get; set; }
}

public class UpdateUserStatusResponse
{
  public bool IsSuccess { get; set; }
  public string Message { get; set; } = string.Empty;
}
