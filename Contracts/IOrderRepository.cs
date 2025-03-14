using Entities.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IOrderRepository
    {
        // Lấy tất cả đơn hàng
        Task<IEnumerable<Order>> GetAllOrdersAsync(bool trackChanges);

        // Lấy đơn hàng theo ID
        Task<Order> GetOrderByIdAsync(Guid orderId, bool trackChanges);

        // Lấy đơn hàng theo mã đơn hàng
        Task<Order> GetOrderByCodeAsync(string orderCode, bool trackChanges);

        // Lấy danh sách đơn hàng theo đại lý
        Task<IEnumerable<Order>> GetOrdersByDistributorAsync(int distributorId, bool trackChanges);

        // Lấy danh sách đơn hàng theo ngày xuất
        Task<IEnumerable<Order>> GetOrdersByExportDateAsync(DateTime exportDate, bool trackChanges);

        // Tạo đơn hàng mới
        void CreateOrder(Order order);

        // Cập nhật thông tin đơn hàng
        void UpdateOrder(Order order);

        // Xóa đơn hàng
        void DeleteOrder(Order order);
    }
}