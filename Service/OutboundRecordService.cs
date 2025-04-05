using AutoMapper;
using Contracts;
using Entities.Exceptions.Stock;
using Entities.Models;
using Microsoft.AspNetCore.Http;
using Service.Contracts;
using Shared.DataTransferObjects.OutboundRecord;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service
{
    internal sealed class OutboundRecordService : IOutboundRecordService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;

        public OutboundRecordService(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<IEnumerable<OutboundRecordDto>> GetAllOutboundRecordsAsync(bool trackChanges)
        {
            var outboundRecords = await _repository.OutboundRecord.GetAllOutboundRecordsAsync(trackChanges);
            return _mapper.Map<IEnumerable<OutboundRecordDto>>(outboundRecords);
        }

        public async Task<OutboundRecordDto> GetOutboundRecordAsync(int outboundRecordId, bool trackChanges)
        {
            var outboundRecord = await _repository.OutboundRecord.GetOutboundRecordByIdAsync(outboundRecordId, trackChanges);
            if (outboundRecord == null)
                throw new StockNotFoundException(outboundRecordId); // Có thể tạo exception riêng
            return _mapper.Map<OutboundRecordDto>(outboundRecord);
        }

        public async Task<IEnumerable<OutboundRecordDto>> GetOutboundRecordsByProductInformationAsync(int productInformationId, bool trackChanges)
        {
            var outboundRecords = await _repository.OutboundRecord.GetOutboundRecordsByProductInformationIdAsync(productInformationId, trackChanges);
            return _mapper.Map<IEnumerable<OutboundRecordDto>>(outboundRecords);
        }

        public async Task<IEnumerable<OutboundRecordDto>> GetOutboundRecordsByDateAsync(DateTime outboundDate, bool trackChanges)
        {
            var outboundRecords = await _repository.OutboundRecord.GetOutboundRecordsByDateAsync(outboundDate, trackChanges);
            return _mapper.Map<IEnumerable<OutboundRecordDto>>(outboundRecords);
        }

        public async Task<OutboundRecordDto> CreateOutboundRecordAsync(OutboundRecordForCreationDto outboundRecord, IHttpContextAccessor httpContextAccessor)
        {
            if (outboundRecord == null)
                throw new ArgumentNullException(nameof(outboundRecord), "OutboundRecordForCreationDto cannot be null.");

            var stock = await _repository.Stock.GetStockByProductInformationIdAsync(outboundRecord.ProductInformationId, true);
            if (stock == null || stock.QuantityUnits < outboundRecord.QuantityUnits || stock.QuantityWeight < outboundRecord.QuantityWeight)
                throw new InvalidOperationException("Insufficient stock to perform this operation.");

            stock.QuantityUnits -= outboundRecord.QuantityUnits;
            stock.QuantityWeight -= outboundRecord.QuantityWeight;
            stock.LastUpdated = DateTime.UtcNow;

            var outboundEntity = _mapper.Map<OutboundRecord>(outboundRecord);
            outboundEntity.SetCreatedBy(httpContextAccessor);
            _repository.OutboundRecord.CreateOutboundRecord(outboundEntity);

            await _repository.SaveAsync();
            return _mapper.Map<OutboundRecordDto>(outboundEntity);
        }
    }
}