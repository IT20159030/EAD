
using System.ComponentModel.DataAnnotations;
using Backend.Models;

namespace Backend.Dtos;
public class GetAllStaffResponse
{
  public bool IsSuccess { get; set; }
  public string Message { get; set; } = string.Empty;
  public List<Staff> Staff { get; set; } = new();
}

public class Staff
{
  public string Id { get; set; } = string.Empty;
  public string Name { get; set; } = string.Empty;
  public string Email { get; set; } = string.Empty;
  public string Role { get; set; } = string.Empty;
  public string NIC { get; set; } = string.Empty;
  public string Status { get; set; } = string.Empty;

}

public class CreateStaffRequest
{
  [Required]
  public string FirstName { get; set; } = string.Empty;

  [Required]
  public string LastName { get; set; } = string.Empty;

  [Required]
  [EmailAddress]
  public string Email { get; set; } = string.Empty;

  [Required]
  public string Password { get; set; } = string.Empty;

  [Required]
  public string Role { get; set; } = string.Empty;

  [Required]
  [RegularExpression(@"^(\d{12}|\d{9}[vV])$", ErrorMessage = "NIC must be either a 12-digit number or a 9-digit number ending with 'v' or 'V'.")]
  public string NIC { get; set; } = string.Empty;
}
public class CreateStaffResponse
{
  public bool IsSuccess { get; set; }
  public string Message { get; set; } = string.Empty;
}

public class UpdateStaffRequest
{
  [Required]
  public string FirstName { get; set; } = string.Empty;

  [Required]
  public string LastName { get; set; } = string.Empty;

  [Required]
  [EmailAddress]
  public string Email { get; set; } = string.Empty;

  [Required]
  public string Role { get; set; } = string.Empty;

  [Required]
  [RegularExpression(@"^(\d{12}|\d{9}[vV])$", ErrorMessage = "NIC must be either a 12-digit number or a 9-digit number ending with 'v' or 'V'.")]
  public string NIC { get; set; } = string.Empty;
}

public class UpdateStaffResponse
{
  public bool IsSuccess { get; set; }
  public string Message { get; set; } = string.Empty;
}

public class DeleteStaffResponse
{
  public bool IsSuccess { get; set; }
  public string Message { get; set; } = string.Empty;
}

public class UpdateStaffAccountStatusRequest
{
  [Required]
  public string Status { get; set; } = string.Empty;
}

public class UpdateStaffAccountStatusResponse
{
  public bool IsSuccess { get; set; }
  public string Message { get; set; } = string.Empty;
}