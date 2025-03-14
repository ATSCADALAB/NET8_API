using Entities.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IOrderLineDetailRepository
    {
        // Lấy tất cả chi tiết dòng đơn hàng
        Task<IEnumerable<OrderLineDetail>> GetAllOrderLineDetailsAsync(bool trackChanges);

        // Lấy chi tiết dòng đơn hàng theo ID
        Task<OrderLineDetail> GetOrderLineDetailByIdAsync(int orderLineDetailId, bool trackChanges);

        // Lấy danh sách chi tiết dòng theo Order ID
        Task<IEnumerable<OrderLineDetail>> GetOrderLineDetailsByOrderIdAsync(Guid orderId, bool trackChanges);

        // Lấy danh sách chi tiết dòng theo Line ID
        Task<IEnumerable<OrderLineDetail>> GetOrderLineDetailsByLineIdAsync(int lineId, bool trackChanges);

        // Tạo chi tiết dòng đơn hàng mới
        void CreateOrderLineDetail(OrderLineDetail orderLineDetail);

        // Cập nhật thông tin chi tiết dòng đơn hàng
        void UpdateOrderLineDetail(OrderLineDetail orderLineDetail);

        // Xóa chi tiết dòng đơn hàng
        void DeleteOrderLineDetail(OrderLineDetail orderLineDetail);
    }
}