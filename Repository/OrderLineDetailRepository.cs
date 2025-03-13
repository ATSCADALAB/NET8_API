using Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repository
{
    internal sealed class OrderLineDetailRepository : RepositoryBase<OrderLineDetail>, IOrderLineDetailRepository
    {
        public OrderLineDetailRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }

        // Lấy tất cả đơn hàng
        public async Task<IEnumerable<OrderLineDetail>> GetOrderLineDetailsAsync(bool trackChanges)
        {
            try
            {
                return await FindAll(trackChanges)
                .Include(o=>o.Order)
                .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error", ex);
            }
        }
        // Lấy SequenceNumber lớn nhất theo Line
        public async Task<int> GetMaxSequenceNumberByLineAsync(int line)
        {
            return await FindByCondition(o => o.Line == line, trackChanges: false)
                .MaxAsync(o => (int?)o.SequenceNumber) ?? 0;
        }
        // Lấy chi tiết đơn hàng theo ID
        public async Task<OrderLineDetail> GetOrderLineDetailByIdAsync(Guid OrderLineDetailId, bool trackChanges)
        {
            return await FindByCondition(o => o.OrderId.Equals(OrderLineDetailId), trackChanges)
                .Include(o => o.Order)
                .FirstOrDefaultAsync();
        }
        // Tạo đơn hàng mới
        public void CreateOrderLineDetail(OrderLineDetail OrderLineDetail)
        {

            Create(OrderLineDetail);
        }

        // Cập nhật đơn hàng
        public void UpdateOrderLineDetail(OrderLineDetail OrderLineDetail)
        {
            Update(OrderLineDetail);
        }

        // Xóa đơn hàng
        public void DeleteOrderLineDetail(OrderLineDetail OrderLineDetail)
        {
            Delete(OrderLineDetail);
        }
    }
}
