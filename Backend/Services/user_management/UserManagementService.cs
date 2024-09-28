using Backend.Dtos;
using Backend.Models;
using Backend.Utils;
using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.OpenApi.Extensions;

namespace Backend.Services;

public class UserManagementService : IUserManagementService
{
  private readonly IMongoCollection<User> _users;
  private readonly UserManager<User> _userManager;
  private readonly RoleManager<ApplicationRole> _roleManager;



  public UserManagementService(MongoDBService mongoDBService, UserManager<User> userManager, RoleManager<ApplicationRole> roleManager
  )
  {
    _users = mongoDBService.Database.GetCollection<User>("users");
    _userManager = userManager;
    _roleManager = roleManager;


  }

  public async Task<CreateUserResponse> CreateUserAsync(CreateUserRequest request)
  {
    var user = await _userManager.FindByEmailAsync(request.Email);
    if (user != null)
    {
      return new CreateUserResponse
      {
        IsSuccess = false,
        Message = "User already exists"
      };
    }

    var newUser = new User
    {
      Name = request.Name,
      Email = request.Email,
      UserName = request.Email,
      Status = AccountStatus.Active,
      UpdatedAt = DateTime.Now,
      CreatedAt = DateTime.Now,
      ConcurrencyStamp = Guid.NewGuid().ToString()
    };

    var result = await _userManager.CreateAsync(newUser, request.Password);

    if (!result.Succeeded)
    {
      return new CreateUserResponse
      {
        IsSuccess = false,
        Message = $"User creation failed: {result.Errors.FirstOrDefault()?.Description}"
      };
    }

    var addUserToRoleResult = await _userManager.AddToRoleAsync(newUser, request.Role);
    if (!addUserToRoleResult.Succeeded)
    {
      return new CreateUserResponse
      {
        IsSuccess = false,
        Message = $"User creation failed: {addUserToRoleResult.Errors.FirstOrDefault()?.Description}"
      };
    }

    return new CreateUserResponse
    {
      IsSuccess = result.Succeeded,
      Message = "User created successfully"
    };
  }

  public async Task<DeleteUserResponse> DeleteUserAsync(string userId)
  {
    throw new NotImplementedException();
  }

  public async Task<GetUserResponse> GetUserAsync(string userId)
  {
    throw new NotImplementedException();
  }

  public async Task<UpdateUserResponse> UpdateUserAsync(string userId, UpdateUserRequest request)
  {
    throw new NotImplementedException();
  }

  public async Task<GetUsersResponse> GetUsersAsync(string role)
  {
    try
    {
      // Find the role by name
      var roleDocument = await _roleManager.FindByNameAsync(role);

      if (roleDocument == null)
      {
        return new GetUsersResponse { IsSuccess = false, Message = $"Role '{role}' not found." };
      }

      // Find users in the role
      var users = await _userManager.GetUsersInRoleAsync(role);

      // Map users to UserDto
      var userDtos = users.Select(u => new UserDto
      {
        Id = u.Id.ToString(),
        Name = u.Name,
        Email = u.Email,
        Status = u.Status.ToString().ToLower(),
        CreatedAt = u.CreatedAt
        // Add other properties as needed
      }).ToList();

      return new GetUsersResponse
      {
        IsSuccess = true,
        Users = userDtos
      };
    }
    catch (Exception ex)
    {
      // Log the exception
      Console.WriteLine($"Error in GetUsersAsync: {ex.Message}");
      return new GetUsersResponse
      {
        IsSuccess = false,
        Message = "An error occurred while retrieving users."
      };
    }
  }
}