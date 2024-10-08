using Backend.Dtos;
using Backend.Models;

/*
*  Interface for the staff management service
*/

public interface IStaffManagementService
{
  Task<GetAllStaffResponse> GetAllStaffAsync();
  Task<CreateStaffResponse> CreateStaffAsync(CreateStaffRequest request);

  Task<UpdateStaffResponse> UpdateStaffAsync(String id, UpdateStaffRequest request);

  Task<DeleteStaffResponse> DeleteStaffAsync(String id);

  Task<UpdateStaffAccountStatusResponse> UpdateStaffAccountStatusAsync(string id, AccountStatus request);
}