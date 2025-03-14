using Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repository
{
    internal sealed class OrderLineDetailRepository : RepositoryBase<OrderLineDetail>, IOrderLineDetailRepository
    {
        public OrderLineDetailRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }

        public async Task<IEnumerable<OrderLineDetail>> GetAllOrderLineDetailsAsync(bool trackChanges) =>
            await FindAll(trackChanges)
                .Include(old => old.Order)
                .Include(old => old.Line)
                .OrderBy(old => old.SequenceNumber)
                .ToListAsync();

        public async Task<OrderLineDetail> GetOrderLineDetailByIdAsync(int orderLineDetailId, bool trackChanges) =>
            await FindByCondition(old => old.Id == orderLineDetailId, trackChanges)
                .Include(old => old.Order)
                .Include(old => old.Line)
                .SingleOrDefaultAsync();

        public async Task<IEnumerable<OrderLineDetail>> GetOrderLineDetailsByOrderIdAsync(Guid orderId, bool trackChanges) =>
            await FindByCondition(old => old.OrderId == orderId, trackChanges)
                .Include(old => old.Order)
                .Include(old => old.Line)
                .OrderBy(old => old.SequenceNumber)
                .ToListAsync();

        public async Task<IEnumerable<OrderLineDetail>> GetOrderLineDetailsByLineIdAsync(int lineId, bool trackChanges) =>
            await FindByCondition(old => old.LineId == lineId, trackChanges)
                .Include(old => old.Order)
                .Include(old => old.Line)
                .OrderBy(old => old.SequenceNumber)
                .ToListAsync();

        public void CreateOrderLineDetail(OrderLineDetail orderLineDetail) => Create(orderLineDetail);

        public void UpdateOrderLineDetail(OrderLineDetail orderLineDetail) => Update(orderLineDetail);

        public void DeleteOrderLineDetail(OrderLineDetail orderLineDetail) => Delete(orderLineDetail);
    }
}