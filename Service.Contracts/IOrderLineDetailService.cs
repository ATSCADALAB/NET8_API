using Shared.DataTransferObjects.OrderLineDetail;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Contracts
{
    public interface IOrderLineDetailService
    {
        Task<IEnumerable<OrderLineDetailDto>> GetAllOrderLineDetailsAsync(bool trackChanges);
        Task<OrderLineDetailDto> GetOrderLineDetailAsync(int orderLineDetailId, bool trackChanges);
        Task<IEnumerable<OrderLineDetailDto>> GetOrderLineDetailsByOrderAsync(Guid orderId, bool trackChanges);
        Task<IEnumerable<OrderLineDetailDto>> GetOrderLineDetailsByLineAsync(int lineId, bool trackChanges);
        Task<OrderLineDetailDto> CreateOrderLineDetailAsync(OrderLineDetailForCreationDto orderLineDetail);
        Task UpdateOrderLineDetailAsync(int orderLineDetailId, OrderLineDetailForUpdateDto orderLineDetailForUpdate, bool trackChanges);
        Task DeleteOrderLineDetailAsync(int orderLineDetailId, bool trackChanges);
        Task<IEnumerable<RunningOrderDto>> GetRunningOrdersByLineAsync(int lineId);
    }
}