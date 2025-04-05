using Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repository
{
    internal sealed class OrderRepository : RepositoryBase<Order>, IOrderRepository
    {
        public OrderRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync(bool trackChanges) =>
            await FindAll(trackChanges)
                .OrderBy(o => o.OrderCode)
                .Include(o => o.CreatedBy)
                .Include(o => o.Distributor)
                .ToListAsync();

        public async Task<Order> GetOrderByIdAsync(Guid orderId, bool trackChanges) =>
            await FindByCondition(o => o.Id == orderId, trackChanges)
                .Include(o => o.Distributor)
                .SingleOrDefaultAsync();

        public async Task<Order> GetOrderByCodeAsync(string orderCode, bool trackChanges) =>
            await FindByCondition(o => o.OrderCode == orderCode, trackChanges)
                .Include(o => o.Distributor)
                .SingleOrDefaultAsync();

        public async Task<IEnumerable<Order>> GetOrdersByDistributorAsync(int distributorId, bool trackChanges) =>
            await FindByCondition(o => o.DistributorId == distributorId, trackChanges)
                .Include(o => o.Distributor)
                .OrderBy(o => o.OrderCode)
                .ToListAsync();

        public async Task<IEnumerable<Order>> GetOrdersByExportDateAsync(DateTime exportDate, bool trackChanges) =>
            await FindByCondition(o => o.ExportDate.Date == exportDate.Date, trackChanges)
                .Include(o => o.Distributor)
                .OrderBy(o => o.OrderCode)
                .ToListAsync();

        public void CreateOrder(Order order) => Create(order);

        public void UpdateOrder(Order order) => Update(order);

        public void DeleteOrder(Order order) => Delete(order);
    }
}