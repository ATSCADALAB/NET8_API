using AutoMapper;
using Contracts;
using Entities.Exceptions.OrderDetail;
using Entities.Models;
using Service.Contracts;
using Shared.DataTransferObjects.OrderDetail;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service
{
    internal sealed class OrderDetailService : IOrderDetailService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;

        public OrderDetailService(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<IEnumerable<OrderDetailDto>> GetAllOrderDetailsAsync(bool trackChanges)
        {
            var orderDetails = await _repository.OrderDetail.GetAllOrderDetailsAsync(trackChanges);
            var orderDetailsDto = _mapper.Map<IEnumerable<OrderDetailDto>>(orderDetails);
            return orderDetailsDto;
        }

        public async Task<OrderDetailDto> GetOrderDetailAsync(int orderDetailId, bool trackChanges)
        {
            var orderDetail = await GetOrderDetailAndCheckIfItExists(orderDetailId, trackChanges);
            var orderDetailDto = _mapper.Map<OrderDetailDto>(orderDetail);
            return orderDetailDto;
        }

        public async Task<IEnumerable<OrderDetailDto>> GetOrderDetailsByOrderAsync(Guid orderId, bool trackChanges)
        {
            var orderDetails = await _repository.OrderDetail.GetOrderDetailsByOrderIdAsync(orderId, trackChanges);
            var orderDetailsDto = _mapper.Map<IEnumerable<OrderDetailDto>>(orderDetails);
            return orderDetailsDto;
        }

        public async Task<IEnumerable<OrderDetailDto>> GetOrderDetailsByProductAsync(int productInformationId, bool trackChanges)
        {
            var orderDetails = await _repository.OrderDetail.GetOrderDetailsByProductAsync(productInformationId, trackChanges);
            var orderDetailsDto = _mapper.Map<IEnumerable<OrderDetailDto>>(orderDetails);
            return orderDetailsDto;
        }

        public async Task<OrderDetailDto> CreateOrderDetailAsync(OrderDetailForCreationDto orderDetail)
        {
            if (orderDetail == null)
                throw new ArgumentNullException(nameof(orderDetail), "OrderDetailForCreationDto cannot be null.");

            var orderDetailEntity = _mapper.Map<OrderDetail>(orderDetail);
            _repository.OrderDetail.CreateOrderDetail(orderDetailEntity);
            await _repository.SaveAsync();

            var orderDetailToReturn = _mapper.Map<OrderDetailDto>(orderDetailEntity);
            return orderDetailToReturn;
        }

        public async Task UpdateOrderDetailAsync(int orderDetailId, OrderDetailForUpdateDto orderDetailForUpdate, bool trackChanges)
        {
            var orderDetail = await GetOrderDetailAndCheckIfItExists(orderDetailId, trackChanges);
            _mapper.Map(orderDetailForUpdate, orderDetail);
            await _repository.SaveAsync();
        }

        public async Task DeleteOrderDetailAsync(int orderDetailId, bool trackChanges)
        {
            var orderDetail = await GetOrderDetailAndCheckIfItExists(orderDetailId, trackChanges);
            _repository.OrderDetail.DeleteOrderDetail(orderDetail);
            await _repository.SaveAsync();
        }

        private async Task<OrderDetail> GetOrderDetailAndCheckIfItExists(int id, bool trackChanges)
        {
            var orderDetail = await _repository.OrderDetail.GetOrderDetailByIdAsync(id, trackChanges);
            if (orderDetail is null)
                throw new OrderDetailNotFoundException(id);
            return orderDetail;
        }
    }
}