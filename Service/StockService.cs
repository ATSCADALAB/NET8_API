using AutoMapper;
using Contracts;
using Entities.Exceptions.Stock;
using Entities.Models;
using Service.Contracts;
using Shared.DataTransferObjects.InboundRecord;
using Shared.DataTransferObjects.Stock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service
{
    internal sealed class StockService : IStockService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;

        public StockService(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<IEnumerable<StockDto>> GetAllStocksAsync(bool trackChanges)
        {
            var stocks = await _repository.Stock.GetAllStocksAsync(trackChanges);
            return _mapper.Map<IEnumerable<StockDto>>(stocks);
        }

        public async Task<StockDto> GetStockAsync(int stockId, bool trackChanges)
        {
            var stock = await GetStockAndCheckIfItExists(stockId, trackChanges);
            return _mapper.Map<StockDto>(stock);
        }

        public async Task<StockDto> GetStockByProductInformationAsync(int productInformationId, bool trackChanges)
        {
            var stock = await _repository.Stock.GetStockByProductInformationIdAsync(productInformationId, trackChanges);
            if (stock is null)
                throw new StockNotFoundException(0);
            return _mapper.Map<StockDto>(stock);
        }

        public async Task<StockDto> CreateStockAsync(StockForCreationDto stock)
        {
            if (stock == null)
                throw new ArgumentNullException(nameof(stock), "StockForCreationDto cannot be null.");

            var stockEntity = _mapper.Map<Stock>(stock);
            _repository.Stock.CreateStock(stockEntity);
            await _repository.SaveAsync();
            return _mapper.Map<StockDto>(stockEntity);
        }

        public async Task UpdateStockAsync(int stockId, StockForUpdateDto stockForUpdate, bool trackChanges)
        {
            var stock = await GetStockAndCheckIfItExists(stockId, trackChanges);
            _mapper.Map(stockForUpdate, stock);
            await _repository.SaveAsync();
        }

        public async Task DeleteStockAsync(int stockId, bool trackChanges)
        {
            var stock = await GetStockAndCheckIfItExists(stockId, trackChanges);
            _repository.Stock.DeleteStock(stock);
            await _repository.SaveAsync();
        }

        public async Task<IEnumerable<InventoryReportDto>> GetDailyInventoryReportAsync(DateTime date)
        {
            var startOfDay = date.Date;
            var endOfDay = startOfDay.AddDays(1).AddTicks(-1);

            var stocks = await _repository.Stock.GetAllStocksAsync(false);
            var inboundRecords = await _repository.InboundRecord.GetInboundRecordsByDateAsync(startOfDay, false);
            var outboundRecords = await _repository.OutboundRecord.GetOutboundRecordsByDateAsync(startOfDay, false);

            var report = from stock in stocks
                         join product in await _repository.ProductInformation.GetAllProductInformationsAsync(false)
                         on stock.ProductInformationId equals product.Id
                         let dailyInbounds = inboundRecords.Where(ir => ir.ProductInformationId == stock.ProductInformationId
                            && ir.InboundDate >= startOfDay && ir.InboundDate <= endOfDay)
                         let dailyOutbounds = outboundRecords.Where(or => or.ProductInformationId == stock.ProductInformationId
                            && or.OutboundDate >= startOfDay && or.OutboundDate <= endOfDay)
                         select new InventoryReportDto
                         {
                             ProductName = product.ProductName,
                             OpeningStockUnits = stock.QuantityUnits - dailyInbounds.Sum(ir => ir.QuantityUnits) + dailyOutbounds.Sum(or => or.QuantityUnits),
                             OpeningStockWeight = stock.QuantityWeight - dailyInbounds.Sum(ir => ir.QuantityWeight) + dailyOutbounds.Sum(or => or.QuantityWeight),
                             InQuantityUnits = dailyInbounds.Sum(ir => ir.QuantityUnits),
                             InQuantityWeight = dailyInbounds.Sum(ir => ir.QuantityWeight),
                             OutQuantityUnits = dailyOutbounds.Sum(or => or.QuantityUnits),
                             OutQuantityWeight = dailyOutbounds.Sum(or => or.QuantityWeight),
                             ClosingStockUnits = stock.QuantityUnits,
                             ClosingStockWeight = stock.QuantityWeight
                         };

            return report.ToList();
        }

        public async Task<IEnumerable<InventoryReportDto>> GetMonthlyInventoryReportAsync(int year, int month)
        {
            var startOfMonth = new DateTime(year, month, 1);
            var endOfMonth = startOfMonth.AddMonths(1).AddTicks(-1);

            var stocks = await _repository.Stock.GetAllStocksAsync(false);
            var inboundRecords = await _repository.InboundRecord.GetAllInboundRecordsAsync(false);
            var outboundRecords = await _repository.OutboundRecord.GetAllOutboundRecordsAsync(false);

            var report = from stock in stocks
                         join product in await _repository.ProductInformation.GetAllProductInformationsAsync(false)
                         on stock.ProductInformationId equals product.Id
                         let monthlyInbounds = inboundRecords.Where(ir => ir.ProductInformationId == stock.ProductInformationId
                            && ir.InboundDate >= startOfMonth && ir.InboundDate <= endOfMonth)
                         let monthlyOutbounds = outboundRecords.Where(or => or.ProductInformationId == stock.ProductInformationId
                            && or.OutboundDate >= startOfMonth && or.OutboundDate <= endOfMonth)
                         select new InventoryReportDto
                         {
                             ProductName = product.ProductName,
                             OpeningStockUnits = stock.QuantityUnits - monthlyInbounds.Sum(ir => ir.QuantityUnits) + monthlyOutbounds.Sum(or => or.QuantityUnits),
                             OpeningStockWeight = stock.QuantityWeight - monthlyInbounds.Sum(ir => ir.QuantityWeight) + monthlyOutbounds.Sum(or => or.QuantityWeight),
                             InQuantityUnits = monthlyInbounds.Sum(ir => ir.QuantityUnits),
                             InQuantityWeight = monthlyInbounds.Sum(ir => ir.QuantityWeight),
                             OutQuantityUnits = monthlyOutbounds.Sum(or => or.QuantityUnits),
                             OutQuantityWeight = monthlyOutbounds.Sum(or => or.QuantityWeight),
                             ClosingStockUnits = stock.QuantityUnits,
                             ClosingStockWeight = stock.QuantityWeight
                         };

            return report.ToList();
        }

        public async Task<IEnumerable<InventoryReportDto>> GetYearlyInventoryReportAsync(int year)
        {
            var startOfYear = new DateTime(year, 1, 1);
            var endOfYear = startOfYear.AddYears(1).AddTicks(-1);

            var stocks = await _repository.Stock.GetAllStocksAsync(false);
            var inboundRecords = await _repository.InboundRecord.GetAllInboundRecordsAsync(false);
            var outboundRecords = await _repository.OutboundRecord.GetAllOutboundRecordsAsync(false);

            var report = from stock in stocks
                         join product in await _repository.ProductInformation.GetAllProductInformationsAsync(false)
                         on stock.ProductInformationId equals product.Id
                         let yearlyInbounds = inboundRecords.Where(ir => ir.ProductInformationId == stock.ProductInformationId
                            && ir.InboundDate >= startOfYear && ir.InboundDate <= endOfYear)
                         let yearlyOutbounds = outboundRecords.Where(or => or.ProductInformationId == stock.ProductInformationId
                            && or.OutboundDate >= startOfYear && or.OutboundDate <= endOfYear)
                         select new InventoryReportDto
                         {
                             ProductName = product.ProductName,
                             OpeningStockUnits = stock.QuantityUnits - yearlyInbounds.Sum(ir => ir.QuantityUnits) + yearlyOutbounds.Sum(or => or.QuantityUnits),
                             OpeningStockWeight = stock.QuantityWeight - yearlyInbounds.Sum(ir => ir.QuantityWeight) + yearlyOutbounds.Sum(or => or.QuantityWeight),
                             InQuantityUnits = yearlyInbounds.Sum(ir => ir.QuantityUnits),
                             InQuantityWeight = yearlyInbounds.Sum(ir => ir.QuantityWeight),
                             OutQuantityUnits = yearlyOutbounds.Sum(or => or.QuantityUnits),
                             OutQuantityWeight = yearlyOutbounds.Sum(or => or.QuantityWeight),
                             ClosingStockUnits = stock.QuantityUnits,
                             ClosingStockWeight = stock.QuantityWeight
                         };

            return report.ToList();
        }

        private async Task<Stock> GetStockAndCheckIfItExists(int id, bool trackChanges)
        {
            var stock = await _repository.Stock.GetStockByIdAsync(id, trackChanges);
            if (stock is null)
                throw new StockNotFoundException(id);
            return stock;
        }
    }
}