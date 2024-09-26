using Backend.Dtos;
using Microsoft.AspNetCore.Identity;

namespace Backend.Services;
public interface IUserManagementService
{
  Task<CreateUserResponse> CreateUserAsync(CreateUserRequest request);
  Task<GetUserResponse> GetUserAsync(string userId);
  Task<UpdateUserResponse> UpdateUserAsync(string userId, UpdateUserRequest request);
  Task<DeleteUserResponse> DeleteUserAsync(string userId);

  Task<GetUsersResponse> GetUsersAsync(string role);
}