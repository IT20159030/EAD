using Backend.Dtos;
using Backend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend.Services
{
    public interface ICancellationRequestService
    {
        Task<IEnumerable<CancellationRequestDto>> GetAllRequestsAsync();
        Task<CancellationRequestDto> CreateRequestAsync(CreateCancellationRequest createRequest);
        Task ProcessRequestAsync(string orderId, ProcessCancellationRequest processRequest);
    }
}
