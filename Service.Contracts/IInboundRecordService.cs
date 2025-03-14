using Shared.DataTransferObjects.InboundRecord;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Contracts
{
    public interface IInboundRecordService
    {
        Task<IEnumerable<InboundRecordDto>> GetAllInboundRecordsAsync(bool trackChanges);
        Task<InboundRecordDto> GetInboundRecordAsync(int inboundRecordId, bool trackChanges);
        Task<IEnumerable<InboundRecordDto>> GetInboundRecordsByProductInformationAsync(int productInformationId, bool trackChanges);
        Task<IEnumerable<InboundRecordDto>> GetInboundRecordsByDateAsync(DateTime inboundDate, bool trackChanges);
        Task<InboundRecordDto> CreateInboundRecordAsync(InboundRecordForCreationDto inboundRecord);
        Task UpdateInboundRecordAsync(int inboundRecordId, InboundRecordForUpdateDto inboundRecordForUpdate, bool trackChanges);
        Task DeleteInboundRecordAsync(int inboundRecordId, bool trackChanges);
    }
}