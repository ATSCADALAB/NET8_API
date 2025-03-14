using Entities.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IOrderDetailRepository
    {
        // Lấy tất cả chi tiết đơn hàng
        Task<IEnumerable<OrderDetail>> GetAllOrderDetailsAsync(bool trackChanges);

        // Lấy chi tiết đơn hàng theo ID
        Task<OrderDetail> GetOrderDetailByIdAsync(int orderDetailId, bool trackChanges);

        // Lấy danh sách chi tiết đơn hàng theo Order ID
        Task<IEnumerable<OrderDetail>> GetOrderDetailsByOrderIdAsync(Guid orderId, bool trackChanges);

        // Lấy danh sách chi tiết đơn hàng theo sản phẩm
        Task<IEnumerable<OrderDetail>> GetOrderDetailsByProductAsync(int productInformationId, bool trackChanges);

        // Tạo chi tiết đơn hàng mới
        void CreateOrderDetail(OrderDetail orderDetail);

        // Cập nhật thông tin chi tiết đơn hàng
        void UpdateOrderDetail(OrderDetail orderDetail);

        // Xóa chi tiết đơn hàng
        void DeleteOrderDetail(OrderDetail orderDetail);
    }
}