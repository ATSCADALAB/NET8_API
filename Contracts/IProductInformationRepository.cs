using Entities.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IProductInformationRepository
    {
        // Lấy tất cả thông tin sản phẩm
        Task<IEnumerable<ProductInformation>> GetAllProductInformationsAsync(bool trackChanges);

        // Lấy thông tin sản phẩm theo ID
        Task<ProductInformation> GetProductInformationByIdAsync(int productInformationId, bool trackChanges);

        // Lấy thông tin sản phẩm theo mã sản phẩm
        Task<ProductInformation> GetProductInformationByCodeAsync(string productCode, bool trackChanges);

        // Tạo thông tin sản phẩm mới
        void CreateProductInformation(ProductInformation productInformation);

        // Cập nhật thông tin sản phẩm
        void UpdateProductInformation(ProductInformation productInformation);

        // Xóa thông tin sản phẩm
        void DeleteProductInformation(ProductInformation productInformation);
    }
}