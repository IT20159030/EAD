using Backend.Dtos;
using Microsoft.AspNetCore.Identity;

namespace Backend.Services;

/*
*  Interface for the web user authentication service
*/
public interface IWebUserAuthService
{
    Task<LoginResponse> LoginAsync(string email, string password);
    Task<ProfileResponse> GetProfileAsync(string userId);
    Task<ProfileResponse> UpdateProfileAsync(string userId, UpdateProfileRequest request);
}