using Shared.DataTransferObjects.OrderDetail;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Contracts
{
    public interface IOrderDetailService
    {
        Task<IEnumerable<OrderDetailDto>> GetAllOrderDetailsAsync(bool trackChanges);
        Task<OrderDetailDto> GetOrderDetailAsync(int orderDetailId, bool trackChanges);
        Task<IEnumerable<OrderDetailDto>> GetOrderDetailsByOrderAsync(Guid orderId, bool trackChanges);
        Task<IEnumerable<OrderDetailDto>> GetOrderDetailsByProductAsync(int productInformationId, bool trackChanges);
        Task<OrderDetailDto> CreateOrderDetailAsync(OrderDetailForCreationDto orderDetail);
        Task UpdateOrderDetailAsync(int orderDetailId, OrderDetailForUpdateDto orderDetailForUpdate, bool trackChanges);
        Task DeleteOrderDetailAsync(int orderDetailId, bool trackChanges);
    }
}