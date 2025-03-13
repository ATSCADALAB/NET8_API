using Entities.Models;
using Shared.DataTransferObjects.Order;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Contracts
{
    public interface IOrderService
    {
        //Lọc đơn hàng theo khoảng ngày
        Task<IEnumerable<OrderDto>> GetOrdersByFilters(DateTime? startDate, DateTime? endDate, string productCode,int status);
        // Lấy tất cả đơn hàng
        Task<IEnumerable<Order>> GetAllOrdersAsync();

        // Lấy chi tiết 1 đơn hàng
        Task<Order> GetOrderByIdAsync(Guid orderId);
        //Lấy chi tiết đơn hàng bằng mã đơn hàng
        Task<OrderDto> GetOrderByOrderCodeAsync(string code);

        // Tạo đơn hàng
        Task<OrderDto> CreateOrderAsync(OrderForCreationDto orderForCreationDto);

        // Cập nhật đơn hàng
        Task<OrderDto> UpdateOrderAsync(Guid orderId, OrderForUpdateDto orderForUpdateDto);

        // Xoá đơn hàng 
        Task DeleteOrderAsync(Guid orderId, bool trackChanges);

        // Nhập khẩu (import) đơn hàng từ file hoặc nguồn bên ngoài
        Task<IEnumerable<OrderDto>> ImportOrdersAsync(List<OrderForCreationDto> orders);
    }
}
