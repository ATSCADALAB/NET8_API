using Entities.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contracts
{
    public interface ISensorRecordRepository
    {
        // Lấy tất cả bản ghi cảm biến
        Task<IEnumerable<SensorRecord>> GetAllSensorRecordsAsync(bool trackChanges);

        // Lấy bản ghi cảm biến theo ID
        Task<SensorRecord> GetSensorRecordByIdAsync(int sensorRecordId, bool trackChanges);

        // Lấy danh sách bản ghi cảm biến theo Order ID
        Task<IEnumerable<SensorRecord>> GetSensorRecordsByOrderIdAsync(Guid orderId, bool trackChanges);

        // Lấy danh sách bản ghi cảm biến theo OrderDetail ID
        Task<IEnumerable<SensorRecord>> GetSensorRecordsByOrderDetailIdAsync(int orderDetailId, bool trackChanges);

        // Lấy danh sách bản ghi cảm biến theo Line ID
        Task<IEnumerable<SensorRecord>> GetSensorRecordsByLineIdAsync(int lineId, bool trackChanges);

        // Lấy danh sách bản ghi cảm biến theo ngày ghi nhận
        Task<IEnumerable<SensorRecord>> GetSensorRecordsByDateAsync(DateTime recordDate, bool trackChanges);

        // Tạo bản ghi cảm biến mới
        void CreateSensorRecord(SensorRecord sensorRecord);

        // Cập nhật thông tin bản ghi cảm biến
        void UpdateSensorRecord(SensorRecord sensorRecord);

        // Xóa bản ghi cảm biến
        void DeleteSensorRecord(SensorRecord sensorRecord);
    }
}