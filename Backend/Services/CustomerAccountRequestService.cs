using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Backend.Dtos;
using Backend.Models;
using MongoDB.Driver;

namespace Backend.Services
{
    public class CustomerAccountRequestService : ICustomerAccountRequestService
    {
        private readonly IMongoCollection<CustomerAccountRequest> _customerRequests;
        private readonly IMapper _mapper;

        public CustomerAccountRequestService(IMongoClient mongoClient, IMapper mapper)
        {
            var database = mongoClient.GetDatabase("ecommerce_ead");
            _customerRequests = database.GetCollection<CustomerAccountRequest>("customerRequests");
            _mapper = mapper;
        }

        public async Task<IEnumerable<CustomerAccountRequestDto>> GetAllRequestsAsync()
        {
            var requests = await _customerRequests.Find(_ => true).ToListAsync();
            return _mapper.Map<IEnumerable<CustomerAccountRequestDto>>(requests);
        }

        public async Task<CustomerAccountRequestDto> CreateRequestAsync(CreateCustomerAccountRequestDto createRequestDto)
        {
            var customerRequest = _mapper.Map<CustomerAccountRequest>(createRequestDto);
            await _customerRequests.InsertOneAsync(customerRequest);
            return _mapper.Map<CustomerAccountRequestDto>(customerRequest);
        }

        public async Task ProcessRequestAsync(string customerId, ProcessCustomerAccountRequestDto processRequestDto)
        {
            var filter = Builders<CustomerAccountRequest>.Filter.Eq(r => r.CustomerId, customerId);
            var update = Builders<CustomerAccountRequest>.Update
                .Set(r => r.Status, processRequestDto.Status)
                .Set(r => r.ProcessedBy, processRequestDto.ProcessedBy)
                .Set(r => r.ProcessedDate, processRequestDto.ProcessedDate);

            var result = await _customerRequests.UpdateOneAsync(filter, update);

            if (result.MatchedCount == 0)
            {
                throw new Exception($"Customer account request for CustomerId {customerId} not found.");
            }
        }
    }
}
