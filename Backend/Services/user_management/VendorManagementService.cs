using Backend.Dtos;
using Backend.Models;
using Backend.Utils;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Backend.Services;

/*
*  Interface for the vendor management service
* Contains methods for CRUD operations on vendors
* and updating vendor account status
*/
public class VendorManagementService : IVendorManagementService
{
  private readonly UserManager<User> _userManager;
  private readonly IMongoCollection<User> _users;

  private readonly IMongoCollection<Vendor> _vendors;

  public VendorManagementService(UserManager<User> userManager, MongoDBService mongoDBService)
  {
    _users = mongoDBService.Database.GetCollection<User>("users");
    _vendors = mongoDBService.Database.GetCollection<Vendor>("Vendors");
    _userManager = userManager;
  }


  public async Task<GetAllVendorResponse> GetAllVendorAsync()
  {
    var response = new GetAllVendorResponse();

    try
    {
      // Get all vendors
      var vendors = await _vendors.Find(new BsonDocument()).ToListAsync();

      // Get all user IDs from vendors
      var userIds = vendors.Select(v => v.Id).ToList();

      // Fetch all corresponding users in one query
      var users = await _users.Find(u => userIds.Contains(u.Id)).ToListAsync();

      // Create a dictionary for quick user lookup
      var userDict = users.ToDictionary(u => u.Id);

      response.Vendors = vendors.Select(vendor =>
      {
        userDict.TryGetValue(vendor.Id, out User? user);

        return new VendorResponse
        {
          Id = vendor.Id.ToString(),
          VendorDetails = new VendorDetails
          {
            VendorName = vendor.VendorName,
            VendorEmail = vendor.VendorEmail,
            VendorPhone = vendor.VendorPhone,
            VendorAddress = vendor.VendorAddress,
            VendorCity = vendor.VendorCity,
          },
          VendorAccountDetails = new VendorAccountResponse
          {
            Name = user?.Name ?? "",
            Email = user?.Email ?? vendor.VendorEmail,
            NIC = user?.NIC ?? "",
            Status = user?.Status.ToString() ?? "Inactive",
          }
        };
      }).ToList();

      response.IsSuccess = true;
      response.Message = "Vendors retrieved successfully";
    }
    catch (Exception e)
    {
      response.IsSuccess = false;
      response.Message = e.Message;
    }

    return response;
  }

  public async Task<CreateVendorResponse> CreateVendorAsync(CreateVendorRequest request)
  {
    var vendorDetails = request.VendorDetails;
    var vendorAccountDetails = request.VendorAccountDetails;

    User? newUser = null;

    try
    {
      newUser = new User
      {
        Name = vendorAccountDetails.Name,
        Email = vendorAccountDetails.Email,
        UserName = vendorAccountDetails.Email,
        NIC = vendorAccountDetails.NIC,
        Status = AccountStatus.Active,
        UpdatedAt = DateTime.Now,
        CreatedAt = DateTime.Now,
      };

      var result = await _userManager.CreateAsync(newUser, vendorAccountDetails.Password);

      if (!result.Succeeded)
      {
        return new CreateVendorResponse
        {
          IsSuccess = false,
          Message = $"User creation failed: {result.Errors.FirstOrDefault()?.Description}"
        };
      }

      var addUserToRoleResult = await _userManager.AddToRoleAsync(newUser, "vendor");
      if (!addUserToRoleResult.Succeeded)
      {
        // Rollback user creation
        await _userManager.DeleteAsync(newUser);
        return new CreateVendorResponse
        {
          IsSuccess = false,
          Message = $"Adding user to vendor role failed: {addUserToRoleResult.Errors.FirstOrDefault()?.Description}"
        };
      }

      // Create Vendor document
      var vendor = new Vendor
      {
        Id = newUser.Id,
        VendorName = vendorDetails.VendorName,
        VendorEmail = vendorDetails.VendorEmail,
        VendorPhone = vendorDetails.VendorPhone,
        VendorAddress = vendorDetails.VendorAddress,
        VendorCity = vendorDetails.VendorCity,
        VendorRating = 0,
        VendorRatingCount = 0,
        Reviews = new List<Review>(),
      };

      await _vendors.InsertOneAsync(vendor);

      return new CreateVendorResponse
      {
        IsSuccess = true,
        Message = "Vendor created successfully",
      };
    }
    catch (Exception e)
    {
      // Rollback user creation if vendor document creation failed
      if (newUser != null)
      {
        await _userManager.DeleteAsync(newUser);
      }

      return new CreateVendorResponse
      {
        IsSuccess = false,
        Message = $"Vendor creation failed: {e.Message}"
      };
    }
  }

