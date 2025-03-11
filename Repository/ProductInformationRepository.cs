using Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;

namespace Repository
{
    internal sealed class ProductInformationRepository : RepositoryBase<ProductInformation>, IProductInformationRepository
    {
        public ProductInformationRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }

        public void CreateProductInformation(ProductInformation productInformation)
        {
            Create(productInformation);
        }

        public void DeleteProductInformation(ProductInformation productInformation)
        {
            Delete(productInformation);
        }

        public async Task<IEnumerable<ProductInformation>> GetProductInformationsAsync(bool trackChanges)
        {
            return await FindAll(trackChanges)
                .OrderBy(pi => pi.ProductName)
                .Include(pi => pi.Products)
                .ToListAsync();
        }
        public async Task<IEnumerable<ProductInformation>> GetDistributorsByNameAsync(string name)
        {
            return await FindAll(false)
                .Where(d => EF.Functions.Like(d.ProductName, $"%{name}%")).ToListAsync(); // Sử dụng LIKE
        }

        public async Task<ProductInformation> GetProductInformationAsync(long productInformationId, bool trackChanges)
        {
            return await FindByCondition(pi => pi.Id.Equals(productInformationId), trackChanges)
                .Include(pi => pi.Products)
                .SingleOrDefaultAsync();
        }

        public async Task SaveChangesAsync()
        {
            await RepositoryContext.SaveChangesAsync();
        }
    }
}