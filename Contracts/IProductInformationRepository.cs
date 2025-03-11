using Entities.Models;

namespace Contracts
{
    public interface IProductInformationRepository
    {
        Task<IEnumerable<ProductInformation>> GetProductInformationsAsync(bool trackChanges);
        Task<IEnumerable<ProductInformation>> GetDistributorsByNameAsync(string name);
        Task<ProductInformation> GetProductInformationAsync(long productInformationId, bool trackChanges);
        void CreateProductInformation(ProductInformation productInformation);
        void DeleteProductInformation(ProductInformation productInformation);
    }
}