  public async Task<UpdateVendorResponse> UpdateVendorAsync(string id, UpdateVendorRequest request)
  {
    var vendorDetails = request.VendorDetails;
    var vendorAccountDetails = request.VendorAccountDetails;

    try
    {
      var vendor = await _vendors.Find(v => v.Id == Guid.Parse(id)).FirstOrDefaultAsync();

      if (vendor == null)
      {
        return new UpdateVendorResponse
        {
          IsSuccess = false,
          Message = "Vendor not found",
        };
      }

      var user = await _userManager.FindByIdAsync(id);

      if (user == null)
      {
        return new UpdateVendorResponse
        {
          IsSuccess = false,
          Message = "User not found",
        };
      }

      user.Name = vendorAccountDetails.Name;
      user.Email = vendorAccountDetails.Email;
      user.NIC = vendorAccountDetails.NIC;

      var userUpdateResult = await _userManager.UpdateAsync(user);

      if (!userUpdateResult.Succeeded)
      {
        return new UpdateVendorResponse
        {
          IsSuccess = false,
          Message = $"User update failed: {userUpdateResult.Errors.FirstOrDefault()?.Description}"
        };
      }

      vendor.VendorName = vendorDetails.VendorName;
      vendor.VendorEmail = vendorDetails.VendorEmail;
      vendor.VendorPhone = vendorDetails.VendorPhone;
      vendor.VendorAddress = vendorDetails.VendorAddress;
      vendor.VendorCity = vendorDetails.VendorCity;

      await _vendors.ReplaceOneAsync(v => v.Id == Guid.Parse(id), vendor);

      return new UpdateVendorResponse
      {
        IsSuccess = true,
        Message = "Vendor updated successfully",
      };
    }
    catch (Exception e)
    {
      return new UpdateVendorResponse
      {
        IsSuccess = false,
        Message = $"Vendor update failed: {e.Message}"
      };
    }
  }

  public async Task<DeleteVendorResponse> DeleteVendorAsync(string id)
  {
    try
    {
      var vendor = await _vendors.Find(v => v.Id == Guid.Parse(id)).FirstOrDefaultAsync();

      if (vendor == null)
      {
        return new DeleteVendorResponse
        {
          IsSuccess = false,
          Message = "Vendor not found",
        };
      }

      var user = await _userManager.FindByIdAsync(id);

      if (user == null)
      {
        return new DeleteVendorResponse
        {
          IsSuccess = false,
          Message = "User not found",
        };
      }

      var userDeleteResult = await _userManager.DeleteAsync(user);

      if (!userDeleteResult.Succeeded)
      {
        return new DeleteVendorResponse
        {
          IsSuccess = false,
          Message = $"User deletion failed: {userDeleteResult.Errors.FirstOrDefault()?.Description}"
        };
      }

      await _vendors.DeleteOneAsync(v => v.Id == Guid.Parse(id));

      return new DeleteVendorResponse
      {
        IsSuccess = true,
        Message = "Vendor deleted successfully",
      };
    }
    catch (Exception e)
    {
      return new DeleteVendorResponse
      {
        IsSuccess = false,
        Message = $"Vendor deletion failed: {e.Message}"
      };
    }
  }

  public async Task<UpdateVendorAccountStatusResponse> UpdateVendorAccountStatusAsync(string id, AccountStatus newStatus)
  {
    try
    {
      var user = await _userManager.FindByIdAsync(id);

      if (user == null)
      {
        return new UpdateVendorAccountStatusResponse
        {
          IsSuccess = false,
          Message = "User not found",
        };
      }

      user.Status = newStatus;

      var updatedUser = await _userManager.UpdateAsync(user);

      if (!updatedUser.Succeeded)
      {
        return new UpdateVendorAccountStatusResponse
        {
          IsSuccess = false,
          Message = $"User status update failed: {updatedUser.Errors.FirstOrDefault()?.Description}"
        };
      }
      return new UpdateVendorAccountStatusResponse
      {
        IsSuccess = true,
        Message = "User status updated successfully",
      };
    }
    catch (Exception e)
    {
      return new UpdateVendorAccountStatusResponse
      {
        IsSuccess = false,
        Message = $"User status update failed: {e.Message}"
      };
    }

  }
}
