using AutoMapper;
using Contracts;
using Entities.Exceptions.InboundRecord;
using Entities.Models;
using Microsoft.AspNetCore.Http;
using Service.Contracts;
using Shared.DataTransferObjects.InboundRecord;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service
{
    internal sealed class InboundRecordService : IInboundRecordService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;

        public InboundRecordService(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<IEnumerable<InboundRecordDto>> GetAllInboundRecordsAsync(bool trackChanges)
        {
            var inboundRecords = await _repository.InboundRecord.GetAllInboundRecordsAsync(trackChanges);
            return _mapper.Map<IEnumerable<InboundRecordDto>>(inboundRecords);
        }

        public async Task<InboundRecordDto> GetInboundRecordAsync(int inboundRecordId, bool trackChanges)
        {
            var inboundRecord = await GetInboundRecordAndCheckIfItExists(inboundRecordId, trackChanges);
            return _mapper.Map<InboundRecordDto>(inboundRecord);
        }

        public async Task<IEnumerable<InboundRecordDto>> GetInboundRecordsByProductInformationAsync(int productInformationId, bool trackChanges)
        {
            var inboundRecords = await _repository.InboundRecord.GetInboundRecordsByProductInformationIdAsync(productInformationId, trackChanges);
            return _mapper.Map<IEnumerable<InboundRecordDto>>(inboundRecords);
        }

        public async Task<IEnumerable<InboundRecordDto>> GetInboundRecordsByDateAsync(DateTime inboundDate, bool trackChanges)
        {
            var inboundRecords = await _repository.InboundRecord.GetInboundRecordsByDateAsync(inboundDate, trackChanges);
            return _mapper.Map<IEnumerable<InboundRecordDto>>(inboundRecords);
        }

        public async Task<InboundRecordDto> CreateInboundRecordAsync(InboundRecordForCreationDto inboundRecord, IHttpContextAccessor httpContextAccessor)
        {
            if (inboundRecord == null)
                throw new ArgumentNullException(nameof(inboundRecord), "InboundRecordForCreationDto cannot be null.");

            var inboundEntity = _mapper.Map<InboundRecord>(inboundRecord);
            inboundEntity.SetCreatedBy(httpContextAccessor);
            _repository.InboundRecord.CreateInboundRecord(inboundEntity);

            // Cập nhật Stock
            var stock = await _repository.Stock.GetStockByProductInformationIdAsync(inboundRecord.ProductInformationId, true);
            if (stock == null)
            {
                stock = new Stock
                {
                    ProductInformationId = inboundRecord.ProductInformationId,
                    QuantityUnits = inboundRecord.QuantityUnits,
                    QuantityWeight = inboundRecord.QuantityWeight,
                    LastUpdated = DateTime.UtcNow
                };
                _repository.Stock.CreateStock(stock);
            }
            else
            {
                stock.QuantityUnits += inboundRecord.QuantityUnits;
                stock.QuantityWeight += inboundRecord.QuantityWeight;
                stock.LastUpdated = DateTime.UtcNow;
            }

            await _repository.SaveAsync();
            return _mapper.Map<InboundRecordDto>(inboundEntity);
        }

        public async Task UpdateInboundRecordAsync(int inboundRecordId, InboundRecordForUpdateDto inboundRecordForUpdate, IHttpContextAccessor httpContextAccessor, bool trackChanges)
        {
            var inboundRecord = await GetInboundRecordAndCheckIfItExists(inboundRecordId, trackChanges);
            inboundRecord.SetUpdatedBy(httpContextAccessor);
            _mapper.Map(inboundRecordForUpdate, inboundRecord);
            await _repository.SaveAsync();
        }

        public async Task DeleteInboundRecordAsync(int inboundRecordId, bool trackChanges)
        {
            var inboundRecord = await GetInboundRecordAndCheckIfItExists(inboundRecordId, trackChanges);
            _repository.InboundRecord.DeleteInboundRecord(inboundRecord);
            await _repository.SaveAsync();
        }

        private async Task<InboundRecord> GetInboundRecordAndCheckIfItExists(int id, bool trackChanges)
        {
            var inboundRecord = await _repository.InboundRecord.GetInboundRecordByIdAsync(id, trackChanges);
            if (inboundRecord is null)
                throw new InboundRecordNotFoundException(id);
            return inboundRecord;
        }
    }
}