using Entities.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IOrderRepository
    {
        // Lấy tất cả đơn hàng
        Task<IEnumerable<Order>> GetOrdersAsync(bool trackChanges);

        // Lấy chi tiết đơn hàng theo ID
        Task<Order> GetOrderByIdAsync(Guid orderId, bool trackChanges);
        // Lấy chi tiết đơn hàng theo mã đơn hàng
        Task<Order> GetOrderByOrderCodeAsync(string code, bool trackChanges);

        // Tạo đơn hàng mới
        void CreateOrder(Order order);

        // Cập nhật thông tin đơn hàng
        void UpdateOrder(Order order);

        // Xóa đơn hàng
        void DeleteOrder(Order order);

    }
}
