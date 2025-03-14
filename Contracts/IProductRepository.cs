using Entities.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IProductRepository
    {
        // Lấy tất cả sản phẩm
        Task<IEnumerable<Product>> GetAllProductsAsync(bool trackChanges);

        // Lấy sản phẩm theo ID
        Task<Product> GetProductByIdAsync(int productId, bool trackChanges);

        // Lấy sản phẩm theo TagID (RFID)
        Task<Product> GetProductByTagIdAsync(string tagId, bool trackChanges);

        // Lấy danh sách sản phẩm theo OrderDetail ID
        Task<IEnumerable<Product>> GetProductsByOrderDetailIdAsync(int orderDetailId, bool trackChanges);

        Task<IEnumerable<Product>> GetProductsByDistributorIdAsync(int distributorId, bool trackChanges);

        // Tạo sản phẩm mới
        void CreateProduct(Product product);

        // Cập nhật thông tin sản phẩm
        void UpdateProduct(Product product);

        // Xóa sản phẩm
        void DeleteProduct(Product product);
    }
}