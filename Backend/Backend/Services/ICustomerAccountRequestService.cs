using Backend.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend.Services
{
    public interface ICustomerAccountRequestService
    {
        Task<IEnumerable<CustomerAccountRequestDto>> GetAllRequestsAsync();
        Task<CustomerAccountRequestDto> CreateRequestAsync(CreateCustomerAccountRequestDto createRequestDto);
        Task ProcessRequestAsync(string customerId, ProcessCustomerAccountRequestDto processRequestDto);
    }
}
