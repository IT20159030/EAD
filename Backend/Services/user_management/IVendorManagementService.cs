using Backend.Dtos;
using Backend.Models;

/*
*  Interface for the vendor management service
*/
public interface IVendorManagementService
{
  Task<GetAllVendorResponse> GetAllVendorAsync();
  Task<CreateVendorResponse> CreateVendorAsync(CreateVendorRequest request);

  Task<UpdateVendorResponse> UpdateVendorAsync(string id, UpdateVendorRequest request);

  Task<DeleteVendorResponse> DeleteVendorAsync(string id);

  Task<UpdateVendorAccountStatusResponse> UpdateVendorAccountStatusAsync(string id, AccountStatus newStatus);
}