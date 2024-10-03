using Backend.Dtos;
using Backend.Models;

public interface ICustomerManagementService
{
  Task<GetAllCustomerResponse> GetAllCustomerAsync();
  Task<GetCustomerByIdResponse> GetCustomerByIdAsync(string id);
  Task<CreateCustomerResponse> CreateCustomerAsync(CreateCustomerRequest request);

  Task<UpdateCustomerResponse> UpdateCustomerAsync(string id, UpdateCustomerRequest request);

  Task<DeleteCustomerResponse> DeleteCustomerAsync(string id);

  Task<UpdateCustomerAccountStatusResponse> UpdateCustomerAccountStatusAsync(string id, AccountStatus request);
}