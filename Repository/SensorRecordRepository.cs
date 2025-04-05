using Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repository
{
    internal sealed class SensorRecordRepository : RepositoryBase<SensorRecord>, ISensorRecordRepository
    {
        public SensorRecordRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }

        public async Task<IEnumerable<SensorRecord>> GetAllSensorRecordsAsync(bool trackChanges) =>
            await FindAll(trackChanges)
                .Include(sr => sr.Order)
                    .ThenInclude(o=>o.CreatedBy)
                .Include(sr => sr.Order)
                    .ThenInclude(o => o.UpdatedBy)
 
                .Include(sr => sr.OrderDetail)
                .Include(sr => sr.Line)
                .Include(sr=>sr.CreatedBy)
                .Include(sr=>sr.UpdatedBy)
                .OrderBy(sr => sr.RecordTime)
                .ToListAsync();

        public async Task<SensorRecord> GetSensorRecordByIdAsync(int sensorRecordId, bool trackChanges) =>
            await FindByCondition(sr => sr.Id == sensorRecordId, trackChanges)
                .Include(sr => sr.Order)
                    .ThenInclude(o => o.CreatedBy)
                .Include(sr => sr.Order)
                    .ThenInclude(o => o.UpdatedBy)
                .Include(sr => sr.OrderDetail)
                .Include(sr => sr.Line)
                .Include(sr => sr.CreatedBy)
                .Include(sr => sr.UpdatedBy)
                .SingleOrDefaultAsync();

        public async Task<IEnumerable<SensorRecord>> GetSensorRecordsByOrderIdAsync(Guid orderId, bool trackChanges) =>
            await FindByCondition(sr => sr.OrderId == orderId, trackChanges)
                .Include(sr => sr.Order)
                    .ThenInclude(o => o.CreatedBy)
                .Include(sr => sr.Order)
                    .ThenInclude(o => o.UpdatedBy)
                .Include(sr => sr.OrderDetail)
                .Include(sr => sr.Line)
                .Include(sr => sr.CreatedBy)
                .Include(sr => sr.UpdatedBy)
                .OrderBy(sr => sr.RecordTime)
                .ToListAsync();

        public async Task<IEnumerable<SensorRecord>> GetSensorRecordsByOrderDetailIdAsync(int orderDetailId, bool trackChanges) =>
            await FindByCondition(sr => sr.OrderDetailId == orderDetailId, trackChanges)
                .Include(sr => sr.Order)
                    .ThenInclude(o => o.CreatedBy)
                .Include(sr => sr.Order)
                    .ThenInclude(o => o.UpdatedBy)
                .Include(sr => sr.OrderDetail)
                .Include(sr => sr.Line)
                .Include(sr => sr.CreatedBy)
                .Include(sr => sr.UpdatedBy)
                .OrderBy(sr => sr.RecordTime)
                .ToListAsync();

        public async Task<IEnumerable<SensorRecord>> GetSensorRecordsByLineIdAsync(int lineId, bool trackChanges) =>
            await FindByCondition(sr => sr.LineId == lineId, trackChanges)
                .Include(sr => sr.Order)
                    .ThenInclude(o => o.CreatedBy)
                .Include(sr => sr.Order)
                    .ThenInclude(o => o.UpdatedBy)
                .Include(sr => sr.OrderDetail)
                .Include(sr => sr.Line)
                .Include(sr => sr.CreatedBy)
                .Include(sr => sr.UpdatedBy)
                .OrderBy(sr => sr.RecordTime)
                .ToListAsync();

        public async Task<IEnumerable<SensorRecord>> GetSensorRecordsByDateAsync(DateTime recordDate, bool trackChanges) =>
            await FindByCondition(sr => sr.RecordTime.Date == recordDate.Date, trackChanges)
                .Include(sr => sr.Order)
                    .ThenInclude(o => o.CreatedBy)
                .Include(sr => sr.Order)
                    .ThenInclude(o => o.UpdatedBy)
                .Include(sr => sr.OrderDetail)
                .Include(sr => sr.Line)
                .Include(sr => sr.CreatedBy)
                .Include(sr => sr.UpdatedBy)
                .OrderBy(sr => sr.RecordTime)
                .ToListAsync();

        public void CreateSensorRecord(SensorRecord sensorRecord) => Create(sensorRecord);

        public void UpdateSensorRecord(SensorRecord sensorRecord) => Update(sensorRecord);

        public void DeleteSensorRecord(SensorRecord sensorRecord) => Delete(sensorRecord);
    }
}