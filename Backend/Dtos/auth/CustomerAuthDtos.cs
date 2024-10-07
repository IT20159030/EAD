using System.ComponentModel.DataAnnotations;

namespace Backend.Dtos;

/*
*  Dtos for the customer authentication
*/

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
  public string FirstName { get; set; } = string.Empty;

  [Required]
  public string LastName { get; set; } = string.Empty;

  [Required, EmailAddress]
  public string Email { get; set; } = string.Empty;

  [Required]
  public string NIC { get; set; } = string.Empty;

  [Required, DataType(DataType.Password)]
  public string Password { get; set; } = string.Empty;


}

public class MRegisterResponse
{
  public bool IsSuccess { get; set; }
  public string Message { get; set; } = string.Empty;
}




public class MUserResponse
{
  public bool IsSuccess { get; set; }
  public string Message { get; set; } = string.Empty;
  public UserInfoDto Data { get; set; } = new UserInfoDto();

}

public class UserInfoDto
{
  public string UserId { get; set; } = string.Empty;
  public string Name { get; set; } = string.Empty;
  public string Email { get; set; } = string.Empty;
  public string NIC { get; set; } = string.Empty;
}


public class MDeactivateResponse
{
  public bool IsSuccess { get; set; }
  public string Message { get; set; } = string.Empty;
}
public class MUpdateUserRequest
{
  [Required]
  public string FirstName { get; set; } = string.Empty;

  [Required]
  public string LastName { get; set; } = string.Empty;

  [Required]
  public string NIC { get; set; } = string.Empty;
}

public class MUpdateUserResponse
{
  public bool IsSuccess { get; set; }
  public string Message { get; set; } = string.Empty;
}

public class MAddressResponse
{
  public bool IsSuccess { get; set; }
  public string Message { get; set; } = string.Empty;
  public AddressDto Data { get; set; } = new AddressDto();
}

public class AddressDto
{
  public string Line1 { get; set; } = string.Empty;
  public string Line2 { get; set; } = string.Empty;
  public string City { get; set; } = string.Empty;
  public string PostalCode { get; set; } = string.Empty;
}

public class MAddAddressRequest
{

  [Required]
  public string Line1 { get; set; } = string.Empty;

  [Required]
  public string Line2 { get; set; } = string.Empty;

  [Required]
  public string City { get; set; } = string.Empty;

  [Required]
  public string PostalCode { get; set; } = string.Empty;
}

public class MAddAddressResponse
{
  public bool IsSuccess { get; set; }
  public string Message { get; set; } = string.Empty;
}


