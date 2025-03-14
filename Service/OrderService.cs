using AutoMapper;
using Contracts;
using Entities.Exceptions.Order;
using Entities.Models;
using Service.Contracts;
using Shared.DataTransferObjects.Order;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service
{
    internal sealed class OrderService : IOrderService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;

        public OrderService(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync(bool trackChanges)
        {
            var orders = await _repository.Order.GetAllOrdersAsync(trackChanges);
            var ordersDto = _mapper.Map<IEnumerable<OrderDto>>(orders);
            return ordersDto;
        }

        public async Task<OrderDto> GetOrderAsync(Guid orderId, bool trackChanges)
        {
            var order = await GetOrderAndCheckIfItExists(orderId, trackChanges);
            var orderDto = _mapper.Map<OrderDto>(order);
            return orderDto;
        }

        public async Task<OrderDto> GetOrderByCodeAsync(string orderCode, bool trackChanges)
        {
            var order = await _repository.Order.GetOrderByCodeAsync(orderCode, trackChanges);
            if (order is null)
                throw new OrderNotFoundException(Guid.Empty); // Guid không quan trọng vì dùng Code

            var orderDto = _mapper.Map<OrderDto>(order);
            return orderDto;
        }

        public async Task<IEnumerable<OrderDto>> GetOrdersByDistributorAsync(int distributorId, bool trackChanges)
        {
            var orders = await _repository.Order.GetOrdersByDistributorAsync(distributorId, trackChanges);
            var ordersDto = _mapper.Map<IEnumerable<OrderDto>>(orders);
            return ordersDto;
        }

        public async Task<IEnumerable<OrderDto>> GetOrdersByExportDateAsync(DateTime exportDate, bool trackChanges)
        {
            var orders = await _repository.Order.GetOrdersByExportDateAsync(exportDate, trackChanges);
            var ordersDto = _mapper.Map<IEnumerable<OrderDto>>(orders);
            return ordersDto;
        }

        public async Task<OrderDto> CreateOrderAsync(OrderForCreationDto order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order), "OrderForCreationDto cannot be null.");

            var orderEntity = _mapper.Map<Order>(order);
            _repository.Order.CreateOrder(orderEntity);
            await _repository.SaveAsync();

            var orderToReturn = _mapper.Map<OrderDto>(orderEntity);
            return orderToReturn;
        }

        public async Task UpdateOrderAsync(Guid orderId, OrderForUpdateDto orderForUpdate, bool trackChanges)
        {
            var order = await GetOrderAndCheckIfItExists(orderId, trackChanges);
            _mapper.Map(orderForUpdate, order);
            await _repository.SaveAsync();
        }

        public async Task DeleteOrderAsync(Guid orderId, bool trackChanges)
        {
            var order = await GetOrderAndCheckIfItExists(orderId, trackChanges);
            _repository.Order.DeleteOrder(order);
            await _repository.SaveAsync();
        }

        private async Task<Order> GetOrderAndCheckIfItExists(Guid id, bool trackChanges)
        {
            var order = await _repository.Order.GetOrderByIdAsync(id, trackChanges);
            if (order is null)
                throw new OrderNotFoundException(id);
            return order;
        }
    }
}