using Entities.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IOrderLineDetailRepository
    {
        // Lấy tất cả đơn hàng
        Task<IEnumerable<OrderLineDetail>> GetOrderLineDetailsAsync(bool trackChanges);

        // Lấy chi tiết đơn hàng theo ID
        Task<OrderLineDetail> GetOrderLineDetailByIdAsync(Guid OrderlId, bool trackChanges);

        // Tạo đơn hàng mới
        void CreateOrderLineDetail(OrderLineDetail OrderLineDetail);

        // Cập nhật thông tin đơn hàng
        void UpdateOrderLineDetail(OrderLineDetail OrderLineDetail);

        // Xóa đơn hàng
        void DeleteOrderLineDetail(OrderLineDetail OrderLineDetail);

        Task<int> GetMaxSequenceNumberByLineAsync(int line); // Thêm dòng này
    }
}
