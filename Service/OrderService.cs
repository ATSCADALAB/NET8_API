using AutoMapper;
using Contracts;
using Entities.Exceptions.Customer;
using Entities.Exceptions.Order;
using Entities.Models;
using MailKit.Search;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Service.Contracts;
using Shared.DataTransferObjects.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Service
{
    internal sealed class OrderService : BaseService, IOrderService
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
        public async Task<IEnumerable<OrderDto>> GetOrdersByFilters(DateTime? startDate, DateTime? endDate, string productCode,int status)
        {
            try
            {
                var orders = await _repository.Order.GetOrdersAsync(trackChanges: false);

                //// Lọc theo product code
                //if (productCode.IsNullOrEmpty())
                //{
                //    orders = orders.Where(o => o.LineID == lineID);
                //}
                // Lọc theo Status
                if(status != null)
                {
                    orders = orders.Where(o => o.Status == status);
                }
                
                // Chuyển IQueryable sang List trước khi áp dụng bộ lọc
                var filteredOrders = GetItemsByDateRangeAsync<Order>(orders.AsQueryable(), startDate, endDate);
                

                return _mapper.Map<IEnumerable<OrderDto>>(filteredOrders.ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching orders: {ex.Message}");
                return null;
            }

        }
        // Lấy tất cả đơn hàng
        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            try
            {
                var orders = await _repository.Order.GetOrdersAsync(trackChanges: false);

                return orders;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching orders: {ex.Message}");
                return null;
            }
        }
        

        // Lấy chi tiết 1 đơn hàng
        public async Task<Order> GetOrderByIdAsync(Guid orderId)
        {
            try
            {
                var order = await _repository.Order.GetOrderByIdAsync(orderId, trackChanges: false);
                
                return order;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching order by id: {ex.Message}");
                return null;
            }
        }
        // Lấy chi tiết 1 đơn hàng theo mã đơn hàng
        public async Task<OrderDto> GetOrderByOrderCodeAsync(string code)
        {
            try
            {
                var order = await _repository.Order.GetOrderByOrderCodeAsync(code, trackChanges: false);
                
                return _mapper.Map<OrderDto>(order);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching order by id: {ex.Message}");
                return null;
            }
        }

        // Tạo đơn hàng
        public async Task<OrderDto> CreateOrderAsync(OrderForCreationDto orderForCreationDto)
        {
            if (orderForCreationDto == null)
            {
                throw new ArgumentNullException(nameof(orderForCreationDto), "Order data must not be null.");
            }

            try
            {
                _logger.LogError($"Attempting to create an order with data: {@orderForCreationDto}");

                var orderEntity = _mapper.Map<Order>(orderForCreationDto);

                // Kiểm tra các trường string trong orderEntity và gán giá trị rỗng nếu null
                foreach (var property in typeof(Order).GetProperties())
                {
                    if (property.PropertyType == typeof(string))
                    {
                        var value = property.GetValue(orderEntity) as string;
                        if (value == null)
                        {
                            property.SetValue(orderEntity, string.Empty);
                        }
                    }
                }

                _repository.Order.CreateOrder(orderEntity);
                await _repository.SaveAsync();

                var orderToReturn = _mapper.Map<OrderDto>(orderEntity);

                _logger.LogError($"Attempting to create an order with data: {@orderForCreationDto}");
                return orderToReturn;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Attempting to create an order with data: {@orderForCreationDto}");
                throw new Exception("An error occurred while creating the order. Please try again later.", ex);
            }
        }


        // Cập nhật đơn hàng
        public async Task<OrderDto> UpdateOrderAsync(Guid orderId, OrderForUpdateDto orderForUpdateDto)
        {
            try
            {
                var orderEntity = await _repository.Order.GetOrderByIdAsync(orderId, trackChanges: true);
                if (orderEntity == null)
                {
                    return null;
                }

                _mapper.Map(orderForUpdateDto, orderEntity);
                _repository.Order.UpdateOrder(orderEntity);
                await _repository.SaveAsync();

                return _mapper.Map<OrderDto>(orderEntity);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating order: {ex.Message}");
                return null;
            }
        }
        public async Task UpdateStatusAsync(Guid orderId, int newStatus)
        {
            await _repository.Order.UpdateStatusAsync(orderId, newStatus);
            await _repository.SaveAsync();
        }
        // Nhập khẩu (import) đơn hàng từ file hoặc nguồn bên ngoài
        public async Task<IEnumerable<OrderDto>> ImportOrdersAsync(List<OrderForCreationDto> orders)
        {
            try
            {
                var orderEntities = _mapper.Map<IEnumerable<Order>>(orders);

                foreach (var order in orderEntities)
                {
                    // Lấy thông tin Distributor từ DB
                    var distributor = await _repository.Distributor.GetDistributorAsync(order.DistributorID, trackChanges: true);
                    if (distributor != null)
                    {
                        order.Distributor = distributor;
                    }

                    // Lấy thông tin ProductInfo từ DB
                    var product = await _repository.ProductInformation.GetProductInformationAsync(order.ProductInformationID, trackChanges: true);
                    if (product != null)
                    {
                        order.ProductInformation = product;
                    }

                    _repository.Order.CreateOrder(order);
                }

                await _repository.SaveAsync();

                return _mapper.Map<IEnumerable<OrderDto>>(orderEntities);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error importing orders: {ex.Message}");
                return null;
            }
        }

        public async Task DeleteOrderAsync(Guid customerId, bool trackChanges)
        {
            var order = await GetOrderAndCheckIfItExists(customerId, trackChanges);

            _repository.Order.DeleteOrder(order);
            await _repository.SaveAsync();
        }
        private async Task<Order> GetOrderAndCheckIfItExists(Guid id, bool trackChanges)
        {
            var customer = await _repository.Order.GetOrderByIdAsync(id, trackChanges);
            if (customer is null)
                throw new OrderNotFoundException(id);

            return customer;
        }
    }
}
