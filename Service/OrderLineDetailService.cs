using AutoMapper;
using Contracts;
using Entities.Exceptions.Customer;
using Entities.Models;
using MailKit.Search;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Service.Contracts;
using Shared.DataTransferObjects;
using Shared.DataTransferObjects.OrderLineDetail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Service
{
    internal sealed class OrderLineDetailService : BaseService, IOrderLineDetailService
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
        public async Task<IEnumerable<OrderLineDetail>> GetAllOrderLineDetailsAsync()
        {
            try
            {
                var OrderLineDetails = await _repository.OrderLineDetail.GetOrderLineDetailsAsync(trackChanges: false);

                return OrderLineDetails;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching OrderLineDetails: {ex.Message}");
                return null;
            }
        }


        // Lấy chi tiết 1 đơn hàng
        public async Task<OrderLineDetail> GetOrderLineDetailByIdAsync(Guid OrderlId)
        {
            try
            {
                var OrderLineDetail = await _repository.OrderLineDetail.GetOrderLineDetailByIdAsync(OrderlId, trackChanges: false);

                return OrderLineDetail;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching OrderLineDetail by id: {ex.Message}");
                return null;
            }
        }
        public async Task<OrderLineDetail> GetOrderLineDetailByLineIDAsync(long lineID)
        {
            try
            {
                var OrderLineDetail = await _repository.OrderLineDetail.GetOrderLineDetailByLineIDAsync(lineID, trackChanges: false);

                return OrderLineDetail;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching OrderLineDetail by id: {ex.Message}");
                return null;
            }
        }
        // Tạo đơn hàng
        public async Task<OrderLineDetailDto> CreateOrderLineDetailAsync(OrderLineDetailForCreationDto OrderLineDetailForCreationDto)
        {
            var maxSequenceNumber = await _repository.OrderLineDetail.GetMaxSequenceNumberByLineAsync(OrderLineDetailForCreationDto.Line);
            var orderLineDetail = new OrderLineDetail
            {
                OrderId = OrderLineDetailForCreationDto.OrderId,
                Line = OrderLineDetailForCreationDto.Line,
                SequenceNumber = maxSequenceNumber + 1 // Tăng dần
            };
            try
            {
                _logger.LogError($"Attempting to create an OrderLineDetail with data: {@OrderLineDetailForCreationDto}");
                _repository.OrderLineDetail.CreateOrderLineDetail(orderLineDetail);
                await _repository.SaveAsync();

                _logger.LogError($"Attempting to create an OrderLineDetail with data: {@OrderLineDetailForCreationDto}");
                return _mapper.Map<OrderLineDetailDto>(orderLineDetail);
                ;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Attempting to create an OrderLineDetail with data: {@OrderLineDetailForCreationDto}");
                throw new Exception("An error occurred while creating the OrderLineDetail. Please try again later.", ex);
            }
        }


        // Cập nhật đơn hàng
        public async Task<OrderLineDetailDto> UpdateOrderLineDetailAsync(Guid OrderLineDetailId, OrderLineDetailForUpdateDto OrderLineDetailForUpdateDto)
        {
            try
            {
                var OrderLineDetailEntity = await _repository.OrderLineDetail.GetOrderLineDetailByIdAsync(OrderLineDetailId, trackChanges: true);
                if (OrderLineDetailEntity == null)
                {
                    return null;
                }

                _mapper.Map(OrderLineDetailForUpdateDto, OrderLineDetailEntity);
                _repository.OrderLineDetail.UpdateOrderLineDetail(OrderLineDetailEntity);
                await _repository.SaveAsync();

                return _mapper.Map<OrderLineDetailDto>(OrderLineDetailEntity);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating OrderLineDetail: {ex.Message}");
                return null;
            }
        }
        public async Task DeleteOrderLineDetailAsync(Guid customerId, bool trackChanges)
        {
            try
            {
                // Tải OrderLineDetail, không cần theo dõi nếu chỉ để xóa
                var orderLineDetail = await GetOrderLineDetailAndCheckIfItExists(customerId, trackChanges: false);

                // Xóa OrderLineDetail
                _repository.OrderLineDetail.DeleteOrderLineDetail(orderLineDetail);
                await _repository.SaveAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting OrderLineDetail with ID {customerId}: {ex.Message}");
                throw; // Ném ngoại lệ để tầng trên xử lý
            }
        }

        private async Task<OrderLineDetail> GetOrderLineDetailAndCheckIfItExists(Guid id, bool trackChanges)
        {
            var orderLineDetail = await _repository.OrderLineDetail.GetOrderLineDetailByIdAsync(id, trackChanges);
            if (orderLineDetail == null)
            {
                throw new Exception($"OrderLineDetail with ID {id} not found.");
            }
            return orderLineDetail;
        }
    }
}
