
using System.ComponentModel.DataAnnotations;
using Backend.Models;

namespace Backend.Dtos;
public class GetAllCustomerResponse
{
  public bool IsSuccess { get; set; }
  public string Message { get; set; } = string.Empty;
  public List<Customer> Customers { get; set; } = new();
}

public class Customer
{
  public string Id { get; set; } = string.Empty;
  public string Name { get; set; } = string.Empty;
  public string Email { get; set; } = string.Empty;
  public string NIC { get; set; } = string.Empty;
  public string Status { get; set; } = string.Empty;
  public DateTime CreatedAt { get; set; } = DateTime.Now;

}

public class GetCustomerByIdResponse
{
  public bool IsSuccess { get; set; }
  public string Message { get; set; } = string.Empty;
  public Customer Customer { get; set; } = new();
}

public class CreateCustomerRequest
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
  [RegularExpression(@"^(\d{12}|\d{9}[vV])$", ErrorMessage = "NIC must be either a 12-digit number or a 9-digit number ending with 'v' or 'V'.")]
  public string NIC { get; set; } = string.Empty;
}


public class CreateCustomerResponse
{
  public bool IsSuccess { get; set; }
  public string Message { get; set; } = string.Empty;
}

public class UpdateCustomerRequest
{
  [Required]
  public string FirstName { get; set; } = string.Empty;

  [Required]
  public string LastName { get; set; } = string.Empty;

  [Required]
  [EmailAddress]
  public string Email { get; set; } = string.Empty;

  [Required]
  [RegularExpression(@"^(\d{12}|\d{9}[vV])$", ErrorMessage = "NIC must be either a 12-digit number or a 9-digit number ending with 'v' or 'V'.")]
  public string NIC { get; set; } = string.Empty;

  [Required]
  public string Status { get; set; } = string.Empty;

}

public class UpdateCustomerResponse
{
  public bool IsSuccess { get; set; }
  public string Message { get; set; } = string.Empty;
}

public class DeleteCustomerResponse
{
  public bool IsSuccess { get; set; }
  public string Message { get; set; } = string.Empty;
}

public class UpdateCustomerAccountStatusRequest
{
  [Required]
  public string Status { get; set; } = string.Empty;
}

public class UpdateCustomerAccountStatusResponse
{
  public bool IsSuccess { get; set; }
  public string Message { get; set; } = string.Empty;
}