using AutoMapper;
using Contracts;
using Entities.Exceptions.InboundRecord;
using Entities.Models;
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
            var inboundRecordsDto = _mapper.Map<IEnumerable<InboundRecordDto>>(inboundRecords);
            return inboundRecordsDto;
        }

        public async Task<InboundRecordDto> GetInboundRecordAsync(int inboundRecordId, bool trackChanges)
        {
            var inboundRecord = await GetInboundRecordAndCheckIfItExists(inboundRecordId, trackChanges);
            var inboundRecordDto = _mapper.Map<InboundRecordDto>(inboundRecord);
            return inboundRecordDto;
        }

        public async Task<IEnumerable<InboundRecordDto>> GetInboundRecordsByProductInformationAsync(int productInformationId, bool trackChanges)
        {
            var inboundRecords = await _repository.InboundRecord.GetInboundRecordsByProductInformationIdAsync(productInformationId, trackChanges);
            var inboundRecordsDto = _mapper.Map<IEnumerable<InboundRecordDto>>(inboundRecords);
            return inboundRecordsDto;
        }

        public async Task<IEnumerable<InboundRecordDto>> GetInboundRecordsByDateAsync(DateTime inboundDate, bool trackChanges)
        {
            var inboundRecords = await _repository.InboundRecord.GetInboundRecordsByDateAsync(inboundDate, trackChanges);
            var inboundRecordsDto = _mapper.Map<IEnumerable<InboundRecordDto>>(inboundRecords);
            return inboundRecordsDto;
        }

        public async Task<InboundRecordDto> CreateInboundRecordAsync(InboundRecordForCreationDto inboundRecord)
        {
            if (inboundRecord == null)
                throw new ArgumentNullException(nameof(inboundRecord), "InboundRecordForCreationDto cannot be null.");

            var inboundRecordEntity = _mapper.Map<InboundRecord>(inboundRecord);
            _repository.InboundRecord.CreateInboundRecord(inboundRecordEntity);
            await _repository.SaveAsync();

            var inboundRecordToReturn = _mapper.Map<InboundRecordDto>(inboundRecordEntity);
            return inboundRecordToReturn;
        }

        public async Task UpdateInboundRecordAsync(int inboundRecordId, InboundRecordForUpdateDto inboundRecordForUpdate, bool trackChanges)
        {
            var inboundRecord = await GetInboundRecordAndCheckIfItExists(inboundRecordId, trackChanges);
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