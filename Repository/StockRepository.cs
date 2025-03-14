using Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repository
{
    internal sealed class StockRepository : RepositoryBase<Stock>, IStockRepository
    {
        public StockRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }

        public async Task<IEnumerable<Stock>> GetAllStocksAsync(bool trackChanges) =>
            await FindAll(trackChanges)
                .Include(s => s.ProductInformation)
                .OrderBy(s => s.ProductInformationId)
                .ToListAsync();

        public async Task<Stock> GetStockByIdAsync(int stockId, bool trackChanges) =>
            await FindByCondition(s => s.Id == stockId, trackChanges)
                .Include(s => s.ProductInformation)
                .SingleOrDefaultAsync();

        public async Task<Stock> GetStockByProductInformationIdAsync(int productInformationId, bool trackChanges) =>
            await FindByCondition(s => s.ProductInformationId == productInformationId, trackChanges)
                .Include(s => s.ProductInformation)
                .SingleOrDefaultAsync();

        public void CreateStock(Stock stock) => Create(stock);

        public void UpdateStock(Stock stock) => Update(stock);

        public void DeleteStock(Stock stock) => Delete(stock);
    }
}