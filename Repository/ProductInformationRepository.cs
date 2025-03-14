using Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repository
{
    internal sealed class ProductInformationRepository : RepositoryBase<ProductInformation>, IProductInformationRepository
    {
        public ProductInformationRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }

        public async Task<IEnumerable<ProductInformation>> GetAllProductInformationsAsync(bool trackChanges) =>
            await FindAll(trackChanges)
                .OrderBy(pi => pi.ProductCode)
                .ToListAsync();

        public async Task<ProductInformation> GetProductInformationByIdAsync(int productInformationId, bool trackChanges) =>
            await FindByCondition(pi => pi.Id == productInformationId, trackChanges)
                .SingleOrDefaultAsync();

        public async Task<ProductInformation> GetProductInformationByCodeAsync(string productCode, bool trackChanges) =>
            await FindByCondition(pi => pi.ProductCode == productCode, trackChanges)
                .SingleOrDefaultAsync();

        public void CreateProductInformation(ProductInformation productInformation) => Create(productInformation);

        public void UpdateProductInformation(ProductInformation productInformation) => Update(productInformation);

        public void DeleteProductInformation(ProductInformation productInformation) => Delete(productInformation);
    }
}