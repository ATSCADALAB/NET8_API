using Entities.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IStockRepository
    {
        // Lấy tất cả tồn kho
        Task<IEnumerable<Stock>> GetAllStocksAsync(bool trackChanges);

        // Lấy tồn kho theo ID
        Task<Stock> GetStockByIdAsync(int stockId, bool trackChanges);

        // Lấy tồn kho theo ProductInformation ID
        Task<Stock> GetStockByProductInformationIdAsync(int productInformationId, bool trackChanges);

        // Tạo bản ghi tồn kho mới
        void CreateStock(Stock stock);

        // Cập nhật thông tin tồn kho
        void UpdateStock(Stock stock);

        // Xóa bản ghi tồn kho
        void DeleteStock(Stock stock);
    }
}