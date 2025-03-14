using AutoMapper;
using Contracts;
using Entities.Exceptions.OrderLineDetail;
using Entities.Models;
using Service.Contracts;
using Shared.DataTransferObjects.OrderLineDetail;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service
{
    internal sealed class OrderLineDetailService : IOrderLineDetailService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;

        public OrderLineDetailService(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<IEnumerable<OrderLineDetailDto>> GetAllOrderLineDetailsAsync(bool trackChanges)
        {
            var orderLineDetails = await _repository.OrderLineDetail.GetAllOrderLineDetailsAsync(trackChanges);
            var orderLineDetailsDto = _mapper.Map<IEnumerable<OrderLineDetailDto>>(orderLineDetails);
            return orderLineDetailsDto;
        }

        public async Task<OrderLineDetailDto> GetOrderLineDetailAsync(int orderLineDetailId, bool trackChanges)
        {
            var orderLineDetail = await GetOrderLineDetailAndCheckIfItExists(orderLineDetailId, trackChanges);
            var orderLineDetailDto = _mapper.Map<OrderLineDetailDto>(orderLineDetail);
            return orderLineDetailDto;
        }

        public async Task<IEnumerable<OrderLineDetailDto>> GetOrderLineDetailsByOrderAsync(Guid orderId, bool trackChanges)
        {
            var orderLineDetails = await _repository.OrderLineDetail.GetOrderLineDetailsByOrderIdAsync(orderId, trackChanges);
            var orderLineDetailsDto = _mapper.Map<IEnumerable<OrderLineDetailDto>>(orderLineDetails);
            return orderLineDetailsDto;
        }

        public async Task<IEnumerable<OrderLineDetailDto>> GetOrderLineDetailsByLineAsync(int lineId, bool trackChanges)
        {
            var orderLineDetails = await _repository.OrderLineDetail.GetOrderLineDetailsByLineIdAsync(lineId, trackChanges);
            var orderLineDetailsDto = _mapper.Map<IEnumerable<OrderLineDetailDto>>(orderLineDetails);
            return orderLineDetailsDto;
        }

        public async Task<OrderLineDetailDto> CreateOrderLineDetailAsync(OrderLineDetailForCreationDto orderLineDetail)
        {
            if (orderLineDetail == null)
                throw new ArgumentNullException(nameof(orderLineDetail), "OrderLineDetailForCreationDto cannot be null.");

            var orderLineDetailEntity = _mapper.Map<OrderLineDetail>(orderLineDetail);
            _repository.OrderLineDetail.CreateOrderLineDetail(orderLineDetailEntity);
            await _repository.SaveAsync();

            var orderLineDetailToReturn = _mapper.Map<OrderLineDetailDto>(orderLineDetailEntity);
            return orderLineDetailToReturn;
        }

        public async Task UpdateOrderLineDetailAsync(int orderLineDetailId, OrderLineDetailForUpdateDto orderLineDetailForUpdate, bool trackChanges)
        {
            var orderLineDetail = await GetOrderLineDetailAndCheckIfItExists(orderLineDetailId, trackChanges);
            _mapper.Map(orderLineDetailForUpdate, orderLineDetail);
            await _repository.SaveAsync();
        }

        public async Task DeleteOrderLineDetailAsync(int orderLineDetailId, bool trackChanges)
        {
            var orderLineDetail = await GetOrderLineDetailAndCheckIfItExists(orderLineDetailId, trackChanges);
            _repository.OrderLineDetail.DeleteOrderLineDetail(orderLineDetail);
            await _repository.SaveAsync();
        }

        private async Task<OrderLineDetail> GetOrderLineDetailAndCheckIfItExists(int id, bool trackChanges)
        {
            var orderLineDetail = await _repository.OrderLineDetail.GetOrderLineDetailByIdAsync(id, trackChanges);
            if (orderLineDetail is null)
                throw new OrderLineDetailNotFoundException(id);
            return orderLineDetail;
        }
    }
}