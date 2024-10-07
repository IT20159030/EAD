using System.ComponentModel.DataAnnotations;
namespace Backend.Dtos;
/*
* Dtos for the vendor management
*/
public class GetAllVendorResponse
{
  public bool IsSuccess { get; set; }
  public string Message { get; set; } = string.Empty;
  public List<VendorResponse> Vendors { get; set; } = new();
}

public class VendorResponse
{

  public string Id { get; set; } = string.Empty;
  public VendorDetails VendorDetails { get; set; } = new();
  public VendorAccountResponse VendorAccountDetails { get; set; } = new();
}

public class CreateVendorRequest
{
  public VendorDetails VendorDetails { get; set; } = new();
  public VendorAccountCreationDetails VendorAccountDetails { get; set; } = new();
}

public class VendorDetails
{
  public string VendorName { get; set; } = string.Empty;
  public string VendorEmail { get; set; } = string.Empty;

  [RegularExpression(@"^(\d{10}|\d{12})$", ErrorMessage = "Phone number must be either a 10-digit number or a 12-digit number.")]
  public string VendorPhone { get; set; } = string.Empty;
  public string VendorAddress { get; set; } = string.Empty;
  public string VendorCity { get; set; } = string.Empty;
}

public class VendorAccountCreationDetails
{
  public string Name { get; set; } = string.Empty;
  public string Email { get; set; } = string.Empty;

  [RegularExpression(@"^(\d{12}|\d{9}[vV])$", ErrorMessage = "NIC must be either a 12-digit number or a 9-digit number ending with 'v' or 'V'.")]
  public string NIC { get; set; } = string.Empty;

  public string Password { get; set; } = string.Empty;

}

public class VendorAccountResponse
{
  public string Name { get; set; } = string.Empty;
  public string Email { get; set; } = string.Empty;
  public string NIC { get; set; } = string.Empty;
  public string Status { get; set; } = string.Empty;
}



public class CreateVendorResponse
{
  public bool IsSuccess { get; set; }
  public string Message { get; set; } = string.Empty;
}

public class UpdateVendorRequest
{
  public VendorDetails VendorDetails { get; set; } = new();
  public VendorAccountCreationDetails VendorAccountDetails { get; set; } = new();
}

public class UpdateVendorResponse
{
  public bool IsSuccess { get; set; }
  public string Message { get; set; } = string.Empty;
}

public class DeleteVendorResponse
{
  public bool IsSuccess { get; set; }
  public string Message { get; set; } = string.Empty;
}

public class UpdateVendorAccountStatusRequest
{
  [Required]
  public string Status { get; set; } = string.Empty;
}

public class UpdateVendorAccountStatusResponse
{
  public bool IsSuccess { get; set; }
  public string Message { get; set; } = string.Empty;
}