using System;
using System.ComponentModel.DataAnnotations;


namespace Backend.Dtos;


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