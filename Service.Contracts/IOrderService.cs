using Shared.DataTransferObjects.Order;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Contracts
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderDto>> GetAllOrdersAsync(bool trackChanges);
        Task<OrderDto> GetOrderAsync(Guid orderId, bool trackChanges);
        Task<OrderDto> GetOrderByCodeAsync(string orderCode, bool trackChanges);
        Task<IEnumerable<OrderDto>> GetOrdersByDistributorAsync(int distributorId, bool trackChanges);
        Task<IEnumerable<OrderDto>> GetOrdersByExportDateAsync(DateTime exportDate, bool trackChanges);
        Task<OrderDto> CreateOrderAsync(OrderForCreationDto order);
        Task UpdateOrderAsync(Guid orderId, OrderForUpdateDto orderForUpdate, bool trackChanges);
        Task DeleteOrderAsync(Guid orderId, bool trackChanges);
        Task<IEnumerable<OrderWithDetailsDto>> GetOrdersByFilterAsync(
            DateTime startDate, // Bắt buộc
            DateTime endDate,   // Bắt buộc
            int? distributorId, // Nếu null thì lấy tất cả
            int? areaId,        // Nếu null thì lấy tất cả
            int? productInformationId, // Nếu null thì lấy tất cả
            int? status,        // Nếu null thì lấy tất cả
            bool trackChanges);
    }
}