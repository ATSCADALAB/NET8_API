using Entities.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IOutboundRecordRepository
    {
        // Lấy tất cả bản ghi nhập kho
        Task<IEnumerable<OutboundRecord>> GetAllOutboundRecordsAsync(bool trackChanges);

        // Lấy bản ghi nhập kho theo ID
        Task<OutboundRecord> GetOutboundRecordByIdAsync(int OutboundRecordId, bool trackChanges);

        // Lấy danh sách bản ghi nhập kho theo ProductInformation ID
        Task<IEnumerable<OutboundRecord>> GetOutboundRecordsByProductInformationIdAsync(int productInformationId, bool trackChanges);

        // Lấy danh sách bản ghi nhập kho theo ngày nhập
        Task<IEnumerable<OutboundRecord>> GetOutboundRecordsByDateAsync(DateTime inboundDate, bool trackChanges);

        // Tạo bản ghi nhập kho mới
        void CreateOutboundRecord(OutboundRecord OutboundRecord);

        // Cập nhật thông tin bản ghi nhập kho
        void UpdateOutboundRecord(OutboundRecord OutboundRecord);

        // Xóa bản ghi nhập kho
        void DeleteOutboundRecord(OutboundRecord OutboundRecord);
    }
}