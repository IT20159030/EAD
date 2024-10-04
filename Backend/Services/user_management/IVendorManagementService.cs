using Backend.Dtos;
using Backend.Models;

public interface IVendorManagementService
{
  // Task<GetAllVendorResponse> GetAllVendorAsync();
  Task<CreateVendorResponse> CreateVendorAsync(CreateVendorRequest request);

  // Task<UpdateVendorResponse> UpdateVendorAsync(String id, UpdateVendorRequest request);

  // Task<DeleteVendorResponse> DeleteVendorAsync(String id);

  // Task<UpdateVendorAccountStatusResponse> UpdateVendorAccountStatusAsync(string id, AccountStatus request);
}