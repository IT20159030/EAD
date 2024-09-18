

using Backend.Dtos;
using Microsoft.AspNetCore.Identity;

namespace Backend.Services;
public interface IMobileUserAuthService
{
    Task<MRegisterResponse> RegisterUserAsync(MRegisterRequest request);
    Task<LoginResponse> LoginAsync(string email, string password);
}