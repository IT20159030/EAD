

using Backend.Dtos;
using Microsoft.AspNetCore.Identity;

namespace Backend.Services;
public interface IWebUserAuthService
{
    Task<RegisterResponse> RegisterUserAsync(RegisterRequest request);
    Task<LoginResponse> LoginAsync(string email, string password);
}