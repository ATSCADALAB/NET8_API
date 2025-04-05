using Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repository
{
    internal sealed class OrderDetailRepository : RepositoryBase<OrderDetail>, IOrderDetailRepository
    {
        public OrderDetailRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }

        public async Task<IEnumerable<OrderDetail>> GetAllOrderDetailsAsync(bool trackChanges) =>
            await FindAll(trackChanges)
                .Include(od => od.Order)
                    .ThenInclude(o => o.CreatedBy)
                .Include(od => od.Order)
                    .ThenInclude(o => o.UpdatedBy)
                .Include(od => od.ProductInformation)
                .ToListAsync();

        public async Task<OrderDetail> GetOrderDetailByIdAsync(int orderDetailId, bool trackChanges) =>
            await FindByCondition(od => od.Id == orderDetailId, trackChanges)
                .Include(od => od.Order)
                    .ThenInclude(o => o.CreatedBy)
                .Include(od => od.Order)
                    .ThenInclude(o => o.UpdatedBy)
                .Include(od => od.ProductInformation)
                .SingleOrDefaultAsync();

        public async Task<IEnumerable<OrderDetail>> GetOrderDetailsByOrderIdAsync(Guid orderId, bool trackChanges) =>
            await FindByCondition(od => od.OrderId == orderId, trackChanges)
                .Include(od => od.Order)
                    .ThenInclude(o => o.Distributor)  //Mapping tới Distributor
                    .ThenInclude(o => o.Area) //Mapping tới Area
                .Include(od => od.Order)
                    .ThenInclude(o => o.CreatedBy)
                .Include(od => od.Order)
                    .ThenInclude(o => o.UpdatedBy)
                .Include(od => od.ProductInformation)
                .ToListAsync();

        public async Task<IEnumerable<OrderDetail>> GetOrderDetailsByProductAsync(int productInformationId, bool trackChanges) =>
            await FindByCondition(od => od.ProductInformationId == productInformationId, trackChanges)
                .Include(od => od.Order)
                    .ThenInclude(o => o.CreatedBy)
                .Include(od => od.Order)
                    .ThenInclude(o => o.UpdatedBy)
                .Include(od => od.ProductInformation)
                .ToListAsync();

        public void CreateOrderDetail(OrderDetail orderDetail) => Create(orderDetail);

        public void UpdateOrderDetail(OrderDetail orderDetail) => Update(orderDetail);

        public void DeleteOrderDetail(OrderDetail orderDetail) => Delete(orderDetail);
    }
}