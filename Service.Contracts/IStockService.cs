using Shared.DataTransferObjects.InboundRecord;
using Shared.DataTransferObjects.Stock;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Contracts
{
    public interface IStockService
    {
        Task<IEnumerable<StockDto>> GetAllStocksAsync(bool trackChanges);
        Task<StockDto> GetStockAsync(int stockId, bool trackChanges);
        Task<StockDto> GetStockByProductInformationAsync(int productInformationId, bool trackChanges);
        Task<StockDto> CreateStockAsync(StockForCreationDto stock);
        Task UpdateStockAsync(int stockId, StockForUpdateDto stockForUpdate, bool trackChanges);
        Task DeleteStockAsync(int stockId, bool trackChanges);
        Task<IEnumerable<InventoryReportDto>> GetDailyInventoryReportAsync(DateTime date);
        Task<IEnumerable<InventoryReportDto>> GetMonthlyInventoryReportAsync(int year, int month);
        Task<IEnumerable<InventoryReportDto>> GetYearlyInventoryReportAsync(int year);
    }
}