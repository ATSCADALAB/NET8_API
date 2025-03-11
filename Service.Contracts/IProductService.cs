using Shared.DataTransferObjects.Product;

namespace Service.Contracts
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetAllProductsAsync(bool trackChanges);
        Task<ProductDto> GetProductAsync(long productId, bool trackChanges);
        Task<ProductDto> GetProductByTagIDAsync(string productId, bool trackChanges);
        Task<IEnumerable<ProductDto>> GetProductsByDistributorAsync(long distributorId, bool trackChanges);
        Task<IEnumerable<ProductDto>> GetProductsByProductInformationAsync(long productInformationId, bool trackChanges);
        Task<ProductDto> CreateProductAsync(ProductForCreationDto product);
        Task DeleteProductAsync(long productId, bool trackChanges);
        Task UpdateProductAsync(long productId, ProductForUpdateDto productForUpdate, bool trackChanges);
    }
}