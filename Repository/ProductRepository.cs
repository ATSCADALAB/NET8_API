using Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    internal sealed class ProductRepository : RepositoryBase<Product>, IProductRepository
    {
        public ProductRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }

        public void CreateProduct(Product product)
        {
            Create(product);
        }

        public void DeleteProduct(Product product)
        {
            Delete(product);
        }

        public async Task<IEnumerable<Product>> GetProductsAsync(bool trackChanges)
        {
            return await FindAll(trackChanges)
                .OrderBy(p => p.TagID)
                .Include(p => p.Distributor)
                .Include(p => p.ProductInformation)
                .ToListAsync();
        }

        public async Task<Product> GetProductAsync(long productId, bool trackChanges)
        {
            return await FindByCondition(p => p.Id.Equals(productId), trackChanges)
                .Include(p => p.Distributor)
                .Include(p => p.ProductInformation)
                .SingleOrDefaultAsync();
        }
        public async Task<Product> GetProductByTagIDAsync(string tagID, bool trackChanges)
        {
            return await FindByCondition(p => p.TagID.Equals(tagID), trackChanges)
                .Include(p => p.Distributor)
                .Include(p => p.ProductInformation)
                .SingleOrDefaultAsync();
        }

        public async Task<Product> GetProductWithDetailsAsync(long productId, bool trackChanges)
        {
            return await FindByCondition(p => p.Id.Equals(productId), trackChanges)
                .Include(p => p.Distributor)
                .Include(p => p.ProductInformation)
                .SingleOrDefaultAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsByDistributorAsync(long distributorId, bool trackChanges)
        {
            return await FindByCondition(p => p.DistributorId.Equals(distributorId), trackChanges)
                .Include(p => p.Distributor)
                .Include(p => p.ProductInformation)
                .OrderBy(p => p.TagID)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsByProductInformationAsync(long productInformationId, bool trackChanges)
        {
            return await FindByCondition(p => p.ProductInformationId.Equals(productInformationId), trackChanges)
                .Include(p => p.Distributor)
                .Include(p => p.ProductInformation)
                .OrderBy(p => p.TagID)
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await RepositoryContext.SaveChangesAsync();
        }
    }
}