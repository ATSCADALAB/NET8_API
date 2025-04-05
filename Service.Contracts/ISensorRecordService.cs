using Microsoft.AspNetCore.Http;
using Shared.DataTransferObjects.SensorRecord;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Contracts
{
    public interface ISensorRecordService
    {
        Task<IEnumerable<SensorRecordDto>> GetAllSensorRecordsAsync(bool trackChanges);
        Task<SensorRecordDto> GetSensorRecordAsync(int sensorRecordId, bool trackChanges);
        Task<IEnumerable<SensorRecordDto>> GetSensorRecordsByOrderAsync(Guid orderId, bool trackChanges);
        Task<IEnumerable<SensorRecordDto>> GetSensorRecordsByOrderDetailAsync(int orderDetailId, bool trackChanges);
        Task<IEnumerable<SensorRecordDto>> GetSensorRecordsByLineAsync(int lineId, bool trackChanges);
        Task<IEnumerable<SensorRecordDto>> GetSensorRecordsByDateAsync(DateTime recordDate, bool trackChanges);
        Task<SensorRecordDto> CreateSensorRecordAsync(SensorRecordForCreationDto sensorRecord, IHttpContextAccessor httpContextAccessor);
        Task UpdateSensorRecordAsync(int sensorRecordId, SensorRecordForUpdateDto sensorRecordForUpdate, IHttpContextAccessor httpContextAccessor, bool trackChanges);
        Task DeleteSensorRecordAsync(int sensorRecordId, bool trackChanges);
    }
}