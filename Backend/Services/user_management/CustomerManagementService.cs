using Backend.Dtos;
using Backend.Models;
using Backend.Utils;
using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;

namespace Backend.Services;

/*
*  Customer management service
* This service is responsible for managing customer accounts
* It provides the functionality to retrieve, create, update, delete and update account status of customers
*/
public class CustomerManagementService : ICustomerManagementService
{
  private readonly UserManager<User> _userManager;
  private readonly IMongoCollection<User> _users;

  public CustomerManagementService(UserManager<User> userManager, MongoDBService mongoDBService)
  {
    _users = mongoDBService.Database.GetCollection<User>("users");
    _userManager = userManager;
  }

  public async Task<GetAllCustomerResponse> GetAllCustomerAsync()
  {
    var response = new GetAllCustomerResponse();
    try
    {
      var customerList = await _userManager.GetUsersInRoleAsync("customer");

      response.Customers = customerList.Select(s => new Customer
      {
        Id = s.Id.ToString(),
        Name = s.Name,
        Email = s.Email ?? "",
        NIC = s.NIC,
        Status = s.Status.ToString(),
        CreatedAt = s.CreatedAt
      }).ToList();

      response.IsSuccess = true;
      response.Message = "Successfully retrieved all customer";
    }
    catch (Exception e)
    {
      response.IsSuccess = false;
      response.Message = e.Message;
    }
    return response;
  }
  public async Task<GetCustomerByIdResponse> GetCustomerByIdAsync(string id)
  {
    var response = new GetCustomerByIdResponse();
    try
    {
      var user = await _userManager.FindByIdAsync(id);

      if (user == null)
      {
        return new GetCustomerByIdResponse
        {
          IsSuccess = false,
          Message = "User not found"
        };
      }

      response.Customer = new Customer
      {
        Id = user.Id.ToString(),
        Name = user.Name,
        Email = user.Email ?? "",
        NIC = user.NIC,
        Status = user.Status.ToString(),
        CreatedAt = user.CreatedAt
      };

      response.IsSuccess = true;
      response.Message = "Successfully retrieved customer";
    }
    catch (Exception e)
    {
      response.IsSuccess = false;
      response.Message = e.Message;
    }

    return response;
  }

  public async Task<CreateCustomerResponse> CreateCustomerAsync(CreateCustomerRequest request)
  {
    var response = new CreateCustomerResponse();
    try
    {
      var newUser = new User
      {
        Name = $"{request.FirstName} {request.LastName}",
        Email = request.Email,
        UserName = request.Email,
        NIC = request.NIC,
        Status = AccountStatus.Active,
        UpdatedAt = DateTime.Now,
        CreatedAt = DateTime.Now,
      };

      var result = await _userManager.CreateAsync(newUser, request.Password);

      if (!result.Succeeded)
      {
        return new CreateCustomerResponse
        {
          IsSuccess = false,
          Message = $"User creation failed: {result.Errors.FirstOrDefault()?.Description}"
        };
      }

      var addUserToRoleResult = await _userManager.AddToRoleAsync(newUser, "customer");
      if (!addUserToRoleResult.Succeeded)
      {
        return new CreateCustomerResponse
        {
          IsSuccess = false,
          Message = $"User creation failed: {addUserToRoleResult.Errors.FirstOrDefault()?.Description}"
        };
      }

      return new CreateCustomerResponse
      {
        IsSuccess = result.Succeeded,
        Message = "User created successfully"
      };
    }
    catch (Exception e)
    {
      response.IsSuccess = false;
      response.Message = e.Message;
    }

    return response;
  }

  public async Task<UpdateCustomerResponse> UpdateCustomerAsync(string id, UpdateCustomerRequest request)
  {
    var response = new UpdateCustomerResponse();
    try
    {
      var user = await _userManager.FindByIdAsync(id);

      if (user == null)
      {
        return new UpdateCustomerResponse
        {
          IsSuccess = false,
          Message = "User not found"
        };
      }

      user.Name = $"{request.FirstName} {request.LastName}";
      user.Email = request.Email;
      user.UserName = request.Email;
      user.NIC = request.NIC;
      user.Status = Enum.Parse<AccountStatus>(request.Status);

      user.UpdatedAt = DateTime.Now;

      var result = await _userManager.UpdateAsync(user);

      if (!result.Succeeded)
      {
        return new UpdateCustomerResponse
        {
          IsSuccess = false,
          Message = $"User update failed: {result.Errors.FirstOrDefault()?.Description}"
        };
      }

      return new UpdateCustomerResponse
      {
        IsSuccess = true,
        Message = "User updated successfully"
      };
    }
    catch (Exception e)
    {
      response.IsSuccess = false;
      response.Message = e.Message;
    }

    return response;
  }

  public async Task<DeleteCustomerResponse> DeleteCustomerAsync(string id)
  {
    var response = new DeleteCustomerResponse();
    try
    {
      var user = await _userManager.FindByIdAsync(id);

      if (user == null)
      {
        return new DeleteCustomerResponse
        {
          IsSuccess = false,
          Message = "User not found"
        };
      }

      var result = await _userManager.DeleteAsync(user);

      if (!result.Succeeded)
      {
        return new DeleteCustomerResponse
        {
          IsSuccess = false,
          Message = $"User deletion failed: {result.Errors.FirstOrDefault()?.Description}"
        };
      }

      return new DeleteCustomerResponse
      {
        IsSuccess = true,
        Message = "User deleted successfully"
      };
    }
    catch (Exception e)
    {
      response.IsSuccess = false;
      response.Message = e.Message;
    }

    return response;
  }

  public async Task<UpdateCustomerAccountStatusResponse> UpdateCustomerAccountStatusAsync(string id, AccountStatus status)
  {
    var response = new UpdateCustomerAccountStatusResponse();
    try
    {
      var user = await _userManager.FindByIdAsync(id);

      if (user == null)
      {
        return new UpdateCustomerAccountStatusResponse
        {
          IsSuccess = false,
          Message = "User not found"
        };
      }

      user.Status = status;
      user.UpdatedAt = DateTime.Now;

      var result = await _userManager.UpdateAsync(user);

      if (!result.Succeeded)
      {
        return new UpdateCustomerAccountStatusResponse
        {
          IsSuccess = false,
          Message = $"User account status update failed: {result.Errors.FirstOrDefault()?.Description}"
        };
      }

      return new UpdateCustomerAccountStatusResponse
      {
        IsSuccess = true,
        Message = "User account status updated successfully"
      };
    }
    catch (Exception e)
    {
      response.IsSuccess = false;
      response.Message = e.Message;
    }
    return response;
  }
}
