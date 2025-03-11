using Shared.DataTransferObjects.ProductInformation;

namespace Service.Contracts
{
    public interface IProductInformationService
    {
        Task<IEnumerable<ProductInformationDto>> GetAllProductInformationsAsync(bool trackChanges);
        Task<ProductInformationDto> GetProductInformationAsync(long productInformationId, bool trackChanges);
        Task<ProductInformationDto> CreateProductInformationAsync(ProductInformationForCreationDto productInformation);
        Task DeleteProductInformationAsync(long productInformationId, bool trackChanges);
        Task<IEnumerable<ProductInformationDto>> GetDistributorsByNameAsync(string name);
        Task UpdateProductInformationAsync(long productInformationId, ProductInformationForUpdateDto productInformationForUpdate, bool trackChanges);
    
    }
}