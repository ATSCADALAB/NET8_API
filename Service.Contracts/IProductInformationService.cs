using Shared.DataTransferObjects.ProductInformation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Contracts
{
    public interface IProductInformationService
    {
        Task<IEnumerable<ProductInformationDto>> GetAllProductInformationsAsync(bool trackChanges);
        Task<ProductInformationDto> GetProductInformationAsync(int productInformationId, bool trackChanges);
        Task<ProductInformationDto> GetProductInformationByCodeAsync(string productCode, bool trackChanges);
        Task<ProductInformationDto> CreateProductInformationAsync(ProductInformationForCreationDto productInformation);
        Task UpdateProductInformationAsync(int productInformationId, ProductInformationForUpdateDto productInformationForUpdate, bool trackChanges);
        Task DeleteProductInformationAsync(int productInformationId, bool trackChanges);
    }
}