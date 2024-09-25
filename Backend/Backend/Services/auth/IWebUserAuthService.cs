

using Backend.Dtos;
using Microsoft.AspNetCore.Identity;

namespace Backend.Services;
public interface IWebUserAuthService
{
    Task<CreateUserResponse> CreateUserAsync(CreateUserRequest request);
    Task<LoginResponse> LoginAsync(string email, string password);
}