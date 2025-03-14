using AutoMapper;
using Contracts;
using Entities.Exceptions.Stock;
using Entities.Models;
using Service.Contracts;
using Shared.DataTransferObjects.Stock;
using System;
using System.Collections.Generic;
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
            var stocksDto = _mapper.Map<IEnumerable<StockDto>>(stocks);
            return stocksDto;
        }

        public async Task<StockDto> GetStockAsync(int stockId, bool trackChanges)
        {
            var stock = await GetStockAndCheckIfItExists(stockId, trackChanges);
            var stockDto = _mapper.Map<StockDto>(stock);
            return stockDto;
        }

        public async Task<StockDto> GetStockByProductInformationAsync(int productInformationId, bool trackChanges)
        {
            var stock = await _repository.Stock.GetStockByProductInformationIdAsync(productInformationId, trackChanges);
            if (stock is null)
                throw new StockNotFoundException(0);

            var stockDto = _mapper.Map<StockDto>(stock);
            return stockDto;
        }

        public async Task<StockDto> CreateStockAsync(StockForCreationDto stock)
        {
            if (stock == null)
                throw new ArgumentNullException(nameof(stock), "StockForCreationDto cannot be null.");

            var stockEntity = _mapper.Map<Stock>(stock);
            _repository.Stock.CreateStock(stockEntity);
            await _repository.SaveAsync();

            var stockToReturn = _mapper.Map<StockDto>(stockEntity);
            return stockToReturn;
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

        private async Task<Stock> GetStockAndCheckIfItExists(int id, bool trackChanges)
        {
            var stock = await _repository.Stock.GetStockByIdAsync(id, trackChanges);
            if (stock is null)
                throw new StockNotFoundException(id);
            return stock;
        }
    }
}