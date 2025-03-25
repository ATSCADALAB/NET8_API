using Shared.DataTransferObjects.Product;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Contracts
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetAllProductsAsync(bool trackChanges);
        Task<ProductDto> GetProductAsync(int productId, bool trackChanges);
        Task<CheckDto> GetProductByTagIDAsync(string tagId, bool trackChanges);
        Task<IEnumerable<ProductDto>> GetProductsByDistributorAsync(int distributorId, bool trackChanges);
        Task<IEnumerable<ProductDto>> GetProductsByOrderDetailAsync(int orderDetailId, bool trackChanges);
        Task<ProductDto> CreateProductAsync(ProductForCreationDto product);
        Task UpdateProductAsync(int productId, ProductForUpdateDto productForUpdate, bool trackChanges);
        Task DeleteProductAsync(int productId, bool trackChanges);
    }
}