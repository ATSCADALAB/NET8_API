using AutoMapper;
using Contracts;
using Entities.Exceptions.SensorRecord;
using Entities.Models;
using Service.Contracts;
using Shared.DataTransferObjects.SensorRecord;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service
{
    internal sealed class SensorRecordService : ISensorRecordService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;

        public SensorRecordService(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<IEnumerable<SensorRecordDto>> GetAllSensorRecordsAsync(bool trackChanges)
        {
            var sensorRecords = await _repository.SensorRecord.GetAllSensorRecordsAsync(trackChanges);
            var sensorRecordsDto = _mapper.Map<IEnumerable<SensorRecordDto>>(sensorRecords);
            return sensorRecordsDto;
        }

        public async Task<SensorRecordDto> GetSensorRecordAsync(int sensorRecordId, bool trackChanges)
        {
            var sensorRecord = await GetSensorRecordAndCheckIfItExists(sensorRecordId, trackChanges);
            var sensorRecordDto = _mapper.Map<SensorRecordDto>(sensorRecord);
            return sensorRecordDto;
        }

        public async Task<IEnumerable<SensorRecordDto>> GetSensorRecordsByOrderAsync(Guid orderId, bool trackChanges)
        {
            var sensorRecords = await _repository.SensorRecord.GetSensorRecordsByOrderIdAsync(orderId, trackChanges);
            var sensorRecordsDto = _mapper.Map<IEnumerable<SensorRecordDto>>(sensorRecords);
            return sensorRecordsDto;
        }

        public async Task<IEnumerable<SensorRecordDto>> GetSensorRecordsByOrderDetailAsync(int orderDetailId, bool trackChanges)
        {
            var sensorRecords = await _repository.SensorRecord.GetSensorRecordsByOrderDetailIdAsync(orderDetailId, trackChanges);
            var sensorRecordsDto = _mapper.Map<IEnumerable<SensorRecordDto>>(sensorRecords);
            return sensorRecordsDto;
        }

        public async Task<IEnumerable<SensorRecordDto>> GetSensorRecordsByLineAsync(int lineId, bool trackChanges)
        {
            var sensorRecords = await _repository.SensorRecord.GetSensorRecordsByLineIdAsync(lineId, trackChanges);
            var sensorRecordsDto = _mapper.Map<IEnumerable<SensorRecordDto>>(sensorRecords);
            return sensorRecordsDto;
        }

        public async Task<IEnumerable<SensorRecordDto>> GetSensorRecordsByDateAsync(DateTime recordDate, bool trackChanges)
        {
            var sensorRecords = await _repository.SensorRecord.GetSensorRecordsByDateAsync(recordDate, trackChanges);
            var sensorRecordsDto = _mapper.Map<IEnumerable<SensorRecordDto>>(sensorRecords);
            return sensorRecordsDto;
        }

        public async Task<SensorRecordDto> CreateSensorRecordAsync(SensorRecordForCreationDto sensorRecord)
        {
            //sensorRecord.RecordTime=Datez
            if (sensorRecord == null)
                throw new ArgumentNullException(nameof(sensorRecord), "SensorRecordForCreationDto cannot be null.");

            var sensorRecordEntity = _mapper.Map<SensorRecord>(sensorRecord);
            _repository.SensorRecord.CreateSensorRecord(sensorRecordEntity);
            await _repository.SaveAsync();

            var sensorRecordToReturn = _mapper.Map<SensorRecordDto>(sensorRecordEntity);
            return sensorRecordToReturn;
        }

        public async Task UpdateSensorRecordAsync(int sensorRecordId, SensorRecordForUpdateDto sensorRecordForUpdate, bool trackChanges)
        {
            var sensorRecord = await GetSensorRecordAndCheckIfItExists(sensorRecordId, trackChanges);
            _mapper.Map(sensorRecordForUpdate, sensorRecord);
            await _repository.SaveAsync();
        }

        public async Task DeleteSensorRecordAsync(int sensorRecordId, bool trackChanges)
        {
            var sensorRecord = await GetSensorRecordAndCheckIfItExists(sensorRecordId, trackChanges);
            _repository.SensorRecord.DeleteSensorRecord(sensorRecord);
            await _repository.SaveAsync();
        }

        private async Task<SensorRecord> GetSensorRecordAndCheckIfItExists(int id, bool trackChanges)
        {
            var sensorRecord = await _repository.SensorRecord.GetSensorRecordByIdAsync(id, trackChanges);
            if (sensorRecord is null)
                throw new SensorRecordNotFoundException(id);
            return sensorRecord;
        }
    }
}