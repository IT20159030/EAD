using Backend.Dtos;
using Backend.Models;
using Backend.Utils;
using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;

namespace Backend.Services;

/*
*  Staff management service
* This service is responsible for managing staff accounts
* It provides the functionality to retrieve, create, update, delete and update account status of staff
*/
public class StaffManagementService : IStaffManagementService
{
  private readonly UserManager<User> _userManager;
  private readonly IMongoCollection<User> _users;

  // Role GUIDs for admin and CSR
  private readonly string _ADMIN_ROLE_GUID = "14496978-03e1-4abf-9d6e-2f08210063d3";
  private readonly string _CSR_ROLE_GUID = "4660dc05-1e74-4d55-9838-adb82d1c56b6";

  public StaffManagementService(UserManager<User> userManager, MongoDBService mongoDBService)
  {
    _users = mongoDBService.Database.GetCollection<User>("users");
    _userManager = userManager;
  }

  public async Task<GetAllStaffResponse> GetAllStaffAsync()
  {
    var response = new GetAllStaffResponse();
    try
    {
      // MongoDB query for checking if roles include the Admin or CSR role GUIDs
      var filter = Builders<User>.Filter.AnyIn("Roles", new List<string> { _ADMIN_ROLE_GUID, _CSR_ROLE_GUID });
      var staffList = await _users.Find(filter).ToListAsync();

      response.Staff = staffList.Select(s => new Staff
      {
        Id = s.Id.ToString(),
        Name = s.Name,
        Email = s.Email ?? "",
        Role = s.Roles.Contains(Guid.Parse(_ADMIN_ROLE_GUID)) ? "admin" : "csr",
        NIC = s.NIC,
        Status = s.Status.ToString()
      }).ToList();

      response.IsSuccess = true;
      response.Message = "Successfully retrieved all staff";
    }
    catch (Exception e)
    {
      response.IsSuccess = false;
      response.Message = e.Message;
    }

    return response;
  }

  public async Task<CreateStaffResponse> CreateStaffAsync(CreateStaffRequest request)
  {
    var response = new CreateStaffResponse();
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
        return new CreateStaffResponse
        {
          IsSuccess = false,
          Message = $"User creation failed: {result.Errors.FirstOrDefault()?.Description}"
        };
      }

      var addUserToRoleResult = await _userManager.AddToRoleAsync(newUser, request.Role);
      if (!addUserToRoleResult.Succeeded)
      {
        return new CreateStaffResponse
        {
          IsSuccess = false,
          Message = $"User creation failed: {addUserToRoleResult.Errors.FirstOrDefault()?.Description}"
        };
      }

      return new CreateStaffResponse
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

  public async Task<UpdateStaffResponse> UpdateStaffAsync(string id, UpdateStaffRequest request)
  {
    var response = new UpdateStaffResponse();
    try
    {
      var user = await _userManager.FindByIdAsync(id);

      if (user == null)
      {
        return new UpdateStaffResponse
        {
          IsSuccess = false,
          Message = "User not found"
        };
      }

      user.Name = $"{request.FirstName} {request.LastName}";
      user.Email = request.Email;
      user.UserName = request.Email;
      user.NIC = request.NIC;
      user.UpdatedAt = DateTime.Now;

      var result = await _userManager.UpdateAsync(user);

      if (!result.Succeeded)
      {
        return new UpdateStaffResponse
        {
          IsSuccess = false,
          Message = $"User update failed: {result.Errors.FirstOrDefault()?.Description}"
        };
      }

      // Handle role update
      var currentRoles = await _userManager.GetRolesAsync(user);
      var currentRole = currentRoles.FirstOrDefault();

      if (currentRole != request.Role)
      {
        if (currentRole != null)
        {
          await _userManager.RemoveFromRoleAsync(user, currentRole);
        }
        var addToRoleResult = await _userManager.AddToRoleAsync(user, request.Role);
        if (!addToRoleResult.Succeeded)
        {
          return new UpdateStaffResponse
          {
            IsSuccess = false,
            Message = $"Role update failed: {addToRoleResult.Errors.FirstOrDefault()?.Description}"
          };
        }
      }

      return new UpdateStaffResponse
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

  public async Task<DeleteStaffResponse> DeleteStaffAsync(string id)
  {
    var response = new DeleteStaffResponse();
    try
    {
      var user = await _userManager.FindByIdAsync(id);

      if (user == null)
      {
        return new DeleteStaffResponse
        {
          IsSuccess = false,
          Message = "User not found"
        };
      }

      var result = await _userManager.DeleteAsync(user);

      if (!result.Succeeded)
      {
        return new DeleteStaffResponse
        {
          IsSuccess = false,
          Message = $"User deletion failed: {result.Errors.FirstOrDefault()?.Description}"
        };
      }

      return new DeleteStaffResponse
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

  public async Task<UpdateStaffAccountStatusResponse> UpdateStaffAccountStatusAsync(string id, AccountStatus status)
  {
    var response = new UpdateStaffAccountStatusResponse();
    try
    {
      var user = await _userManager.FindByIdAsync(id);

      if (user == null)
      {
        return new UpdateStaffAccountStatusResponse
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
        return new UpdateStaffAccountStatusResponse
        {
          IsSuccess = false,
          Message = $"User account status update failed: {result.Errors.FirstOrDefault()?.Description}"
        };
      }

      return new UpdateStaffAccountStatusResponse
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
