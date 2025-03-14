using Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repository
{
    internal sealed class OrderRepository : RepositoryBase<Order>, IOrderRepository
    {
        public OrderRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }

        // Lấy tất cả đơn hàng
        public async Task<IEnumerable<Order>> GetOrdersAsync(bool trackChanges)
        {
            try
            {
                return await FindAll(trackChanges)
                .Include(o=>o.ProductInformation)
                .Include(o=>o.Distributor)
                .OrderBy(o => o.CreatedDate) // Sắp xếp theo ngày tạo
                .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error", ex);
            }
        }

        // Lấy chi tiết đơn hàng theo ID
        public async Task<Order> GetOrderByIdAsync(Guid orderId, bool trackChanges)
        {
            return await FindByCondition(o => o.Id.Equals(orderId), trackChanges)
                .Include(o => o.ProductInformation)
                .Include(o => o.Distributor)
                .FirstOrDefaultAsync();
        }
        // Lấy chi tiết đơn hàng theo mã đơn hàng với những đơn hàng chưa được hoàn thành
        public async Task<Order> GetOrderByOrderCodeAsync(string code, bool trackChanges)
        {
            return await FindByCondition(o => o.Code.Equals(code) && o.Status == 0, trackChanges)
                .FirstOrDefaultAsync();
        }
        public async Task UpdateStatusAsync(Guid orderId, int newStatus)
        {
            try
            {
                var order = await FindByCondition(o => o.Id.Equals(orderId), trackChanges: true)
                    .FirstOrDefaultAsync();

                if (order == null)
                {
                    throw new Exception($"Order with ID {orderId} not found.");
                }

                order.Status = newStatus;
                order.UpdatedDate = DateTime.UtcNow; // Cập nhật thời gian chỉnh sửa
                UpdateOrder(order);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating status for order with ID {orderId}", ex);
            }
        }
        // Tạo đơn hàng mới
        public void CreateOrder(Order order)
        {
            Create(order);
        }

        // Cập nhật đơn hàng
        public void UpdateOrder(Order order)
        {
            Update(order);
        }

        // Xóa đơn hàng
        public void DeleteOrder(Order order)
        {
            Delete(order);
        }
    }
}
