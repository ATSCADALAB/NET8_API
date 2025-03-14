using Entities.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IInboundRecordRepository
    {
        // Lấy tất cả bản ghi nhập kho
        Task<IEnumerable<InboundRecord>> GetAllInboundRecordsAsync(bool trackChanges);

        // Lấy bản ghi nhập kho theo ID
        Task<InboundRecord> GetInboundRecordByIdAsync(int inboundRecordId, bool trackChanges);

        // Lấy danh sách bản ghi nhập kho theo ProductInformation ID
        Task<IEnumerable<InboundRecord>> GetInboundRecordsByProductInformationIdAsync(int productInformationId, bool trackChanges);

        // Lấy danh sách bản ghi nhập kho theo ngày nhập
        Task<IEnumerable<InboundRecord>> GetInboundRecordsByDateAsync(DateTime inboundDate, bool trackChanges);

        // Tạo bản ghi nhập kho mới
        void CreateInboundRecord(InboundRecord inboundRecord);

        // Cập nhật thông tin bản ghi nhập kho
        void UpdateInboundRecord(InboundRecord inboundRecord);

        // Xóa bản ghi nhập kho
        void DeleteInboundRecord(InboundRecord inboundRecord);
    }
}