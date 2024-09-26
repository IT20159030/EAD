using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Backend.Dtos;
using Backend.Models;
using MongoDB.Driver;
using AutoMapper;

namespace Backend.Services
{
    public class CancellationRequestService : ICancellationRequestService
    {
        private readonly IMongoCollection<CancellationRequest> _cancellationRequests;
        private readonly IMapper _mapper;

        public CancellationRequestService(IMongoClient mongoClient, IMapper mapper)
        {
            var database = mongoClient.GetDatabase("ecommerce_ead");
            _cancellationRequests = database.GetCollection<CancellationRequest>("cancellationRequests");
            _mapper = mapper;
        }

        public async Task<IEnumerable<CancellationRequestDto>> GetAllRequestsAsync()
        {
            var requests = await _cancellationRequests.Find(_ => true).ToListAsync();
            return _mapper.Map<IEnumerable<CancellationRequestDto>>(requests);
        }

        public async Task<CancellationRequestDto> CreateRequestAsync(CreateCancellationRequest createRequest)
        {
            var cancellationRequest = _mapper.Map<CancellationRequest>(createRequest);
            cancellationRequest.RequestDate = DateTime.UtcNow;
            cancellationRequest.Status = "Pending";

            await _cancellationRequests.InsertOneAsync(cancellationRequest);
            return _mapper.Map<CancellationRequestDto>(cancellationRequest);
        }

        public async Task ProcessRequestAsync(string orderId, ProcessCancellationRequest processRequest)
        {
            var filter = Builders<CancellationRequest>.Filter.Eq(cr => cr.OrderId, orderId);
            var update = Builders<CancellationRequest>.Update
                .Set(cr => cr.Status, processRequest.Status)
                .Set(cr => cr.ProcessedBy, processRequest.ProcessedBy)
                .Set(cr => cr.ProcessedDate, DateTime.UtcNow)
                .Set(cr => cr.DecisionNote, processRequest.DecisionNote);

            var result = await _cancellationRequests.UpdateOneAsync(filter, update);

            if (result.MatchedCount == 0)
            {
                throw new Exception($"Cancellation request for OrderId {orderId} not found.");
            }
        }
    }
}
