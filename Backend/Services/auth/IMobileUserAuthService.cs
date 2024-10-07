using Backend.Dtos;

namespace Backend.Services;

/*
*  Interface for the mobile user authentication service
*/
public interface IMobileUserAuthService
{
    Task<MRegisterResponse> RegisterUserAsync(MRegisterRequest request);
    Task<MLoginResponse> LoginAsync(string email, string password);
    Task<MUserResponse> UserDetailsAsync(string userId);
    Task<MUserResponse> DeactivateUserAsync(string userId);
    Task<MUpdateUserResponse> UpdateUserAsync(string userId, MUpdateUserRequest request);
    Task<MAddressResponse> GetAddressAsync(string userId);
    Task<MAddressResponse> UpdateAddressAsync(string userId, MAddAddressRequest request);

}