using Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repository
{
    internal sealed class ProductRepository : RepositoryBase<Product>, IProductRepository
    {
        public ProductRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync(bool trackChanges) =>
            await FindAll(trackChanges)
                .OrderBy(p => p.TagID)
                .Include(p => p.OrderDetail)
                .Include(p => p.Distributor)
                .ToListAsync();

        public async Task<Product> GetProductByIdAsync(int productId, bool trackChanges) =>
            await FindByCondition(p => p.Id == productId, trackChanges)
                .Include(p => p.OrderDetail)
                .Include(p => p.Distributor)
                .SingleOrDefaultAsync();

        public async Task<Product> GetProductByTagIdAsync(string tagId, bool trackChanges) =>
            await FindByCondition(p => p.TagID == tagId, trackChanges)
                .Include(p => p.OrderDetail)
                .Include(p => p.Distributor)
                .SingleOrDefaultAsync();

        public async Task<IEnumerable<Product>> GetProductsByOrderDetailIdAsync(int orderDetailId, bool trackChanges) =>
            await FindByCondition(p => p.OrderDetailId == orderDetailId, trackChanges)
                .Include(p => p.OrderDetail)
                .Include(p => p.Distributor)
                .OrderBy(p => p.TagID)
                .ToListAsync();

        public async Task<IEnumerable<Product>> GetProductsByDistributorIdAsync(int distributorId, bool trackChanges) =>
            await FindByCondition(p => p.DistributorId == distributorId, trackChanges)
                .Include(p => p.OrderDetail)
                .Include(p => p.Distributor)
                .OrderBy(p => p.TagID)
                .ToListAsync();

        public void CreateProduct(Product product) => Create(product);

        public void UpdateProduct(Product product) => Update(product);

        public void DeleteProduct(Product product) => Delete(product);
    }
}