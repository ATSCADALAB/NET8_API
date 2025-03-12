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
                .FirstOrDefaultAsync();
        }
        // Lấy chi tiết đơn hàng theo mã đơn hàng với những đơn hàng chưa được hoàn thành
        public async Task<Order> GetOrderByOrderCodeAsync(string code, bool trackChanges)
        {
            return await FindByCondition(o => o.Code.Equals(code) && o.Status == false, trackChanges)
                .FirstOrDefaultAsync();
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
