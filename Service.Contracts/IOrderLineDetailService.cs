using Entities.Models;
using Shared.DataTransferObjects;
using Shared.DataTransferObjects.OrderLineDetail;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Contracts
{
    public interface IOrderLineDetailService
    {
        Task<IEnumerable<OrderLineDetail>> GetAllOrderLineDetailsAsync();

        Task<OrderLineDetail> GetOrderLineDetailByIdAsync(Guid OrderlId);
        Task<OrderLineDetail> GetOrderLineDetailByLineIDAsync(long lineID);

        Task<OrderLineDetailDto> CreateOrderLineDetailAsync(OrderLineDetailForCreationDto OrderLineDetailForCreationDto);

        Task<OrderLineDetailDto> UpdateOrderLineDetailAsync(Guid OrderLineDetailId, OrderLineDetailForUpdateDto OrderLineDetailForUpdateDto);

        Task DeleteOrderLineDetailAsync(Guid OrderLineDetailId, bool trackChanges);
    }
}
